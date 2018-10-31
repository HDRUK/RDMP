﻿using System;
using System.IO;
using CatalogueLibrary.DataFlowPipeline.Requirements;
using CsvHelper;
using CsvHelper.Configuration;
using LoadModules.Generic.Exceptions;
using ReusableLibraryCode.Progress;

namespace LoadModules.Generic.DataFlowSources.SubComponents
{

    /// <summary>
    /// This class is a sub component of <see cref="DelimitedFlatFileDataFlowSource"/>, it is responsible for responding to errors processing the file
    /// being loaded according to the <see cref="BadDataHandlingStrategy"/>. It also includes settings for how to respond to empty files.
    /// </summary>
    public class FlatFileEventHandlers
    {
        private readonly FlatFileToLoad _fileToLoad;
        private readonly FlatFileToDataTablePusher _dataPusher;
        private readonly bool _throwOnEmptyFiles;
        private readonly BadDataHandlingStrategy _strategy;
        private readonly IDataLoadEventListener _listener;
        
        /// <summary>
        /// File where we put error rows
        /// </summary>
        public FileInfo DivertErrorsFile;

        public FlatFileEventHandlers(FlatFileToLoad fileToLoad, FlatFileToDataTablePusher dataPusher, bool throwOnEmptyFiles, BadDataHandlingStrategy strategy, IDataLoadEventListener listener)
        {
            _fileToLoad = fileToLoad;
            _dataPusher = dataPusher;
            _throwOnEmptyFiles = throwOnEmptyFiles;
            _strategy = strategy;
            _listener = listener;
        }

        public void FileIsEmpty()
        {
            if (_throwOnEmptyFiles)
                throw new FlatFileLoadException("File " + _fileToLoad + " is empty");

            _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "File " + _fileToLoad + " is empty"));
        }
        
        public void ReadingExceptionOccurred(CsvHelperException obj)
        {
            switch (_strategy)
            {
                case BadDataHandlingStrategy.IgnoreRows:
                    _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "Ignored ReadingException on line " + obj.ReadingContext.RawRow, obj));
                    //move to next line
                    _dataPusher.BadLines.Add(obj.ReadingContext.RawRow);

                    break;
                case BadDataHandlingStrategy.DivertRows:

                    DivertErrorRow(obj.ReadingContext, obj);
                    break;

                case BadDataHandlingStrategy.ThrowException:
                    throw new FlatFileLoadException("Bad data found on line " + obj.ReadingContext.RawRow, obj);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void BadDataFound(ReadingContext obj)
        {
            switch (_strategy)
            {
                case BadDataHandlingStrategy.IgnoreRows:
                    _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "Ignored BadData on line " + obj.RawRow));

                    //move to next line
                    _dataPusher.BadLines.Add(obj.RawRow);

                    break;
                case BadDataHandlingStrategy.DivertRows:
                    DivertErrorRow(obj, null);
                    break;

                case BadDataHandlingStrategy.ThrowException:
                    throw new FlatFileLoadException("Bad data found on line " + obj.RawRow);


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void DivertErrorRow(ReadingContext context, Exception ex)
        {
            if (DivertErrorsFile == null)
            {
                DivertErrorsFile = new FileInfo(Path.Combine(_fileToLoad.File.Directory.FullName, Path.GetFileNameWithoutExtension(_fileToLoad.File.Name) + "_Errors.txt"));

                //delete any old version
                if (DivertErrorsFile.Exists)
                    DivertErrorsFile.Delete();
            }

            _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "Diverting Error on line " + context.RawRow + " to '" + DivertErrorsFile.FullName + "'", ex));

            File.AppendAllText(DivertErrorsFile.FullName, context.RawRecord);

            //move to next line
            _dataPusher.BadLines.Add(context.RawRow);
        }

        public void RegisterEvents(IReaderConfiguration configuration)
        {
            configuration.BadDataFound = BadDataFound;
            configuration.ReadingExceptionOccurred = ReadingExceptionOccurred;
        }
    }
}
