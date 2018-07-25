﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataExportLibrary.CohortCreationPipeline;
using DataExportLibrary.CohortCreationPipeline.Destinations.IdentifierAllocation;
using DataExportLibrary.CohortDatabaseWizard;
using DataExportLibrary.Data.DataTables;
using DataExportLibrary.Interfaces.Pipeline;
using NUnit.Framework;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.DatabaseHelpers.Discovery;
using ReusableLibraryCode.DatabaseHelpers.Discovery.TypeTranslation;
using Rhino.Mocks;
using Tests.Common;

namespace DataExportLibrary.Tests.Cohort
{
    class ProjectConsistentGuidReleaseIdentifierAllocatorTests:DatabaseTests
    {
        [TestCase(DatabaseType.MicrosoftSQLServer)]
        [TestCase(DatabaseType.MYSQLServer)]
        public void TestPreserveHistoricalReleaseIdentifiers(DatabaseType databaseType)
        {
            var db = GetCleanedServer(databaseType);
            
            var privateIdentifierDataType = db.Server.GetQuerySyntaxHelper().TypeTranslater.GetSQLDBTypeForCSharpType(new DatabaseTypeRequest(typeof(string),10));

            var wizard = new CreateNewCohortDatabaseWizard(db,CatalogueRepository,DataExportRepository,false);
            var ect = wizard.CreateDatabase(new PrivateIdentifierPrototype("chi", privateIdentifierDataType),new AcceptAllCheckNotifier());
            
            var defTable = db.ExpectTable(ect.DefinitionTableName);
            var cohortTable = db.ExpectTable(ect.TableName);

            Project p = new Project(DataExportRepository,"MyProject");
            p.ProjectNumber = 10;
            p.SaveToDatabase();

            var req = new CohortCreationRequest(p,new CohortDefinition(null,"TestCohort1",1,p.ProjectNumber.Value,ect),DataExportRepository,"Ignoreme");

            var allocator = new ProjectConsistentGuidReleaseIdentifierAllocator();
            allocator.Initialize(req);

            //allocator is being asked to allocate when there are no cohorts at all defined
            Assert.AreEqual(0, defTable.GetRowCount());
            Assert.IsNotNull(allocator.AllocateReleaseIdentifier("0101010101"));
            
            //Now lets define a cohort identifier for someone (0202020202) who is not in our project
            defTable.Insert(new Dictionary<string, object>()
            {
                {"projectNumber", 11}, //project is not our project
                {"version", 1},
                {"description","flibble"}
            });

            Assert.AreEqual(1,defTable.GetRowCount());

            cohortTable.Insert(new Dictionary<string, object>()
            {
                {ect.DefinitionTableForeignKeyField, 1},
                {"chi", "0202020202"},
                {"ReleaseId", "0x0123"}
            });
            
            //recreate allocator to clear map
            allocator = new ProjectConsistentGuidReleaseIdentifierAllocator();
            allocator.Initialize(req);

            //allocator is being asked to allocate when there are cohorts defined including one with our person 02020202 but that person was in a different project
            Assert.AreEqual(1, defTable.GetRowCount());
            Assert.AreEqual(1,cohortTable.GetRowCount());
            Assert.IsNotNull(allocator.AllocateReleaseIdentifier("0202020202"));
            Assert.AreNotEqual("0x0123",allocator.AllocateReleaseIdentifier("0202020202"));


            //Now lets define a cohort identifier for someone (0202020202) who IS in our project
            defTable.Insert(new Dictionary<string, object>()
            {
                {"projectNumber", 10}, //this is our project number!
                {"version", 1},
                {"description","flibble"}
            });

            Assert.AreEqual(2, defTable.GetRowCount());

            cohortTable.Insert(new Dictionary<string, object>()
            {
                {ect.DefinitionTableForeignKeyField, 2},
                {"chi", "0202020202"},
                {"ReleaseId", "0x0127"}
            });

            //recreate allocator to clear map
            allocator = new ProjectConsistentGuidReleaseIdentifierAllocator();
            allocator.Initialize(req);

            //allocator is being asked to allocate when the person 0202020202 has previously appeared under our project (10)
            Assert.AreEqual(2, defTable.GetRowCount());
            Assert.AreEqual(2, cohortTable.GetRowCount());
            Assert.IsNotNull(allocator.AllocateReleaseIdentifier("0202020202"));
            Assert.AreEqual("0x0127", allocator.AllocateReleaseIdentifier("0202020202"));
        }

    }
}