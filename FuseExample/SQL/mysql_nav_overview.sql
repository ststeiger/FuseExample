-- View: nav_overview

DROP VIEW IF EXISTS nav_overview;

CREATE OR REPLACE VIEW nav_overview 
AS 
SELECT 
	 site.site_uid AS obj_uid 
	,CAST(null AS char(36)) as obj_parent_uid 
	,CAST(site.site_no AS signed integer) AS obj_no 
	,CONCAT_WS('', site_no, ' ', site_text) AS obj_text 
	,CONCAT_WS('', '/', site_no, ' ', site_text) AS obj_path 
	,1 AS isdir
FROM site 

UNION

SELECT 
	 building.building_uid AS obj_uid 
	,building.building_site_uid AS obj_parent_uid 
	,building.building_no AS obj_no 
	,building.building_text AS obj_text 
	,CONCAT_WS('', '/', site_no, ' ', site_text, '/', building.building_text) AS obj_path 
    ,1 AS isdir 
FROM building 
LEFT JOIN site ON site_uid = building_site_uid 
	
UNION

SELECT 
	floor.floor_uid AS obj_uid
	,floor.floor_building_uid AS obj_parent_uid
	,floor.floor_no AS obj_no 
	,CONCAT_WS('', floortype.floortype_short_de, ' ', right(CONCAT('00', floor.floor_no), 2) ) AS obj_text
	,CONCAT_WS('', '/', site_no, ' ', site_text, '/', building.building_text, '/', floortype.floortype_short_de, ' ', right(CONCAT('00', floor.floor_no), 2) ) AS obj_path 
	,0 AS isdir
FROM floor
LEFT JOIN floortype ON floor.floor_floortype_uid = floortype.floortype_uid 
LEFT JOIN building ON building_uid = floor_building_uid 
LEFT JOIN site ON site_uid = building_site_uid 
WHERE floor_isexterior = 'f'
