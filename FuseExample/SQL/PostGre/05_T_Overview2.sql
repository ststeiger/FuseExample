
DROP TABLE IF EXISTS T_Overview2 ;
CREATE TABLE T_Overview2
(
	obj_uid uuid NULL,
	obj_parent_uid uuid NULL,
	obj_no int NULL,
	obj_text text NULL,
	obj_path text NULL,
	obj_isdir int NULL,
	obj_level int NULL,
	obj_sort int NULL,
	obj_id int NULL
);




;WITH RECURSIVE CTE AS 
(
	SELECT 
		 T_Overview.obj_uid
		,T_Overview.obj_parent_uid
		,T_Overview.obj_no
		,T_Overview.obj_text
		,T_Overview.obj_path
		,T_Overview.isdir
		,0 AS Level
		,ROW_NUMBER() OVER(ORDER BY T_Overview.obj_text) AS rn 
	FROM T_Overview WHERE obj_parent_uid IS NULL
	
	UNION ALL 
	
	SELECT 
		 T_Overview.obj_uid
		,T_Overview.obj_parent_uid
		,T_Overview.obj_no
		,T_Overview.obj_text
		,T_Overview.obj_path
		,T_Overview.isdir
		,CTE.Level + 1 AS Level
		,ROW_NUMBER() OVER(ORDER BY T_Overview.obj_text) AS rn 
	FROM CTE 
	INNER JOIN T_Overview
		ON T_Overview.obj_parent_uid = CTE.obj_uid 
) 
INSERT INTO T_Overview2
(
	 obj_uid
	,obj_parent_uid
	,obj_no
	,obj_text
	,obj_path
	,obj_isdir
	,obj_level
	,obj_sort 
	,obj_id
) 
SELECT 
	 CTE.obj_uid
	,CTE.obj_parent_uid
	,CTE.obj_no
	,CTE.obj_text
	,CTE.obj_path
	,CTE.isdir
	,CTE.Level
	,CTE.rn 
	,ROW_NUMBER() OVER(ORDER BY obj_text) AS id 
FROM CTE 
ORDER BY Level, obj_text 
