﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatalogueLibrary.Data;
using CatalogueLibrary.Data.Automation;
using CatalogueLibrary.Data.Cache;
using CatalogueLibrary.DataFlowPipeline;
using CatalogueLibrary.Repositories;
using RDMPAutomationService.Interfaces;
using RDMPAutomationService.Logic.Cache;
using ReusableLibraryCode.Progress;

namespace RDMPAutomationService.Pipeline.Sources
{
    /// <summary>
    /// Identifies when a new CacheProgress can be executed (according to CacheRunFinder), packages it as an AutomatedCacheRun and releases it into the 
    /// automation pipeline for scheduling/execution.
    /// </summary>
    public class CacheAutomationSource:IAutomationSource
    {
        private AutomationServiceSlot _slot;
        private IDataLoadEventListener _listener;

        public OnGoingAutomationTask GetChunk(IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            if (_slot.IsAcceptingNewJobs(AutomationJobType.Cache))
            {
                _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Slot is accepting new CacheLoad jobs... let's find one."));
                var finder = new CacheRunFinder((ICatalogueRepository) _slot.Repository, listener);
                CacheProgress toRun = finder.SuggestCacheProgress();

                if (toRun != null)
                    return new AutomatedCacheRun(_slot, toRun).GetTask();

                return null;
            }

            _listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Slot is not accepting new CacheLoad jobs..."));
            return null;
        }

        public void Dispose(IDataLoadEventListener listener, Exception pipelineFailureExceptionIfAny)
        {
        }

        public void Abort(IDataLoadEventListener listener)
        {
            
        }

        public OnGoingAutomationTask TryGetPreview()
        {
            throw new NotSupportedException();
        }

        public void PreInitialize(AutomationServiceSlot value, IDataLoadEventListener listener)
        {
            _slot = value;
            _listener = listener;
        }
    }
}