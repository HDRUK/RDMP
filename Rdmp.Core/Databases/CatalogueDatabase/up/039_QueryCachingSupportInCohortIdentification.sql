--Version:1.33.0.0
--Description: Allows you to choose an ExternalDatabaseServer for Cohort identification query result caching. Add NOT NULL constraint to AggregateConfiguration..Name.
if not exists (select 1 from sys.columns where name = 'QueryCachingServer_ID')
begin
	ALTER TABLE CohortIdentificationConfiguration Add QueryCachingServer_ID int 
end
 
if not exists (select 1 from sys.foreign_keys where name = 'FK_CohortIdentificationConfiguration_ExternalDatabaseServer')
 begin
	ALTER TABLE [dbo].[CohortIdentificationConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_CohortIdentificationConfiguration_ExternalDatabaseServer] FOREIGN KEY([QueryCachingServer_ID])
	REFERENCES [dbo].[ExternalDatabaseServer] ([ID])
end

UPDATE [AggregateConfiguration] SET Name = 'No name' WHERE [Name] IS NULL
ALTER TABLE [AggregateConfiguration] ALTER COLUMN Name varchar(500) NOT NULL