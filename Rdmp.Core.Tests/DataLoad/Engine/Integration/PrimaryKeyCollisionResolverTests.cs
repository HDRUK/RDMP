// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using NUnit.Framework;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.QueryBuilding;
using Rdmp.Core.DataLoad.Modules.Mutilators;
using ReusableLibraryCode.Checks;
using Tests.Common;

namespace Rdmp.Core.Tests.DataLoad.Engine.Integration
{
    public class PrimaryKeyCollisionResolverTests : DatabaseTests
    {

        [Test]
        public void PrimaryKeyCollisionResolverMultilation_Check_Passes()
        {
            TableInfo t;
            ColumnInfo c1;
            ColumnInfo c2;
            ColumnInfo c3;
            SetupTableInfos(out t, out c1, out c2, out c3);
            try
            {
                var mutilation = new PrimaryKeyCollisionResolverMutilation();
                mutilation.TargetTable = t;
                
                c1.IsPrimaryKey = true;
                c1.SaveToDatabase();

                c2.DuplicateRecordResolutionOrder = 1;
                c2.DuplicateRecordResolutionIsAscending = true;
                c2.SaveToDatabase();

                c3.DuplicateRecordResolutionOrder = 2;
                c3.DuplicateRecordResolutionIsAscending = false;
                c3.SaveToDatabase();

                Assert.DoesNotThrow(() => mutilation.Check(new ThrowImmediatelyCheckNotifier()));
            
            }
            finally
            {
                t.DeleteInDatabase();
            }
        }


        [Test]
        public void PrimaryKeyCollisionResolverMultilation_Check_ThrowsBecauseNoColumnOrderConfigured()
        {
              TableInfo t;
            ColumnInfo c1;
            ColumnInfo c2;
            ColumnInfo c3;
            SetupTableInfos(out t, out c1, out c2,out c3);
            try
            {
                var mutilation = new PrimaryKeyCollisionResolverMutilation();
                mutilation.TargetTable = t;
                try
                {

                    mutilation.Check(new ThrowImmediatelyCheckNotifier());
                    Assert.Fail("Should have crashed before here");
                }
                catch (Exception e)
                {
                    Assert.AreEqual("Failed to check PrimaryKeyCollisionResolver on PrimaryKeyCollisionResolverTests", e.Message);
                    Assert.AreEqual("TableInfo PrimaryKeyCollisionResolverTests does not have any primary keys defined so cannot resolve primary key collisions",e.InnerException.Message);
                }
            }
            finally
            {
                t.DeleteInDatabase();
            }
        }

        [Test]
        public void PrimaryKeyCollisionResolverMultilation_Check_ThrowsBecauseNotInitialized()
        {
            var mutilation = new PrimaryKeyCollisionResolverMutilation();
            
            var ex = Assert.Throws<Exception>(()=>mutilation.Check(new ThrowImmediatelyCheckNotifier()));
            StringAssert.Contains("Target table is null, a table must be specified upon which to resolve primary key duplication (that TableInfo must have a primary key collision resolution order)",ex.Message);
        }

        [Test]     
        public void GenerateSQL_OrderCorrect()
        {
            TableInfo t;
            ColumnInfo c1;
            ColumnInfo c2;
            ColumnInfo c3;
            SetupTableInfos(out t, out c1, out c2,out c3);
            try
            {
                c1.IsPrimaryKey = true;
                c1.SaveToDatabase();

                c2.DuplicateRecordResolutionOrder = 1;
                c2.DuplicateRecordResolutionIsAscending = true;
                c2.SaveToDatabase();

                c3.DuplicateRecordResolutionOrder = 2;
                c3.DuplicateRecordResolutionIsAscending = false;
                c3.SaveToDatabase();

                PrimaryKeyCollisionResolver resolver = new PrimaryKeyCollisionResolver(t);
                string sql = resolver.GenerateSQL();

                Console.WriteLine(sql);

                Assert.IsTrue(sql.Contains(c2.Name));
                Assert.IsTrue(sql.Contains(c3.Name));

                //column 2 has the following null substitute, is Ascending order and is the first of two
                Assert.IsTrue(sql.Contains("ISNULL([col2],-9223372036854775808) ASC,"));

                //column 3 has the following null substitute and is descending and is not followed by another column 
                Assert.IsTrue(sql.Contains("ISNULL([col3],-2147483648) DESC"));
            }
            finally
            {
                t.DeleteInDatabase();
            }
        }

        [Test]
        public void NoColumnOrdersConfigured_ThrowsException()
        {
            TableInfo t;
            ColumnInfo c1;
            ColumnInfo c2;
            ColumnInfo c3;
            SetupTableInfos(out t, out c1, out c2, out c3);
            try
            {
                c1.IsPrimaryKey = true;
                c1.SaveToDatabase();

                PrimaryKeyCollisionResolver resolver = new PrimaryKeyCollisionResolver(t);
                var ex = Assert.Throws<Exception>(()=>Console.WriteLine(resolver.GenerateSQL()));
                StringAssert.Contains("The ColumnInfos of TableInfo PrimaryKeyCollisionResolverTests do not have primary key resolution orders configured (do not know which order to use non primary key column values in to resolve collisions).  Fix this by right clicking a TableInfo in CatalogueManager and selecting 'Configure Primary Key Collision Resolution'.",ex.Message);
            }
            finally
            {
                t.DeleteInDatabase();
            }
        }

        [Test]
        public void NoPrimaryKeys_ThrowsException()
        {
            TableInfo t;
            ColumnInfo c1;
            ColumnInfo c2;
            ColumnInfo c3;
            SetupTableInfos(out t, out c1, out c2,out c3);
           
            try
            {
                PrimaryKeyCollisionResolver resolver = new PrimaryKeyCollisionResolver(t);
                var ex = Assert.Throws<Exception>(()=>Console.WriteLine(resolver.GenerateSQL()));
                StringAssert.Contains("does not have any primary keys defined so cannot resolve primary key collisions",ex.Message);
            }
            finally
            {
                t.DeleteInDatabase();
            }
        }

        private void SetupTableInfos(out TableInfo tableInfo, out ColumnInfo c1, out ColumnInfo c2, out ColumnInfo c3)
        {
            tableInfo = new TableInfo(CatalogueRepository, "PrimaryKeyCollisionResolverTests");

            c1 = new ColumnInfo(CatalogueRepository, "col1", "varchar(100)", tableInfo);
            c2 = new ColumnInfo(CatalogueRepository, "col2", "float", tableInfo);
            c3 = new ColumnInfo(CatalogueRepository, "col3", "int", tableInfo);
        }
    }
}
