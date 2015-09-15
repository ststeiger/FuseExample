
-- Use default-schema

IF 0 <> (SELECT COUNT(*) FROM INFORMATION_SCHEMA.views WHERE table_name = 'NAV_Overview') 
EXEC('DROP VIEW NAV_Overview;');
GO 


CREATE VIEW NAV_Overview 
AS 
SELECT 
	 site.site_uid AS obj_uid
	,CAST(NULL AS uniqueidentifier) AS obj_parent_uid
	,CAST(site.site_no AS integer) AS obj_no
	,site.site_no + ' ' + site.site_text AS obj_text
	,CAST('/' + site.site_no + ' ' + site.site_text AS varchar(8000)) AS obj_path
	,1 AS isdir 
FROM site
   
UNION

SELECT 
	 building.building_uid AS obj_uid 
	,building.building_site_uid AS obj_parent_uid 
	,building.building_no AS obj_no 
	,building.building_text AS obj_text 
	,CAST('/' + site.site_no + ' ' + site.site_text + '/' + building.building_text AS varchar(8000)) AS obj_path 
	,1 AS isdir 
FROM building 
LEFT JOIN site ON site_uid = building_site_uid 

UNION

SELECT 
	 floor.floor_uid AS obj_uid
	,floor.floor_building_uid AS obj_parent_uid
	,floor.floor_no AS obj_no
	,(floortype.floortype_short_de + ' ') + RIGHT('00' + floor.floor_no, 2) AS obj_text
	,
	CAST
	(
	    '/' + site.site_no + ' ' + site.site_text 
		+ '/' + building.building_text 
		+ '/' + (floortype.floortype_short_de + ' ') + RIGHT('00' + floor.floor_no, 2) 
		AS varchar(8000)
	) AS obj_path
	,0 AS isdir
FROM floor
LEFT JOIN floortype ON floor.floor_floortype_uid = floortype.floortype_uid 
LEFT JOIN building ON building_uid = floor_building_uid 
LEFT JOIN site ON site_uid = building_site_uid 
WHERE floor_isexterior = 'false'
