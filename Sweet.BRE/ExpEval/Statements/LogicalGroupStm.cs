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
    public sealed class LogicalGroupStm : BooleanStm
    {
        private BooleanStm _rootE;

        public LogicalGroupStm(BooleanStm item)
            : base(true)
        {
            _rootE = CheckItem(item);
        }

        public BooleanStm Root
        {
            get
            {
                return _rootE;
            }
        }

        public static LogicalGroupStm As(BooleanStm item)
        {
            return new LogicalGroupStm(item);
        }

        private BooleanStm CheckItem(BooleanStm item)
        {
            if ((object)item == null)
            {
                item = BooleanStm.True;
            }

            return item;
        }

        new public LogicalGroupStm And(BooleanStm item)
        {
            _rootE = AndStm.As(_rootE, CheckItem(item));
            return this;
        }

        new public LogicalGroupStm Or(BooleanStm item)
        {
            _rootE = OrStm.As(_rootE, CheckItem(item));
            return this;
        }


        public override object Clone()
        {
            return LogicalGroupStm.As((BooleanStm)_rootE.Clone());
        }

        public override void Dispose()
        {
            if (!object.Equals(_rootE, BooleanStm.True))
            {
                _rootE.Dispose();
                _rootE = BooleanStm.True;
            }

            base.Dispose();
        }

        public override bool Equals(object obj)
        {
            LogicalGroupStm objA = obj as LogicalGroupStm;
            return ReferenceEquals(this, obj) || (!ReferenceEquals(objA, null) &&
                object.Equals(_rootE, objA.Root));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private void Write(StringBuilder builder, BooleanStm boolE)
        {
            if (!(boolE is LogicalStm))
            {
                builder.Append(boolE.ToString());
                return;
            }

            LogicalStm logicalE = boolE as LogicalStm;

            Write(builder, logicalE.Left);
            if (logicalE is AndStm)
            {
                builder.AppendFormat(" {0} ", StmConstants.OP_AND);
            }
            else if (logicalE is OrStm)
            {
                builder.AppendFormat(" {0} ", StmConstants.OP_OR);
            }
            else 
            {
                builder.Append(" ? ");
            }

            builder.Append(logicalE.Right.ToString());
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

        private int DepthOf(LogicalStm logicE)
        {
            int result = 0;
            if (!ReferenceEquals(logicE, null))
            {
                result = 1;

                Statement l = logicE.Left;
                if (l is LogicalStm)
                {
                    result = 1 + DepthOf((LogicalStm)l);
                }
            }

            return result;
        }

        private int GetDepth()
        {
            int result = 0;
            if (!ReferenceEquals(_rootE, null))
            {
                result = 1 + DepthOf(_rootE as LogicalStm);
            }

            return result;
        }

        private OperationArgs[] Split(LogicalStm logicE)
        {
            List<OperationArgs> list = new List<OperationArgs>();

            string opStr = logicE.Operator;
            opStr = (opStr != null ? opStr.Trim() : null);

            char opChar = ((opStr != null) && (opStr.Length > 0)) ? opStr[0] : '\0';

            Statement l = logicE.Left;
            Statement r = logicE.Right;

            list.Insert(0, new OperationArgs(opChar, r));

            if (l is LogicalStm)
            {
                OperationArgs[] ops = Split((LogicalStm)l);
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

        private BooleanStm PrecedeOperations(OperationArgs[] args)
        {
            BooleanStm leftStm = null;

            int argsLength = args.Length;
            if (argsLength > 0)
            {
                leftStm = args[0].Statement as BooleanStm;

                if (argsLength > 1)
                {
                    int i = 0;
                    while (i < argsLength - 1)
                    {
                        i++;
                        OperationArgs currArg = args[i];

                        char currOp = currArg.Operation;
                        BooleanStm currStm = currArg.Statement as BooleanStm;

                        if ((i == argsLength - 1) || !((currOp == 'O') || (currOp == '|')))
                        {
                            leftStm = LogicalStm.As(currOp.ToString(), leftStm, currStm);
                            continue;
                        }

                        OperationArgs nextArg = args[i + 1];
                        char nextOp = nextArg.Operation;

                        if ((nextOp == 'O') || (nextOp == '|'))
                        {
                            leftStm = LogicalStm.As(currOp.ToString(), leftStm, currStm);
                            continue;
                        }

                        BooleanStm nextStm = nextArg.Statement as BooleanStm;

                        currStm = LogicalStm.As(nextOp.ToString(), currStm, nextStm);
                        args[i + 1] = new OperationArgs(currOp, currStm);
                    }
                }
            }

            return leftStm;
        }

        public LogicalGroupStm Precede()
        {
            if ((GetDepth() > 2) && (_rootE is LogicalStm))
            {
                OperationArgs[] args = Split((LogicalStm)_rootE);

                BooleanStm rootE = PrecedeOperations(args);
                return LogicalGroupStm.As(rootE);
            }

            return this;
        }

        protected override object Evaluate(IEvaluationContext context, params object[] args)
        {
            LogicalGroupStm groupE = Precede();

            IStatement rootE = groupE.Root;
            return !ReferenceEquals(rootE, null) ? rootE.Evaluate(context) : false;
        }

        # endregion
    }
}
