
-- No foreign keys, this is just a test

-- Use default-schema

IF 0 = (SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'site' AND table_type = 'BASE TABLE') 
EXEC('
CREATE TABLE site
(
  site_uid uniqueidentifier NOT NULL,
  site_no character varying(10),
  site_text character varying(200),
  CONSTRAINT pk_site PRIMARY KEY (site_uid)
);
');


IF 0 = (SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'building' AND table_type = 'BASE TABLE') 
EXEC('
CREATE TABLE building
(
  building_uid uniqueidentifier NOT NULL,
  building_site_uid uniqueidentifier,
  building_nr character varying(10),
  building_text character varying(200),
  building_no integer,
  CONSTRAINT pk_building PRIMARY KEY (building_uid)
);
');


IF 0 = (SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'floor' AND table_type = 'BASE TABLE') 
EXEC('
CREATE TABLE floor
(
  floor_uid uniqueidentifier NOT NULL,
  floor_building_uid uniqueidentifier,
  floor_floortype_uid uniqueidentifier,
  floor_no integer,
  floor_isexterior bit,
  CONSTRAINT pk_floor PRIMARY KEY (floor_uid)
);
');


IF 0 = (SELECT COUNT(*) FROM information_schema.tables WHERE table_name = 'floortype' AND table_type = 'BASE TABLE') 
EXEC('
CREATE TABLE floortype
(
  floortype_uid uniqueidentifier NOT NULL,
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
');
