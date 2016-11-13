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
    public sealed class ArithmeticGroupStm : NumericStm
    {
        private Statement _rootE;

        public ArithmeticGroupStm(Statement item)
            : base(0)
        {
            _rootE = CheckItem(item);
        }

        public Statement Root
        {
            get
            {
                return _rootE;
            }
        }

        public static ArithmeticGroupStm As(Statement item)
        {
            return new ArithmeticGroupStm(item);
        }

        private Statement CheckItem(Statement item)
        {
            if ((object)item == null)
            {
                item = NumericStm.Zero;
            }

            return item;
        }

        public ArithmeticGroupStm Add(Statement item)
        {
            _rootE = Statement.Add(_rootE, CheckItem(item));
            return this;
        }

        public ArithmeticGroupStm Divide(Statement item)
        {
            _rootE = Statement.Divide(_rootE, CheckItem(item));
            return this;
        }

        public ArithmeticGroupStm Mod(Statement item)
        {
            _rootE = Statement.Mod(_rootE, CheckItem(item));
            return this;
        }

        public ArithmeticGroupStm Multiply(Statement item)
        {
            _rootE = Statement.Multiply(_rootE, CheckItem(item));
            return this;
        }

        public ArithmeticGroupStm Subtract(Statement item)
        {
            _rootE = Statement.Subtract(_rootE, CheckItem(item));
            return this;
        }

        public override object Clone()
        {
            return ArithmeticGroupStm.As((Statement)_rootE.Clone());
        }

        public override void Dispose()
        {
            if (!object.Equals(_rootE, NumericStm.Zero))
            {
                _rootE.Dispose();
                _rootE = NumericStm.Zero;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            ArithmeticGroupStm objA = obj as ArithmeticGroupStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_rootE, objA.Root));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private void Write(StringBuilder builder, Statement numberE)
        {
            if (!(numberE is ArithmeticStm))
            {
                builder.Append(StmCommon.PrepareToString(numberE));
                return;
            }

            ArithmeticStm arithE = numberE as ArithmeticStm;

            Write(builder, arithE.Left);
            if (arithE is AddStm)
            {
                builder.Append(" + ");
            }
            else if (arithE is DivideStm)
            {
                builder.Append(" / ");
            }
            else if (arithE is ModuloStm)
            {
                builder.Append(" % ");
            }
            else if (arithE is MultiplyStm)
            {
                builder.Append(" * ");
            }
            else if (arithE is SubtractStm)
            {
                builder.Append(" - ");
            }
            else
            {
                builder.Append(" ? ");
            }

            builder.Append(StmCommon.PrepareToString(arithE.Right));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(");
            Write(builder, _rootE);
            
            builder.Append(")");

            return builder.ToString();
        }

        # region Evaluate

        private int DepthOf(ArithmeticStm arithE)
        {
            int result = 0;
            if (!ReferenceEquals(arithE, null))
            {
                result = 1;

                Statement l = arithE.Left;
                if (l is ArithmeticStm)
                {
                    result = 1 + DepthOf((ArithmeticStm)l);
                }
            }

            return result;
        }

        private int GetDepth()
        {
            int result = 0;
            if (!ReferenceEquals(_rootE, null))
            {
                result = 1 + DepthOf(_rootE as ArithmeticStm);
            }

            return result;
        }

        private OperationArgs[] Split(ArithmeticStm arithE)
        {
            List<OperationArgs> list = new List<OperationArgs>();

            string opStr = arithE.Operator;
            opStr = (opStr != null ? opStr.Trim() : null);

            char opChar = ((opStr != null) && (opStr.Length > 0)) ? opStr[0] : '\0';

            Statement l = arithE.Left;
            Statement r = arithE.Right;

            list.Insert(0, new OperationArgs(opChar, r));

            if (l is ArithmeticStm)
            {
                OperationArgs[] ops = Split((ArithmeticStm)l);
                for (int i = ops.Length - 1; i > -1; i--)
                {
                    list.Insert(0, ops[i]);
                }
            }
            else
            {
                list.Insert(0, new OperationArgs('\0', l));
            }

            return list.ToArray();
        }

        private Statement PrecedeOperations(OperationArgs[] args)
        {
            Statement leftStm = null;

            int argsLength = args.Length;
            if (argsLength > 0)
            {
                leftStm = args[0].Statement;

                if (argsLength > 1)
                {
                    int i = 0;
                    while (i < argsLength - 1)
                    {
                        i++;
                        OperationArgs currArg = args[i];

                        char currOp = currArg.Operation;
                        Statement currStm = currArg.Statement;

                        if ((i == argsLength - 1) || !((currOp == '+') || (currOp == '-')))
                        {
                            leftStm = ArithmeticStm.As(currOp, leftStm, currStm);
                            continue;
                        }

                        OperationArgs nextArg = args[i + 1];
                        char nextOp = nextArg.Operation;

                        if ((nextOp == '+') || (nextOp == '-'))
                        {
                            leftStm = ArithmeticStm.As(currOp, leftStm, currStm);
                            continue;
                        }

                        Statement nextStm = nextArg.Statement;

                        currStm = ArithmeticStm.As(nextOp, currStm, nextStm);
                        args[i + 1] = new OperationArgs(currOp, currStm);
                    }
                }
            }

            return leftStm;
        }

        private ArithmeticGroupStm Precede()
        {
            if ((GetDepth() > 2) && (_rootE is ArithmeticStm))
            {
                OperationArgs[] args = Split((ArithmeticStm)_rootE);

                Statement rootE = PrecedeOperations(args);
                return new ArithmeticGroupStm(rootE);
            }

            return this;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            ArithmeticGroupStm groupE = Precede();
            
            IStatement rootE = groupE.Root;
            return !ReferenceEquals(rootE, null) ? rootE.Evaluate(context) : null;
        }

        # endregion
    }
}
