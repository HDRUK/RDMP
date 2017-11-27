﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Data;
using CatalogueLibrary.Repositories;
using DataExportLibrary.Data.DataTables;
using NUnit.Framework;
using Tests.Common;

namespace CatalogueLibraryTests.Integration.ObscureDependencyTests
{
    public class ObjectSharingObscureDependencyFinderTests: DatabaseTests
    {
        private ShareManager _share;

        [SetUp]
        public void StoreShareManager()
        {
            _share = RepositoryLocator.CatalogueRepository.ShareManager;
        }

        [Test]
        public void TestPruning()
        {
            Catalogue c = new Catalogue(CatalogueRepository,"Catapault");
            var ci = new CatalogueItem(CatalogueRepository, c, "string");
            
            Catalogue c2 = new Catalogue(CatalogueRepository,"Catapault (Import)");
            var ci2 = new CatalogueItem(CatalogueRepository, c2, "string (Import)");

            Assert.AreEqual(CatalogueRepository.GetAllObjects<ObjectExport>().Count(), 0);
            var ec = _share.GetExportFor(c);
            var eci = _share.GetExportFor(ci);

            _share.GetImportAs(ec.SharingUID, c2);
            _share.GetImportAs(eci.SharingUID, ci2);
            
            Assert.AreEqual(2,CatalogueRepository.GetAllObjects<ObjectExport>().Count());
            Assert.AreEqual(2,CatalogueRepository.GetAllObjects<ObjectImport>().Count());
            Assert.AreEqual(2,CatalogueRepository.GetAllObjects<ObjectImport>().Count());//successive calls shouldhn't generate extra entries since they are same obj
            Assert.AreEqual(2,CatalogueRepository.GetAllObjects<ObjectImport>().Count());

            //cannot delete the shared object
            Assert.Throws<Exception>(c.DeleteInDatabase);

            //can delete the import because that's ok
            Assert.DoesNotThrow(c2.DeleteInDatabase);

            //now that we deleted the import it should have deleted everything else including the CatalogueItem import which magically disapeared when we deleted the Catalogue via database level cascade events
            Assert.AreEqual(0,CatalogueRepository.GetAllObjects<ObjectImport>().Count());

            _share.GetImportAs(eci.SharingUID, ci2);
        }

        [Test]
        public void CannotDeleteSharedObjectTest()
        {
            //create a test catalogue
            Catalogue c = new Catalogue(CatalogueRepository,"blah");

            Assert.IsFalse(_share.IsExportedObject(c));

            //make it exportable
            var exportDefinition = _share.GetExportFor(c);

            Assert.IsTrue(_share.IsExportedObject(c));

            //cannot delete because object is shared externally
            Assert.Throws<Exception>(c.DeleteInDatabase);

            //no longer exportable
            exportDefinition.DeleteInDatabase();

            //no longer shared
            Assert.IsFalse(_share.IsExportedObject(c));

            //now we can delete it
            c.DeleteInDatabase();
        }

        [Test]
        public void CascadeDeleteImportDefinitions()
        {
            Project p = new Project(DataExportRepository, "prah");

            var exportDefinition = _share.GetExportFor(p);

            Project p2 = new Project(DataExportRepository, "prah2");

            var importDefinition = _share.GetImportAs(exportDefinition.SharingUID, p2);

            //import definition exists
            Assert.IsTrue(importDefinition.Exists());

            //delete local import
            p2.DeleteInDatabase();

            //cascade should have deleted the import definition since the imported object version is gone
            Assert.IsFalse(importDefinition.Exists());

            //clear up the exported version too 
            exportDefinition.DeleteInDatabase();
            p.DeleteInDatabase();
        }
    }
}
