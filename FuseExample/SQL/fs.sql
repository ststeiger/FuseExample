
IF  EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_FileSystem' )
DROP TABLE dbo.T_FileSystem  
-- DROP TABLE table_name CASCADE;
-- DROP TABLE IF EXISTS table_name CASCADE;
GO




CREATE TABLE dbo.T_FileSystem
(
	 FS_Id bigint NOT NULL
	,FS_TargetFL bigint NOT NULL
	,FS_ParentFL bigint NULL
	,FS_Path national character varying(4000) NULL 
	,FS_LowerCasePath national character varying(4000) NULL 
	,FS_Text national character varying(4000) NULL 
	,FS_LowerCaseText national character varying(4000) NULL 
	,FS_NameWithoutExtension national character varying(4000) NULL 
	,FS_Extension national character varying(255) NULL 
	,FS_IsCompressed bit NULL 
	,FS_IsEncrypted bit NULL 
	,FS_IsReadOnly bit NULL 
	,FS_ReadLock bit NULL 
	,FS_WriteLock bit NULL 
	,FS_CreationTime datetime2(7) NULL 
	,FS_CreationTimeUTC datetime2(7) NULL 
	,FS_LastAccessTime datetime2(7) NULL 
	,FS_LastAccessTimeUTC datetime2(7) NULL 
	,FS_LastWriteTime datetime2(7) NULL 
	,FS_LastWriteTimeUTC datetime2(7) NULL 
	,FS_OwnerId bigint NULL 
	,FS_OwnerGroupId bigint NULL 
	,FS_UnixPermissions int NULL 
	 
	,CONSTRAINT PK_T_FileSystem PRIMARY KEY(FS_Id) 
); 


GO

