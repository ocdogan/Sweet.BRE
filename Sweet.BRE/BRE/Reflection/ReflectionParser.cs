/*
The MIT License(MIT)
=====================

Copyright(c) 2008, Cagatay Dogan

Permission is hereby granted, free of charge, to any person obtaining a cop
of this software and associated documentation files (the "Software"), to deal  
in the Software without restriction, including without limitation the right
to use, copy, modify, merge, publish, distribute, sublicense, and/or sel
copies of the Software, and to permit persons to whom the Software is  
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in  
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS O
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,  
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL TH
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHE
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS I
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Sweet.BRE
{
    public sealed class ReflectionParser
    {
        private const char Escape = '\\';
        private const char ListSeparator = ',';
        private const char DecimalSeparator = '.';
        private const char ExponentL = 'e';
        private const char ExponentU = 'E';

        private int pos;
        private int start;

        private char[] text;
        private MemberToken token;

        private MemberNode _root;

        private Stack<MemberNode> _parentStack;
        private Stack<Stack<MemberNode>> _frameStack;

        internal ReflectionParser(char[] path)
        {
            text = path;

            _root = new MemberNode(MemberType.Root);

            _parentStack = new Stack<MemberNode>();
            _frameStack = new Stack<Stack<MemberNode>>();
        }

        public static MemberNode[] Parse(string path)
        {
            path = (path != null ? path.Trim() : null);
            char[] data = (path != null) ? path.ToCharArray() : new char[0];

            return (new ReflectionParser(data).Parse());
        }

        private MemberNode[] Parse()
        {
            StartScan();
            
            while (pos < text.Length)
            {
                Scan();

                string constant = new string(text, start, pos - start);
                constant = constant.Trim();

                start = pos;

                MemberNode node = null;
                MemberNode parent = _parentStack.Peek();

                Stack<MemberNode> frame = _frameStack.Peek();

                MemberNode prevSibling = ((frame.Count > 0) ? frame.Peek() : null);
                MemberToken prevToken = ((prevSibling != null) ? ReflectionCommon.TypeToToken(prevSibling.Type) : MemberToken.None);

                if (prevSibling == null)
                {
                    prevToken = ((parent != null) ? ReflectionCommon.TypeToToken(parent.Type) : MemberToken.None);
                }

                bool updateSiblings = false;
                CheckToken(prevToken, token);

                switch (token)
                {
                    case MemberToken.Name:
                        {
                            updateSiblings = true;
                            node = new MemberNode(ReflectionCommon.TokenToType(token), constant);
                            break;
                        }
                    case MemberToken.Decimal:
                    case MemberToken.Float:
                    case MemberToken.Numeric:
                        {
                            node = new MemberNode(ReflectionCommon.TokenToType(token), constant);
                            break;
                        }
                    case MemberToken.Date:
                    case MemberToken.StringConst:
                        {
                            constant = constant.Substring(1, constant.Length - 2);
                            node = new MemberNode(ReflectionCommon.TokenToType(token), constant);
                            break;
                        }
                    case MemberToken.Argument:
                        {
                            constant = constant.Substring(1, constant.Length - 1);
                            node = new MemberNode(ReflectionCommon.TokenToType(token), constant);
                            break;
                        }
                    case MemberToken.LeftParen:
                    case MemberToken.LeftIndex:
                        {
                            updateSiblings = true;
                            node = new MemberNode(ReflectionCommon.TokenToType(token));
                            _parentStack.Push(node);
                            _frameStack.Push(new Stack<MemberNode>());
                            break;
                        }
                    case MemberToken.Dot:
                        {
                            updateSiblings = true;
                            node = new MemberNode(ReflectionCommon.TokenToType(token));
                            break;
                        }
                    case MemberToken.ListSeparator:
                        {
                            frame.Clear();
                            break;
                        }
                    case MemberToken.RightParen:
                    case MemberToken.RightIndex:
                        {
                            _parentStack.Pop();
                            parent = _parentStack.Peek();
                            _frameStack.Pop();
                            break;
                        }
                    default:
                        if (token != MemberToken.EOS)
                        {
                            throw new Exception(BreResStrings.GetString("NullToken"));
                        }
                        break;
                }

                if (node != null)
                {
                    frame.Push(node);

                    if (prevSibling == null)
                    {
                        parent.Arguments.Add(node);
                    }

                    if (updateSiblings && (prevSibling != null))
                    {
                        prevSibling.SetNextSibling(node);
                        node.SetPrevSibling(prevSibling);
                    }
                }

                ChangeToken(MemberToken.None);
            }

            MemberNode[] result = new MemberNode[_root.Arguments.Count];
            _root.Arguments.CopyTo(result, 0);

            return result;
        }

        private void StartScan()
        {
            pos = 0;
            start = 0;
            
            token = MemberToken.None;

            _root.Arguments.Clear();

            _parentStack.Clear();
            _parentStack.Push(_root);

            _frameStack.Clear();
            _frameStack.Push(new Stack<MemberNode>());
        }

        private bool IsAlphaNumeric(char ch)
        {
            return char.IsLetter(ch) || char.IsNumber(ch) || (ch == '_');
        }

        private bool IsDigit(char ch)
        {
            return char.IsNumber(ch); 
        }

        private bool IsWhiteSpace(char ch)
        {
            return char.IsWhiteSpace(ch) || (ch == '\0'); 
        }

        private void ScanWhite()
        {
            while ((pos < text.Length) && IsWhiteSpace(text[pos]))
            {
                pos++;
            }
        }

        private void ScanArgument()
        {
            pos++;
            while (pos < text.Length)
            {
                char ch = text[pos++];
                if (!IsDigit(ch))
                {
                    pos--;
                    break;
                }
            }

            int length = pos - (start + 1);
            if (length < 1)
            {
                ThrowUnknownToken();
            }

            ChangeToken(MemberToken.Argument);
        }

        private void ScanDate()
        {
            pos++;
            while ((pos < text.Length) && (text[pos] != '#'))
            {
                pos++;
            }

            if ((pos >= text.Length) || (text[pos] != '#'))
            {
                int stop = pos;
                if (pos >= text.Length)
                {
                    stop = (pos - 1);
                }

                throw new Exception(String.Format(BreResStrings.GetString("InvalidDate"),
                    new string(text, start, stop - start)));
            }

            ChangeToken(MemberToken.Date);
            pos++;
        }

        private void ScanName()
        {
            while ((pos < text.Length) && IsAlphaNumeric(text[pos]))
            {
                pos++;
            }
            
            ChangeToken(MemberToken.Name);
        }

        private void ScanText(char chEnd, char esc, char[] charsToEscape)
        {
            while ((pos < text.Length))
            {
                if (((text[pos] == esc) && ((pos + 1) < text.Length)) && 
                    (Array.IndexOf(charsToEscape, text[pos + 1]) >= 0))
                {
                    pos++;
                }

                pos++;
                if ((pos < text.Length) && (text[pos] != chEnd))
                {
                    if ((pos > 0) && (text[pos - 1] == Escape))
                        continue;

                    break;
                }
            }

            if (pos >= text.Length)
            {
                throw new Exception(String.Format(BreResStrings.GetString("InvalidNameBracketing"), 
                    new string(text, start, (pos - 1) - start)));
            }
            
            ChangeToken(MemberToken.Name);
        }

        private void ThrowInvalidChar()
        {
            throw new Exception(String.Format(BreResStrings.GetString("InvalidCharacterAtPosition"),
                pos.ToString()));
        }

        private void ThrowUnknownToken()
        {
            throw new Exception(String.Format(BreResStrings.GetString("UnknownTokenFoundAt"),
                new string(text, start, pos - start), pos.ToString()));
        }

        private void CheckToken(MemberToken prev, MemberToken token)
        {
            bool valid = true;
            switch (token)
            {
                case MemberToken.Name:
                    valid = ((prev == MemberToken.None) || (prev == MemberToken.Dot) ||
                        (prev == MemberToken.LeftParen) || (prev == MemberToken.LeftIndex) ||
                        (prev == MemberToken.ListSeparator));
                    break;

                case MemberToken.Decimal:
                case MemberToken.Float:
                case MemberToken.Numeric:
                case MemberToken.Date:
                case MemberToken.StringConst:
                    valid = ((prev == MemberToken.None) || (prev == MemberToken.LeftParen) || 
                        (prev == MemberToken.LeftIndex) || (prev == MemberToken.ListSeparator));
                    break;

                case MemberToken.Argument:
                    valid = ((prev == MemberToken.None) || (prev == MemberToken.ListSeparator) ||
                        (prev == MemberToken.LeftParen) || (prev == MemberToken.LeftIndex));
                    break;

                case MemberToken.LeftParen:
                    valid = ((prev == MemberToken.None) || (prev == MemberToken.Name) ||
                        (prev == MemberToken.LeftParen) || (prev == MemberToken.LeftIndex) ||
                        (prev == MemberToken.ListSeparator));
                    break;

                case MemberToken.LeftIndex:
                    valid = ((prev == MemberToken.None) || (prev == MemberToken.Name) || 
                        (prev == MemberToken.LeftParen) || (prev == MemberToken.LeftIndex) ||
                        (prev == MemberToken.ListSeparator) || (prev == MemberToken.Argument));
                    break;

                case MemberToken.Dot:
                case MemberToken.ListSeparator:
                    valid = (prev != MemberToken.None) && (prev != token) &&
                        (prev != MemberToken.Dot) && (prev != MemberToken.ListSeparator);
                    break;

                case MemberToken.RightIndex:
                case MemberToken.RightParen:
                    valid = (prev != MemberToken.None) && (prev != MemberToken.Dot) && 
                        (prev != MemberToken.ListSeparator);
                    break;
            }

            if (!valid)
            {
                ThrowUnknownToken();
            }
        }

        private void ScanNumeric()
        {
            bool isFloat = false;
            bool isDecimal = false;
            bool isText = false;

            while (pos < text.Length)
            {
                char ch = text[pos++];

                if (IsDigit(ch))
                    continue;

                bool validChar = true;
                switch (ch)
                {
                    case DecimalSeparator:
                        if (isDecimal)
                        {
                            ThrowInvalidChar();
                        }

                        isDecimal = true;
                        break;

                    case ExponentL:
                    case ExponentU:
                        if (isFloat)
                        {
                            ThrowInvalidChar();
                        }

                        isFloat = true;
                        break;

                    case '+':
                    case '-':
                        break;

                    default:
                        validChar = false;
                        break;
                }

                if (!validChar)
                {
                    if (IsAlphaNumeric(ch))
                    {
                        isText = true;
                        continue;
                    }

                    pos--;
                    break;
                }
            }

            if (isText)
            {
                ChangeToken(MemberToken.Name);
            }
            else if (isFloat)
            {
                ChangeToken(MemberToken.Float);
            }
            else if (isDecimal)
            {
                ChangeToken(MemberToken.Decimal);
            }
            else
            {
                ChangeToken(MemberToken.Numeric);
            }
        }

        private void ChangeToken(MemberToken value)
        {
            token = value;
        }

        internal MemberToken Scan()
        {
            if (pos > text.Length - 1)
            {
                ChangeToken(MemberToken.EOS);
                return token;
            }

            char ch = text[pos++];

            if (IsWhiteSpace(ch))
            {
                ScanWhite();
                ChangeToken(Scan());

                return token;
            }

            switch (ch)
            {
                case '#':
                    ScanDate();
                    ChangeToken(MemberToken.Date);
                    break;

                case '$':
                    ScanArgument();
                    ChangeToken(MemberToken.Argument);
                    break;

                case '(':
                    ChangeToken(MemberToken.LeftParen);
                    break;

                case ')':
                    ChangeToken(MemberToken.RightParen);
                    break;

                case '[':
                    ChangeToken(MemberToken.LeftIndex);
                    break;

                case ']':
                    ChangeToken(MemberToken.RightIndex);
                    break;

                case '`':
                case '\'':
                case '"':
                    ScanText(ch, ch, new char[] { ch });
                    ChangeToken(MemberToken.StringConst);
                    break;

                default:
                    if (ch == ListSeparator)
                    {
                        ChangeToken(MemberToken.ListSeparator);
                    }
                    else if (ch == '.')
                    {
                        ChangeToken(MemberToken.Dot);
                    }
                    else
                    {
                        pos--;
                        if (IsDigit(ch))
                        {
                            ScanNumeric();
                        }
                        else if (token == MemberToken.None)
                        {
                            if (IsAlphaNumeric(ch))
                            {
                                ScanName();
                                if (token == MemberToken.Name)
                                    break;
                            }

                            ChangeToken(MemberToken.Unknown);

                            pos = start;
                            ThrowUnknownToken();
                        }
                    }

                    break;
            }

            return token;
        }
    }
}
