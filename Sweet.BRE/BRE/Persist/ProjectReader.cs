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
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sweet.BRE
{
    public class ProjectReader : IDisposable
    {
        private const string StrName = "name";
        private const string StrNil = "nil";
        private const string StrTrue = "true";
        private const string StrType = "type";
        private const string StrVar = "var";

        private Project _project;
        private XmlDocument _document;
        private Stack<object> _readStack;

        private ProjectReader(XmlDocument document)
        {
            _document = document;
            _readStack = new Stack<object>();
        }

        public static Project Read(XmlDocument document)
        {
            if (ReferenceEquals(document, null))
            {
                throw new ArgumentNullException("document");
            }

            using (ProjectReader reader = new ProjectReader(document))
            {
                return reader.ReadProject();
            }
        }

        void IDisposable.Dispose()
        {
            _document = null;
            _project = null;
        }

        private XmlElement ReadProperty(XmlElement parent, string propName)
        {
            XmlElement elm = parent.SelectSingleNode(propName) as XmlElement;
            if (elm != null)
            {
                bool isNull = (String.Compare(elm.GetAttribute(StrNil), StrTrue) == 0);
                if (!isNull)
                {
                    foreach (XmlNode child in elm.ChildNodes)
                    {
                        if (child is XmlElement)
                        {
                            return (XmlElement)child;
                        }
                    }
                }
            }

            return null;
        }

        private XmlElement[] ReadParameters(XmlElement parent, string propName)
        {
            XmlElement elm = parent.SelectSingleNode(propName) as XmlElement;
            if (elm != null)
            {
                List<XmlElement> elms = new List<XmlElement>();

                foreach (XmlNode child in elm.ChildNodes)
                {
                    if (child is XmlElement)
                    {
                        elms.Add((XmlElement)child);
                    }
                }

                return elms.ToArray();
            }

            return new XmlElement[0];
        }

        private Project ReadProject()
        {
            XmlElement root = _document.DocumentElement;
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (root.Name != "Project")
            {
                throw new RuleException(BreResStrings.GetString("DocumentRootIsNotAsExpected"));
            }

            _project = new Project();

            ReadVariables(root);
            ReadFunctionAliases(root);

            ReadDecisionTables(root);
            ReadDecisionTrees(root);

            ReadRulesets(root);

            return _project;
        }

        # region Variables

        private void ReadVariables(XmlElement node)
        {
            XmlNodeList varNodes = node.SelectNodes("Variables/Variable");
            if (varNodes != null)
            {
                foreach (XmlNode nd in varNodes)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm != null)
                    {
                        ValueType type = ValueType.Null;

                        string str = CommonHelper.TrimString(elm.GetAttribute(StrType));
                        if (!String.IsNullOrEmpty(str))
                        {
                            type = (ValueType)Enum.Parse(typeof(ValueType), str);
                        }

                        string name = CommonHelper.TrimString(elm.GetAttribute(StrName));

                        string value = null;
                        if (String.Compare(elm.GetAttribute(StrNil), StrTrue, true) != 0)
                        {
                            value = elm.InnerText;
                        }

                        switch (type)
                        {
                        	case ValueType.Null:
                                _project.Variables.Set(name, (string)null);
                        		break;

                            case ValueType.Boolean:
                                _project.Variables.Set(name, StmCommon.ToBoolean(value));
                                break;

                            case ValueType.Char:
                                _project.Variables.Set(name, StmCommon.ToChar(value));
                                break;

                            case ValueType.DateTime:
                                _project.Variables.Set(name, StmCommon.ToDate(value));
                                break;

                            case ValueType.Float:
                                _project.Variables.Set(name, StmCommon.ToDouble(value));
                                break;

                            case ValueType.Integer:
                                _project.Variables.Set(name, StmCommon.ToInteger(value));
                                break;

                            case ValueType.TimeSpan:
                                _project.Variables.Set(name, StmCommon.ToTime(value));
                                break;

                            default:
                                _project.Variables.Set(name, value);
                                break;
                        }
                    }
                }
            }
        }

        # endregion

        # region Function Aliases

        private void ReadFunctionAliases(XmlElement node)
        {
            XmlNodeList aliasNodes = node.SelectNodes("FunctionAliases/Alias");
            if (aliasNodes != null)
            {
                foreach (XmlNode nd in aliasNodes)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm != null)
                    {
                        string alias = CommonHelper.TrimString(elm.GetAttribute(StrName));

                        if (!String.IsNullOrEmpty(alias))
                        {
                            string function = CommonHelper.TrimString(elm.GetAttribute("function"));
                            _project.RegisterFunctionAlias(alias, function);
                        }
                    }
                }
            }
        }

        # endregion

        # region Rulesets

        private void ReadRules(XmlElement node)
        {
            Ruleset ruleset = (Ruleset)_readStack.Peek();

            XmlNodeList rules = node.SelectNodes("Rule");
            if (rules != null)
            {
                foreach (XmlNode nd in rules)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm == null)
                        continue;

                    IRule rule = ruleset.DefineRule(elm.GetAttribute(StrName));
                    _readStack.Push(rule);
                    try
                    {
                        rule.SetDescription(ReadText(elm, "Description"));

                        XmlElement[] elms = ReadParameters(elm, "If");
                        if ((elms != null) && (elms.Length > 0))
                        {                            
                            BooleanStm condition = ReadStatement(elms[0]) as BooleanStm;
                            if (!ReferenceEquals(condition, null))
                            {
                                rule.If(condition);
                            }
                        }

                        ActionStm[] actions = new ActionStm[0];

                        elms = ReadParameters(elm, "Do");
                        if ((elms != null) && (elms.Length > 0))
                        {
                            actions = new ActionStm[elms.Length];

                            int i = 0;
                            foreach (XmlElement nd2 in elms)
                            {
                                actions[i++] = (ActionStm)ReadStatement(nd2);
                            }
                        }

                        rule.Do(actions);
                    }
                    finally
                    {
                        _readStack.Pop();
                    }
                }
            }
        }

        private void ReadRulesets(XmlElement node)
        {
            XmlNodeList rulesets = node.SelectNodes("Rulesets/Ruleset");
            if (rulesets != null)
            {
                foreach (XmlNode nd in rulesets)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm == null)
                        continue;

                    IRuleset ruleset = _project.DefineRuleset(elm.GetAttribute(StrName));
                    _readStack.Push(ruleset);
                    try
                    {
                        ruleset.SetDescription(ReadText(elm, "Description"));
                        ReadRules(elm);
                    }
                    finally
                    {
                        _readStack.Pop();
                    }
                }
            }
        }

        # endregion

        # region Decisions Common

        private DecisionCell CreateCell(XmlElement elm)
        {
            DecisionCell cell = null;
            XmlElement childNode = elm.SelectSingleNode("Condition") as XmlElement;

            bool isTable = (_readStack.Peek() is DecisionTable);
            if (childNode != null)
            {
                if (isTable)
                {
                    cell = new DecisionConditionCell();
                }
                else
                {
                    cell = new DecisionConditionNode();
                }
            }
            else
            {
                childNode = elm.SelectSingleNode("Set") as XmlElement;
                if (childNode != null)
                {
                    if (isTable)
                    {
                        cell = new DecisionActionCell();
                    }
                    else
                    {
                        cell = new DecisionActionNode();
                    }
                }
            }

            return cell;
        }

        private void ReadDecisionCellOnMatch(DecisionConditionCell condition, XmlElement elm)
        {
            XmlElement matchNode = elm.SelectSingleNode("OnMatch") as XmlElement;
            if (matchNode != null)
            {
                DecisionCell onMatch = CreateCell(matchNode);

                if (onMatch != null)
                {
                    condition.SetOnMatch(onMatch);

                    XmlElement childNode = matchNode.SelectSingleNode("Condition") as XmlElement;
                    if (childNode == null)
                    {
                        childNode = matchNode.SelectSingleNode("Set") as XmlElement;
                    }

                    ReadDecisionCell(onMatch, childNode);
                }
            }
        }

        private void ReadDecisionCellElse(DecisionConditionCell condition, XmlElement elm)
        {
            XmlElement elseNode = elm.SelectSingleNode("Else") as XmlElement;
            if (elseNode != null)
            {
                DecisionCell elseCell = CreateCell(elseNode);

                if (elseCell != null)
                {
                    condition.SetElse(elseCell);

                    XmlElement childNode = elseNode.SelectSingleNode("Condition") as XmlElement;
                    if (childNode == null)
                    {
                        childNode = elseNode.SelectSingleNode("Set") as XmlElement;
                    }

                    ReadDecisionCell(elseCell, childNode);
                }
            }
        }

        private void ReadDecisionCell(DecisionCell cell, XmlElement elm)
        {
            string variable = null;
            DecisionValueType type = DecisionValueType.String;

            if ((cell is DecisionActionNode) || (cell is DecisionConditionNode))
            {
                // Type
                string str = CommonHelper.TrimString(elm.GetAttribute(StrType));

                if (!String.IsNullOrEmpty(str))
                {
                    type = (DecisionValueType)Enum.Parse(typeof(DecisionValueType), str);
                }

                cell.SetType(type);

                // Variable
                variable = CommonHelper.TrimString(elm.GetAttribute(StrVar));
            }

            if (cell is DecisionActionCell)
            {
                DecisionActionCell action = (DecisionActionCell)cell;
                if (cell is DecisionActionNode)
                {
                    ((DecisionActionNode)cell).SetVariable(variable);
                }

                // Value
                XmlElement valueNode = elm.SelectSingleNode("Value") as XmlElement;
                if (valueNode != null)
                {
                    string value = null;
                    if (String.Compare(valueNode.GetAttribute(StrNil), StrTrue, true) != 0)
                    {
                        value = valueNode.InnerText;
                    }

                    action.SetValue(value);
                }

                // Sibling
                XmlElement siblingNode = elm.SelectSingleNode("FurtherMore") as XmlElement;
                if (siblingNode != null)
                {
                    XmlElement childNode = siblingNode.SelectSingleNode("Set") as XmlElement;
                    if (childNode != null)
                    {
                        DecisionCell sibling = CreateCell(siblingNode);

                        if (sibling is DecisionActionCell)
                        {
                            action.SetFurtherMore((DecisionActionCell)sibling);
                            ReadDecisionCell(sibling, childNode);
                        }
                    }
                }
            }
            else if (cell is DecisionConditionCell)
            {
                DecisionConditionCell condition = (DecisionConditionCell)cell;
                if (cell is DecisionConditionNode)
                {
                    ((DecisionConditionNode)cell).SetVariable(variable);
                }

                // Operation
                DecisionOperation op = DecisionOperation.Equals;
                string opStr = CommonHelper.TrimString(elm.GetAttribute("op"));

                if (!String.IsNullOrEmpty(opStr))
                {
                    op = (DecisionOperation)Enum.Parse(typeof(DecisionOperation), opStr);
                }

                // Values
                List<string> values = new List<string>();

                XmlNodeList valueList = elm.SelectNodes("Values/Value");
                if (valueList != null)
                {
                    foreach (XmlNode nd2 in valueList)
                    {
                        XmlElement elm2 = (XmlElement)nd2;

                        string value = null;
                        if (String.Compare(elm2.GetAttribute(StrNil), StrTrue, true) != 0)
                        {
                            value = elm2.InnerText;
                        }

                        values.Add(value);
                    }
                }

                condition.SetOperation(op);
                condition.SetValues(values.ToArray());

                ReadDecisionCellOnMatch(condition, elm);
                ReadDecisionCellElse(condition, elm);
            }
        }

        # endregion

        # region Decision tables

        private void ReadDecisionTableColumns(XmlNodeList colList, DecisionColumnList columns)
        {
            if ((colList != null) && (columns != null))
            {
                foreach (XmlNode node in colList)
                {
                    XmlElement elm = node as XmlElement;
                    if (elm == null)
                        continue;

                    string description = null;
                    DecisionValueType type = DecisionValueType.String;

                    string name = CommonHelper.TrimString(elm.GetAttribute(StrName));

                    string str = CommonHelper.TrimString(elm.GetAttribute(StrType));
                    if (!String.IsNullOrEmpty(str))
                    {
                        type = (DecisionValueType)Enum.Parse(typeof(DecisionValueType), str);
                    }

                    description = ReadText(elm, "Description");

                    columns.Add(name, description, type);
                }
            }
        }

        private void ReadDecisionTableRows(DecisionTable table, XmlElement node)
        {
            XmlNodeList rowList = node.SelectNodes("Rows/Row");
            if (rowList != null)
            {
                foreach (XmlNode nd in rowList)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm == null)
                        continue;

                    DecisionCell cell = CreateCell(elm);
                    if (cell != null)
                    {
                        DecisionRow row = table.Rows.Add(false);
                        row.SetRoot(cell);

                        XmlElement childNode = elm.SelectSingleNode("Condition") as XmlElement;
                        if (childNode == null)
                        {
                            childNode = elm.SelectSingleNode("Set") as XmlElement;
                        }

                        ReadDecisionCell(cell, childNode);
                    }
                }
            }
        }

        private void ReadDecisionTables(XmlElement node)
        {
            XmlNodeList tableNodes = node.SelectNodes("Tables/Table");
            if (tableNodes != null)
            {
                foreach (XmlNode nd in tableNodes)
                {
                    XmlElement elm = nd as XmlElement;
                    if (elm == null)
                        continue;

                    DecisionTable table = _project.DefineTable(elm.GetAttribute(StrName));
                    _readStack.Push(table);
                    try
                    {
                        XmlNodeList condList = elm.SelectNodes("Columns/Conditions/Column");
                        ReadDecisionTableColumns(condList, table.Conditions);

                        XmlNodeList actionList = elm.SelectNodes("Columns/Actions/Column");
                        ReadDecisionTableColumns(actionList, table.Actions);

                        ReadDecisionTableRows(table, elm);
                    }
                    finally
                    {
                        _readStack.Pop();
                    }
                }
            }
        }

        # endregion

        # region Decision trees

        private void ReadDecisionTrees(XmlElement node)
        {
            XmlNodeList treeNodes = node.SelectNodes("Trees/Tree");
            if (treeNodes != null)
            {
                foreach (XmlNode nd in treeNodes)
                {
                    if (nd is XmlElement)
                    {
                        XmlElement elm = nd as XmlElement;
                        if (elm == null)
                            continue;

                        DecisionTree tree = _project.DefineTree(elm.GetAttribute(StrName));
                        _readStack.Push(tree);
                        try
                        {
                            ReadDecisionCell(tree, elm);
                        }
                        finally
                        {
                            _readStack.Pop();
                        }
                    }
                }
            }
        }

        # endregion

        # region Arithmetic statements

        private AddStm ReadAdd(XmlElement node)
        {
            if ((node != null) && (node.Name == "Add"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return AddStm.As(left, right);
            }

            return null;
        }

        private DivideStm ReadDivide(XmlElement node)
        {
            if ((node != null) && (node.Name == "Divide"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return DivideStm.As(left, right);
            }

            return null;
        }

        private ModuloStm ReadModulo(XmlElement node)
        {
            if ((node != null) && (node.Name == "Modulo"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return ModuloStm.As(left, right);
            }

            return null;
        }

        private MultiplyStm ReadMultiply(XmlElement node)
        {
            if ((node != null) && (node.Name == "Multiply"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return MultiplyStm.As(left, right);
            }

            return null;
        }

        private SubtractStm ReadSubtract(XmlElement node)
        {
            if ((node != null) && (node.Name == "Subtract"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return SubtractStm.As(left, right);
            }

            return null;
        }

        private Statement ReadArithmeticStm(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "Add":
                    return ReadAdd(node);

                case "Divide":
                    return ReadDivide(node);

                case "Modulo":
                    return ReadModulo(node);

                case "Multiply":
                    return ReadMultiply(node);

                case "Subtract":
                    return ReadSubtract(node);
            }

            return null;
        }

        # endregion

        # region Logical statements

        private AndStm ReadAnd(XmlElement node)
        {
            if ((node != null) && (node.Name == "And"))
            {
                BooleanStm left = null;
                BooleanStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = (BooleanStm)ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (BooleanStm)ReadStatement(elm);
                }

                return AndStm.As(left, right);
            }

            return null;
        }

        private OrStm ReadOr(XmlElement node)
        {
            if ((node != null) && (node.Name == "Or"))
            {
                BooleanStm left = null;
                BooleanStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = (BooleanStm)ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (BooleanStm)ReadStatement(elm);
                }

                return OrStm.As(left, right);
            }

            return null;
        }

        private Statement ReadLogicalStm(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "And":
                    return ReadAnd(node);

                case "Or":
                    return ReadOr(node);
            }

            return null;
        }

        # endregion

        # region Comparator statements

        private EqualToStm ReadEqualTo(XmlElement node)
        {
            if ((node != null) && (node.Name == "EqualTo"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return EqualToStm.As(left, right);
            }

            return null;
        }

        private GreaterThanStm ReadGreaterThan(XmlElement node)
        {
            if ((node != null) && (node.Name == "GreaterThan"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return GreaterThanStm.As(left, right);
            }

            return null;
        }

        private GreaterThanOrEqualsStm ReadGreaterThanOrEquals(XmlElement node)
        {
            if ((node != null) && (node.Name == "GreaterThanOrEquals"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return GreaterThanOrEqualsStm.As(left, right);
            }

            return null;
        }

        private InStm ReadIn(XmlElement node)
        {
            if ((node != null) && (node.Name == "In"))
            {
                Statement left = null;
                SetStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (SetStm)ReadStatement(elm);
                }

                return InStm.As(left, right);
            }

            return null;
        }

        private LessThanStm ReadLessThan(XmlElement node)
        {
            if ((node != null) && (node.Name == "LessThan"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return LessThanStm.As(left, right);
            }

            return null;
        }

        private LessThanOrEqualsStm ReadLessThanOrEquals(XmlElement node)
        {
            if ((node != null) && (node.Name == "LessThanOrEquals"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return LessThanOrEqualsStm.As(left, right);
            }

            return null;
        }

        private LikeStm ReadLike(XmlElement node)
        {
            if ((node != null) && (node.Name == "Like"))
            {
                Statement left = null;
                StringStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (StringStm)ReadStatement(elm);
                }

                return LikeStm.As(left, right);
            }

            return null;
        }

        private NotEqualToStm ReadNotEqualTo(XmlElement node)
        {
            if ((node != null) && (node.Name == "NotEqualTo"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return NotEqualToStm.As(left, right);
            }

            return null;
        }

        private NotGreaterThanStm ReadNotGreaterThan(XmlElement node)
        {
            if ((node != null) && (node.Name == "NotGreaterThan"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return NotGreaterThanStm.As(left, right);
            }

            return null;
        }

        private NotInStm ReadNotIn(XmlElement node)
        {
            if ((node != null) && (node.Name == "NotIn"))
            {
                Statement left = null;
                SetStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (SetStm)ReadStatement(elm);
                }

                return NotInStm.As(left, right);
            }

            return null;
        }

        private NotLessThanStm ReadNotLessThan(XmlElement node)
        {
            if ((node != null) && (node.Name == "NotLessThan"))
            {
                Statement left = null;
                Statement right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = ReadStatement(elm);
                }

                return NotLessThanStm.As(left, right);
            }

            return null;
        }

        private NotLikeStm ReadNotLike(XmlElement node)
        {
            if ((node != null) && (node.Name == "NotLike"))
            {
                Statement left = null;
                StringStm right = null;

                XmlElement elm = ReadProperty(node, "Left");
                if (elm != null)
                {
                    left = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Right");
                if (elm != null)
                {
                    right = (StringStm)ReadStatement(elm);
                }

                return NotLikeStm.As(left, right);
            }

            return null;
        }

        private Statement ReadComparatorStm(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "EqualTo":
                    return ReadEqualTo(node);

                case "GreaterThan":
                    return ReadGreaterThan(node);

                case "GreaterThanOrEquals":
                    return ReadGreaterThanOrEquals(node);

                case "In":
                    return ReadIn(node);

                case "LessThan":
                    return ReadLessThan(node);

                case "LessThanOrEquals":
                    return ReadLessThanOrEquals(node);

                case "Like":
                    return ReadLike(node);

                case "NotEqualTo":
                    return ReadNotEqualTo(node);

                case "NotGreaterThan":
                    return ReadNotGreaterThan(node);

                case "NotIn":
                    return ReadNotIn(node);

                case "NotLessThan":
                    return ReadNotLessThan(node);

                case "NotLike":
                    return ReadNotLike(node);
            }

            return null;
        }

        # endregion

        # region Action statements

        private SetVariableStm ReadSetVariable(XmlElement node)
        {
            if ((node != null) && (node.Name == "SetVariable"))
            {
                Statement to = null;
                Statement value = null;

                XmlElement elm = ReadProperty(node, "To");
                if (elm != null)
                {
                    to = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Value");
                if (elm != null)
                {
                    value = ReadStatement(elm);
                }

                return SetVariableStm.As(to, value);
            }

            return null;
        }

        private BreakStm ReadBreak(XmlElement node)
        {
            if ((node != null) && (node.Name == "Break"))
            {
                BooleanStm on = null;

                XmlElement elm = ReadProperty(node, "On");
                if (elm != null)
                {
                    on = (BooleanStm)ReadStatement(elm);
                }

                return BreakStm.As(on);
            }

            return null;
        }

        private CallRulesetStm ReadCall(XmlElement node)
        {
            if ((node != null) && (node.Name == "Call"))
            {
                Statement ruleset = null;

                XmlElement elm = ReadProperty(node, "Ruleset");
                if (elm != null)
                {
                    ruleset = ReadStatement(elm);
                }

                return CallRulesetStm.As(ruleset);
            }

            return null;
        }

        private ConditionTestStm ReadTest(XmlElement node)
        {
            if ((node != null) && (node.Name == "Test"))
            {
                BooleanStm condition = null;
                Statement ifTrue = null;
                Statement ifFalse = null;

                XmlElement elm = ReadProperty(node, "Condition");
                if (elm != null)
                {
                    condition = (BooleanStm)ReadStatement(elm);
                }

                elm = ReadProperty(node, "IfTrue");
                if (elm != null)
                {
                    ifTrue = ReadStatement(elm);
                }

                elm = ReadProperty(node, "IfFalse");
                if (elm != null)
                {
                    ifFalse = ReadStatement(elm);
                }

                return ConditionTestStm.As(condition, ifTrue, ifFalse);
            }

            return null;
        }

        private ContinueStm ReadContinue(XmlElement node)
        {
            if ((node != null) && (node.Name == "Continue"))
            {
                BooleanStm on = null;

                XmlElement elm = ReadProperty(node, "On");
                if (elm != null)
                {
                    on = (BooleanStm)ReadStatement(elm);
                }

                return ContinueStm.As(on);
            }

            return null;
        }

        private DefineStm ReadDefine(XmlElement node)
        {
            if ((node != null) && (node.Name == "Define"))
            {
                StringStm name = null;

                XmlElement elm = ReadProperty(node, "Name");
                if (elm != null)
                {
                    name = (StringStm)ReadStatement(elm);
                }

                return DefineStm.As(name);
            }

            return null;
        }

        private EvaluateTableStm ReadEvaluateTable(XmlElement node)
        {
            if ((node != null) && (node.Name == "EvaluateTable"))
            {
                Statement name = null;

                XmlElement elm = ReadProperty(node, "Table");
                if (elm != null)
                {
                    name = ReadStatement(elm);
                }

                return EvaluateTableStm.As(name);
            }

            return null;
        }

        private EvaluateTreeStm ReadEvaluateTree(XmlElement node)
        {
            if ((node != null) && (node.Name == "EvaluateTree"))
            {
                Statement name = null;

                XmlElement elm = ReadProperty(node, "Tree");
                if (elm != null)
                {
                    name = ReadStatement(elm);
                }

                return EvaluateTreeStm.As(name);
            }

            return null;
        }

        private ForStm ReadFor(XmlElement node)
        {
            if ((node != null) && (node.Name == "For"))
            {
                StringStm variable = null;
                Statement from = null;
                Statement to = null;
                NumericStm step = null;

                XmlElement elm = ReadProperty(node, "Var");
                if (elm != null)
                {
                    variable = (StringStm)ReadStatement(elm);
                }

                elm = ReadProperty(node, "From");
                if (elm != null)
                {
                    from = ReadStatement(elm);
                }

                elm = ReadProperty(node, "To");
                if (elm != null)
                {
                    to = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Step");
                if (elm != null)
                {
                    step = (NumericStm)ReadStatement(elm);
                }

                ActionStm[] actions = new ActionStm[0];

                XmlElement[] elms = ReadParameters(node, "Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    actions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        actions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                ForStm result = ForStm.As(variable, from, to, step);
                result.Do(actions);

                return result;
            }

            return null;
        }

        private FunctionStm ReadFunction(XmlElement node)
        {
            if ((node != null) && (node.Name == "Function"))
            {
                StringStm name = null;
                Statement[] args = new Statement[0];

                XmlElement elm = ReadProperty(node, "Name");
                if (elm != null)
                {
                    name = (StringStm)ReadStatement(elm);
                }

                XmlElement[] elms = ReadParameters(node, "Parameters");
                if ((elms != null) && (elms.Length > 0))
                {
                    args = new Statement[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        args[i++] = ReadStatement(nd);
                    }
                }

                return FunctionStm.As(name, args);
            }

            return null;
        }

        private HaltStm ReadHalt(XmlElement node)
        {
            if ((node != null) && (node.Name == "Halt"))
            {
                BooleanStm on = null;

                XmlElement elm = ReadProperty(node, "On");
                if (elm != null)
                {
                    on = (BooleanStm)ReadStatement(elm);
                }

                return HaltStm.As(on);
            }

            return null;
        }

        private IfThenStm ReadIfThen(XmlElement node)
        {
            if ((node != null) && (node.Name == "If"))
            {
                BooleanStm cond = null;
                IfThenStm elseIf = null;

                ActionStm[] actions = new ActionStm[0];

                XmlElement elm = ReadProperty(node, "Condition");
                if (elm != null)
                {
                    cond = (BooleanStm)ReadStatement(elm);
                }

                XmlElement[] elms = ReadParameters(node, "Then/Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    actions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        actions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                IfThenStm result = IfThenStm.As(cond);
                result.Then(actions);

                elm = ReadProperty(node, "Else");
                if (elm != null)
                {
                    elseIf = ReadIfThen(elm);

                    result.ElseIf(elseIf);
                }

                return result;
            }

            return null;
        }

        private ItemOfStm ReadItemOf(XmlElement node)
        {
            if ((node != null) && (node.Name == "ItemOf"))
            {
                Statement instance = null;
                Statement index = null;
                Statement value = null;

                XmlElement elm = ReadProperty(node, "Instance");
                if (elm != null)
                {
                    instance = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Index");
                if (elm != null)
                {
                    index = ReadStatement(elm);
                }

                elm = ReadProperty(node, "SetValue");
                if (elm != null)
                {
                    value = ReadStatement(elm);
                }

                return ItemOfStm.As(instance, index, value);
            }

            return null;
        }

        private RaiseErrorStm ReadRaiseError(XmlElement node)
        {
            if ((node != null) && (node.Name == "Raise"))
            {
                BooleanStm on = null;
                Statement message = null;

                XmlElement elm = ReadProperty(node, "On");
                if (elm != null)
                {
                    on = (BooleanStm)ReadStatement(elm);
                }

                elm = ReadProperty(node, "Message");
                if (elm != null)
                {
                    message = ReadStatement(elm);
                }

                return RaiseErrorStm.As(message, on);
            }

            return null;
        }

        private ReflectionStm ReadReflection(XmlElement node)
        {
            if ((node != null) && (node.Name == "Reflection"))
            {
                Statement instance = null;
                Statement method = null;
                Statement[] args = new Statement[0];

                XmlElement elm = ReadProperty(node, "Instance");
                if (elm != null)
                {
                    instance = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Method");
                if (elm != null)
                {
                    method = ReadStatement(elm);
                }

                XmlElement[] elms = ReadParameters(node, "Parameters");
                if ((elms != null) && (elms.Length > 0))
                {
                    args = new Statement[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        args[i++] = ReadStatement(nd);
                    }
                }

                return ReflectionStm.As(instance, method, args);
            }

            return null;
        }

        private RepeatUntilStm ReadRepeatUntil(XmlElement node)
        {
            if ((node != null) && (node.Name == "Repeat"))
            {
                BooleanStm until = null;

                XmlElement elm = ReadProperty(node, "Until");
                if (elm != null)
                {
                    until = (BooleanStm)ReadStatement(elm);
                }

                ActionStm[] actions = new ActionStm[0];

                XmlElement[] elms = ReadParameters(node, "Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    actions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        actions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                RepeatUntilStm result = RepeatUntilStm.As(until);
                result.Do(actions);

                return result;
            }

            return null;
        }

        private ReturnStm ReadReturn(XmlElement node)
        {
            if ((node != null) && (node.Name == "Return"))
            {
                BooleanStm on = null;

                XmlElement elm = ReadProperty(node, "On");
                if (elm != null)
                {
                    on = (BooleanStm)ReadStatement(elm);
                }

                return ReturnStm.As(on);
            }

            return null;
        }

        private SetFactStm ReadSetFact(XmlElement node)
        {
            if ((node != null) && (node.Name == "SetFact"))
            {
                Statement fact = null;
                Statement value = null;

                XmlElement elm = ReadProperty(node, "Fact");
                if (elm != null)
                {
                    fact = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Value");
                if (elm != null)
                {
                    value = ReadStatement(elm);
                }

                return SetFactStm.As(fact, value);
            }

            return null;
        }

        private SwitchStm ReadSwitch(XmlElement node)
        {
            if ((node != null) && (node.Name == "Switch"))
            {
                Statement switchCase = null;
                ActionStm[] defActions = new ActionStm[0];

                XmlElement elm = ReadProperty(node, "SwitchCase");
                if (elm != null)
                {
                    switchCase = ReadStatement(elm);
                }

                XmlElement[] elms = ReadParameters(node, "Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    defActions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        defActions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                SwitchStm result = SwitchStm.As(switchCase);
                if (defActions.Length > 0)
                {
                    result.Default(defActions);
                }

                XmlNodeList cases = node.SelectNodes("Cases/Case");
                if (cases != null)
                {
                    foreach (XmlElement caseNode in cases)
                    {
                        Statement[] matches = new Statement[0];
                        ActionStm[] actions = new ActionStm[0];

                        XmlNodeList matchNodes = caseNode.SelectNodes("Match");
                        if (matchNodes != null)
                        {
                            matches = new Statement[matchNodes.Count];

                            int i = 0;
                            foreach (XmlNode matchNode in matchNodes)
                            {
                                XmlElement stmNode = null;
                                foreach (XmlNode nd in matchNode.ChildNodes)
                                {
                                    if (nd is XmlElement)
                                    {
                                        stmNode = (XmlElement)nd;
                                        break;
                                    }
                                }

                                matches[i++] = ReadStatement(stmNode);
                            }


                        }

                        elms = ReadParameters(node, "Do");
                        if ((elms != null) && (elms.Length > 0))
                        {
                            actions = new ActionStm[elms.Length];

                            int i = 0;
                            foreach (XmlElement nd in elms)
                            {
                                actions[i++] = (ActionStm)ReadStatement(nd);
                            }
                        }

                        result.Case(matches, actions);
                    }
                }

                return result;
            }

            return null;
        }

        private TryStm ReadTry(XmlElement node)
        {
            if ((node != null) && (node.Name == "Try"))
            {
                ActionStm[] actions = new ActionStm[0];

                XmlElement[] elms = ReadParameters(node, "Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    actions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        actions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                TryStm result = TryStm.As();
                result.Actions.AddRange(actions);

                XmlNodeList errors = node.SelectNodes("OnError");
                if (errors != null)
                {
                    foreach (XmlElement errNode in errors)
                    {
                        actions = new ActionStm[0];
                        StringStm error = null;

                        XmlElement elm = ReadProperty(errNode, "Error");
                        if (elm != null)
                        {
                            error = (StringStm)ReadStatement(elm);
                        }

                        elms = ReadParameters(errNode, "Do");
                        if ((elms != null) && (elms.Length > 0))
                        {
                            actions = new ActionStm[elms.Length];

                            int i = 0;
                            foreach (XmlElement nd in elms)
                            {
                                actions[i++] = (ActionStm)ReadStatement(nd);
                            }
                        }

                        result.OnError(error.Value, actions);
                    }
                }

                XmlElement elm2 = ReadProperty(node, "Finally");
                if (elm2 != null)
                {
                    actions = new ActionStm[0];

                    elms = ReadParameters(node, "Do");
                    if ((elms != null) && (elms.Length > 0))
                    {
                        actions = new ActionStm[elms.Length];

                        int i = 0;
                        foreach (XmlElement nd in elms)
                        {
                            actions[i++] = (ActionStm)ReadStatement(nd);
                        }
                    }

                    result.FinallyDo(actions);
                }

                return result;
            }

            return null;
        }

        private WhileStm ReadWhile(XmlElement node)
        {
            if ((node != null) && (node.Name == "Repeat"))
            {
                BooleanStm cond = null;

                XmlElement elm = ReadProperty(node, "Condition");
                if (elm != null)
                {
                    cond = (BooleanStm)ReadStatement(elm);
                }

                ActionStm[] actions = new ActionStm[0];

                XmlElement[] elms = ReadParameters(node, "Do");
                if ((elms != null) && (elms.Length > 0))
                {
                    actions = new ActionStm[elms.Length];

                    int i = 0;
                    foreach (XmlElement nd in elms)
                    {
                        actions[i++] = (ActionStm)ReadStatement(nd);
                    }
                }

                WhileStm result = WhileStm.As(cond);
                result.Do(actions);

                return result;
            }

            return null;
        }

        private Statement ReadActionStmA2H(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "Break":
                    return ReadBreak(node);

                case "Call":
                    return ReadCall(node);

                case "Continue":
                    return ReadContinue(node);

                case "Define":
                    return ReadDefine(node);

                case "EvaluateTable":
                    return ReadEvaluateTable(node);

                case "EvaluateTree":
                    return ReadEvaluateTree(node);

                case "For":
                    return ReadFor(node);

                case "Function":
                    return ReadFunction(node);

                case "Halt":
                    return ReadHalt(node);
            }

            return null;
        }

        private Statement ReadActionStmI2Z(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "If":
                    return ReadIfThen(node);

                case "ItemOf":
                    return ReadItemOf(node);

                case "Raise":
                    return ReadRaiseError(node);

                case "Reflection":
                    return ReadReflection(node);

                case "Repeat":
                    return ReadRepeatUntil(node);

                case "Return":
                    return ReadReturn(node);

                case "SetFact":
                    return ReadSetFact(node);

                case "SetVariable":
                    return ReadSetVariable(node);

                case "Switch":
                    return ReadSwitch(node);

                case "Try":
                    return ReadTry(node);

                case "While":
                    return ReadWhile(node);

                case "Test":
                    return ReadTest(node);
            }

            return null;
        }

        private Statement ReadActionStm(XmlElement node)
        {
            Statement result = ReadActionStmA2H(node);
            if (!ReferenceEquals(result, null))
            {
                return result;
            }

            return ReadActionStmI2Z(node);
        }

        # endregion

        # region Value statements

        private ArithmeticGroupStm ReadArithmeticGroup(XmlElement node)
        {
            if ((node != null) && (node.Name == "ArithmeticGroup"))
            {
                XmlElement rootNode = null;
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd is XmlElement)
                    {
                        rootNode = (XmlElement)nd;
                        break;
                    }
                }

                Statement root = null;
                if (rootNode != null)
                {
                    root = ReadStatement(rootNode);
                }

                return ArithmeticGroupStm.As(root);
            }

            return null;
        }

        private BetweenStm ReadBetween(XmlElement node)
        {
            if ((node != null) && (node.Name == "Between"))
            {
                Statement target = null;
                Statement min = null;
                Statement max = null;

                XmlElement elm = ReadProperty(node, "Target");
                if (elm != null)
                {
                    target = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Min");
                if (elm != null)
                {
                    min = ReadStatement(elm);
                }

                elm = ReadProperty(node, "Max");
                if (elm != null)
                {
                    max = ReadStatement(elm);
                }

                return BetweenStm.As(target, min, max);
            }

            return null;
        }

        private ContextStm ReadContext(XmlElement node)
        {
            if ((node != null) && (node.Name == "Context"))
            {
                return ContextStm.As();
            }

            return null;
        }

        private DateStm ReadDate(XmlElement node)
        {
            if ((node != null) && (node.Name == "Date"))
            {
                DateTime value = StmCommon.ToDate(node.InnerText);
                return DateStm.As(value);
            }

            return null;
        }

        private FactStm ReadFact(XmlElement node)
        {
            if ((node != null) && (node.Name == "Fact"))
            {
                StringStm name = null;

                XmlElement elm = ReadProperty(node, "Name");
                if (elm != null)
                {
                    name = (StringStm)ReadStatement(elm);
                }

                return FactStm.As(name);
            }

            return null;
        }

        private FalseStm ReadFalse(XmlElement node)
        {
            if ((node != null) && (node.Name == "False"))
            {
                return (FalseStm)BooleanStm.False;
            }

            return null;
        }

        private NumericStm ReadNumeric(XmlElement node)
        {
            if ((node != null) && (node.Name == "Numeric"))
            {
                double value = StmCommon.ToDouble(node.InnerText);
                return NumericStm.As(value);
            }

            return null;
        }

        private StringStm ReadString(XmlElement node)
        {
            if ((node != null) && (node.Name == "String"))
            {
                string value = null;
                if (String.Compare(node.GetAttribute(StrNil), StrTrue, true) != 0)
                {
                    value = node.InnerText;
                }

                return StringStm.As(value);
            }

            return null;
        }

        private TimeStm ReadTime(XmlElement node)
        {
            if ((node != null) && (node.Name == "Time"))
            {
                TimeSpan value = StmCommon.ToTime(node.InnerText);
                return TimeStm.As(value);
            }

            return null;
        }

        private string ReadText(XmlElement node, string propertyName)
        {
            if (node != null)
            {
                XmlElement elm = ReadProperty(node, propertyName);
                if (elm != null)
                {
                    return CommonHelper.TrimString(elm.InnerText);
                }
            }

            return null;
        }

        private TrueStm ReadTrue(XmlElement node)
        {
            if ((node != null) && (node.Name == "True"))
            {
                return (TrueStm)BooleanStm.True;
            }

            return null;
        }

        private UnaryNotStm ReadUnaryNot(XmlElement node)
        {
            if ((node != null) && (node.Name == "Not"))
            {
                BooleanStm target = null;
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd is XmlElement)
                    {
                        target = (BooleanStm)ReadStatement((XmlElement)nd);
                        break;
                    }
                }

                return UnaryNotStm.As(target);
            }

            return null;
        }

        private VariableStm ReadVariable(XmlElement node)
        {
            if ((node != null) && (node.Name == "Variable"))
            {
                StringStm name = null;

                XmlElement elm = ReadProperty(node, "Name");
                if (elm != null)
                {
                    name = (StringStm)ReadStatement(elm);
                }

                return VariableStm.As(name);
            }

            return null;
        }

        private Statement ReadValueStm(XmlElement node)
        {
            string type = node.Name;
            switch (type)
            {
                case "ArithmeticGroup":
                    return ReadArithmeticGroup(node);

                case "Between":
                    return ReadBetween(node);

                case "Context":
                    return ReadContext(node);

                case "Date":
                    return ReadDate(node);

                case "Fact":
                    return ReadFact(node);

                case "False":
                    return ReadFalse(node);

                case "Numeric":
                    return ReadNumeric(node);

                case "String":
                    return ReadString(node);

                case "Time":
                    return ReadTime(node);

                case "True":
                    return ReadTrue(node);

                case "Not":
                    return ReadUnaryNot(node);

                case "Variable":
                    return ReadVariable(node);
            }

            return null;
        }

        # endregion

        # region Other statements

        private LogicalGroupStm ReadLogicalGroup(XmlElement node)
        {
            if ((node != null) && (node.Name == "LogicalGroup"))
            {
                XmlElement rootNode = null;
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd is XmlElement)
                    {
                        rootNode = (XmlElement)nd;
                        break;
                    }
                }

                BooleanStm root = null;
                if (rootNode != null)
                {
                    root = (BooleanStm)ReadStatement(rootNode);
                }

                return LogicalGroupStm.As(root);
            }

            return null;
        }

        private Statement ReadNil(XmlElement node)
        {
            if ((node != null) && (node.Name == "Nil"))
            {
                return (NullStm)Statement.Null;
            }

            return null;
        }

        private NullStm ReadNull(XmlElement node)
        {
            if ((node != null) && (node.Name == "Null"))
            {
                return (NullStm)Statement.Null;
            }

            return null;
        }

        private SetStm ReadSet(XmlElement node)
        {
            if ((node != null) && (node.Name == "Set"))
            {
                List<Statement> list = new List<Statement>();
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd is XmlElement)
                    {
                        list.Add(ReadStatement((XmlElement)nd));
                    }
                }

                return SetStm.As(list.ToArray());
            }

            return null;
        }

        private UnaryMinusStm ReadUnaryMinus(XmlElement node)
        {
            if ((node != null) && (node.Name == "Minus"))
            {
                Statement target = null;
                foreach (XmlNode nd in node.ChildNodes)
                {
                    if (nd is XmlElement)
                    {
                        target = ReadStatement((XmlElement)nd);
                    }
                }

                return UnaryMinusStm.As(target);
            }

            return null;
        }

        # endregion

        # region Statement type decision

        private Statement ReadStatement(XmlElement node)
        {
            if (!ReferenceEquals(node, null))
            {
                Statement result;

                result = ReadNil(node);
                if (ReferenceEquals(result, Statement.Null))
                {
                    return null;
                }

                result = ReadNull(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadLogicalGroup(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadUnaryMinus(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadSet(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadArithmeticStm(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadLogicalStm(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadComparatorStm(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadActionStm(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                result = ReadValueStm(node);
                if (!ReferenceEquals(result, null))
                {
                    return result;
                }

                string type = (node != null) ? node.Name : "Empty";
                throw new RuleException(String.Format(BreResStrings.GetString("IsNotAKnownTypeByReader"), type));
            }

            return null;
        }
        
        # endregion
    }
}
