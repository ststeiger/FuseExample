
-- DELETE FROM T_FileSystem; 

INSERT INTO T_FileSystem
(
	 FS_Id
	,FS_TargetFL
	,FS_ParentFL
	,FS_Path
	,FS_LowerCasePath
	,FS_Text
	,FS_LowerCaseText
	,FS_NameWithoutExtension
	,FS_Extension
	,FS_IsCompressed
	,FS_IsEncrypted
	,FS_IsReadOnly
	,FS_ReadLock
	,FS_WriteLock
	,FS_CreationTime
	,FS_CreationTimeUTC
	,FS_LastAccessTime
	,FS_LastAccessTimeUTC
	,FS_LastWriteTime
	,FS_LastWriteTimeUTC
	,FS_OwnerId
	,FS_OwnerGroupId
	,FS_UnixPermissions
) 
SELECT 
	 obj_id AS FS_Id -- bigint
	,obj_id AS FS_TargetFL -- bigint
	,obj_parent_id AS FS_ParentFL -- bigint
	,obj_path AS FS_Path -- nvarchar(4000)
	,LOWER(obj_path) AS FS_LowerCasePath -- nvarchar(4000)
	,obj_text AS FS_Text -- nvarchar(4000)
	,LOWER(obj_text) AS FS_LowerCaseText -- nvarchar(4000)
	,NULL AS FS_NameWithoutExtension -- nvarchar(4000)
	,'false' AS FS_Extension -- nvarchar(255)
	,'false' AS FS_IsCompressed -- bit
	,'false' AS FS_IsEncrypted -- bit
	,'false' AS FS_IsReadOnly -- bit
	,'false' AS FS_ReadLock -- bit
	,'false' AS FS_WriteLock -- bit
	,CURRENT_TIMESTAMP AS FS_CreationTime -- datetime2
	,CURRENT_TIMESTAMP AS FS_CreationTimeUTC -- datetime2
	,CURRENT_TIMESTAMP AS FS_LastAccessTime -- datetime2
	,CURRENT_TIMESTAMP AS FS_LastAccessTimeUTC -- datetime2
	,CURRENT_TIMESTAMP AS FS_LastWriteTime -- datetime2
	,CURRENT_TIMESTAMP AS FS_LastWriteTimeUTC -- datetime2
	,0 AS FS_OwnerId -- bigint
	,0 AS FS_OwnerGroupId -- bigint
	,999 AS FS_UnixPermissions -- int 
FROM T_Overview3 
; 
