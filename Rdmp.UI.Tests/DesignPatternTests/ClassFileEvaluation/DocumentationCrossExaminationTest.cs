// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace Rdmp.UI.Tests.DesignPatternTests.ClassFileEvaluation
{
    class DocumentationCrossExaminationTest
    {
        Regex matchComments = new Regex(@"///[^;\r\n]*");

        private string[] _mdFiles;
        Regex matchMdReferences = new Regex(@"`(.*)`");

        //words that are in Pascal case and you can use in comments despite not being in the codebase... this is an ironic variable to be honest
        //since the very fact that you add something to _whitelist means that it is in the codebase after all!
        private string[] _whitelist = new []
        {
            "NormalCohorts",
            "FreakyCohorts",
            "PublicKeyToken",
            "DatabaseEntities",
            "MyDateCol",
            "NumberOfResults",
            "ANOGPCode",
            "ANOPatientIdentifier",
            "PracticeGP",
            "UNIONing",
            "PatientId",
            "PatientIds",
            "MotherId",
            "BabyId",
            "MyColumn",
            "DrugCode",
            "DrugCode_Desc",
            "DrugName",
            "LabNumber",
            "DrugName",
            "SendingLocation",
            "DischargeLocation",
            "SendingLocation_Desc",
            "DischargeLocation_Desc",
            "MyDb",
            "NewBiochemistry",
            "AdmissionDateTime",
            "DataAge",
            "PatientCareNumber",
            "MyDb",
            "MyTransform",
            "PatCHI",
            "PatientCHI",
            "MotherCHI",
            "FatherChiNo",
            "LabNumber",
            "ANOchi",
            "INFOs",
            "PluginDatabase",
            "PatientDateOfBirth",
            "PatientDateOfBirthApprox",
            "ANOLocation",
            "ConditionList",
            "DataAnalyst",
            "HumanReadableDrugName",
            "DrugPrescribed",
            "DrugAbuse",
            "RoutineLoaderAccount",
            "ReadonlyUserAccount",
            "HBA1c",
            "PrescribedDate",
            "NodaTime",
            "MotherCHI",
            "BabyCHI",
            "ANOIdentifier",
            "ANOLabNumber",
            "LabNumber",
            "EmailAddressOfAuthorizor",
            "GrandParent",
            "TestLabCode",
            "DataAge",
            "CatalogueItem1",
            "CatalogueItem2",
            "CatalogueItem1",
            "CatalogueItem2",
            "UPDATEd",
            "GUIDs",
            "MyColRenamed",
            "DateConsentedToStudy",
            "PrescribingGP",
            "CasesForProject123",
            "ControlsForProject123",
            "GUIDs",
            "UPPERd",
            "StackOverflow",
            "LabNumbers",
            "LabNumber",
            "ANOLabNumber",
            "PatientName",
            "PatientDob",
            "DataAge",
            "DemographyLoading",
            "MyDb",
            "MyTbl",
            "CapsName",
            "SummaryComment",
            "MySoftwareSuite",
            "MyApplication",
            "MyResources",
            "MyClass1",
            "FishFishFish",
            "CASCADEing",
            "MyAssembly",
            "HBA1c",
            "EnumNameFavourite",
            "EnumName",
            "CatalogueFavourite",
            "PatientDeleted",
            "MyDoc",
            "MyDataFiles",
            "MyServer",
            "MyData",
            "RemoteDataFetcher",
            "EndpointDefinition",
            "LoadID_Data_STAGING",
            "AuditObject",
            "EmailAddressOfAuthorizor",
            "FixedSource",
            "AverageResult",
            "MaxResult",
            "FormatFile",
            "IndexedExtractionIdentifierList_AggregateConfiguration5",
            "YAXLib",
            "DependsOn",
            "DependingOnThis",
            "ProposedFixes",
            "PropertyX",
            "FamilyMembers",
            
            //CreatingANewCollectionTreeNode.md
            "FolderOfX", 

            //PluginWriting.md
            "MyPluginUserInterface",
            "ExecuteCommandRenameCatalogueToBunnies",
            "BasicDataTableAnonymiser1",
            "BasicDataTableAnonymiser3",
            "CodeExamples",
            "MyExamplePluginTests",
            "TEST_Catalogue",
            "BasicDataTableAnonymiser4",
            "GetCommonNamesTable",
            "TestBasicDataTableAnonymiser3",
            "BasicDataTableAnonymiser5",
            "TestAnonymisationPlugins",
            "LoggerTestCase",
            "ToConsole",
            "ToMemory",
            "ToDatabase",
            "TEST_Logging",
            "DatePrescribed",

            //This is a legit verb
            "ANDed",
            "FetchBytesExpensive",

            "RecordLoadedDate",
            "TblPatIndx",
            "DataLoadRunId",

            "DataStructures", //class diagram
            "MyYetToExistTable",

            "SexCode",
            "SexCode_Desc",
            "SendingLocationCode",
            "LocationTable",
            "AddressLine1",
            "AddressLine2",
            "PatientSexCode",
            "SexDescription",
            "SexDescriptionLong",
            "MyTransform",
            
            "MyObject",
            "MyObjectMenu",
            "AllServersNodeMenu",
            "SomeClass",
            "ProposeExecutionWhenCommandIs",
            "Log4Net",
            "ReleaseLocation",
            "MyCoolDb",
            "OperationType",
            "DrugList",
            "DrugCodeFormat",
            "GenderCodeDescription",
            "MyParam",

            "GPCode",
            "RepoType",
            "LoadingBiochem",

            //Stuff now in FAnsi
            "MySqlAggregateHelper",
            "MicrosoftSQLAggregateHelper",
            "DbCommandBuilder",
            "EnvironmentPotential",
            "MySqlConnection",
            "DbCommandBuilder",
            "DbCommandBuilder",
            "IDecideTypesForStrings",
            "TypeCompatibilityGroup",
            "DatatypeComputerTests",
            "DataTypeRequest",
            "CrossPlatformTests",
            "IDecideTypesForStrings",
            "TypeCompatibilityGroup",
            "TypeTranslaterTests",
            "DatatypeComputerTests",
            "LoadRunID",
            "HelpDocs",
            "HicServices",
            "FAnsiSql",
            "ProposeExecutionWhenTargetIsX",
            "InternalsVisibleTo",
            "AxisDimension",
            "MyDataset1",
             "HicHash",
            "UserManual",
            "ObjectType",
            "UserInterfaceOverview",
            "MyPipelinePlugin",
            "TestAnonymisationPluginsDatabaseTests",
            "PDFs"
        };

        public DocumentationCrossExaminationTest(DirectoryInfo slndir)
        {
            var mdDirectory = Path.Combine(slndir.FullName, @"Documentation", "CodeTutorials");

            _mdFiles = Directory.GetFiles(mdDirectory, "*.md");
        }

        public void FindProblems(List<string> csFilesFound)
        {
            //find all non coment code and extract all unique tokens

            //find all .md files and extract all `` code blocks

            //for each commend and `` code block

            //identify Pascal case words

            //are they in the codebase tokens?

            List<string> problems = new List<string>();

            HashSet<string> codeTokens = new HashSet<string>();
            Dictionary<string, HashSet<string>> fileCommentTokens = new Dictionary<string, HashSet<string>>();

            //find all comments in class files
            foreach (string file in csFilesFound)
            {
                bool isDesignerFile = file.Contains(".Designer.cs");
                
                if(file.Contains("CodeTutorials"))
                    continue;
                
                foreach (string line in File.ReadAllLines(file))
                {
                    //if it is a comment
                    if (matchComments.IsMatch(line))
                    {
                        if (isDesignerFile)
                            continue;

                        if (!fileCommentTokens.ContainsKey(file))
                            fileCommentTokens.Add(file, new HashSet<string>());
                        
                        //its a comment extract all pascal case words
                        foreach (Match word in Regex.Matches(line, @"\b([A-Z]\w+){2,}"))
                            fileCommentTokens[file].Add(word.Value);
                    }
                    else
                    {
                        //else it is a code line, extract all tokens
                        foreach (Match word in Regex.Matches(line, @"\w+"))
                            codeTokens.Add(word.Value);
                    }
                }
            }

            //find all comments in .md tutorials
            foreach (string mdFile in _mdFiles)
            {
                fileCommentTokens.Add(mdFile,new HashSet<string>());
                var fileContents = File.ReadAllText(mdFile);
                
                foreach (Match m in matchMdReferences.Matches(fileContents))
                    foreach (Match word in Regex.Matches(m.Groups[1].Value, @"([A-Z]\w+){2,}"))
                        fileCommentTokens[mdFile].Add(word.Value);
            }


            foreach (KeyValuePair<string, HashSet<string>> kvp in fileCommentTokens)
            {
                foreach (string s in kvp.Value)
                {
                    if(!codeTokens.Contains(s))
                    {
                        if (_whitelist.Contains(s))
                            continue;

                        //it's SHOUTY TEXT
                        if (s.ToUpper() == s)
                            continue;

                        //if it's a plural e.g. TableInfos then we are still ok if we find TableInfo
                        if (s.Length > 2 && s.EndsWith("s"))
                        {
                            if (codeTokens.Contains(s.Substring(0, s.Length - 1)))
                                continue;
                        }
                        
                        problems.Add("FATAL PROBLEM: File '" + Path.GetFileName(kvp.Key) +" talks about something which isn't in the codebase, called a:" +Environment.NewLine + s);
                        
                    }
                }
            }

            if (problems.Any())
            {
                Console.WriteLine("Found problem words in comments (Scroll down to see by file then if you think they are fine add them to DocumentationCrossExaminationTest._whitelist):");
                foreach (var pLine in problems.Select(p => p.Split('\n')))
                    Console.WriteLine("\"" + pLine[1] + "\",");
                
            }

            foreach (string problem in problems)
                Console.WriteLine(problem);

            Assert.AreEqual(0,problems.Count,"Expected there to be nothing talked about in comments that doesn't appear in the codebase somewhere");
        }
    }
}
