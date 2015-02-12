
-- No foreign keys, this is just a test

DROP TABLE IF EXISTS site;

CREATE TABLE site
(
  site_uid uuid NOT NULL,
  site_no character varying(10),
  site_text character varying(200),
  CONSTRAINT pk_site PRIMARY KEY (site_uid)
);


DROP TABLE IF EXISTS building;

CREATE TABLE building
(
  building_uid uuid NOT NULL,
  building_site_uid uuid,
  building_nr character varying(10),
  building_text character varying(200),
  building_no integer,
  CONSTRAINT pk_building PRIMARY KEY (building_uid)
);


DROP TABLE IF EXISTS floor;

CREATE TABLE floor
(
  floor_uid uuid NOT NULL,
  floor_building_uid uuid,
  floor_floortype_uid uuid,
  floor_no integer,
  floor_isexterior bit,
  CONSTRAINT pk_floor PRIMARY KEY (floor_uid)
);


DROP TABLE IF EXISTS floortype;

CREATE TABLE floortype
(
  floortype_uid uuid NOT NULL,
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
 