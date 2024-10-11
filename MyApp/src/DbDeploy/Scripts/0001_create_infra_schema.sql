CREATE SCHEMA infra;

CREATE TABLE infra.instances (
	id						INT				NOT NULL GENERATED ALWAYS AS IDENTITY,
	name 					VARCHAR(30) 	NOT NULL,
	is_cleanup_enabled 		BOOLEAN 		NOT NULL,
	created_at				TIMESTAMP		NOT NULL,
	
	CONSTRAINT pk_instances_id 			PRIMARY KEY (id),
	CONSTRAINT uq_instances_name 		UNIQUE (name)
);

INSERT INTO infra.instances (name, is_cleanup_enabled, created_at)
VALUES ('main', false, now() at time zone 'utc');