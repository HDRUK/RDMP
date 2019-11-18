// Copyright (c) The University of Dundee 2018-2019
// This file is part of the Research Data Management Platform (RDMP).
// RDMP is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// RDMP is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with RDMP. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Rdmp.Core.Caching.Pipeline;
using Rdmp.Core.Curation.Data;
using Rdmp.Core.Curation.Data.Cache;
using Rdmp.Core.DataFlowPipeline;
using Rdmp.Core.Repositories;
using ReusableLibraryCode;
using ReusableLibraryCode.Progress;

namespace Rdmp.Core.Caching
{
    /// <summary>
    /// The CachingHost has two public interfaces: 'Start' and 'StartDaemon'. 
    /// 'Start' is a one-shot mode where any available CacheProgress records are cached until completion (this could be a very long 'one-shot').
    /// 'StartDaemon' continually attempt to cache available CacheProgress records until cancelled. This mode will keep the caches up-to-date.
    /// </summary>
    public class CachingHost
    {
        /// <summary>
        /// The cacheable tasks that the host will be running
        /// </summary>
        public List<ICacheProgress> CacheProgressList { get; set; }
        
        /// <summary>
        /// True if the host is attempting to back fill the cache with failed date ranges from the past
        /// </summary>
        public bool RetryMode { get; set; }
        
        private readonly ICatalogueRepository _repository;
        
        // this is more because we can't retrieve CacheWindows from LoadProgresss (yet) 
        private List<PermissionWindowCacheDownloader> _downloaders;
        
        /// <summary>
        /// True to shut down once the <see cref="PermissionWindow"/> for the <see cref="CacheProgressList"/> is exceeded.  False
        /// to sleep until it becomes permissible again.
        /// </summary>
        public bool TerminateIfOutsidePermissionWindow { get; set; }

        /// <summary>
        /// Creates a new <see cref="CachingHost"/> connected to the RDMP <paramref name="repository"/>.
        /// </summary>
        /// <param name="repository"></param>
        public CachingHost(ICatalogueRepository repository)
        {
            _repository = repository;
            RetryMode = false;
        }

        /// <summary>
        /// Runs the first (which must be the only) <see cref="CacheProgressList"/> 
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="cancellationToken"></param>
        public void Start(IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            if (CacheProgressList.Count > 1)
                throw new InvalidOperationException(
                    "Currently this entrypoint only supports single CacheProgress retrieval, ensure CacheProgressList only has one item (it has " +
                    CacheProgressList.Count + ")");

            var cacheProgress = CacheProgressList[0];
            var permissionWindow = cacheProgress.PermissionWindow;

            _downloaders = new List<PermissionWindowCacheDownloader>
            {
                new PermissionWindowCacheDownloader(permissionWindow, CacheProgressList, _repository, new RoundRobinPipelineExecution())
            };


            RetrieveNewDataForCache(listener, cancellationToken);
        }

        public void StartDaemon(IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            const int sleepInSeconds = 60;

            while (!cancellationToken.IsCancellationRequested)
            {
                RetrieveNewDataForCache(listener, cancellationToken);
                listener.OnNotify(this,
                    new NotifyEventArgs(ProgressEventType.Information, "Sleeping for " + sleepInSeconds + " seconds"));

                // wake up every sleepInSeconds to re-check if we can download any new data, but check more regularly to see if cancellation has been requested
                var beenAsleepFor = new Stopwatch();
                beenAsleepFor.Start();
                while (beenAsleepFor.ElapsedMilliseconds < (sleepInSeconds*1000))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        listener.OnNotify(this,
                            new NotifyEventArgs(ProgressEventType.Information, "Cancellation has been requested"));
                        break;
                    }

                    Task.Delay(100).Wait();
                }
            }

            listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Daemon has stopped"));
        }

        public void CacheUsingPermissionWindows(List<IPermissionWindow> permissionWindowList, IDataLoadEventListener listener, GracefulCancellationToken token)
        {
            _downloaders = new List<PermissionWindowCacheDownloader>();

            foreach (var permissionWindow in permissionWindowList)
            {
                listener.OnNotify(this,
                    new NotifyEventArgs(ProgressEventType.Information,
                        "Creating download for permission window: " + permissionWindow.Name));
                _downloaders.Add(new PermissionWindowCacheDownloader(permissionWindow,  _repository,
                    new RoundRobinPipelineExecution()));
            }

            StartDaemon(listener, token);
        }

        public void CacheUsingCacheProgresses(List<ICacheProgress> cacheProgressList, IDataLoadEventListener listener, GracefulCancellationToken token)
        {
            // organise the CacheProgress items into CacheSets, based on any PermissionWindows
            if (CacheProgressList == null)
                CacheProgressList = cacheProgressList;
            else
                throw new NotSupportedException("CacheProgressList property has already been set... thats probably a problem right? TN2016-08-25");

            _downloaders = new List<PermissionWindowCacheDownloader>();
            _downloaders.Add(new PermissionWindowCacheDownloader(null, CacheProgressList, _repository, new RoundRobinPipelineExecution()));

            StartDaemon(listener, token);
        }

        private void RetrieveNewDataForCache(IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Retrieving new data"));

            var combinedToken = cancellationToken.CreateLinkedSource().Token;

            // Start a task for each cache download permission window and wait until completion
            var tasks =
                _downloaders.Select(
                    downloader =>
                        Task.Run(() => DownloadUntilFinished(downloader, listener, cancellationToken), combinedToken))
                    .ToArray();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException e)
            {
                var operationCanceledException = e.GetExceptionIfExists<OperationCanceledException>();
                if (operationCanceledException != null)
                    listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "Operation cancelled", e));
                else
                {
                    listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Error, "Exception in downloader task whilst caching data", e));
                    throw;
                }
            }
        }

        private void DownloadUntilFinished(PermissionWindowCacheDownloader downloader, IDataLoadEventListener listener, GracefulCancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = RetryMode ? 
                        downloader.RetryDownload(listener, cancellationToken) :
                        downloader.Download(listener, cancellationToken);

                    switch (result)
                    {
                        case RetrievalResult.NotPermitted:

                            if(TerminateIfOutsidePermissionWindow)
                            {
                                listener.OnNotify(this,
                                    new NotifyEventArgs(ProgressEventType.Information,
                                        "Download not permitted at this time so exitting"));

                                return;
                            }

                            listener.OnNotify(this,
                                new NotifyEventArgs(ProgressEventType.Information,
                                    "Download not permitted at this time, sleeping for 60 seconds"));

                            // Sleep for a while, but keep one eye open for cancellation requests
                            const int sleepTime = 60000;
                            const int cancellationCheckInterval = 1000;
                            var elapsedTime = 0;
                            while (elapsedTime < sleepTime)
                            {
                                Task.Delay(cancellationCheckInterval).Wait();
                                cancellationToken.ThrowIfCancellationRequested();
                                elapsedTime += cancellationCheckInterval;
                            }
                            break;
                        case RetrievalResult.Complete:
                            listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Download completed successfully."));
                            return;
                        default:
                            listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Information, "Download ended: " + result));
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                listener.OnNotify(this, new NotifyEventArgs(ProgressEventType.Warning, "Cache download cancelled: " + downloader));
            }
        }
    }
}
