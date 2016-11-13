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

using Sweet.BRE;

namespace Sweet.BRETest
{
    class Program
    {
        static void Main(string[] args)
        {
            // SaveTest();
            FahrenheitToCelciusTest();
            // CelciusToFahrenheitTest();
            // IndexTest();
            // ReflectionTest();
            // GeneralTest();
            // TryCatchTest();
        }

        private static void IndexTest()
        {
            Project project = Project.As();

            FactList facts = new FactList();
            List<object> list = new List<object>(new object[] { 1, "a", DateTime.Now, double.NaN });

            facts.Set("fact1", list);
            facts.Set("celcius", 18);

            project.RegisterFunctionAlias("Month name", "MonthName");
            project.RegisterFunctionAlias("Convert to string", "String");
            project.RegisterFunctionAlias("Write to console", "WriteLn");

            Ruleset ruleset = project.DefineRuleset("main");

            ruleset.DefineRule("2").Do(
                    ((FunctionStm)"Write to console")
                        .Params(
                            ((FunctionStm)"Round")
                                .Params(
                                    DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                    2,
                                    "awayFromZero"
                                )
                        )
                )
                .SetSubPriority(46)
                ;

            ruleset.DefineRule("1").Do(
                    ((FunctionStm)"Write to console")
                        .Params(
                            ((FunctionStm)"Month name")
                                .Params(
                                    ((FunctionStm)"Convert to string")
                                        .Params(
                                            ItemOfStm.As((FactStm)"fact1", 2)
                                        )
                                    , "nl"
                                    )
                        )
                )
                ;

            DefaultRuleDebugger debugger = new DefaultRuleDebugger();
            debugger.OnDebug += delegate(object sender, DebugEventArgs e)
            {
                Console.WriteLine(e.Status);
                if (e.Error != null)
                {
                    Console.WriteLine(e.Error);
                }
            };

            RuleEngineRuntime.RegisterDebugger(debugger);

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project);
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));
                ec.StopOnError = false;

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(facts, null);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");


                Console.WriteLine("Eval time: {0} ms\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void CelciusToFahrenheitTest()
        {
            Project project = Project.As();

            FactList facts = new FactList();

            facts.Set("celcius", 18);

            Ruleset ruleset = project.DefineRuleset("main");

            ruleset.DefineRule("1").Do(
                    ((FunctionStm)"Print")
                        .Params(
                            ((FunctionStm)"Round")
                                .Params(
                                    DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                    2,
                                    "awayFromZero"
                                )
                        )
                )
                ;

            ruleset.DefineRule("2").Do(
                    ((FunctionStm)"Print")
                        .Params(
                            ((FunctionStm)"Round")
                                .Params(
                                    DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                    2,
                                    "awayFromZero"
                                )
                        )
                )
                ;

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(facts, null);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");


                Console.WriteLine("Eval time: {0}\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void FahrenheitToCelciusTest()
        {
            Project project = Project.As();

            FactList facts = new FactList();

            facts.Set("fahrenheit", 64.4);

            project.DefineRuleset("main")
                .DefineRule("main")
                .Do(
                    ((FunctionStm)"Print")
                        .Params(
                            ((FunctionStm)"Round")
                                .Params(
                                    MultiplyStm.As((FactStm)"fahrenheit" - 32, 5) / 9,
                                    2,
                                    "awayFromZero"
                                )
                        )
                )
                ;

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(facts, null);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");


                Console.WriteLine("Eval time: {0}\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void ReflectionTest()
        {
            Project project = Project.As();

            FactList facts = new FactList();

            List<object> list = new List<object>(new object[] { 1, "a", DateTime.Now, double.NaN });

            facts.Set("fact1", list);
            facts.Set("fact2", new char[] {' '});

            project.DefineRuleset("main")
                .DefineRule("main")
                .Do(
                    ((FunctionStm)"Print")
                        .Params(
                            ReflectionStm.As((FactStm)"fact1", 
                                "[2].ToString('s').Replace('T', ' ').Split($0)[1].ToCharArray()[2]",
                                (FactStm)"fact2")
                        )
                )
                ;

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(facts, null);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");


                Console.WriteLine("Eval time: {0}\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void TryCatchTest()
        {
            Project project = Project.As();

            FactList facts = new FactList();

            facts.Set("fact1", 7);
            facts.Set("project", project);
            facts.Set("celcius", 18);

            Ruleset ruleset = project.DefineRuleset("main");

            ruleset.DefineRule("CelciusToFahrenheit")
                .Do(
                    ((FunctionStm)"Print")
                        .Params(
                            ((FunctionStm)"Round")
                                .Params(
                                    DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                    2,
                                    "awayFromZero"
                                )
                        )
                )
                ;

            ruleset.DefineRule("TryCatch")
                .Do(
                    TryStm.As()
                        .Do(
                            ((FunctionStm)"Print")
                                .Params(
                                    (StringStm)"Fact1: " + ReflectionStm.As((FactStm)"fact1", "ToString()")
                                ),
                            RaiseErrorStm.As("test")
                        )
                        .OnError(
                            ((FunctionStm)"Print")
                                .Params(
                                    "Status: " + ReflectionStm.As(Statement.Null, "Status")
                                ),
                            SetFactStm.As("error")
                                .Set(
                                    ReflectionStm.As(ContextStm.As(), "GetLastError()")
                                ),
                            ((FunctionStm)"Print")
                                .Params(
                                    ReflectionStm.As((FactStm)"error", (string)null)
                                )
                        )
                    );

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(facts, null);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");


                Console.WriteLine("Eval time: {0}\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void GeneralTest()
        {
            Project project = Project.As();

            // table
            DecisionTable table = project.DefineTable("decisionTable1");

            table.Conditions.Add("@var1", DecisionValueType.Integer);
            table.Conditions.Add("@var2", DecisionValueType.Integer);
            table.Conditions.Add("@var3", DecisionValueType.Integer);

            table.Actions.Add("@var1", DecisionValueType.Integer);
            table.Actions.Add("@var2", DecisionValueType.Integer);
            table.Actions.Add("@var3", DecisionValueType.Integer);

            table.Rows.Add(new string[] { "0", "0", "0" }, new string[] { "0", "0", "1" });
            table.Rows.Add(new string[] { "0", "0", "1" }, new string[] { "0", "0", "2" });
            table.Rows.Add(new string[] { "0", "0", "2" }, new string[] { "0", "1", "0" });
            table.Rows.Add(new string[] { "0", "1", "0" }, new string[] { "0", "1", "1" });
            table.Rows.Add(new string[] { "0", "1", "1" }, new string[] { "0", "1", "2" });
            table.Rows.Add(new string[] { "0", "1", "2" }, new string[] { "0", "2", "0" });
            table.Rows.Add(new string[] { "0", "2", "0" }, new string[] { "0", "2", "1" });
            table.Rows.Add(new string[] { "0", "2", "1" }, new string[] { "0", "2", "2" });
            table.Rows.Add(new string[] { "0", "2", "2" }, new string[] { "1", "0", "0" });

            table.Rows.Add(new string[] { "1", "0", "0" }, new string[] { "1", "0", "1" });
            table.Rows.Add(new string[] { "1", "0", "1" }, new string[] { "1", "0", "2" });
            table.Rows.Add(new string[] { "1", "0", "2" }, new string[] { "1", "1", "0" });
            table.Rows.Add(new string[] { "1", "1", "0" }, new string[] { "1", "1", "1" });
            table.Rows.Add(new string[] { "1", "1", "1" }, new string[] { "1", "1", "2" });
            table.Rows.Add(new string[] { "1", "1", "2" }, new string[] { "1", "2", "0" });
            table.Rows.Add(new string[] { "1", "2", "0" }, new string[] { "1", "2", "1" });
            table.Rows.Add(new string[] { "1", "2", "1" }, new string[] { "1", "2", "2" });
            table.Rows.Add(new string[] { "1", "2", "2" }, new string[] { "2", "0", "0" });

            table.Rows.Add(new string[] { "2", "0", "0" }, new string[] { "2", "0", "1" });
            table.Rows.Add(new string[] { "2", "0", "1" }, new string[] { "2", "0", "2" });
            table.Rows.Add(new string[] { "2", "0", "2" }, new string[] { "2", "1", "0" });
            table.Rows.Add(new string[] { "2", "1", "0" }, new string[] { "2", "1", "1" });
            table.Rows.Add(new string[] { "2", "1", "1" }, new string[] { "2", "1", "2" });
            table.Rows.Add(new string[] { "2", "1", "2" }, new string[] { "2", "2", "0" });
            table.Rows.Add(new string[] { "2", "2", "0" }, new string[] { "2", "2", "1" });
            table.Rows.Add(new string[] { "2", "2", "1" }, new string[] { "2", "2", "2" });
            table.Rows.Add(new string[] { "2", "2", "2" }, new string[] { "0", "0", "0" });

            table.MergeCells();

            // tree
            // row1
            DecisionConditionNode tc1 = project.DefineTree("tree1");
            tc1.SetVariable("@var1").SetValues(new string[] { "0" });

            DecisionConditionNode tc2 = new DecisionConditionNode("@var2", new string[] { "0" });
            tc1.SetOnMatch(tc2);

            DecisionActionNode ta1 = new DecisionActionNode("@var1", "0");
            tc2.SetOnMatch(ta1);

            ta1.SetFurtherMore(new DecisionActionNode("@var2", "1"));

            // row2
            DecisionConditionNode tc3 = new DecisionConditionNode("@var2", new string[] { "1" });
            tc2.SetElse(tc3);

            DecisionActionNode ta2 = new DecisionActionNode("@var1", "1");
            tc3.SetOnMatch(ta2);

            ta1.SetFurtherMore(new DecisionActionNode("@var2", "0"));

            // row3
            DecisionConditionNode tc4 = new DecisionConditionNode("@var1", new string[] { "1" });
            tc1.SetElse(tc4);

            DecisionConditionNode tc5 = new DecisionConditionNode("@var2", new string[] { "0" });
            tc4.SetOnMatch(tc5);

            DecisionActionNode ta3 = new DecisionActionNode("@var1", "1");
            tc5.SetOnMatch(ta3);

            ta3.SetFurtherMore(new DecisionActionNode("@var2", "1"));

            // row4
            DecisionConditionNode tc6 = new DecisionConditionNode("@var2", new string[] { "1" });
            tc5.SetElse(tc6);

            DecisionActionNode ta4 = new DecisionActionNode("@var1", "0");
            tc6.SetOnMatch(ta4);

            ta4.SetFurtherMore(new DecisionActionNode("@var2", "0"));

            // ruleset
            project.DefineRuleset("main")
                .DefineRule("main")
                .Do(
                    ForStm.As("i", 1000)
                    .Do(
                        EvaluateTableStm.As("DecisionTable1")
                    )
                    .Do(
                        EvaluateTreeStm.As("tree1")
                    )
                    .Do(
                        ((FunctionStm)"Print")
                            .Params(
                                    ArithmeticGroupStm.As((StringStm)"@var1: ")
                                        .Add((VariableStm)"@var1")
                                        .Add((StringStm)", @var2: ")
                                        .Add((VariableStm)"@var2")
                                        .Add((StringStm)", @var3: ")
                                        .Add((VariableStm)"@var3")
                                   )
                    )
                    .Do(
                        ContinueStm.As(
                            (VariableStm)"Count" >= 1000
                           )
                    )
                    .Do(
                        IfThenStm.As(
                            (VariableStm)"VarA" > 0
                           )
                        .Then(
                            ((SetVariableStm)"VarA")
                                .Set(
                                    ArithmeticGroupStm.As((VariableStm)"VarA")
                                        .Add(2)
                                        .Subtract(1)
                                    )
                             )
                    )
                    .Do(
                        IfThenStm.As(
                            (VariableStm)"VarA" > 10
                           )
                        .Then(
                            ((SetVariableStm)"VarA")
                                .Set(1)
                             )
                    )
                    .Do(
                        ((SetVariableStm)"Count")
                            .Set(
                                ArithmeticGroupStm.As((VariableStm)"Count").Add(1)
                                )
                        ,
                        ((FunctionStm)"Print")
                            .Params(
                                    "Count: " + (VariableStm)"Count"
                                   )
                        ,
                        ((FunctionStm)"Print")
                            .Params(
                                    "VarA: " + (VariableStm)"VarA"
                                   )
                        ,
                        ((FunctionStm)"Print")
                            .Params(
                                    (DateStm)"12.10.2005" + (TimeStm)"11.00:00:00" + (VariableStm)"Count"
                                    )
                        ,
                        ((FunctionStm)"Print")
                            .Params(
                                    ((FunctionStm)"AddDate")
                                        .Params(
                                                (DateStm)"12.10.2005",
                                                -2,
                                                -1,
                                                 3
                                               )
                                   )
                    )
                )
                ;

            IEvaluationContext ec = RuleEngineRuntime.Initialize(project.GetRuleset("main"));

            VariableList vars = new VariableList();

            vars.Set("VarA", 7);
            vars.Set("Count", 0);

            vars.Set("@var1", 0);
            vars.Set("@var2", 0);
            vars.Set("@var3", 0);

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                double evalTime = 0;
                DateTime dt1 = DateTime.Now;

                try
                {
                    ec.Evaluate(vars);
                    vars.Copy(ec.Variables);
                }
                finally
                {
                    evalTime = (DateTime.Now - dt1).TotalMilliseconds;
                }

                Console.WriteLine();
                Console.WriteLine("--------------------------------");

                foreach (IVariable obj in ec.Variables)
                {
                    Console.WriteLine("{0} = {1}", obj.Name, obj.Value);
                }

                Console.WriteLine();
                Console.WriteLine("Eval time: {0}\r\n", evalTime);

                Console.WriteLine("--------------------------------");
                Console.WriteLine();

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void SaveTest()
        {
            Project project = Project.As();

            // Variables
            project.Variables.Set("@var1", (string)null);
            project.Variables.Set("@var2", 1);
            project.Variables.Set("@bool", true);

            // Function aliases
            project.RegisterFunctionAlias("Month name", "MonthName");
            project.RegisterFunctionAlias("Convert to string", "String");
            project.RegisterFunctionAlias("Write to console", "PrintLine");

            // table
            DecisionTable table = project.DefineTable("decisionTable1");

            table.Conditions.Add("@var1", DecisionValueType.Integer);
            table.Conditions.Add("@var2", DecisionValueType.Integer);
            table.Conditions.Add("@var3", DecisionValueType.Integer);

            table.Actions.Add("@var1", DecisionValueType.Integer);
            table.Actions.Add("@var2", DecisionValueType.Integer);
            table.Actions.Add("@var3", DecisionValueType.Integer);

            table.Rows.Add(new string[] { "0", "0", "0" }, new string[] { "0", "0", "1" });
            table.Rows.Add(new string[] { "0", "0", "1" }, new string[] { "0", "0", "2" });
            table.Rows.Add(new string[] { "0", "0", "2" }, new string[] { "0", "1", "0" });
            table.Rows.Add(new string[] { "0", "1", "0" }, new string[] { "0", "1", "1" });
            table.Rows.Add(new string[] { "0", "1", "1" }, new string[] { "0", "1", "2" });
            table.Rows.Add(new string[] { "0", "1", "2" }, new string[] { "0", "2", "0" });
            table.Rows.Add(new string[] { "0", "2", "0" }, new string[] { "0", "2", "1" });
            table.Rows.Add(new string[] { "0", "2", "1" }, new string[] { "0", "2", "2" });
            table.Rows.Add(new string[] { "0", "2", "2" }, new string[] { "1", "0", "0" });

            table.Rows.Add(new string[] { "1", "0", "0" }, new string[] { "1", "0", "1" });
            table.Rows.Add(new string[] { "1", "0", "1" }, new string[] { "1", "0", "2" });
            table.Rows.Add(new string[] { "1", "0", "2" }, new string[] { "1", "1", "0" });
            table.Rows.Add(new string[] { "1", "1", "0" }, new string[] { "1", "1", "1" });
            table.Rows.Add(new string[] { "1", "1", "1" }, new string[] { "1", "1", "2" });
            table.Rows.Add(new string[] { "1", "1", "2" }, new string[] { "1", "2", "0" });
            table.Rows.Add(new string[] { "1", "2", "0" }, new string[] { "1", "2", "1" });
            table.Rows.Add(new string[] { "1", "2", "1" }, new string[] { "1", "2", "2" });
            table.Rows.Add(new string[] { "1", "2", "2" }, new string[] { "2", "0", "0" });

            table.Rows.Add(new string[] { "2", "0", "0" }, new string[] { "2", "0", "1" });
            table.Rows.Add(new string[] { "2", "0", "1" }, new string[] { "2", "0", "2" });
            table.Rows.Add(new string[] { "2", "0", "2" }, new string[] { "2", "1", "0" });
            table.Rows.Add(new string[] { "2", "1", "0" }, new string[] { "2", "1", "1" });
            table.Rows.Add(new string[] { "2", "1", "1" }, new string[] { "2", "1", "2" });
            table.Rows.Add(new string[] { "2", "1", "2" }, new string[] { "2", "2", "0" });
            table.Rows.Add(new string[] { "2", "2", "0" }, new string[] { "2", "2", "1" });
            table.Rows.Add(new string[] { "2", "2", "1" }, new string[] { "2", "2", "2" });
            table.Rows.Add(new string[] { "2", "2", "2" }, new string[] { "0", "0", "0" });

            table.MergeCells();

            // tree
            // row1
            DecisionConditionNode tc1 = project.DefineTree("tree1");
            tc1.SetVariable("@var1").SetValues(new string[] {"0"});

            DecisionConditionNode tc2 = new DecisionConditionNode("@var2", new string[] { "0"});
            tc1.SetOnMatch(tc2);

            DecisionActionNode ta1 = new DecisionActionNode("@var1", "0");
            tc2.SetOnMatch(ta1);

            ta1.SetFurtherMore(new DecisionActionNode("@var2", "1"));

            // row2
            DecisionConditionNode tc3 = new DecisionConditionNode("@var2", new string[] { "1" });
            tc2.SetElse(tc3);

            DecisionActionNode ta2 = new DecisionActionNode("@var1", "1");
            tc3.SetOnMatch(ta2);

            ta1.SetFurtherMore(new DecisionActionNode("@var2", "0"));

            // row3
            DecisionConditionNode tc4 = new DecisionConditionNode("@var1", new string[] { "1" });
            tc1.SetElse(tc4);

            DecisionConditionNode tc5 = new DecisionConditionNode("@var2", new string[] { "0" });
            tc4.SetOnMatch(tc5);

            DecisionActionNode ta3 = new DecisionActionNode("@var1", "1");
            tc5.SetOnMatch(ta3);

            ta3.SetFurtherMore(new DecisionActionNode("@var2", "1"));

            // row4
            DecisionConditionNode tc6 = new DecisionConditionNode("@var2", new string[] { "1" });
            tc5.SetElse(tc6);

            DecisionActionNode ta4 = new DecisionActionNode("@var1", "0");
            tc6.SetOnMatch(ta4);

            ta4.SetFurtherMore(new DecisionActionNode("@var2", "0"));

            // ruleset
            project.DefineRuleset("main")
                .DefineRule("main")
                .Do(
                    TryStm.As()
                        .Do(
                            ((FunctionStm)"Write to console")
                                .Params(
                                    ReflectionStm.As((FactStm)"fact1",
                                        "[2].ToString('s').Replace('T', ' ').Split($0)[1].ToCharArray()[2]",
                                        (FactStm)"fact2")
                                )
                        )
                        .Do(
                            ((FunctionStm)"Print")
                                .Params(
                                    ReflectionStm.As((FactStm)"fact1", "ToString()")
                                    )
                            , RaiseErrorStm.As("test")
                        )
                        .OnError(
                            SetFactStm.As("error").Set(
                                ReflectionStm.As(ContextStm.As(), "GetLastError()")
                            )
                            , ((FunctionStm)"PrintLine")
                                .Params(
                                    (StringStm)"Error: " + ReflectionStm.As((FactStm)"error", ".")
                                    )
                            )
                )
                .Do(
                    ((FunctionStm)"PrintLn")
                        .Params(
                            ItemOfStm.As((FactStm)"fact1", 2)
                        )
                )
                .Do(
                    ForStm.As("i", 50)
                    .Do(
                        EvaluateTableStm.As("DecisionTable1")
                    )
                    .Do(
                        ((FunctionStm)"WriteLine")
                            .Params(
                                    ArithmeticGroupStm.As((StringStm)"@var1: ")
                                        .Add((VariableStm)"@var1")
                                        .Add((StringStm)", @var2: ")
                                        .Add((VariableStm)"@var2")
                                   )
                    )
                    .Do(
                        ContinueStm.As(
                            (VariableStm)"Count" >= 1000
                           )
                    )
                    .Do(
                        IfThenStm.As(
                            (VariableStm)"VarA" > 0
                           )
                        .Then(
                            ((SetVariableStm)"VarA")
                                .Set(
                                    ArithmeticGroupStm.As((VariableStm)"VarA")
                                        .Add(2)
                                        .Subtract(1)
                                    )
                             )
                    )
                    .Do(
                        IfThenStm.As(
                            (VariableStm)"VarA" > 10
                           )
                        .Then(
                            ((SetVariableStm)"VarA")
                                .Set(1)
                             )
                    )
                    .Do(
                        ((SetVariableStm)"Count")
                            .Set(
                                ArithmeticGroupStm.As((VariableStm)"Count").Add(1)
                                )
                        ,
                        ((FunctionStm)"WriteLn")
                            .Params(
                                    "Count: " + (VariableStm)"Count"
                                   )
                        ,
                        ((FunctionStm)"PrintLn")
                            .Params(
                                    "VarA: " + (VariableStm)"VarA"
                                   )
                        ,
                        ((FunctionStm)"PrintLn")
                            .Params(
                                    (DateStm)"12.10.2005" + (TimeStm)"11.00:00:00" + (VariableStm)"Count"
                                    )
                        ,
                        ((FunctionStm)"PrintLn")
                            .Params(
                                    ((FunctionStm)"AddDate")
                                        .Params(
                                                (DateStm)"12.10.2005",
                                                -2,
                                                -1,
                                                 3
                                               )
                                   )
                    )
                )
                ;

            while (true)
            {
                Console.Clear();

                Console.WriteLine(project.ToString());
                Console.WriteLine();
                Console.WriteLine("-----------------");

                double evalTime1 = 0;
                DateTime dt1 = DateTime.Now;

                XmlDocument doc = null;
                try
                {
                    doc = ProjectWriter.Write(project);
                }
                finally
                {
                    evalTime1 = (DateTime.Now - dt1).TotalMilliseconds;
                    doc.Save(@"c:\a.xml");
                }

                double evalTime2 = 0;
                DateTime dt2 = DateTime.Now;

                Project prj = null;
                try
                {
                    prj = ProjectReader.Read(doc);
                }
                finally
                {
                    evalTime2 = (DateTime.Now - dt2).TotalMilliseconds;
                    Console.WriteLine(prj.ToString());
                }

                Console.WriteLine();
                Console.WriteLine("-----------------");

                Console.WriteLine("Equal: {0}", prj.IsEqualTo(project).ToString()); // (prj.ToString() == project.ToString()).ToString());
                Console.WriteLine("Eval time 1: {0}", evalTime1);
                Console.WriteLine("Eval time 2: {0}", evalTime2);

                Console.WriteLine("Press ESC to exit, any key to continue.");
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            }
        }
    }
}
