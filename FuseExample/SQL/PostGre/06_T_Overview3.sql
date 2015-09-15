
DROP TABLE IF EXISTS T_Overview3 ;

CREATE TABLE T_Overview3
(
	obj_id bigint NULL,
	obj_parent_id bigint NULL,
	obj_no int NULL,
	obj_text text NULL,
	obj_path text NULL,
	obj_isdir int NULL,
	obj_level int NULL,
	obj_sort int NULL
);

INSERT INTO T_Overview3
(
	 obj_id
	,obj_parent_id
	,obj_no
	,obj_text
	,obj_path
	,obj_isdir
	,obj_level
	,obj_sort
)
SELECT 
	 T_Overview2.obj_id
	,parent.obj_id AS ParentId
	,T_Overview2.obj_no
	,T_Overview2.obj_text
	,T_Overview2.obj_path
	,T_Overview2.obj_isdir
	,T_Overview2.obj_level
	,T_Overview2.obj_sort
FROM T_Overview2

LEFT JOIN T_Overview2 AS Parent 
	ON Parent.obj_uid = T_Overview2.obj_parent_uid

ORDER BY obj_level, obj_sort
;
