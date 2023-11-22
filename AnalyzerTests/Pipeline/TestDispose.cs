﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Analyzer.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analyzer.Parsing;

namespace Analyzer.Pipeline.Tests
{
    [TestClass()]
    public class TestDispose
    {
        [TestMethod()]
        public void Test1()
        {
            // Specify the path to the DLL file
            string path = "..\\..\\..\\..\\Analyzer\\TestDLLs\\disposetest.dll";

            // Create a list of DLL paths
            ParsedDLLFile dllFile = new(path);

            //DllFilePaths.Add(path);

            List<ParsedDLLFile> dllFiles = new() { dllFile };

            // Create an instance of RemoveUnusedLocalVariablesRule
            DisposableFieldsShouldBeDisposedRule analyzer = new(dllFiles);

            // Run the analyzer
            Dictionary<string , AnalyzerResult> result = analyzer.AnalyzeAllDLLs();

            foreach (KeyValuePair<string , AnalyzerResult> dll in result)
            {
                Console.WriteLine(dll.Key);

                AnalyzerResult res = dll.Value;

                Console.WriteLine(res.AnalyserID + " " + res.Verdict + " " + res.ErrorMessage);
            }

        }
    }
}
