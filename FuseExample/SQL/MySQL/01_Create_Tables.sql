
-- No foreign keys, this is just a test

DROP TABLE IF EXISTS site;

CREATE TABLE site
(
  site_uid char(36) NOT NULL,
  site_no character varying(10),
  site_text character varying(200),
  CONSTRAINT pk_site PRIMARY KEY (site_uid)
);  -- ENGINE=InnoDB DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS building;

CREATE TABLE building
(
  building_uid char(36) NOT NULL,
  building_site_uid character varying(36),
  building_nr character varying(10),
  building_text character varying(200),
  building_no integer,
  CONSTRAINT pk_building PRIMARY KEY (building_uid)
);


DROP TABLE IF EXISTS floor;

CREATE TABLE floor
(
  floor_uid char(36) NOT NULL,
  floor_building_uid char(36),
  floor_floortype_uid char(36),
  floor_no integer,
  floor_isexterior boolean,
  CONSTRAINT pk_floor PRIMARY KEY (floor_uid)
);


DROP TABLE IF EXISTS floortype;

CREATE TABLE floortype
(
  floortype_uid char(36) NOT NULL,
  floortype_code character varying(2),
  floortype_short_de character varying(50),
  floortype_short_fr character varying(50),
  floortype_short_it character varying(50),
  floortype_short_en character varying(50),
  floortype_long_de character varying(255),
  floortype_long_fr character varying(255),
  floortype_long_it character varying(255),
  floortype_long_en character varying(255),
  floortype_sort integer NOT NULL,
  floortype_multiplicatorno integer,
  floortype_mez_sort integer,
  CONSTRAINT pk_floortype PRIMARY KEY (floortype_uid)
);




-- DROP INDEX ix_site_site_uid ON site; 

CREATE UNIQUE INDEX ix_site_site_uid 
ON site (site_uid) USING BTREE
;

-- DROP INDEX ix_building_building_uid ON building; 

CREATE UNIQUE INDEX ix_building_building_uid 
    ON building (building_uid) USING BTREE 
;

-- DROP INDEX ix_floor_floor_uid ON floor; 

CREATE UNIQUE INDEX ix_floor_floor_uid 
ON floor (floor_uid) USING BTREE
;


-- DROP INDEX ix_floortype_floortype_uid ON floortype; 

CREATE UNIQUE INDEX ix_floortype_floortype_uid 
ON floortype (floortype_uid) USING BTREE
;
