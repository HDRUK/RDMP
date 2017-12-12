﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CatalogueLibrary.Repositories;
using NUnit.Framework;

namespace CatalogueLibraryTests.SourceCodeEvaluation.ClassFileEvaluation
{
    public class AllImportantClassesDocumented
    {
        private List<string> _csFilesList;
        private List<string> problems = new List<string>();
        private int commentedCount = 0;
        private bool strict = false;

        public void FindProblems(List<string> csFilesList)
        {
            _csFilesList = csFilesList;

            foreach (var f in _csFilesList)
            {
                var text = File.ReadAllText(f);

                int startAt = text.IndexOf("public class");
                if(startAt == -1)
                    startAt = text.IndexOf("public interface");

                if (startAt != -1)
                {
                    var beforeDeclaration = text.Substring(0, startAt);

                    var mNamespace = Regex.Match(beforeDeclaration, "namespace (.*)");

                    if(!mNamespace.Success)
                        Assert.Fail("No namespace found in class file " + f);//no namespace in class!
                    
                    var nameSpace= mNamespace.Groups[1].Value;

                    //skip tests
                    if (nameSpace.Contains("Tests"))
                        continue;
                    
                    //are there comments?
                    if (!beforeDeclaration.Contains("<summary>"))
                    {
                        //no!
                        if (!strict) //are we being strict?
                        {
                            if(nameSpace.Contains("CatalogueManager"))
                                continue;
                            if (nameSpace.Contains("Nodes"))
                                continue;
                            if (nameSpace.Contains("DataExportManager"))
                                continue;
                            
                        }

                        problems.Add("FAIL UNDOCUMENTED CLASS:" + f);
                    }
                    else
                        commentedCount++;
                }
            }
            
            foreach (string fail in problems)
                Console.WriteLine(fail);

            Console.WriteLine("Total Documented Classes:" + commentedCount);
            

            Assert.AreEqual(0, problems.Count);
        }
    }
}
