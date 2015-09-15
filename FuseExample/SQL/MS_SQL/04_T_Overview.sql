-- For MS_SQL 
-- merge data from view nav_overview to T_Overview 

IF 0 <> (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'T_Overview' AND TABLE_SCHEMA = 'dbo' AND TABLE_TYPE = 'BASE TABLE') 
DROP TABLE dbo.T_Overview 
GO



CREATE TABLE T_Overview 
(
	 obj_uid uniqueidentifier  
	,obj_parent_uid uniqueidentifier 
	,obj_no int 
	,obj_text nvarchar(MAX) 
	,obj_path nvarchar(MAX)
	,isdir int  
);
GO

INSERT INTO t_overview(obj_uid, obj_parent_uid, obj_no, obj_text, obj_path, isdir)
SELECT obj_uid, obj_parent_uid, obj_no, obj_text, obj_path, isdir FROM nav_overview;
