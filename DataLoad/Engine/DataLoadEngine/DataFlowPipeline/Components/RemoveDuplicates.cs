using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using CatalogueLibrary.DataFlowPipeline;
using CatalogueLibrary.Repositories;
using ReusableLibraryCode.Checks;
using ReusableLibraryCode.Progress;

namespace DataLoadEngine.DataFlowPipeline.Components
{
    /// <summary>
    /// PipelineComponent which removes 100% duplicate rows from a DataTable during Pipeline execution based on row hashes.
    /// </summary>
    public class RemoveDuplicates :IPluginDataFlowComponent<DataTable>
    {
        Stopwatch sw = new Stopwatch();
        private int totalRecordsProcessed = 0;
        private int totalDuplicatesFound = 0;

        Dictionary<int, List<DataRow>> unqiueHashesSeen = new Dictionary<int, List<DataRow>>();

        public DataTable ProcessPipelineData( DataTable toProcess, IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            sw.Start();
            
            DataTable toReturn = toProcess.Clone();

            //now sort rows
            foreach (DataRow row in toProcess.Rows)
            {
                totalRecordsProcessed++;
                int hashOfItems = GetHashCode(row.ItemArray);

                if (unqiueHashesSeen.ContainsKey(hashOfItems))
                {
                    //GetHashCode on ItemArray of row has been seen before but it could be a collision so call Enumerable.SequenceEqual just incase.
                    if (unqiueHashesSeen[hashOfItems].Any(r => r.ItemArray.SequenceEqual(row.ItemArray)))
                    {
                        totalDuplicatesFound++;
                        continue; //it's a duplicate
                    }

                    unqiueHashesSeen[hashOfItems].Add(row);
                }
                else
                {
                    //its not a duplicate hashcode so add it to the return array and the record of everything we have seen so far (in order that we do not run across issues across batches)
                    unqiueHashesSeen.Add(hashOfItems, new List<DataRow>(new[] { row }));
                }

                toReturn.Rows.Add(row.ItemArray);
            }
            
            sw.Stop();

            listener.OnProgress(this, new ProgressEventArgs("Evaluating For Duplicates", new ProgressMeasurement(totalRecordsProcessed, ProgressType.Records), sw.Elapsed));
            listener.OnProgress(this,new ProgressEventArgs("Discarding Duplicates",new ProgressMeasurement(totalDuplicatesFound, ProgressType.Records),sw.Elapsed));
            
            return toReturn;
        }
        
        public void Dispose(IDataLoadEventListener listener, Exception pipelineFailureExceptionIfAny)
        {
        }

        public void Abort(IDataLoadEventListener listener)
        {
            
        }

        public void Check(ICheckNotifier notifier)
        {
            
        }

        /// <summary>
        /// Gets the hash code for the contents of the array since the default hash code
        /// for an array is unique even if the contents are the same.
        /// </summary>
        /// <remarks>
        /// See Jon Skeet (C# MVP) response in the StackOverflow thread 
        /// http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
        /// </remarks>
        /// <param name="array">The array to generate a hash code for.</param>
        /// <returns>The hash code for the values in the array.</returns>
        public int GetHashCode(object[] array)
        {
            // if non-null array then go into unchecked block to avoid overflow
            if (array != null)
            {
                unchecked
                {
                    int hash = 17;

                    // get hash code for all items in array
                    foreach (var item in array)
                    {
                        hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
                    }

                    return hash;
                }
            }

            // if null, hash code is zero
            return 0;
        }

    }
}
