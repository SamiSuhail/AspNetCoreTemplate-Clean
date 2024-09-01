CREATE SCHEMA auth;
CREATE TABLE auth.users (
	id					INT				NOT NULL GENERATED ALWAYS AS IDENTITY,
	username 			VARCHAR(30) 	NOT NULL,
	password_hash 		CHAR(60) 		NOT NULL,
	email 				VARCHAR(320) 	NOT NULL,
	is_email_confirmed 	BOOLEAN 		NOT NULL,
	created_at			TIMESTAMP		NOT NULL,
	
	CONSTRAINT pk_users_id 			PRIMARY KEY (id),
	CONSTRAINT uq_users_username 	UNIQUE (username),
	CONSTRAINT uq_users_email 		UNIQUE (email)
);

CREATE INDEX idx_users_is_email_confirmed
ON auth.users (is_email_confirmed);

CREATE TABLE auth.email_confirmations (
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id		INT			NOT NULL,
	code		CHAR(6)		NOT NULL,
	created_at	TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_email_confirmations_id		PRIMARY KEY (id),
	CONSTRAINT fk_email_confirmations_user_id	FOREIGN KEY (user_id) REFERENCES auth.users (id),
	CONSTRAINT uq_email_confirmations_user_id	UNIQUE (user_id),
	CONSTRAINT uq_email_confirmations_code		UNIQUE (code)
);

CREATE TABLE auth.password_reset_confirmations(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id		INT			NOT NULL,
	code		CHAR(6)		NOT NULL,
	created_at	TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_password_reset_confirmations_id		PRIMARY KEY (id),
	CONSTRAINT fk_password_reset_confirmations_user_id	FOREIGN KEY (user_id) REFERENCES auth.users (id),
	CONSTRAINT uq_password_reset_confirmations_user_id	UNIQUE (user_id),
	CONSTRAINT uq_password_reset_confirmations_code		UNIQUE (code)
)