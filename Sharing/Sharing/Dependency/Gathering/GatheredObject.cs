using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.ImportExport;
using CatalogueLibrary.Data.Serialization;
using MapsDirectlyToDatabaseTable;
using MapsDirectlyToDatabaseTable.Attributes;
using ReusableLibraryCode;

namespace Sharing.Dependency.Gathering
{
    /// <summary>
    /// The described Object is only tenously related to the original object and you shouldn't worry too much if during refactoring you don't find any references. 
    /// An example of this would be all Filters in a Catalogue where a single ColumnInfo is being renamed.  Any filter in the catalogue could contain a reference to
    /// the ColumnInfo but most won't.
    ///
    /// <para>Describes an RDMP object that is related to another e.g. a ColumnInfo can have 0+ CatalogueItems associated with it.  This differs from IHasDependencies by the fact that
    /// it is a more constrained set rather than just spider webbing out everywhere.</para>
    /// </summary>
    public class GatheredObject : IHasDependencies, IMasqueradeAs
    {
        public IMapsDirectlyToDatabaseTable Object { get; set; } 
        public List<GatheredObject> Children { get; private set; }

        public GatheredObject(IMapsDirectlyToDatabaseTable o)
        {
            Object = o;
            Children = new List<GatheredObject>();
        }

        /// <summary>
        /// True if the gathered object is a data export object (e.g. it is an ExtractableColumn or DeployedExtractionFilter) and it is part of a frozen (released)
        /// ExtractionConfiguration 
        /// </summary>
        public bool IsReleased { get; set; }
        
        /// <summary>
        /// Creates a sharing export (<see cref="ObjectExport"/>) for the current <see cref="GatheredObject.Object"/> and then serializes it as a <see cref="ShareDefinition"/>.  
        /// This includes mapping any [<see cref="RelationshipAttribute"/>] properties on the <see cref="GatheredObject.Object"/> to the relevant Share Guid (which must
        /// exist in branchParents).
        /// 
        /// <para>ToShareDefinitionWithChildren if you want a full list of shares for the whole tree</para>
        /// </summary>
        /// <param name="shareManager"></param>
        /// <param name="branchParents"></param>
        /// <returns></returns>
        public ShareDefinition ToShareDefinition(ShareManager shareManager,List<ShareDefinition> branchParents)
        {
            var export = shareManager.GetNewOrExistingExportFor(Object);

            Dictionary<string,object> properties = new Dictionary<string, object>();
            Dictionary<RelationshipAttribute,Guid> relationshipProperties = new Dictionary<RelationshipAttribute, Guid>();

            AttributePropertyFinder<RelationshipAttribute> relationshipFinder = new AttributePropertyFinder<RelationshipAttribute>(Object);
            AttributePropertyFinder<NoMappingToDatabase> noMappingFinder = new AttributePropertyFinder<NoMappingToDatabase>(Object);

            
            //for each property in the Object class
            foreach (PropertyInfo property in Object.GetType().GetProperties())
            {
                //if it's the ID column skip it
                if(property.Name == "ID")
                    continue;
                
                //skip [NoMapping] columns
                if(noMappingFinder.GetAttribute(property) != null)
                    continue;

                //skip IRepositories (these tell you where the object came from)
                if (typeof(IRepository).IsAssignableFrom(property.PropertyType))
                    continue;

                RelationshipAttribute attribute = relationshipFinder.GetAttribute(property);

                //if it's a relationship
                if (attribute != null)
                {
                    var idOfParent = property.GetValue(Object);
                    Type typeOfParent = attribute.Cref;

                    var parent = branchParents.Single(d => d.Type == typeOfParent && d.ID.Equals(idOfParent));
                    relationshipProperties.Add(attribute, parent.SharingGuid);
                }
                else
                    properties.Add(property.Name, property.GetValue(Object));
            }

            return new ShareDefinition(export.SharingUIDAsGuid,Object.ID,Object.GetType(),properties,relationshipProperties);
        }

        /// <summary>
        /// Creates sharing exports (<see cref="ObjectExport"/>) for the current <see cref="GatheredObject.Object"/> and all <see cref="GatheredObject.Children"/> and 
        /// then serializes them as <see cref="ShareDefinition"/>
        /// </summary>
        /// <param name="shareManager"></param>
        /// <returns></returns>
        public List<ShareDefinition> ToShareDefinitionWithChildren(ShareManager shareManager)
        {
            return ToShareDefinitionWithChildren(shareManager, new List<ShareDefinition>());
        }

        private List<ShareDefinition> ToShareDefinitionWithChildren(ShareManager shareManager, List<ShareDefinition> branchParents)
        {
            var me = ToShareDefinition(shareManager, branchParents);
            
            var toReturn = new List<ShareDefinition>();
            var parents = new List<ShareDefinition>(branchParents);
            parents.Add(me);
            toReturn.Add(me);

            foreach (GatheredObject child in Children)
                toReturn.AddRange(child.ToShareDefinitionWithChildren(shareManager, parents));

            return toReturn;
        }

        #region Equality
        protected bool Equals(GatheredObject other)
        {
            return Equals(Object, other.Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GatheredObject) obj);
        }

        public override int GetHashCode()
        {
            return (Object != null ? Object.GetHashCode() : 0);
        }

        public object MasqueradingAs()
        {
            return Object;
        }

        public static bool operator ==(GatheredObject left, GatheredObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GatheredObject left, GatheredObject right)
        {
            return !Equals(left, right);
        }
        #endregion

        public IHasDependencies[] GetObjectsThisDependsOn()
        {
            return new IHasDependencies[0];
        }

        public IHasDependencies[] GetObjectsDependingOnThis()
        {
            return Children.ToArray();
        }
    }
}
