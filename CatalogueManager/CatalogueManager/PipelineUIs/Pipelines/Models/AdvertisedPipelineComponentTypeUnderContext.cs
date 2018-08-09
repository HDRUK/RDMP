﻿using System;
using System.Collections.Generic;
using System.Linq;
using CatalogueLibrary.Data.Pipelines;
using CatalogueManager.PipelineUIs.DataObjects;

namespace CatalogueManager.PipelineUIs.Pipelines.Models
{
    /// <summary>
    /// Describes an IDataFlowComponent which may or may not be compatible with a specific DataFlowPipelineContext.  It describes how/if it's requirements conflict with the context
    /// e.g. a DelimitedDataFlowSource requires a FlatFileToLoad and is therefore incompatible under any context where that object is not available.
    /// </summary>
    internal class AdvertisedPipelineComponentTypeUnderContext
    {
        private bool _allowableUnderContext;
        private string _allowableReason;

        private readonly DataFlowComponentVisualisation.Role _role;
        private Type _componentType;


        private List<Type> unmetRequirements = new List<Type>();

        public AdvertisedPipelineComponentTypeUnderContext(Type componentType, IPipelineUseCase useCase)
        {
            _componentType = componentType;

            _role = DataFlowComponentVisualisation.GetRoleFor(componentType);

            var context = useCase.GetContext();

            _allowableUnderContext = context.IsAllowable(componentType, out _allowableReason);

            Type[] initializationTypes;

            var initializationObjects = useCase.GetInitializationObjects();

            //it is permitted to specify only Types as initialization objects if it is design time and the user hasn't picked any objects to execute the use case under
            if (useCase.IsDesignTime && initializationObjects.All(t=>t is Type))
                initializationTypes = initializationObjects.Cast<Type>().ToArray();
            else
                initializationTypes = useCase.GetInitializationObjects().Select(o => o.GetType()).ToArray();

            foreach (var requiredInputType in context.GetIPipelineRequirementsForType(componentType))
                //if there are no initialization objects that are instances of an IPipelineRequirement<T> then we cannot satisfy the components pipeline requirements (e.g. a component  DelimitedFlatFileDataFlowSource requires a FlatFileToLoad but pipeline is trying to load from a database reference)
                if (!initializationTypes.Any(available => requiredInputType == available || requiredInputType.IsAssignableFrom(available)))
                    unmetRequirements.Add(requiredInputType);
        }

        public Type GetComponentType()
        {
            return _componentType;
        }

        public string Namespace()
        {
            return _componentType.Namespace;
        }

        public override string ToString()
        {
            return _componentType.Name;
        }

        public DataFlowComponentVisualisation.Role GetRole()
        {
            return _role;
        }
        public string UIDescribeCompatible()
        {
            return _allowableUnderContext && !unmetRequirements.Any()? "Yes" : "No";
        }

        public bool IsCompatible()
        {
            return _allowableUnderContext && !unmetRequirements.Any();
        }

        public string GetReasonIncompatible()
        {
            string toReturn = _allowableReason;

            if (unmetRequirements.Any())
            {
                if (!string.IsNullOrWhiteSpace(toReturn))
                    toReturn += " and the";
                else
                toReturn += "The";

                toReturn +=
                    " following types are required by the component but not available as input objects to the pipeline " +
                    string.Join(",", unmetRequirements);

            }
            
            return toReturn;
        }
    }
}