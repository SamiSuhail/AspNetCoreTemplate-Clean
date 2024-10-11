CREATE SCHEMA auth;
CREATE TABLE auth.users (
	id						INT				NOT NULL GENERATED ALWAYS AS IDENTITY,
	instance_id				INT				NOT NULL,
	username 				VARCHAR(30) 	NOT NULL,
	password_hash 			CHAR(60) 		NOT NULL,
	email 					VARCHAR(320) 	NOT NULL,
	is_email_confirmed 		BOOLEAN 		NOT NULL,
	refresh_token_version 	INT 			NOT NULL,
	created_at				TIMESTAMP		NOT NULL,
	
	CONSTRAINT pk_users_id 						PRIMARY KEY (id),
	CONSTRAINT fk_users_instance_id				FOREIGN KEY (instance_id) REFERENCES infra.instances (id) ON DELETE CASCADE,
	CONSTRAINT uq_users_username_instance_id 	UNIQUE (username, instance_id),
	CONSTRAINT uq_users_email_instance_id 		UNIQUE (email, instance_id)
);

CREATE INDEX idx_users_is_email_confirmed
ON auth.users (is_email_confirmed);

CREATE TABLE auth.user_confirmations (
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id		INT			NOT NULL,
	code		CHAR(6)		NOT NULL,
	created_at	TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_user_confirmations_id		PRIMARY KEY (id),
	CONSTRAINT fk_user_confirmations_user_id	FOREIGN KEY (user_id) REFERENCES auth.users (id) ON DELETE CASCADE,
	CONSTRAINT uq_user_confirmations_user_id	UNIQUE (user_id),
	CONSTRAINT uq_user_confirmations_code		UNIQUE (code)
);

CREATE TABLE auth.password_reset_confirmations(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id		INT			NOT NULL,
	code		CHAR(6)		NOT NULL,
	created_at	TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_password_reset_confirmations_id		PRIMARY KEY (id),
	CONSTRAINT fk_password_reset_confirmations_user_id	FOREIGN KEY (user_id) REFERENCES auth.users (id) ON DELETE CASCADE,
	CONSTRAINT uq_password_reset_confirmations_user_id	UNIQUE (user_id),
	CONSTRAINT uq_password_reset_confirmations_code		UNIQUE (code)
)