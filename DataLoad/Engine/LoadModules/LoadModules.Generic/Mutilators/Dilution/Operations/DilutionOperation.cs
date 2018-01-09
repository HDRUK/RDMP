﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Data.DataLoad;
using LoadModules.Generic.Mutilators.Dilution.Exceptions;
using ReusableLibraryCode.Checks;

namespace LoadModules.Generic.Mutilators.Dilution.Operations
{
    /// <summary>
    /// See IDilutionOperation
    /// </summary>
    public abstract class DilutionOperation : IPluginDilutionOperation
    {
        public IPreLoadDiscardedColumn ColumnToDilute { set; protected get; }

        public virtual void Check(ICheckNotifier notifier)
        {
            if(ColumnToDilute == null)
                throw new DilutionColumnNotSetException("ColumnToDilute has not been set yet, this is the column which will be diluted and is usually set by the DilutionOperationFactory but it is null");

            if (string.IsNullOrWhiteSpace(ColumnToDilute.SqlDataType))
                notifier.OnCheckPerformed(new CheckEventArgs("IPreLoadDiscardedColumn " + ColumnToDilute + " is of unknown datatype", CheckResult.Fail));
        }
        
        public abstract string GetMutilationSql();
    }
}
