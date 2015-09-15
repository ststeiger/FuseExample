
-- For PostGreSQL 
-- merge data from view nav_overview to T_Overview 
DROP TABLE IF EXISTS T_Overview ;
CREATE TABLE T_Overview 
(
obj_uid uuid 
,obj_parent_uid uuid 
,obj_no int 
,obj_text text 
,obj_path text
,isdir int  
);


INSERT INTO t_overview(obj_uid, obj_parent_uid, obj_no, obj_text, obj_path, isdir)
SELECT obj_uid, obj_parent_uid, obj_no, obj_text, obj_path, isdir FROM nav_overview;

