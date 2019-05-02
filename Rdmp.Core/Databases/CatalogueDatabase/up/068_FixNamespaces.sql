--Version:2.13.0.10
--Description: Fixes the namespaces of refactored load modules / pipeline components (see Patch68FixNamespacesTest for tests of this patch)

-- Update in all places
  UPDATE PipelineComponent SET Class = REPLACE(Class,'CatalogueLibrary.Data','Rdmp.Core.Curation.Data')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'DataLoadEngine.DataFlowPipeline','Rdmp.Core.DataLoad.Engine.Pipeline')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'DataLoadEngine.LoadModules','Rdmp.Core.DataLoad.LoadModules')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'CachingEngine.PipelineExecution.','Rdmp.Core.Caching.Pipeline.')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'CatalogueLibrary.ExitCodeType','Rdmp.Core.DataLoad.ExitCodeType')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'CatalogueLibrary.Repositories','Rdmp.Core.Repositories')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'DataExportLibrary.CohortCreationPipeline','Rdmp.Core.CohortCommitting.Pipeline')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'DataExportLibrary.DataRelease.ReleasePipeline','Rdmp.Core.DataExport.DataRelease.Pipeline')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'DataExportLibrary.ExtractionTime.ExtractionPipeline.','Rdmp.Core.DataExport.DataExtraction.Pipeline.')
  UPDATE PipelineComponent SET Class = REPLACE(Class,'LoadModules.Generic.','Rdmp.Core.DataLoad.Modules.')

  
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'CatalogueLibrary.Data','Rdmp.Core.Curation.Data')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'DataLoadEngine.DataFlowPipeline','Rdmp.Core.DataLoad.Engine.Pipeline')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'DataLoadEngine.LoadModules','Rdmp.Core.DataLoad.LoadModules')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'CachingEngine.PipelineExecution.','Rdmp.Core.Caching.Pipeline.')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'CatalogueLibrary.ExitCodeType','Rdmp.Core.DataLoad.ExitCodeType')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'CatalogueLibrary.Repositories','Rdmp.Core.Repositories')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'DataExportLibrary.CohortCreationPipeline','Rdmp.Core.CohortCommitting.Pipeline')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'DataExportLibrary.DataRelease.ReleasePipeline','Rdmp.Core.DataExport.DataRelease.Pipeline')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'DataExportLibrary.ExtractionTime.ExtractionPipeline.','Rdmp.Core.DataExport.DataExtraction.Pipeline.')
  UPDATE PipelineComponentArgument SET Type = REPLACE(Type,'LoadModules.Generic.','Rdmp.Core.DataLoad.Modules.')

  UPDATE ProcessTask SET Path = REPLACE(Path,'CatalogueLibrary.Data','Rdmp.Core.Curation.Data')
  UPDATE ProcessTask SET Path = REPLACE(Path,'DataLoadEngine.DataFlowPipeline','Rdmp.Core.DataLoad.Engine.Pipeline')
  UPDATE ProcessTask SET Path = REPLACE(Path,'DataLoadEngine.LoadModules','Rdmp.Core.DataLoad.LoadModules')
  UPDATE ProcessTask SET Path = REPLACE(Path,'CachingEngine.PipelineExecution.','Rdmp.Core.Caching.Pipeline.')
  UPDATE ProcessTask SET Path = REPLACE(Path,'CatalogueLibrary.ExitCodeType','Rdmp.Core.DataLoad.ExitCodeType')
  UPDATE ProcessTask SET Path = REPLACE(Path,'CatalogueLibrary.Repositories','Rdmp.Core.Repositories')
  UPDATE ProcessTask SET Path = REPLACE(Path,'DataExportLibrary.CohortCreationPipeline','Rdmp.Core.CohortCommitting.Pipeline')
  UPDATE ProcessTask SET Path = REPLACE(Path,'DataExportLibrary.DataRelease.ReleasePipeline','Rdmp.Core.DataExport.DataRelease.Pipeline')
  UPDATE ProcessTask SET Path = REPLACE(Path,'DataExportLibrary.ExtractionTime.ExtractionPipeline.','Rdmp.Core.DataExport.DataExtraction.Pipeline.')
  UPDATE ProcessTask SET Path = REPLACE(Path,'LoadModules.Generic.','Rdmp.Core.DataLoad.Modules.')
  
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'CatalogueLibrary.Data','Rdmp.Core.Curation.Data')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'DataLoadEngine.DataFlowPipeline','Rdmp.Core.DataLoad.Engine.Pipeline')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'DataLoadEngine.LoadModules','Rdmp.Core.DataLoad.LoadModules')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'CachingEngine.PipelineExecution.','Rdmp.Core.Caching.Pipeline.')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'CatalogueLibrary.ExitCodeType','Rdmp.Core.DataLoad.ExitCodeType')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'CatalogueLibrary.Repositories','Rdmp.Core.Repositories')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'DataExportLibrary.CohortCreationPipeline','Rdmp.Core.CohortCommitting.Pipeline')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'DataExportLibrary.DataRelease.ReleasePipeline','Rdmp.Core.DataExport.DataRelease.Pipeline')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'DataExportLibrary.ExtractionTime.ExtractionPipeline.','Rdmp.Core.DataExport.DataExtraction.Pipeline.')
  UPDATE ProcessTaskArgument SET Type = REPLACE(Type,'LoadModules.Generic.','Rdmp.Core.DataLoad.Modules.')


  