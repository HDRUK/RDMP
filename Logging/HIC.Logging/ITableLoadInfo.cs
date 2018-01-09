﻿using ReusableLibraryCode.DatabaseHelpers.Discovery;

namespace HIC.Logging
{
    /// <summary>
    /// See TableLoadInfo
    /// </summary>
    public interface ITableLoadInfo
    {
        int ID { get; }
        int Inserts { get; set; }
        int Updates { get; set; }
        int DiscardedDuplicates { get; set; }

        string Notes { get; set; }
        DiscoveredServer DatabaseSettings { get; }

        void CloseAndArchive();
        void IncrementErrorRows();
    }
}