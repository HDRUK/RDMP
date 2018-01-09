﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Data.DataLoad;
using ReusableLibraryCode.Checks;

namespace LoadModules.Generic.Mutilators.Dilution
{
    /// <summary>
    /// Describes a way of anonymising a field (ColumnToDilute) by dilution (making data less granular) e.g. rounding dates to the nearest quarter.  Implementation 
    /// must be based on running an SQL query in AdjustStaging.  See Dilution for more information.
    /// </summary>
    public interface IDilutionOperation:ICheckable
    {
        IPreLoadDiscardedColumn ColumnToDilute { set; }

        string GetMutilationSql();
    }
}
