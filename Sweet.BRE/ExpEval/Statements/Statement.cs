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
    public abstract class Statement : IStatement, IDisposable, ICloneable
    {
        public static readonly Statement Null = new NullStm();

        protected Statement()
        {
        }

        protected virtual bool ExecutionBroken(IEvaluationContext context)
        {
            return (context.Canceled || context.Halted);
        }

        protected virtual object Evaluate(IEvaluationContext context, params object[] args)
        {
            return double.NaN;
        }

        private object DoEvaluate(IEvaluationContext context, params object[] args)
        {
            object result = null;
            try
            {
                if (args == null)
                {
                    result = Evaluate(context);
                }
                else
                {
                    result = Evaluate(context, args);
                }
            }
            catch (Exception e)
            {
                if (context.StopOnError)
                    throw;

                bool handled;
                context.HandleError(e, out handled);
            }

            return result;
        }

        object IStatement.Evaluate(IEvaluationContext context, params object[] args)
        {
            object result = null;

            if ((context != null) && !ExecutionBroken(context))
            {
                context.EvaluationStarted(this);
                if (args == null)
                {
                    result = DoEvaluate(context);
                }
                else
                {
                    result = DoEvaluate(context, args);
                }

                context.EvaluationCompleted(this);
            }

            return result;
        }

        public virtual object Clone()
        {
            return Statement.Null;
        }

        public virtual void Dispose()
        {
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static AddStm Add(Statement left, Statement right)
        {
            return new AddStm(left, right);
        }

        public static AndStm And(BooleanStm left, BooleanStm right)
        {
            return new AndStm(left, right);
        }

        public static DivideStm Divide(Statement left, Statement right)
        {
            return new DivideStm(left, right);
        }

        public EqualToStm EqualTo(Statement right)
        {
            return EqualTo(this, right);
        }

        public static EqualToStm EqualTo(Statement left, Statement right)
        {
            return new EqualToStm(left, right);
        }

        public InStm In(Statement right)
        {
            return In(this, right);
        }

        public static InStm In(Statement left, Statement right)
        {
            return new InStm(left, right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public GreaterThanStm GreaterThan(Statement right)
        {
            return GreaterThan(this, right);
        }

        public static GreaterThanStm GreaterThan(Statement left, Statement right)
        {
            return new GreaterThanStm(left, right);
        }

        public GreaterThanOrEqualsStm GreaterThanOrEquals(Statement right)
        {
            return GreaterThanOrEquals(this, right);
        }

        public static GreaterThanOrEqualsStm GreaterThanOrEquals(Statement left, Statement right)
        {
            return new GreaterThanOrEqualsStm(left, right);
        }

        public LessThanStm LessThan(Statement right)
        {
            return LessThan(this, right);
        }

        public static LessThanStm LessThan(Statement left, Statement right)
        {
            return new LessThanStm(left, right);
        }

        public LessThanOrEqualsStm LessThanOrEquals(Statement right)
        {
            return LessThanOrEquals(this, right);
        }

        public static LessThanOrEqualsStm LessThanOrEquals(Statement left, Statement right)
        {
            return new LessThanOrEqualsStm(left, right);
        }

        public LikeStm Like(Statement right)
        {
            return Like(this, right);
        }

        public static LikeStm Like(Statement left, Statement right)
        {
            return new LikeStm(left, right);
        }

        public static ModuloStm Mod(Statement left, Statement right)
        {
            return new ModuloStm(left, right);
        }

        public static MultiplyStm Multiply(Statement left, Statement right)
        {
            return new MultiplyStm(left, right);
        }

        public static UnaryNotStm Not(BooleanStm target)
        {
            return new UnaryNotStm(target);
        }

        public NotEqualToStm NotEqualTo(Statement right)
        {
            return NotEqualTo(this, right);
        }

        public static NotEqualToStm NotEqualTo(Statement left, Statement right)
        {
            return new NotEqualToStm(left, right);
        }

        public NotGreaterThanStm NotGreaterThan(Statement right)
        {
            return NotGreaterThan(this, right);
        }

        public static NotGreaterThanStm NotGreaterThan(Statement left, Statement right)
        {
            return new NotGreaterThanStm(left, right);
        }

        public NotInStm NotIn(Statement right)
        {
            return NotIn(this, right);
        }

        public static NotInStm NotIn(Statement left, Statement right)
        {
            return new NotInStm(left, right);
        }

        public NotLessThanStm NotLessThan(Statement right)
        {
            return NotLessThan(this, right);
        }

        public static NotLessThanStm NotLessThan(Statement left, Statement right)
        {
            return new NotLessThanStm(left, right);
        }

        public NotLikeStm NotLike(Statement right)
        {
            return NotLike(this, right);
        }

        public static NotLikeStm NotLike(Statement left, Statement right)
        {
            return new NotLikeStm(left, right);
        }

        public static OrStm Or(BooleanStm left, BooleanStm right)
        {
            return new OrStm(left, right);
        }

        public static SetStm Set(params Statement[] items)
        {
            return new SetStm(items);
        }

        public static SubtractStm Subtract(Statement left, Statement right)
        {
            return new SubtractStm(left, right);
        }

        # region Operators

        public static implicit operator Statement(bool value)
        {
            return (value ? BooleanStm.True : BooleanStm.False);
        }

        public static implicit operator Statement(byte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(char value)
        {
            return new StringStm(value.ToString());
        }

        public static implicit operator Statement(DateTime value)
        {
            return new DateStm(value);
        }

        public static implicit operator Statement(decimal value)
        {
            return new NumericStm((double)value);
        }

        public static implicit operator Statement(double value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(short value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(int value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(long value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(sbyte value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(float value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(string value)
        {
            return new StringStm(value);
        }

        public static implicit operator Statement(TimeSpan value)
        {
            return new TimeStm(value);
        }

        public static implicit operator Statement(ushort value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(uint value)
        {
            return new NumericStm(value);
        }

        public static implicit operator Statement(ulong value)
        {
            return new NumericStm(value);
        }

        public static BooleanStm operator ==(Statement left, Statement right)
        {
            return Statement.EqualTo(left, right);
        }

        public static BooleanStm operator >(Statement left, Statement right)
        {
            return Statement.GreaterThan(left, right);
        }

        public static BooleanStm operator >=(Statement left, Statement right)
        {
            return Statement.GreaterThanOrEquals(left, right);
        }

        public static BooleanStm operator !=(Statement left, Statement right)
        {
            return Statement.NotEqualTo(left, right);
        }

        public static BooleanStm operator <(Statement left, Statement right)
        {
            return Statement.LessThan(left, right);
        }

        public static BooleanStm operator <=(Statement left, Statement right)
        {
            return Statement.LessThanOrEquals(left, right);
        }

        public static AddStm operator +(Statement left, Statement right)
        {
            return Statement.Add(left, right);
        }

        public static SubtractStm operator -(Statement left, Statement right)
        {
            return Statement.Subtract(left, right);
        }

        public static DivideStm operator /(Statement left, Statement right)
        {
            return Statement.Divide(left, right);
        }

        public static MultiplyStm operator *(Statement left, Statement right)
        {
            return Statement.Multiply(left, right);
        }

        public static ModuloStm operator %(Statement left, Statement right)
        {
            return Statement.Mod(left, right);
        }

        # endregion
    }
}
