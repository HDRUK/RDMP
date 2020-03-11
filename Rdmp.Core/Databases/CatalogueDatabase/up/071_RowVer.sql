--Version:4.0.3
--Description: Adds RowVer column to all tables

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CohortAggregateContainer' AND COLUMN_NAME='RowVer')
	ALTER TABLE CohortAggregateContainer                         ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CohortAggregateSubContainer' AND COLUMN_NAME='RowVer')
	ALTER TABLE CohortAggregateSubContainer                      ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CohortIdentificationConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE CohortIdentificationConfiguration                ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CohortAggregateContainer_AggregateConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE CohortAggregateContainer_AggregateConfiguration  ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='GovernanceDocument' AND COLUMN_NAME='RowVer')
	ALTER TABLE GovernanceDocument                               ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='GovernancePeriod' AND COLUMN_NAME='RowVer')
	ALTER TABLE GovernancePeriod                                 ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='GovernancePeriod_Catalogue' AND COLUMN_NAME='RowVer')
	ALTER TABLE GovernancePeriod_Catalogue                       ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='StandardRegex' AND COLUMN_NAME='RowVer')
	ALTER TABLE StandardRegex                                    ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AnyTableSqlParameter' AND COLUMN_NAME='RowVer')
	ALTER TABLE AnyTableSqlParameter                             ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PasswordEncryptionKeyLocation' AND COLUMN_NAME='RowVer')
	ALTER TABLE PasswordEncryptionKeyLocation                    ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Plugin' AND COLUMN_NAME='RowVer')
	ALTER TABLE Plugin                                           ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ANOTable' AND COLUMN_NAME='RowVer')
	ALTER TABLE ANOTable                                         ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateConfiguration                           ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateContinuousDateAxis' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateContinuousDateAxis                      ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateDimension' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateDimension                               ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateFilter' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateFilter                                  ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateFilterContainer' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateFilterContainer                         ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateFilterParameter' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateFilterParameter                         ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateFilterSubContainer' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateFilterSubContainer                      ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateForcedJoin' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateForcedJoin                              ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Catalogue' AND COLUMN_NAME='RowVer')
	ALTER TABLE Catalogue                                        ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CatalogueItem' AND COLUMN_NAME='RowVer')
	ALTER TABLE CatalogueItem                                    ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CatalogueItemIssue' AND COLUMN_NAME='RowVer')
	ALTER TABLE CatalogueItemIssue                               ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ColumnInfo' AND COLUMN_NAME='RowVer')
	ALTER TABLE ColumnInfo                                       ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='JoinableCohortAggregateConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE JoinableCohortAggregateConfiguration             ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='JoinableCohortAggregateConfigurationUse' AND COLUMN_NAME='RowVer')
	ALTER TABLE JoinableCohortAggregateConfigurationUse          ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExternalDatabaseServer' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExternalDatabaseServer                           ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionFilter' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionFilter                                 ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionFilterParameter' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionFilterParameter                        ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionInformation' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionInformation                            ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='IssueSystemUser' AND COLUMN_NAME='RowVer')
	ALTER TABLE IssueSystemUser                                  ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='JoinInfo' AND COLUMN_NAME='RowVer')
	ALTER TABLE JoinInfo                                         ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionFilterParameterSet' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionFilterParameterSet                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadMetadata' AND COLUMN_NAME='RowVer')
	ALTER TABLE LoadMetadata                                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ExtractionFilterParameterSetValue' AND COLUMN_NAME='RowVer')
	ALTER TABLE ExtractionFilterParameterSetValue                ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadModuleAssembly' AND COLUMN_NAME='RowVer')
	ALTER TABLE LoadModuleAssembly                               ADD RowVer rowversion



IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LoadProgress' AND COLUMN_NAME='RowVer')
	ALTER TABLE LoadProgress                                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Favourite' AND COLUMN_NAME='RowVer')
	ALTER TABLE Favourite                                        ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Pipeline' AND COLUMN_NAME='RowVer')
	ALTER TABLE Pipeline                                         ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Lookup' AND COLUMN_NAME='RowVer')
	ALTER TABLE Lookup                                           ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='AggregateTopX' AND COLUMN_NAME='RowVer')
	ALTER TABLE AggregateTopX                                    ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PipelineComponent' AND COLUMN_NAME='RowVer')
	ALTER TABLE PipelineComponent                                ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='LookupCompositeJoinInfo' AND COLUMN_NAME='RowVer')
	ALTER TABLE LookupCompositeJoinInfo                          ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PipelineComponentArgument' AND COLUMN_NAME='RowVer')
	ALTER TABLE PipelineComponentArgument                        ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PreLoadDiscardedColumn' AND COLUMN_NAME='RowVer')
	ALTER TABLE PreLoadDiscardedColumn                           ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProcessTask' AND COLUMN_NAME='RowVer')
	ALTER TABLE ProcessTask                                      ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DashboardLayout' AND COLUMN_NAME='RowVer')
	ALTER TABLE DashboardLayout                                  ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ProcessTaskArgument' AND COLUMN_NAME='RowVer')
	ALTER TABLE ProcessTaskArgument                              ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ServerDefaults' AND COLUMN_NAME='RowVer')
	ALTER TABLE ServerDefaults                                   ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DashboardControl' AND COLUMN_NAME='RowVer')
	ALTER TABLE DashboardControl                                 ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DataAccessCredentials' AND COLUMN_NAME='RowVer')
	ALTER TABLE DataAccessCredentials                            ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SupportingDocument' AND COLUMN_NAME='RowVer')
	ALTER TABLE SupportingDocument                               ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DataAccessCredentials_TableInfo' AND COLUMN_NAME='RowVer')
	ALTER TABLE DataAccessCredentials_TableInfo                  ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='DashboardObjectUse' AND COLUMN_NAME='RowVer')
	ALTER TABLE DashboardObjectUse                               ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='SupportingSQLTable' AND COLUMN_NAME='RowVer')
	ALTER TABLE SupportingSQLTable                               ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TableInfo' AND COLUMN_NAME='RowVer')
	ALTER TABLE TableInfo                                        ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='RemoteRDMP' AND COLUMN_NAME='RowVer')
	ALTER TABLE RemoteRDMP                                       ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ObjectImport' AND COLUMN_NAME='RowVer')
	ALTER TABLE ObjectImport                                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ObjectExport' AND COLUMN_NAME='RowVer')
	ALTER TABLE ObjectExport                                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CacheProgress' AND COLUMN_NAME='RowVer')
	ALTER TABLE CacheProgress                                    ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ConnectionStringKeyword' AND COLUMN_NAME='RowVer')
	ALTER TABLE ConnectionStringKeyword                          ADD RowVer rowversion

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='WindowLayout' AND COLUMN_NAME='RowVer')
	ALTER TABLE WindowLayout                                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='PermissionWindow' AND COLUMN_NAME='RowVer')
	ALTER TABLE PermissionWindow                                 ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TicketingSystemConfiguration' AND COLUMN_NAME='RowVer')
	ALTER TABLE TicketingSystemConfiguration                     ADD RowVer rowversion


IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CacheFetchFailure' AND COLUMN_NAME='RowVer')
	ALTER TABLE CacheFetchFailure                                ADD RowVer rowversion
