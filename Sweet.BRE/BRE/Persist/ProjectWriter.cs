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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sweet.BRE
{
    public class ProjectWriter : IDisposable
    {
        # region WriterBlock

        private class WriterBlock : IDisposable
        {
            private ProjectWriter _persister;

            internal WriterBlock(ProjectWriter persister)
            {
                _persister = persister;
            }

            void IDisposable.Dispose()
            {
                _persister.PopElement();
            }
        }

        # endregion

        private const string StrName = "name";
        private const string StrNil = "nil";
        private const string StrTrue = "true";
        private const string StrType = "type";
        private const string StrVar = "var";

        private XmlDocument _document;
        private Stack<XmlElement> _stack;

        private ProjectWriter()
        {
        }

        public static XmlDocument Write(Project project)
        {
            if (ReferenceEquals(project, null))
            {
                throw new ArgumentNullException("project");
            }

            ProjectWriter writer = new ProjectWriter();
            return writer.WriteProject(project);
        }

        void IDisposable.Dispose()
        {
            _document = null;
            if (_stack != null)
            {
                _stack.Clear();
            }
        }

        private void Start()
        {
            _document = new XmlDocument();
            _stack = new Stack<XmlElement>();

            XmlElement root = _document.CreateElement("Project");
            _document.AppendChild(root);

            PushElement(root);
        }

        private void WriteAttribute(string name, string value)
        {
            XmlElement parent = PeekElement();

            XmlAttribute attr = _document.CreateAttribute(name);
            parent.Attributes.Append(attr);

            if (value != null)
            {
                attr.Value = value;
            }
        }

        private WriterBlock WriteElement(string name)
        {
            return WriteElement(name, null, true);
        }

        private WriterBlock WriteElement(string name, bool push)
        {
            return WriteElement(name, null, push);
        }

        private WriterBlock WriteElement(string name, string value)
        {
            return WriteElement(name, value, true);
        }

        private WriterBlock WriteElement(string name, string value, bool push)
        {
            XmlElement parent = PeekElement();

            XmlElement elm = _document.CreateElement(name);
            parent.AppendChild(elm);

            if (value != null)
            {
                elm.InnerText = value;
            }

            if (push)
            {
                PushElement(elm);
            }

            return new WriterBlock(this);
        }

        private XmlElement PeekElement()
        {
            return _stack.Peek();
        }

        private XmlElement PopElement()
        {
            return _stack.Pop();
        }

        private void PushElement(XmlElement elm)
        {
            _stack.Push(elm);
        }

        private void WriteRule(IRule rule)
        {
            string desc = rule.Description;
            if (!String.IsNullOrEmpty(desc))
            {
                WriteElement("Description", desc, false);
            }

            WriteWhen(rule.Condition);
            WriteDo(rule.Actions);
        }

        private void WriteRuleset(IRuleset ruleset)
        {
            string desc = ruleset.Description;
            if (!String.IsNullOrEmpty(desc))
            {
                WriteElement("Description", desc, false);
            }

            
            // Rules
            foreach (string name in ruleset.RuleNames)
            {
                using (WriteElement("Rule"))
                {
                    WriteAttribute(StrName, name);
                    WriteRule(ruleset.GetRule(name));
                }
            }
        }

        private void WriteAdd(AddStm stm)
        {
            using (WriteElement("Add"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            } 
        }

        private void WriteAnd(AndStm stm)
        {
            using (WriteElement("And"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteArithmeticGroup(ArithmeticGroupStm stm)
        {
            using (WriteElement("ArithmeticGroup"))
            {
                if (!ReferenceEquals(stm.Root, null))
                {
                    WriteStatement(stm.Root);
                }
            }
        }

        private void WriteBetween(BetweenStm stm)
        {
            using (WriteElement("Between"))
            {
                WriteElement("Target");
                WriteStatement(stm.Target);
                PopElement();

                WriteElement("Min");
                WriteStatement(stm.Min);
                PopElement();

                WriteElement("Max");
                WriteStatement(stm.Max);
                PopElement();
            }
        }

        private void WriteBreak(BreakStm stm)
        {
            using (WriteElement("Break"))
            {
                WriteElement("On");
                WriteStatement(stm.Condition);
                PopElement();
            }
        }

        private void WriteCall(CallRulesetStm stm)
        {
            using (WriteElement("Call"))
            {
                WriteElement("Ruleset");
                WriteStatement(stm.Ruleset);
                PopElement();
            }
        }

        private void WriteCase(CaseStm stm)
        {
            using (WriteElement("Case"))
            {
                Statement[] matches = stm.Case;
                foreach (Statement match in matches)
                {
                    WriteElement("Match");
                    WriteStatement(match);
                    PopElement();
                }

                WriteDo(stm.Actions);
            }
        }

        private void WriteConditionTest(ConditionTestStm stm)
        {
            using (WriteElement("Test"))
            {
                WriteElement("Condition");
                WriteStatement(stm.Condition);
                PopElement();

                WriteElement("IfTrue");
                WriteStatement(stm.IfTrue);
                PopElement();

                WriteElement("IfFalse");
                WriteStatement(stm.IfFalse);
                PopElement();
            }
        }

        private void WriteContext(ContextStm stm)
        {
            using (WriteElement("Context"))
            {
            }
        }

        private void WriteContinue(ContinueStm stm)
        {
            using (WriteElement("Continue"))
            {
                WriteElement("On");
                WriteStatement(stm.Condition);
                PopElement();
            }
        }

        private void WriteDate(DateStm stm)
        {
            WriteElement("Date", stm.Value.ToString("s"), false);
        }

        private void WriteDecisionColumns(DecisionColumnList conditions)
        {
            foreach (DecisionColumn col in conditions)
            {
                // Condition
                using (WriteElement("Column"))
                {
                    WriteAttribute(StrName, col.Name);
                    if (col.Type != DecisionValueType.String)
                    {
                        WriteAttribute(StrType, col.Type.ToString());
                    }

                    string desc = col.Description;
                    if (!String.IsNullOrEmpty(desc))
                    {
                        WriteElement("Description", desc, false);
                    }
                }
            }
        }

        private void WriteDecisionCell(DecisionCell cell)
        {
            string tag = ((cell is DecisionActionCell) ? "Set" : "Condition");

            using (WriteElement(tag))
            {
                if ((cell is DecisionActionNode) || (cell is DecisionConditionNode))
                {
                    // Type
                    if (cell.Type != DecisionValueType.String)
                    {
                        WriteAttribute(StrType, cell.Type.ToString());
                    }

                    // Variable
                    string variable = cell.Variable;

                    if (!String.IsNullOrEmpty(variable))
                    {
                        WriteAttribute(StrVar, variable);
                    }
                }

                DecisionConditionCell condition = cell as DecisionConditionCell;

                if (cell is DecisionActionCell)
                {
                    DecisionActionCell action = ((DecisionActionCell)cell);

                    string value = action.Value;
                    using (WriteElement("Value", value))
                    {
                        if (value == null)
                        {
                            WriteAttribute(StrNil, StrTrue);
                        }
                    }

                    if (!ReferenceEquals(action.FurtherMore, null))
                    {
                        using (WriteElement("FurtherMore"))
                        {
                            WriteDecisionCell(action.FurtherMore);
                        }
                    }
                }
                else if (condition != null)
                {
                    if (condition.Operation != DecisionOperation.Equals)
                    {
                        WriteAttribute("op", condition.Operation.ToString());
                    }

                    using (WriteElement("Values"))
                    {
                        // Values
                        foreach (string value in cell.Values)
                        {
                            using (WriteElement("Value", value))
                            {
                                if (value == null)
                                {
                                    WriteAttribute(StrNil, StrTrue);
                                }
                            }
                        }
                    }

                    if (!ReferenceEquals(condition.OnMatch, null))
                    {
                        using (WriteElement("OnMatch"))
                        {
                            WriteDecisionCell(condition.OnMatch);
                        }
                    }

                    if (!ReferenceEquals(condition.Else, null))
                    {
                        using (WriteElement("Else"))
                        {
                            WriteDecisionCell(condition.Else);
                        }
                    }
                }
            }
        }

        private void WriteDecisionTree(DecisionTree tree, string name)
        {
            using (WriteElement("Tree"))
            {
                if (!String.IsNullOrEmpty(name))
                {
                    WriteAttribute(StrName, name);
                }

                WriteAttribute(StrVar, tree.Variable);
                if (tree.Type != DecisionValueType.String)
                {
                    WriteAttribute(StrType, tree.Type.ToString());
                }

                DecisionConditionNode cond = tree as DecisionConditionNode;

                if ((cond != null) && (cond.Operation != DecisionOperation.Equals))
                {
                    WriteAttribute("op", cond.Operation.ToString());
                }

                using (WriteElement("Values"))
                {
                    // Values
                    foreach (string value in tree.Values)
                    {
                        WriteElement("Value", value, false);
                    }
                }

                if (!ReferenceEquals(cond.OnMatch, null))
                {
                    using (WriteElement("OnMatch"))
                    {
                        WriteDecisionCell(cond.OnMatch);
                    }
                }

                if (!ReferenceEquals(cond.Else, null))
                {
                    using (WriteElement("Else"))
                    {
                        WriteDecisionCell(cond.Else);
                    }
                }
            }
        }

        private void WriteDecisionRows(DecisionRowList rows)
        {
            using (WriteElement("Rows"))
            {
                foreach (DecisionRow row in rows)
                {
                    using (WriteElement("Row"))
                    {
                        WriteDecisionCell(row.Root);
                    }
                }
            }
        }

        private void WriteDecisionTable(DecisionTable table)
        {
            // Columns
            using (WriteElement("Columns"))
            {
                using (WriteElement("Conditions"))
                {
                    WriteDecisionColumns(table.Conditions);
                }

                using (WriteElement("Actions"))
                {
                    WriteDecisionColumns(table.Actions);
                }
            }

            // Rows
            WriteDecisionRows(table.Rows);
        }

        private void WriteDefine(DefineStm stm)
        {
            using (WriteElement("Define"))
            {
                WriteElement("Name");
                WriteStatement(stm.Name);
                PopElement();
            }
        }

        private void WriteDivide(DivideStm stm)
        {
            using (WriteElement("Divide"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteWhen(BooleanStm condition)
        {
            if (!ReferenceEquals(condition, null))
            {
                using (WriteElement("When"))
                {
                    WriteStatement(condition);
                }
            }
        }

        private void WriteDo(ActionList actions)
        {
            if (actions.Count > 0)
            {
                using (WriteElement("Do"))
                {
                    foreach (Statement action in actions)
                    {
                        WriteStatement(action);
                    }
                }
            }
        }

        private void WriteEqualTo(EqualToStm stm)
        {
            using (WriteElement("EqualTo"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteErrorHandler(CatchErrorStm stm)
        {
            using (WriteElement("OnError"))
            {
                WriteElement("Error");
                WriteStatement(stm.OnError);
                PopElement();

                WriteDo(stm.Actions);
            }
        }

        private void WriteEvaluateTable(EvaluateTableStm stm)
        {
            using (WriteElement("EvaluateTable"))
            {
                WriteElement("Table");
                WriteStatement(stm.Table);
                PopElement();
            }
        }

        private void WriteEvaluateTree(EvaluateTreeStm stm)
        {
            using (WriteElement("EvaluateTree"))
            {
                WriteElement("Tree");
                WriteStatement(stm.Tree);
                PopElement();
            }
        }

        private void WriteFact(FactStm stm)
        {
            using (WriteElement("Fact"))
            {
                WriteElement("Name");
                WriteStatement(stm.Name);
                PopElement();
            }
        }

        private void WriteFalse(FalseStm stm)
        {
            WriteElement("False", null, false);
        }

        private void WriteFor(ForStm stm)
        {
            using (WriteElement("For"))
            {
                WriteElement("Var");
                WriteStatement(stm.Variable);
                PopElement();

                WriteElement("From");
                WriteStatement(stm.From);
                PopElement();

                WriteElement("To");
                WriteStatement(stm.To);
                PopElement();

                WriteElement("Step");
                WriteStatement(stm.Step);
                PopElement();

                WriteDo(stm.Actions);
            }
        }

        private void WriteFunction(FunctionStm stm)
        {
            using (WriteElement("Function"))
            {
                WriteElement("Name");
                WriteStatement(stm.Name);
                PopElement();

                Statement[] prms = stm.Parameters;
                if ((prms != null) && (prms.Length > 0))
                {
                    using (WriteElement("Parameters"))
                    {
                        foreach (Statement prm in prms)
                        {
                            WriteStatement(prm);
                        }
                    }
                }
            }
        }

        private void WriteFunctionAliases(Project project)
        {
            string[] aliases = project.GetFunctionAliases();

            if ((aliases != null) && (aliases.Length > 0))
            {
                using (WriteElement("FunctionAliases"))
                {
                    foreach (string alias in aliases)
                    {
                        using (WriteElement("Alias"))
                        {
                            WriteAttribute(StrName, alias);
                            WriteAttribute("function", project.ResolveFunctionAlias(alias));
                        }
                    }
                }
            }
        }

        private void WriteGreaterThan(GreaterThanStm stm)
        {
            using (WriteElement("GreaterThan"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteGreaterThanOrEquals(GreaterThanOrEqualsStm stm)
        {
            using (WriteElement("GreaterThanOrEquals"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteHalt(HaltStm stm)
        {
            using (WriteElement("Halt"))
            {
                WriteElement("On");
                WriteStatement(stm.Condition);
                PopElement();
            }
        }

        private void WriteIfThen(IfThenStm stm)
        {
            using (WriteElement("If"))
            {
                using (WriteElement("Condition"))
                {
                    WriteStatement(stm.Condition);
                }

                using (WriteElement("Then"))
                {
                    WriteDo(stm.Actions);	
                }

                if (!ReferenceEquals(stm.Else, null))
                {
                    using (WriteElement("Else"))
                    {
                        WriteIfThen(stm.Else);
                    }
                }
            }
        }

        private void WriteIn(InStm stm)
        {
            using (WriteElement("In"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteItemOf(ItemOfStm stm)
        {
            using (WriteElement("ItemOf"))
            {
                WriteElement("Instance");
                WriteStatement(stm.Instance);
                PopElement();

                WriteElement("Index");
                WriteStatement(stm.Index);
                PopElement();

                WriteElement("SetValue");
                WriteStatement(stm.SetValue);
                PopElement();
            }
        }

        private void WriteLessThan(LessThanStm stm)
        {
            using (WriteElement("LessThan"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteLessThanOrEquals(LessThanOrEqualsStm stm)
        {
            using (WriteElement("LessThanOrEquals"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteLike(LikeStm stm)
        {
            using (WriteElement("Like"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteLogicalGroup(LogicalGroupStm stm)
        {
            using (WriteElement("LogicalGroup"))
            {
                if (!ReferenceEquals(stm.Root, null))
                {
                    WriteStatement(stm.Root);
                }
            }
        }

        private void WriteModulo(ModuloStm stm)
        {
            using (WriteElement("Modulo"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteMultiply(MultiplyStm stm)
        {
            using (WriteElement("Multiply"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNotEqualTo(NotEqualToStm stm)
        {
            using (WriteElement("NotEqualTo"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNotGreaterThan(NotGreaterThanStm stm)
        {
            using (WriteElement("NotGreaterThan"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNotIn(NotInStm stm)
        {
            using (WriteElement("NotIn"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNotLessThan(NotLessThanStm stm)
        {
            using (WriteElement("NotLessThan"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNotLike(NotLikeStm stm)
        {
            using (WriteElement("NotLike"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteNil()
        {
            WriteElement("Nil", null, false);
        }

        private void WriteNull(NullStm stm)
        {
            WriteElement("Null", null, false);
        }

        private void WriteNumeric(NumericStm stm)
        {
            WriteElement("Numeric", StmCommon.ToString(stm.Value), false);
        }

        private void WriteOr(OrStm stm)
        {
            using (WriteElement("Or"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private XmlDocument WriteProject(Project project)
        {
            Start();

            // Variables
            WriteVariables(project.Variables);

            // Function Aliases
            WriteFunctionAliases(project);

            // Decision tables
            using (WriteElement("Tables"))
            {
                foreach (string name in project.DecisionTableNames)
                {
                    using (WriteElement("Table"))
                    {
                        WriteAttribute(StrName, name);
                        WriteDecisionTable(project.GetDecisionTable(name));
                    }
                }
            }

            // Decision trees
            using (WriteElement("Trees"))
            {
                foreach (string name in project.DecisionTreeNames)
                {
                    WriteDecisionTree(project.GetDecisionTree(name), name);
                }
            }

            // Rulesets
            using (WriteElement("Rulesets"))
            {
                foreach (string name in project.RulesetNames)
                {
                    using (WriteElement("Ruleset"))
                    {
                        WriteAttribute(StrName, name);
                        WriteRuleset(project.GetRuleset(name));
                    }
                }
            }

            return _document;
        }

        private void WriteRaiseError(RaiseErrorStm stm)
        {
            using (WriteElement("Raise"))
            {
                WriteElement("On");
                WriteStatement(stm.Condition);
                PopElement();

                WriteElement("Message");
                WriteStatement(stm.Message);
                PopElement();
            }
        }

        private void WriteReflection(ReflectionStm stm)
        {
            using (WriteElement("Reflection"))
            {
                WriteElement("Instance");
                WriteStatement(stm.Instance);
                PopElement();

                WriteElement("Method");
                WriteStatement(stm.Expression);
                PopElement();

                Statement[] prms = stm.Parameters;
                if ((prms != null) && (prms.Length > 0))
                {
                    using (WriteElement("Parameters"))
                    {
                        foreach (Statement prm in prms)
                        {
                            WriteStatement(prm);
                        }
                    }
                }
            }
        }

        private void WriteRepeatUntil(RepeatUntilStm stm)
        {
            using (WriteElement("Repeat"))
            {
                WriteDo(stm.Actions);

                WriteElement("Until");
                WriteStatement(stm.Condition);
                PopElement();
            }
        }

        private void WriteReturn(ReturnStm stm)
        {
            using (WriteElement("Return"))
            {
                WriteElement("On");
                WriteStatement(stm.Condition);
                PopElement();
            }
        }

        private void WriteSet(SetStm stm)
        {
            using (WriteElement("Set"))
            {
                foreach (Statement item in stm)
                {
                    WriteStatement(item);
                }
            }
        }

        private void WriteSetFact(SetFactStm stm)
        {
            using (WriteElement("SetFact"))
            {
                WriteElement("Fact");
                WriteStatement(stm.Fact);
                PopElement();

                WriteElement("Value");
                WriteStatement(stm.Value);
                PopElement();
            }
        }

        private void WriteSetVariable(SetVariableStm stm)
        {
            using (WriteElement("SetVariable"))
            {
                WriteElement("To");
                WriteStatement(stm.To);
                PopElement();

                WriteElement("Value");
                WriteStatement(stm.Value);
                PopElement();
            }
        }

        private void WriteString(StringStm stm)
        {
            using (WriteElement("String", stm.Value))
            {
                if (stm.Value == null)
                {
                    WriteAttribute(StrNil, StrTrue);
                }
            }
        }

        private void WriteSubtract(SubtractStm stm)
        {
            using (WriteElement("Subtract"))
            {
                WriteElement("Left");
                WriteStatement(stm.Left);
                PopElement();

                WriteElement("Right");
                WriteStatement(stm.Right);
                PopElement();
            }
        }

        private void WriteSwitch(SwitchStm stm)
        {
            using (WriteElement("Switch"))
            {
                WriteElement("SwitchCase");
                WriteStatement(stm.SwitchCase);
                PopElement();

                CaseStm[] cases = stm.CaseList;
                if ((cases != null) && (cases.Length > 0))
                {
                    using (WriteElement("Cases"))
                    {
                        foreach (CaseStm cs in cases)
                        {
                            WriteCase(cs);
                        }
                    }
                }

                if (stm.Actions.Count > 0)
                {
                    WriteElement("Default");
                    WriteDo(stm.Actions);
                    PopElement();
                }
            }
        }

        private void WriteTime(TimeStm stm)
        {
            WriteElement("Time", StmCommon.ToString(stm.Value), false);
        }

        private void WriteTrue(TrueStm stm)
        {
            WriteElement("True", null, false);
        }

        private void WriteTry(TryStm stm)
        {
            using (WriteElement("Try"))
            {
                WriteDo(stm.Actions);

                CatchErrorStm[] handlers = stm.Handlers;
                if ((handlers != null) && (handlers.Length > 0))
                {
                    foreach (CatchErrorStm handler in handlers)
                    {
                        WriteErrorHandler(handler);
                    }
                }

                if (stm.Finally.Actions.Count > 0)
                {
                    WriteElement("Finally");
                    WriteDo(stm.Finally.Actions);
                    PopElement();
                }
            }
        }

        private void WriteUnaryMinus(UnaryMinusStm stm)
        {
            using (WriteElement("Minus"))
            {
                WriteStatement(stm.Target);
            }
        }

        private void WriteUnaryNot(UnaryNotStm stm)
        {
            using (WriteElement("Not"))
            {
                WriteStatement(stm.Target);
            }
        }

        private void WriteVariables(IVariableList variables)
        {
            if (!variables.IsEmpty)
            {
                using (WriteElement("Variables"))
                {
                    foreach (IVariable obj in variables)
                    {
                        string sValue = null;
                        ValueType type = obj.Type;

                        object value = obj.Value;
                        if (type == ValueType.DateTime)
                        {
                            sValue = ((DateTime)value).ToString("s");
                        }
                        else
                        {
                            sValue = StmCommon.ToString(value);
                        }

                        using (WriteElement("Variable", sValue))
                        {
                            WriteAttribute(StrName, obj.Name);
                            if (sValue == null)
                            {
                                WriteAttribute(StrNil, StrTrue);
                                continue;
                            }

                            WriteAttribute(StrType, type.ToString());
                        }
                    }
                }
            }
        }

        private void WriteVariable(VariableStm stm)
        {
            using (WriteElement("Variable"))
            {
                WriteElement("Name");
                WriteStatement(stm.Name);
                PopElement();
            }
        }

        private void WriteWhile(WhileStm stm)
        {
            using (WriteElement("While"))
            {
                WriteElement("Condition");
                WriteStatement(stm.Condition);
                PopElement();

                WriteDo(stm.Actions);
            }
        }

        # region Statement type decission

        private void WriteArithmeticStm(ArithmeticStm stm)
        {
            string type = stm.GetType().Name;
            switch (type)
            {
                case "AddStm":
                    WriteAdd((AddStm)stm);
                    break;

                case "DivideStm":
                    WriteDivide((DivideStm)stm);
                    break;

                case "ModuloStm":
                    WriteModulo((ModuloStm)stm);
                    break;

                case "MultiplyStm":
                    WriteMultiply((MultiplyStm)stm);
                    break;

                case "SubtractStm":
                    WriteSubtract((SubtractStm)stm);
                    break;

                default:
                    ThrowUndefinedStatement(stm);
                    break;
            }
        }

        private void WriteComparatorStm(ComparatorStm stm)
        {
            string type = stm.GetType().Name;
            switch (type)
            {
                case "EqualToStm":
                    WriteEqualTo((EqualToStm)stm);
                    break;

                case "GreaterThanStm":
                    WriteGreaterThan((GreaterThanStm)stm);
                    break;

                case "GreaterThanOrEqualsStm":
                    WriteGreaterThanOrEquals((GreaterThanOrEqualsStm)stm);
                    break;

                case "InStm":
                    WriteIn((InStm)stm);
                    break;

                case "LessThanStm":
                    WriteLessThan((LessThanStm)stm);
                    break;

                case "LessThanOrEqualsStm":
                    WriteLessThanOrEquals((LessThanOrEqualsStm)stm);
                    break;

                case "LikeStm":
                    WriteLike((LikeStm)stm);
                    break;

                case "NotEqualToStm":
                    WriteNotEqualTo((NotEqualToStm)stm);
                    break;

                case "NotGreaterThanStm":
                    WriteNotGreaterThan((NotGreaterThanStm)stm);
                    break;

                case "NotInStm":
                    WriteNotIn((NotInStm)stm);
                    break;

                case "NotLessThanStm":
                    WriteNotLessThan((NotLessThanStm)stm);
                    break;

                case "NotLikeStm":
                    WriteNotLike((NotLikeStm)stm);
                    break;

                default:
                    ThrowUndefinedStatement(stm);
                    break;
            }
        }

        private void WriteLogicalStm(LogicalStm stm)
        {
            string type = stm.GetType().Name;
            switch (type)
            {
                case "AndStm":
                    WriteAnd((AndStm)stm);
                    break;

                case "OrStm":
                    WriteOr((OrStm)stm);
                    break;

                default:
                    ThrowUndefinedStatement(stm);
                    break;
            }
        }

        private void WriteActionStm(ActionStm stm)
        {
            string type = stm.GetType().Name;
            switch (type)
            {
                case "BreakStm":
                    WriteBreak((BreakStm)stm);
                    break;

                case "CallStm":
                    WriteCall((CallRulesetStm)stm);
                    break;

                case "ConditionTestStm":
                    WriteConditionTest((ConditionTestStm)stm);
                    break;

                case "ContinueStm":
                    WriteContinue((ContinueStm)stm);
                    break;

                case "DefineStm":
                    WriteDefine((DefineStm)stm);
                    break;

                case "EvaluateTableStm":
                    WriteEvaluateTable((EvaluateTableStm)stm);
                    break;

                case "EvaluateTreeStm":
                    WriteEvaluateTree((EvaluateTreeStm)stm);
                    break;

                case "ForStm":
                    WriteFor((ForStm)stm);
                    break;

                case "FunctionStm":
                    WriteFunction((FunctionStm)stm);
                    break;

                case "HaltStm":
                    WriteHalt((HaltStm)stm);
                    break;

                case "IfThenStm":
                    WriteIfThen((IfThenStm)stm);
                    break;

                case "ItemOfStm":
                    WriteItemOf((ItemOfStm)stm);
                    break;

                case "RaiseErrorStm":
                    WriteRaiseError((RaiseErrorStm)stm);
                    break;

                case "ReflectionStm":
                    WriteReflection((ReflectionStm)stm);
                    break;

                case "RepeatUntilStm":
                    WriteRepeatUntil((RepeatUntilStm)stm);
                    break;

                case "ReturnStm":
                    WriteReturn((ReturnStm)stm);
                    break;

                case "SetFactStm":
                    WriteSetFact((SetFactStm)stm);
                    break;

                case "SetVariableStm":
                    WriteSetVariable((SetVariableStm)stm);
                    break;

                case "SwitchStm":
                    WriteSwitch((SwitchStm)stm);
                    break;

                case "TryStm":
                    WriteTry((TryStm)stm);
                    break;

                case "WhileStm":
                    WriteWhile((WhileStm)stm);
                    break;

                default:
                    ThrowUndefinedStatement(stm);
                    break;
            }            
        }

        private void WriteValueStm(ValueStm stm)
        {
            string type = stm.GetType().Name;
            switch (type)
            {
                case "ContextStm":
                    WriteContext((ContextStm)stm);
                    break;

                case "FactStm":
                    WriteFact((FactStm)stm);
                    break;

                case "VariableStm":
                    WriteVariable((VariableStm)stm);
                    break;

                case "ArithmeticGroupStm":
                    WriteArithmeticGroup((ArithmeticGroupStm)stm);
                    break;

                case "BetweenStm":
                    WriteBetween((BetweenStm)stm);
                    break;

                case "DateStm":
                    WriteDate((DateStm)stm);
                    break;

                case "FalseStm":
                    WriteFalse((FalseStm)stm);
                    break;

                case "NumericStm":
                    WriteNumeric((NumericStm)stm);
                    break;

                case "StringStm":
                    WriteString((StringStm)stm);
                    break;

                case "TimeStm":
                    WriteTime((TimeStm)stm);
                    break;

                case "TrueStm":
                    WriteTrue((TrueStm)stm);
                    break;

                case "UnaryNotStm":
                    WriteUnaryNot((UnaryNotStm)stm);
                    break;

                default:
                    ThrowUndefinedStatement(stm);
                    break;
            }
        }

        private void WriteStatement(Statement stm)
        {
            if (ReferenceEquals(stm, null))
            {
                WriteNil();
                return;
            }

            string type = stm.GetType().Name;
            switch (type)
            {
                case "NullStm":
                    WriteNull((NullStm)stm);
                    break;

                case "SetStm":
                    WriteSet((SetStm)stm);
                    break;

                case "UnaryMinusStm":
                    WriteUnaryMinus((UnaryMinusStm)stm);
                    break;

                case "LogicalGroupStm":
                    WriteLogicalGroup((LogicalGroupStm)stm);
                    break;

                default:
                    {
                        if (stm is ArithmeticStm)
                        {
                            WriteArithmeticStm((ArithmeticStm)stm);
                        }
                        else if (stm is ComparatorStm)
                        {
                            WriteComparatorStm((ComparatorStm)stm);
                        }
                        else if (stm is LogicalStm)
                        {
                            WriteLogicalStm((LogicalStm)stm);
                        }
                        else if (stm is ActionStm)
                        {
                            WriteActionStm((ActionStm)stm);
                        }
                        else if (stm is ValueStm)
                        {
                            WriteValueStm((ValueStm)stm);
                        }
                        else
                        {
                            ThrowUndefinedStatement(stm);
                        }
                        break;
                    }
            }
        }
        
        # endregion

        private void ThrowUndefinedStatement(Statement stm)
        {
            throw new RuleException(String.Format(BreResStrings.GetString("IsNotAKnownTypeByWriter"),
                stm.GetType().FullName));
        }
    }
}
