using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.Aggregation;
using CatalogueLibrary.Data.Automation;
using CatalogueLibrary.Data.Cache;
using CatalogueLibrary.Data.Cohort;
using CatalogueLibrary.Data.ImportExport;
using CatalogueLibrary.Data.PerformanceImprovement;
using CatalogueLibrary.Data.Serialization;
using CatalogueLibrary.Properties;
using CatalogueLibrary.Reports;
using CatalogueLibrary.Repositories.Construction;
using HIC.Logging;
using MapsDirectlyToDatabaseTable;
using MapsDirectlyToDatabaseTable.Attributes;
using ReusableLibraryCode;
using ReusableLibraryCode.Checks;

namespace CatalogueLibrary.Repositories
{
    /// <summary>
    /// Pointer to the Catalogue Repository database in which all DatabaseEntities declared in CatalogueLibrary.dll are stored.  Ever DatabaseEntity class must exist in a
    /// Microsoft Sql Server Database (See DatabaseEntity) and each object is compatible only with a specific type of TableRepository (i.e. the database that contains the
    /// table matching their name).  CatalogueLibrary.dll objects in CatalogueRepository, DataExportLibrary.dll objects in DataExportRepository, DataQualityEngine.dll objects
    /// in DQERepository etc.
    /// 
    /// <para>This class allows you to fetch objects and should be passed into constructors of classes you want to construct in the Catalogue database.  </para>
    /// 
    /// <para>It also includes helper properties for setting up relationships and controling records in the non DatabaseEntity tables in the database e.g. AggregateForcedJoiner</para>
    /// </summary>
    public class CatalogueRepository : TableRepository, ICatalogueRepository
    {
        public AggregateForcedJoin AggregateForcedJoiner { get; set; }
        public TableInfoToCredentialsLinker TableInfoToCredentialsLinker { get; set; }
        public PasswordEncryptionKeyLocation PasswordEncryptionKeyLocation { get; set; }
        public JoinInfoFinder JoinInfoFinder { get; set; }
        public MEF MEF { get; set; }
        
        readonly ObjectConstructor _constructor = new ObjectConstructor();
        
        /// <summary>
        /// By default CatalogueRepository will execute DocumentationReportMapsDirectlyToDatabase which will load all the Types and find documentation in the source code for 
        /// them obviously this affects test performance so set this to true if you want it to skip this process.  Note where this is turned on, it's in the static constructor
        /// of DatabaseTests which means if you stick a static constructor in your test you can override it if you need access to the help text somehow in your test
        /// </summary>
        public static bool? SuppressHelpLoading;

        public Dictionary<string,string> HelpText = new Dictionary<string, string>();
        
        public CatalogueRepository(DbConnectionStringBuilder catalogueConnectionString): base(null,catalogueConnectionString)
        {
            AggregateForcedJoiner = new AggregateForcedJoin(this);
            TableInfoToCredentialsLinker = new TableInfoToCredentialsLinker(this);
            PasswordEncryptionKeyLocation = new PasswordEncryptionKeyLocation(this);
            JoinInfoFinder = new JoinInfoFinder(this);
            MEF = new MEF(this);
            
            ObscureDependencyFinder = new CatalogueObscureDependencyFinder(this);

            AddToHelp(Resources.KeywordHelp);

            AddToHelp(GetType().Assembly);
        }

        private void AddToHelp(string keywordHelpFileContents)
        {
            //null is true for us loading help
            if (SuppressHelpLoading != null && SuppressHelpLoading.Value)
                return;
            
            var lines = keywordHelpFileContents.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if(string.IsNullOrWhiteSpace(line))
                    continue;

                var split = line.Split(':');

                if(split.Length != 2)
                    throw new Exception("Malformed line in Resources.KeywordHelp, line is:"+Environment.NewLine + line +Environment.NewLine + "We expected it to have exactly one colon in it");

                if(!HelpText.ContainsKey(split[0]))
                    HelpText.Add(split[0], split[1]);
            }
        }

        public void AddToHelp(Assembly assembly)
        {
            //null is true for us loading help
            if (SuppressHelpLoading != null && SuppressHelpLoading.Value)
                return;
            
            Console.WriteLine("Setting up help for assembly " + assembly);

            DocumentationReportMapsDirectlyToDatabase types = new DocumentationReportMapsDirectlyToDatabase(assembly);
            types.Check(new IgnoreAllErrorsCheckNotifier());

            if (types.Summaries != null)
                foreach (var kvp in types.Summaries)
                    if (!HelpText.ContainsKey(kvp.Key.Name))
                        HelpText.Add(kvp.Key.Name, kvp.Value + Environment.NewLine+"(DatabaseEntity)");
        }

        public IEnumerable<CatalogueItem> GetAllCatalogueItemsNamed(string name, bool ignoreCase)
        {
            string sql;
            if (ignoreCase)
                sql = "WHERE UPPER(Name)='" + name.ToUpper() + "'";
            else
                sql = "WHERE Name='" + name + "'";

            return GetAllObjects<CatalogueItem>(sql);
        }


        /// <summary>
        /// If the configuration is part of any aggregate container anywhere this method will return the order within that container
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public int? GetOrderIfExistsFor(AggregateConfiguration configuration)
        {
            if (configuration.Repository != this)
                if (((CatalogueRepository)configuration.Repository).ConnectionString != ConnectionString)
                    throw new NotSupportedException("AggregateConfiguration is from a different repository than this with a different connection string");

            using (var con = GetConnection())
            {
                DbCommand cmd = DatabaseCommandHelper.GetCommand("SELECT [Order] FROM CohortAggregateContainer_AggregateConfiguration WHERE AggregateConfiguration_ID = @AggregateConfiguration_ID", con.Connection, con.Transaction);

                cmd.Parameters.Add(DatabaseCommandHelper.GetParameter("@AggregateConfiguration_ID", cmd));
                cmd.Parameters["@AggregateConfiguration_ID"].Value = configuration.ID;

                return DatabaseEntity.ObjectToNullableInt(cmd.ExecuteScalar());
            }
        }
        
        /// <summary>
        /// Returns Catalogue1.CatalogueItem1, Catalogue1.CatalogueItem2 etc, a CatalogueItem does not know the name of it's parent so 
        /// for performance reasons this is a big saver it means we only have database query instead of having to construct and dereference
        /// every CatalogueItem and Every Catalogue in the database.
        /// </summary>
        /// <returns></returns>
        public List<FriendlyNamedCatalogueItem> GetFullNameOfAllCatalogueItems()
        {
            List<FriendlyNamedCatalogueItem> toReturn = new List<FriendlyNamedCatalogueItem>();

            using (var con = GetConnection())
            {

                //get parent name and child name, separate with . (after removing any dots that our users might have put into the name (bad user!))
                DbCommand cmd = DatabaseCommandHelper.GetCommand(@"SELECT REPLACE([Catalogue].Name,'.','') + '.' + REPLACE(CatalogueItem.Name,'.','') as FriendlyName, CatalogueItem.ID from Catalogue join CatalogueItem on Catalogue.ID = Catalogue_ID", con.Connection,con.Transaction);

                DbDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    toReturn.Add(new FriendlyNamedCatalogueItem(Convert.ToInt32(r["ID"]),r["FriendlyName"].ToString()));
                }
                //deals with crudy decimal/short types that SQL might throw at us
            }
            return toReturn;
        }
         
        /// <inheritdoc/>
        public LogManager GetDefaultLogManager()
        {
            ServerDefaults defaults = new ServerDefaults(this);
            return new LogManager(defaults.GetDefaultFor(ServerDefaults.PermissableDefaults.LiveLoggingServer_ID));
        }

        public Catalogue[] GetAllCatalogues(bool includeDeprecatedCatalogues = false)
        {
            return GetAllObjects<Catalogue>().Where(cata => (!cata.IsDeprecated) || includeDeprecatedCatalogues).ToArray();
        }

        public Catalogue[] GetAllCataloguesWithAtLeastOneExtractableItem()
        {
            return
                GetAllObjects<Catalogue>(
                    @"WHERE exists (select 1 from CatalogueItem ci where Catalogue_ID = Catalogue.ID AND exists (select 1 from ExtractionInformation where CatalogueItem_ID = ci.ID)) ")
                    .ToArray();
        }

        


        public IEnumerable<CohortIdentificationConfiguration> GetAllCohortIdentificationConfigurationsWithDependencyOn(AggregateConfiguration aggregate)
        {
            //1 query to fetch all these
            //get all the ones that have a container system setup on them
            var allConfigurations = GetAllObjects<CohortIdentificationConfiguration>().Where(c => c.RootCohortAggregateContainer_ID != null).ToArray();

            foreach (CohortIdentificationConfiguration config in allConfigurations)
            {
                //get the root container
                //see if the root container (or any of it's children) contain the aggregate you are looking for
                if (config.RootCohortAggregateContainer.HasChild(aggregate))
                    yield return config;
            }
        }

        public IEnumerable<AnyTableSqlParameter> GetAllParametersForParentTable(IMapsDirectlyToDatabaseTable parent)
        {
            var type = parent.GetType();

            if (!AnyTableSqlParameter.IsSupportedType(type))
                throw new NotSupportedException("This table does not support parents of type " + type.Name);

            return GetAllObjects<AnyTableSqlParameter>("where ParentTable = '" + type.Name + "' and Parent_ID =" + parent.ID);
        }

        /// <summary>
        /// Returns all ColumnInfos which have names exactly matching name, this must be a fully qualified string e.g. [MyDatabase]..[MyTable].[MyColumn].  You can use
        /// IQuerySyntaxHelper.EnsureFullyQualified to get this.  Return is an array because you can have an identical table/database structure on two different servers
        /// in each case the ColumnInfo will have the same fully qualified name (or you could have duplicate references to the same ColumnInfo/TableInfo for some reason)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ColumnInfo[] GetColumnInfosWithNameExactly(string name)
        {
            return SelectAllWhere<ColumnInfo>("SELECT * FROM ColumnInfo WHERE Name = @name","ID",
                new Dictionary<string, object>
                {
                    {"name", name}
                }).ToArray();
        }

        public TicketingSystemConfiguration GetTicketingSystem()
        {
            var configuration = GetAllObjects<TicketingSystemConfiguration>().Where(t => t.IsActive).ToArray();

            if (configuration.Length == 0)
                return null;

            if (configuration.Length == 1)
                return configuration[0];

            throw new NotSupportedException("There should only ever be one active ticketing system, something has gone very wrong, there are currently " + configuration.Length);
        }

        public IEnumerable<CacheProgress> GetAllCacheProgressWithoutAPermissionWindow()
        {
            return GetAllObjects<CacheProgress>().Where(p => p.PermissionWindow_ID == null);
        }

        public TableInfo GetTableWithNameApproximating(string tableName, string database)
        {
            int id;
            using (var con = GetConnection())
            {
                tableName = tableName.Trim(']', '[');

                if (tableName.Contains("."))
                    throw new ArgumentException("Must be a table name only must not have a database/schema prefix");

                //add a percent  and dot so that we are matching Bob..MyTable or Dave.MyTable (MySQL) but either way we are throwing out [ ] and definetly not matching Bob..SomePrefixTextMyTable
                var cmd = DatabaseCommandHelper.GetCommand("SELECT * FROM TableInfo WHERE " +
                    "REPLACE(REPLACE(Name,']',''),'[','') LIKE @nameToFind", con.Connection, con.Transaction);

                cmd.Parameters.Add(DatabaseCommandHelper.GetParameter("@nameToFind", cmd));
                cmd.Parameters["@nameToFind"].Value = database + "%..%" + tableName;

                DbDataReader r = cmd.ExecuteReader();
                try
                {
                    if (!r.Read())
                        return null;

                    id = Convert.ToInt32(r["ID"]);

                    if (r.Read())
                        throw new Exception("Found 2+ TableInfos named " + tableName);
                }
                finally
                {

                    r.Close();
                }
            }

            if (id == 0)
                throw new InvalidOperationException("A table was found, but it doesn't appear to have a valid ID");

            return GetObjectByID<TableInfo>(id);
        }

        

        protected override IMapsDirectlyToDatabaseTable ConstructEntity(Type t, DbDataReader reader)
        {
            return _constructor.ConstructIMapsDirectlyToDatabaseObject<ICatalogueRepository>(t, this, reader);
        }

        public void TickLifeline(IMapsDirectlyToDatabaseTable ticker)
        {
            if(!(ticker is ILifelineable))
                throw new NotSupportedException(ticker + " did not implement ILifelineable");

            Update("UPDATE "+ticker.GetType().Name+" SET Lifeline=@machineTime WHERE ID = " + ticker.ID , new Dictionary<string, object>()
            {
                {"@machineTime", DateTime.Now}
            });
        }


        
        public DateTime? GetTickLifeline(IMapsDirectlyToDatabaseTable ticker)
        {
            if (!(ticker is ILifelineable))
                throw new NotSupportedException(ticker + " did not implement ILifelineable");

            using (var con = GetConnection())
                return
                    ObjectToNullableDateTime(
                        DatabaseCommandHelper.GetCommand(
                            "SELECT Lifeline from " + ticker.GetType().Name + " WHERE ID = " + ticker.ID, con.Connection)
                            .ExecuteScalar());
        }

        public void RefreshLockPropertiesFromDatabase(IMapsDirectlyToDatabaseTable lockable)
        {
            var l = lockable as ILockable;

            if (l == null)
                throw new NotSupportedException(lockable + " did not implement ILockable");

            using (var con = GetConnection())
            {
                DbDataReader r = DatabaseCommandHelper.GetCommand(
                    "Select LockedBecauseRunning,LockHeldBy from " + lockable.GetType().Name + " WHERE ID = " +
                    lockable.ID, con.Connection).ExecuteReader();

                if (!r.Read())
                    throw new ObjectDeletedException(lockable);

                l.LockedBecauseRunning = Convert.ToBoolean(r["LockedBecauseRunning"]);
                l.LockHeldBy = Convert.ToString(r["LockHeldBy"]);
            }
        }


        public Catalogue[] GetAllAutomationLockedCatalogues()
        {
            return SelectAll<Catalogue>("SELECT * FROM AutomationLockedCatalogues", "Catalogue_ID").ToArray();
        }

        public ExternalDatabaseServer[] GetAllTier2Databases(Tier2DatabaseType type)
        {
            var servers = GetAllObjects<ExternalDatabaseServer>();
            string assembly;

            switch (type)
            {
                case Tier2DatabaseType.Logging:
                    assembly = "HIC.Logging.Database";
                    break;
                case Tier2DatabaseType.DataQuality:
                    assembly = "DataQualityEngine.Database";
                    break;
                case Tier2DatabaseType.QueryCaching:
                    assembly = "QueryCaching.Database";
                    break;
                case Tier2DatabaseType.ANOStore:
                    assembly = "ANOStore.Database";
                    break;
                case Tier2DatabaseType.IdentifierDump:
                    assembly = "IdentifierDump.Database";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }

            return servers.Where(s => s.CreatedByAssembly == assembly).ToArray();
        }

        public void UpsertAndHydrate<T>(T toCreate, ShareManager shareManager, ShareDefinition shareDefinition) where T : class,IMapsDirectlyToDatabaseTable
        {
            //Make a dictionary of the normal properties we are supposed to be importing
            Dictionary<string,object> propertiesDictionary = shareDefinition.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            //for finding properties decorated with [Relationship]
            var finder = new AttributePropertyFinder<RelationshipAttribute>(toCreate);

            //remove null arguments they won't help us here
            foreach (string key in propertiesDictionary.Keys.ToArray())
            {
                if (propertiesDictionary[key] == null || propertiesDictionary[key] as string == string.Empty)
                    propertiesDictionary.Remove(key);
            }

            //If we have already got a local copy of this shared object?
            //either as an import or as an export
            T actual = (T)shareManager.GetExistingImportObject(shareDefinition.SharingGuid) ?? (T)shareManager.GetExistingExportObject(shareDefinition.SharingGuid);
            
            //we already have a copy imported of the shared object
            if (actual != null)
            {
                //It's an UPDATE i.e. take the new shared properties and apply them to the database copy / memory copy

                //copy all the values out of the share definition / database copy
                foreach (PropertyInfo prop in GetPropertyInfos(typeof(T)))
                {
                    //don't update any ID columns or any with relationships on UPDATE
                    if (propertiesDictionary.ContainsKey(prop.Name) && finder.GetAttribute(prop) == null)
                    {
                        //sometimes json decided to swap types on you e.g. int64 for int32
                        var val = propertiesDictionary[prop.Name];
                        var propertyType = prop.PropertyType;

                        //if it is a nullable int etc
                        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof (Nullable<>))
                            propertyType = propertyType.GetGenericArguments()[0]; //lets pretend it's just int / whatever
                        
                        if (val != null && val != DBNull.Value && !propertyType.IsInstanceOfType(val))
                            if (propertyType == typeof(CatalogueFolder))
                            {
                                //will be passed as a string
                                string folderAsString = shareDefinition.Properties["Folder"].ToString();
                                val = new CatalogueFolder((Catalogue)(object)toCreate, folderAsString);
                            }
                            else
                            if (typeof(Enum).IsAssignableFrom(propertyType))
                                val = Enum.ToObject(propertyType, val);//if the property is an enum
                            else
                                val = Convert.ChangeType(val, propertyType); //the property is not an enum

                        
                        prop.SetValue(toCreate, val); //if it's a shared property (most properties) use the new shared value being imported
                        
                    }
                    else
                        prop.SetValue(toCreate, prop.GetValue(actual)); //or use the database one if it isn't shared (e.g. ID, MyParent_ID etc)

                }

                toCreate.Repository = actual.Repository;
                
                //commit the updated values to the database
                SaveToDatabase(toCreate);
            }
            else
            {
                //It's an INSERT i.e. create a new database copy with the correct foreign key values and update the memory copy
                
                //for each relationship property on the class we are trying to hydrate
                foreach (PropertyInfo property in GetPropertyInfos(typeof(T)))
                {
                    RelationshipAttribute relationshipAttribute = finder.GetAttribute(property);

                    //if it has a relationship attribute then we would expect the ShareDefinition to include a dependency relationship with the sharing UID of the parent
                    //and also that we had already imported it since dependencies must be imported in order
                    if(relationshipAttribute != null)
                    {
                        int? newValue;

                        switch (relationshipAttribute.Type)
                        {
                            case RelationshipType.SharedObject:
                                //Confirm that the share definition includes the knowledge that theres a parent class to this object
                                if (!shareDefinition.RelationshipProperties.ContainsKey(relationshipAttribute))
                                    throw new Exception("Share Definition for object of Type " + typeof(T) + " is missing an expected RelationshipProperty called " + property.Name);

                                //Get the SharingUID of the parent for this property
                                Guid importGuidOfParent = shareDefinition.RelationshipProperties[relationshipAttribute];

                                //Confirm that we have a local import of the parent
                                var parentImport = shareManager.GetExistingImport(importGuidOfParent);

                                if (parentImport == null)
                                    throw new Exception("Cannot import an object of type " + typeof(T) + " because the ShareDefinition specifies a relationship to an object that has not yet been imported (A " + relationshipAttribute.Cref + " with a SharingUID of " + importGuidOfParent);

                                newValue = parentImport.LocalObjectID;
                                break;
                            case RelationshipType.LocalReference:
                                newValue = shareManager.GetLocalReference(property, relationshipAttribute, shareDefinition);
                                break;
                            case RelationshipType.IgnoreableLocalReference:
                                newValue = null;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        
                        //get the ID of the local import of the parent
                        if (propertiesDictionary.ContainsKey(property.Name))
                            propertiesDictionary[property.Name] = newValue;
                        else
                            propertiesDictionary.Add(property.Name,newValue);
                    }
                }

                //insert the full dictionary into the database under the Type
                InsertAndHydrate(toCreate,propertiesDictionary);

                //document that a local import of the share now exists and should be updated/reused from now on when that same GUID comes in / gets used by child objects
                shareManager.GetImportAs(shareDefinition.SharingGuid.ToString(), toCreate);
            }
            

        }
    }

    public enum Tier2DatabaseType
    {
        Logging,
        DataQuality,
        QueryCaching,
        ANOStore,
        IdentifierDump
    }
}
