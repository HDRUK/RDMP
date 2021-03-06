--Version:1.24.0.0
--Description: Removes obsolete fields from the CacheProgress table
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CacheProgress' AND COLUMN_NAME='AlternativeCacheLocation')
BEGIN
  ALTER TABLE CacheProgress DROP COLUMN AlternativeCacheLocation
END

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='CacheProgress' AND COLUMN_NAME='RetrieverType')
BEGIN
  ALTER TABLE CacheProgress DROP COLUMN RetrieverType
END