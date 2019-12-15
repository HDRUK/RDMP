// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FAnsi.Discovery;
using MapsDirectlyToDatabaseTable;
using MapsDirectlyToDatabaseTable.Attributes;
using MapsDirectlyToDatabaseTable.Versioning;
using Rdmp.Core.CommandExecution.AtomicCommands;
using Rdmp.Core.CommandLine.Interactive.Picking;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Repositories;
using Rdmp.Core.Repositories.Construction;
using ReusableLibraryCode.Checks;

namespace Rdmp.Core.CommandExecution
{
    public class CommandInvoker
    {
        private readonly IBasicActivateItems _basicActivator;
        private readonly IRDMPPlatformRepositoryServiceLocator _repositoryLocator;
        
        /// <summary>
        /// Delegates provided by <see cref="_basicActivator"/> for fulfilling constructor arguments of the key Type
        /// </summary>
        private List<CommandInvokerDelegate> _argumentDelegates;

        /// <summary>
        /// Called when the user attempts to run a command marked <see cref="ICommandExecution.IsImpossible"/>
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandImpossible;
        
        /// <summary>
        /// Called when a command completes successfully
        /// </summary>
        public event EventHandler<CommandEventArgs> CommandCompleted;

        public CommandInvoker(IBasicActivateItems basicActivator)
        {
            _basicActivator = basicActivator;
            _repositoryLocator = basicActivator.RepositoryLocator;

            _argumentDelegates = _basicActivator.GetDelegates();

            
            AddDelegate(typeof(ICatalogueRepository),true,(p)=>_repositoryLocator.CatalogueRepository);
            AddDelegate(typeof(IDataExportRepository),true,(p)=>_repositoryLocator.DataExportRepository);
            AddDelegate(typeof(IBasicActivateItems),true,(p)=>_basicActivator);
            AddDelegate(typeof(IRDMPPlatformRepositoryServiceLocator),true,(p)=>_repositoryLocator);
            AddDelegate(typeof(DirectoryInfo), false,(p) => _basicActivator.SelectDirectory($"Enter Directory for '{p.Name}'"));
            AddDelegate(typeof(FileInfo), false,(p) => _basicActivator.SelectFile($"Enter File for '{p.Name}'"));

            AddDelegate(typeof(string), false,(p) =>
                _basicActivator.TypeText("Value needed for parameter", p.Name, 1000, null, out string result, false)
                ? result
                : null);

            AddDelegate(typeof(Type), false,(p) =>_basicActivator.SelectType($"Type needed for {p.Name} ",p.DemandIfAny?.TypeOf));
                

            AddDelegate(typeof(DiscoveredDatabase),false,(p)=>_basicActivator.SelectDatabase(true,"Value needed for parameter " + p.Name));
            AddDelegate(typeof(DiscoveredTable),false,(p)=>_basicActivator.SelectTable(true,"Value needed for parameter " + p.Name));

            AddDelegate(typeof(DatabaseEntity),false, (p) =>_basicActivator.SelectOne(p.Name, GetAllObjectsOfType(p.Type)));
            AddDelegate(typeof(IMightBeDeprecated),false, SelectOne<IMightBeDeprecated>);
            AddDelegate(typeof(IDisableable),false, SelectOne<IDisableable>);
            AddDelegate(typeof(INamed),false, SelectOne<INamed>);
            AddDelegate(typeof(IDeleteable),false, SelectOne<IDeleteable>);

            AddDelegate(typeof(Enum),false,(p)=>_basicActivator.SelectEnum("Value needed for parameter " + p.Name , p.Type, out Enum chosen)?chosen:null);


            _argumentDelegates.Add(new CommandInvokerArrayDelegate(typeof(IMapsDirectlyToDatabaseTable),false,(p)=>
            {
                IMapsDirectlyToDatabaseTable[] available = GetAllObjectsOfType(p.Type.GetElementType());
                return _basicActivator.SelectMany(p.Name,p.Type.GetElementType(), available);
              
            }));
                   

            AddDelegate(typeof(ICheckable), false,
                (p)=>_basicActivator.SelectOne(p.Name, 
                    _basicActivator.GetAll<ICheckable>()
                        .Where(p.Type.IsInstanceOfType)
                        .Cast<IMapsDirectlyToDatabaseTable>()
                        .ToArray())
                );

            AddDelegate(typeof(IPatcher),false, (p) =>
                {
                    var patcherType = _basicActivator.SelectType("Select Patcher (if any)", typeof(IPatcher));
                    if (patcherType == null)
                        return null;
                    
                    try
                    {
                        return Activator.CreateInstance(patcherType);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to call/find blank constructor of IPatcher Type '{patcherType}'",e);
                    }
                }
            );

            _argumentDelegates.Add(new CommandInvokerValueTypeDelegate((p)=>
            _basicActivator.SelectValueType(p.Name,p.Type, null)));

        }

        private void AddDelegate(Type type,bool isAuto, Func<RequiredArgument, object> func)
        {
            _argumentDelegates.Add(new CommandInvokerDelegate(type,isAuto,func));
        }

        
        public IEnumerable<Type> GetSupportedCommands()
        {
            
            return _basicActivator.RepositoryLocator.CatalogueRepository.MEF.GetAllTypes().Where(IsSupported);
        }

        /// <summary>
        /// Constructs an instance of the <see cref="IAtomicCommand"/> and executes it.  Constructor parameters
        /// are populated from the (optional) <paramref name="picker"/> or the <see cref="IBasicActivateItems"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="picker"></param>
        public void ExecuteCommand(Type type, CommandLineObjectPicker picker)
        {
            ExecuteCommand(GetConstructor(type),picker);
        }
        private void ExecuteCommand(ConstructorInfo constructorInfo, CommandLineObjectPicker picker)
        {
            List<object> parameterValues = new List<object>();
            
            int idx = 0;

            foreach (var parameterInfo in constructorInfo.GetParameters())
            {
                object value = null;

                //if we have argument values specified
                if (picker != null)
                {
                    //and the specified value matches the expected parameter type
                    if (picker.HasArgumentOfType(idx, parameterInfo.ParameterType))
                    {
                        //consume a value
                        value = picker[idx].GetValueForParameterOfType(parameterInfo.ParameterType);
                        idx++;
                    }
                }
                
                if(value == null) 
                    value = GetValueForParameterOfType(parameterInfo);
                
                //if it's a null and not a default null
                if(value == null && !parameterInfo.HasDefaultValue)
                    throw new OperationCanceledException("Could not figure out a value for property '" + parameterInfo + "' for constructor '" + constructorInfo + "'.  Parameter Type was '" + parameterInfo.ParameterType + "'");

                parameterValues.Add(value);
            }
            if(picker != null && idx < picker.Length)
                throw new Exception("Unrecognised extra parameter " + picker[idx].RawValue);

            var instance = (IAtomicCommand)constructorInfo.Invoke(parameterValues.ToArray());
        
            if (instance.IsImpossible)
            {
                CommandImpossible?.Invoke(this,new CommandEventArgs(instance));
                return;
            }

            instance.Execute();
            CommandCompleted?.Invoke(this,new CommandEventArgs(instance));
        }

        public object GetValueForParameterOfType(PropertyInfo propertyInfo)
        {
            return GetValueFor(new RequiredArgument(propertyInfo));
        }

        public object GetValueForParameterOfType(ParameterInfo parameterInfo)
        {
            return GetValueFor(new RequiredArgument(parameterInfo));
        }

        private object GetValueFor(RequiredArgument a)
        {
            return GetDelegate(a)?.Run(a);
        }

        private T SelectOne<T>(RequiredArgument parameterInfo)
        {
            return (T)_basicActivator.SelectOne(parameterInfo.Name,_basicActivator.GetAll<T>().Cast<IMapsDirectlyToDatabaseTable>().ToArray());
        }

        public bool IsSupported(ConstructorInfo c)
        {
            return c.GetParameters().All(IsSupported);
        }

        private bool IsSupported(ParameterInfo p)
        {
            return GetDelegate(new RequiredArgument(p)) != null;
        }

        public CommandInvokerDelegate GetDelegate(RequiredArgument required)
        {
            var match =  _argumentDelegates.FirstOrDefault(k=>k.CanHandle(required.Type));

            if(match != null)
                return match;


            if(required.DefaultValue != null)
                return new CommandInvokerFixedValueDelegate(required.DefaultValue);

            return null;

        }
        public bool IsSupported(Type t)
        {
            bool acceptableType = typeof(IAtomicCommand).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface;

            if (!acceptableType)
                return false;

            if (_basicActivator.GetIgnoredCommands().Contains(t))
                return false;

            try
            {
                var constructor = GetConstructor(t);

                if (constructor == null)
                    return false;

                return IsSupported(constructor);

            }
            catch (Exception)
            {
                return false;
            }
        }


        public virtual ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length == 0)
                return null;

            var importDecorated = constructors.Where(c => Attribute.IsDefined(c, typeof(UseWithObjectConstructorAttribute))).ToArray();

            if (importDecorated.Any())
                return importDecorated[0];

            return constructors[0];
        }
        private IMapsDirectlyToDatabaseTable[] GetAllObjectsOfType(Type type)
        {
            if (type.IsAbstract || type.IsInterface)
                return _basicActivator.GetAll(type).Cast<IMapsDirectlyToDatabaseTable>().ToArray();

            if (_repositoryLocator.CatalogueRepository.SupportsObjectType(type))
                return  _repositoryLocator.CatalogueRepository.GetAllObjects(type).ToArray();
            if (_repositoryLocator.DataExportRepository.SupportsObjectType(type))
                return _repositoryLocator.DataExportRepository.GetAllObjects(type).ToArray();
            
            return null;
        }
    }
}
