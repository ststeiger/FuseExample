
-- DROP TABLE table_name CASCADE;
-- DROP TABLE IF EXISTS table_name CASCADE;


CREATE TABLE IF NOT EXISTS T_FileSystem
(
	 FS_Id bigint NOT NULL
	,FS_Target_FS_Id bigint NOT NULL
	,FS_Parent_FS_Id bigint NULL
	,FS_Path national character varying(4000) NULL 
	,FS_LowerCasePath national character varying(4000) NULL 
	,FS_Text national character varying(4000) NULL 
	,FS_LowerCaseText national character varying(4000) NULL 
	,FS_NameWithoutExtension national character varying(4000) NULL 
	,FS_Extension national character varying(255) NULL 
	,FS_IsFolder bit NULL 
	,FS_IsCompressed bit NULL 
	,FS_IsEncrypted bit NULL 
	,FS_IsReadOnly bit NULL 
	,FS_ReadLock bit NULL 
	,FS_WriteLock bit NULL 


	,FS_CreationTime timestamp without time zone
	,FS_CreationTimeUTC timestamp without time zone

	,FS_LastAccessTime timestamp without time zone
	,FS_LastAccessTimeUTC timestamp without time zone

	,FS_LastWriteTime timestamp without time zone
	,FS_LastWriteTimeUTC timestamp without time zone

	,FS_OwnerId bigint NULL 
	,FS_OwnerGroupId bigint NULL 
	,FS_UnixPermissions int NULL 
	 
	,CONSTRAINT PK_T_FileSystem PRIMARY KEY(FS_Id) 
); 
