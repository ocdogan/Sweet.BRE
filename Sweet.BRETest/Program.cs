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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using Sweet.BRE;

namespace Sweet.BRETest
{
    class Program
    {
        private delegate void RunTestMethod(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger);

        private delegate IProject ProjectMethod();

        private class Test
        {
            public string Name;
            public RunTestMethod RunMethod;
            public ProjectMethod ProjectMethod;
        }

        private class NativeTest
        {
            public string Name;
            public Action RunMethod;
        }

        private static List<Test> testMethods = new List<Test>();
        private static List<NativeTest> nativeTestMethods = new List<NativeTest>();

        private static ConsoleKey GetTestKey()
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    return key;
                }

                int testNo = (int)key;
                if (testNo >= (int)ConsoleKey.D1 && testNo <= ((int)ConsoleKey.D0 + testMethods.Count))
                {
                    testNo -= (int)ConsoleKey.D1;
                    return (ConsoleKey)testNo;
                }

                if (testNo >= (int)ConsoleKey.NumPad1 && testNo <= ((int)ConsoleKey.NumPad0 + testMethods.Count))
                {
                    testNo -= (int)ConsoleKey.NumPad1;
                    return (ConsoleKey)testNo;
                }
            }
        }

        private static ConsoleKey GetNativeTestKey()
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    return key;
                }

                int testNo = (int)key;
                if (testNo >= (int)ConsoleKey.D1 && testNo <= ((int)ConsoleKey.D0 + nativeTestMethods.Count))
                {
                    testNo -= (int)ConsoleKey.D1;
                    return (ConsoleKey)testNo;
                }

                if (testNo >= (int)ConsoleKey.NumPad1 && testNo <= ((int)ConsoleKey.NumPad0 + nativeTestMethods.Count))
                {
                    testNo -= (int)ConsoleKey.NumPad1;
                    return (ConsoleKey)testNo;
                }
            }
        }

        private static ConsoleKey GetTypeKey()
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                {
                    return key;
                }

                int testNo = (int)key;
                if (testNo == (int)ConsoleKey.D1 || testNo == (int)ConsoleKey.D2 || testNo == (int)ConsoleKey.D3)
                {
                    testNo -= (int)ConsoleKey.D1;
                    return (ConsoleKey)testNo;
                }

                if (testNo == (int)ConsoleKey.NumPad1 || testNo == (int)ConsoleKey.NumPad2 || testNo == (int)ConsoleKey.NumPad3)
                {
                    testNo -= (int)ConsoleKey.NumPad1;
                    return (ConsoleKey)testNo;
                }
            }
        }

        static void Main(string[] args)
        {
            testMethods.Add(new Test { Name = "Fahrenheit to Celcius Test", RunMethod = FahrenheitToCelciusTest, ProjectMethod = FahrenheitToCelciusTestProject });
            testMethods.Add(new Test { Name = "Celcius to Fahrenheit Test", RunMethod = CelciusToFahrenheitTest, ProjectMethod = CelciusToFahrenheitTestProject });
            testMethods.Add(new Test { Name = "Index Test", RunMethod = IndexTest, ProjectMethod = IndexTestProject });
            testMethods.Add(new Test { Name = "Reflection Test", RunMethod = ReflectionTest, ProjectMethod = ReflectionTestProject });
            testMethods.Add(new Test { Name = "Try & Catch Test", RunMethod = TryCatchTest, ProjectMethod = TryCatchTestProject });
            testMethods.Add(new Test { Name = "General Test", RunMethod = GeneralTest, ProjectMethod = GeneralTestProject });
            testMethods.Add(new Test { Name = "Save Test", RunMethod = SaveTest, ProjectMethod = SaveTestProject });

            nativeTestMethods.Add(new NativeTest { Name = "Fahrenheit to Celcius Test", RunMethod = FahrenheitToCelciusNativeTest });
            nativeTestMethods.Add(new NativeTest { Name = "Celcius to Fahrenheit Test", RunMethod = CelciusToFahrenheitNativeTest });

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Select an action:");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("1. Run project");
                Console.WriteLine("2. Run native projects");
                Console.WriteLine("3. Print project");
                Console.WriteLine("--------------------------------");
                Console.WriteLine("Press ESC to exit or select the test to run...");
                Console.WriteLine();

                ConsoleKey key = GetTypeKey();
                if (key == ConsoleKey.Escape)
                    break;

                int testNo = (int)key;
                if (testNo == 0)
                {
                    RunTests();
                }
                else if (testNo == 1)
                {
                    RunNativeTests();
                }
                else
                {
                    PrintTests();
                }
            }
        }

        private static void RunNativeTests()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("RUN NATIVE");
                Console.WriteLine("--------------------------------");

                for (int i = 0; i < nativeTestMethods.Count; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, nativeTestMethods[i].Name);
                }

                Console.WriteLine("--------------------------------");
                Console.WriteLine("Press ESC to exit or select the test to run...");
                Console.WriteLine();

                ConsoleKey key = GetNativeTestKey();
                if (key == ConsoleKey.Escape)
                    break;

                int testNo = (int)key;

                Console.WriteLine("Test: " + nativeTestMethods[testNo].Name);
                Console.WriteLine("--------------------------------");

                try
                {
                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    try
                    {
                        nativeTestMethods[testNo].RunMethod();
                    }
                    finally
                    {
                        sw.Stop();
                        Console.WriteLine();
                    }

                    Console.WriteLine("Eval time: {0} ms", sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(e);
                }

                Console.WriteLine("--------------------------------");

                Console.WriteLine("Press ESC to exit, any key to continue...");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void RunTests()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("RUN");
                Console.WriteLine("--------------------------------");

                for (int i = 0; i < testMethods.Count; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, testMethods[i].Name);
                }

                Console.WriteLine("--------------------------------");
                Console.WriteLine("Press ESC to exit or select the test to run...");
                Console.WriteLine();

                ConsoleKey key = GetTestKey();
                if (key == ConsoleKey.Escape)
                    break;

                int testNo = (int)key;

                Console.WriteLine("Test: " + testMethods[testNo].Name);
                Console.WriteLine("--------------------------------");

                try
                {
                    IRuleset rs;
                    IFactList facts;
                    IVariableList vars;
                    IRuleDebugger debugger;

                    Stopwatch sw = new Stopwatch();

                    sw.Start();
                    try
                    {
                        testMethods[testNo].RunMethod(out rs, out facts, out vars, out debugger);
                    }
                    finally
                    {
                        sw.Stop();
                    }

                    if (!ReferenceEquals(rs, null))
                    {
                        using (IEvaluationContext ec = RuleEngineRuntime.Initialize((Ruleset)rs, debugger))
                        {
                            ec.StopOnError = true;
                            try
                            {
                                sw.Reset();
                                sw.Start();
                                ec.Evaluate(facts, vars);
                            }
                            finally
                            {
                                sw.Stop();
                                Console.WriteLine();
                            }
                        }
                    }
                    Console.WriteLine("Eval time: {0} ms", sw.Elapsed.TotalMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(e);
                }

                Console.WriteLine("--------------------------------");

                Console.WriteLine("Press ESC to exit, any key to continue...");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static void PrintTests()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("PRINT");
                Console.WriteLine("--------------------------------");

                for (int i = 0; i < testMethods.Count; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, testMethods[i].Name);
                }

                Console.WriteLine("--------------------------------");
                Console.WriteLine("Press ESC to exit or select the test to print...");
                Console.WriteLine();

                ConsoleKey key = GetTestKey();
                if (key == ConsoleKey.Escape)
                    break;

                int testNo = (int)key;

                Console.Clear();
                Console.WriteLine("Project: " + testMethods[testNo].Name);
                Console.WriteLine("--------------------------------");

                try
                {
                    IProject project = testMethods[testNo].ProjectMethod();

                    if (!ReferenceEquals(project, null))
                    {
                        Console.WriteLine(project);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine(e);
                }

                Console.WriteLine("--------------------------------");

                Console.WriteLine("Press ESC to exit, any key to continue...");
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    break;
            }
        }

        private static IProject IndexTestProject()
        {
            IProject project = Project.As()
                .RegisterFunctionAlias("Month name", "MonthName")
                .RegisterFunctionAlias("Convert to string", "String")
                .RegisterFunctionAlias("Write to console", "WriteLn");

            project
                .DefineRuleset("main")
                    .DefineRule("2").Do(
                        ((FunctionStm)"Write to console")
                            .Params(
                                Predef.Round()
                                    .Params(
                                        DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                        2,
                                        "awayFromZero"
                                    )
                            )
                    )
                    .SetSubPriority(46)
                .Ruleset
                    .DefineRule("1").Do(
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
            return project;
        }

        private static void IndexTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;

            List<object> list = new List<object>(new object[] { 1, "a", DateTime.Now, double.NaN });

            facts = new FactList()
                .Set("fact1", list)
                .Set("celcius", 18);

            debugger = new DefaultRuleDebugger(delegate (DebugEventArgs e)
            {
                Console.WriteLine(e.Status);
                if (e.Error != null)
                {
                    Console.WriteLine(e.Error);
                }
            });

            rs = IndexTestProject().GetRuleset("main");
        }

        private static void CelciusToFahrenheitNativeTest()
        {
            Dictionary<string, object> facts = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            facts["celcius"] = 18;
            facts["fahrenheit"] = 64;

            object obj;
            double celcius;
            double fahrenheit;

            facts.TryGetValue("celcius", out obj);

            celcius = Convert.ToDouble(obj);
            if (celcius == 18)
            {
                fahrenheit = Math.Round(((celcius * 9) / 5) + 32);
                Console.WriteLine(String.Format("Fahrenheit: ", fahrenheit));
            }

            facts.TryGetValue("fahrenheit", out obj);

            fahrenheit = Convert.ToDouble(obj);
            if (fahrenheit == 64)
            {
                celcius = Math.Round((fahrenheit - 32) * 5 / 9);
                Console.WriteLine(String.Format("Celcius: ", celcius));
            }
        }

        private static IProject CelciusToFahrenheitTestProject()
        {
            IProject project = Project.As();

            project
                .DefineRuleset("main")
                    .DefineRule("1")
                    .When((FactStm)"celcius" == 18)
                    .Do(
                        Predef.PrintLine()
                            .Params(
                                "Fahrenheit: ",
                                Predef.Round()
                                    .Params(
                                        DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                        2,
                                        "awayFromZero"
                                    )
                            )
                    )
                .Ruleset
                    .DefineRule("2")
                    .When((FactStm)"fahrenheit" == (NumericStm)64)
                    .Do(
                        Predef.PrintLine()
                            .Params(
                                "Celcius: ",
                                Predef.Round()
                                    .Params(
                                        MultiplyStm.As((FactStm)"fahrenheit" - 32, 5) / 9,
                                        2,
                                        "awayFromZero"
                                    )
                            )
                    )
                    ;
            return project;
        }

        private static void CelciusToFahrenheitTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;
            debugger = null;

            facts = new FactList()
                .Set("fahrenheit", 64)
                .Set("celcius", 18);

            rs = CelciusToFahrenheitTestProject().GetRuleset("main");
        }

        private static void FahrenheitToCelciusNativeTest()
        {
            Dictionary<string, object> facts = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            facts["fahrenheit"] = 64.4;

            object obj;
            facts.TryGetValue("fahrenheit", out obj);

            double fahrenheit = Convert.ToDouble(obj);
            double celcius = Math.Round((fahrenheit - 32) * 5 / 9);

            Console.WriteLine(celcius);
        }

        private static IProject FahrenheitToCelciusTestProject()
        {
            IProject project = Project.As();

            project
                .DefineRuleset("main")
                    .DefineRule("main")
                    .Do(
                        Predef.Print()
                            .Params(
                                Predef.Round()
                                    .Params(
                                        MultiplyStm.As((FactStm)"fahrenheit" - 32, 5) / 9,
                                        2,
                                        "awayFromZero"
                                    )
                            )
                    )
                    ;
            return project;
        }

        private static void FahrenheitToCelciusTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;
            debugger = null;

            facts = new FactList()
                .Set("fahrenheit", 64.4);

            rs = FahrenheitToCelciusTestProject().GetRuleset("main");
        }

        private static IProject ReflectionTestProject()
        {
            IProject project = Project.As();

            project
                .DefineRuleset("main")
                    .DefineRule("1")
                    .Do(
                        Predef.PrintLine()
                            .Params(
                                Predef.Format()
                                    .Params(
                                        (FactStm)"fact3",
                                        "s"
                                    )
                            )
                    )
                .Ruleset
                    .DefineRule("2")
                    .Do(
                        Predef.PrintLine()
                            .Params(
                                ReflectionStm.As((FactStm)"fact1",
                                    "[2].ToString('s').Replace('T', ' ').Split($0)[1].ToCharArray()[2]",
                                    (FactStm)"fact2")
                            )
                    )
                    ;
            return project;
        }

        private static void ReflectionTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;
            debugger = null;

            DateTime now = DateTime.Now;
            List<object> list = new List<object>(new object[] { 1, "a", now, double.NaN });

            facts = new FactList()
                .Set("fact1", list)
                .Set("fact2", new char[] { ' ' })
                .Set("fact3", now);

            rs = ReflectionTestProject().GetRuleset("main");
        }

        private static IProject TryCatchTestProject()
        {
            IProject project = Project.As();

            project
                .DefineRuleset("main")
                   .DefineRule("CelciusToFahrenheit")
                    .Do(
                        Predef.Print()
                            .Params(
                                Predef.Round()
                                    .Params(
                                        DivideStm.As((FactStm)"celcius" * 9, 5) + 32,
                                        2,
                                        "awayFromZero"
                                    )
                            )
                          )
                    .Ruleset
                    .DefineRule("TryCatch")
                    .Do(
                        TryStm.As()
                            .Do(
                                Predef.Print()
                                    .Params(
                                        (StringStm)"Fact1: " + ReflectionStm.As((FactStm)"fact1", "ToString()")
                                    ),
                                RaiseErrorStm.As("test")
                            )
                            .OnError(
                                Predef.Print()
                                    .Params(
                                        "Status: " + ReflectionStm.As(Statement.Null, "Status")
                                    ),
                                SetFactStm.As("error")
                                    .Set(
                                        ReflectionStm.As(ContextStm.As(), "GetLastError()")
                                    ),
                                Predef.Print()
                                    .Params(
                                        ReflectionStm.As((FactStm)"error", (string)null)
                                    )
                            )
                        );
            return project;
        }

        private static void TryCatchTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;
            debugger = null;

            IProject project = TryCatchTestProject();

            facts = new FactList()
                .Set("fact1", 7)
                .Set("project", project)
                .Set("celcius", 18);

            rs = project.GetRuleset("main");
        }

        private static IProject GeneralTestProject()
        {
            IProject project = Project.As();

            // table
            DecisionTable table = project.DefineDecisionTable("decisionTable1");

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
            DecisionConditionNode tc1 = project.DefineDecisionTree("tree1");
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
                        Predef.Printfln()
                            .Params(
                                    "@var1: {0}, @var2: {1}, @var3: {2}",
                                    (VariableStm)"@var1",
                                    (VariableStm)"@var2",
                                    (VariableStm)"@var3"
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
                        Predef.PrintLine()
                            .Params(
                                    "Count: " + (VariableStm)"Count"
                                   )
                        ,
                        Predef.PrintLine()
                            .Params(
                                    "VarA: " + (VariableStm)"VarA"
                                   )
                        ,
                        Predef.Printfln()
                            .Params(
                                    "Date: {0}, Count: {1}",
                                    Predef.AddTimeToDate()
                                        .Params(
                                            (DateStm)"12.10.2005",
                                            (TimeStm)"1.23:45:56"
                                            ),
                                    (VariableStm)"Count"
                                    )
                        ,
                        Predef.PrintLine()
                            .Params(
                                    "AddDate: " +
                                    Predef.AddDate()
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
            return project;
        }

        private static void GeneralTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            vars = null;
            facts = null;
            debugger = null;

            vars = new VariableList()
                .Set("VarA", 7)
                .Set("Count", 0)
                .Set("@var1", 0)
                .Set("@var2", 0)
                .Set("@var3", 0);

            rs = GeneralTestProject().GetRuleset("main");
        }

        private static IProject SaveTestProject()
        {
            IProject project = Project.As();

            // Variables
            project.Variables
                    .Set("@var1", (string)null)
                    .Set("@var2", 1)
                    .Set("@bool", true);

            // Function aliases
            project
                .RegisterFunctionAlias("Month name", "MonthName")
                .RegisterFunctionAlias("Convert to string", "String")
                .RegisterFunctionAlias("Write to console", "PrintLine");

            // table
            DecisionTable table = project.DefineDecisionTable("decisionTable1");

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
            DecisionConditionNode tc1 = project.DefineDecisionTree("tree1");
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
            project
                .DefineRuleset("main")
                .DefineRule("main")
                .When((VariableStm)"@var2" == 1)
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
                            Predef.Print()
                                .Params(
                                    ReflectionStm.As((FactStm)"fact1", "ToString()")
                                    )
                            , RaiseErrorStm.As("test")
                        )
                        .OnError(
                            SetFactStm.As("error").Set(
                                ReflectionStm.As(ContextStm.As(), "GetLastError()")
                            )
                            , Predef.PrintLine()
                                .Params(
                                    (StringStm)"Error: " + ReflectionStm.As((FactStm)"error", ".")
                                    )
                            )
                )
                .Do(
                    Predef.PrintLn()
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
                        Predef.WriteLine()
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
                        Predef.WriteLn()
                            .Params(
                                    "Count: " + (VariableStm)"Count"
                                   )
                        ,
                        Predef.PrintLn()
                            .Params(
                                    "VarA: " + (VariableStm)"VarA"
                                   )
                        ,
                        Predef.PrintLn()
                            .Params(
                                    (DateStm)"12.10.2005" + (TimeStm)"11.00:00:00" + (VariableStm)"Count"
                                    )
                        ,
                        Predef.PrintLn()
                            .Params(
                                    Predef.AddDate()
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
            return project;
        }

        private static void SaveTest(out IRuleset rs, out IFactList facts, out IVariableList vars,
                                         out IRuleDebugger debugger)
        {
            rs = null;
            vars = null;
            facts = null;
            debugger = null;

            IProject project = SaveTestProject();

            XmlDocument doc = null;
            try
            {
                doc = ProjectWriter.Write((Project)project);
            }
            finally
            {
                doc.Save("." + Path.DirectorySeparatorChar.ToString() + "a.xml");
            }

            Project prj = null;
            try
            {
                prj = ProjectReader.Read(doc);
            }
            finally
            {
                Console.WriteLine(prj);
            }
        }
    }
}
