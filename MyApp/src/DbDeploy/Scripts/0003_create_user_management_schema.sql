CREATE SCHEMA user_management;

CREATE TABLE user_management.email_change_confirmations(
	id				INT				NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id			INT				NOT NULL,
	old_email_code	CHAR(6)			NOT NULL,
	new_email_code	CHAR(6)			NOT NULL,
	new_email 		VARCHAR(320) 	NOT NULL,
	created_at		TIMESTAMP		NOT NULL,
	
	CONSTRAINT pk_email_change_confirmations_id					PRIMARY KEY (id),
	CONSTRAINT fk_email_change_confirmations_user_id			FOREIGN KEY (user_id) REFERENCES auth.users (id) ON DELETE CASCADE,
	CONSTRAINT uq_email_change_confirmations_user_id			UNIQUE (user_id),
	CONSTRAINT uq_email_change_confirmations_old_email_code		UNIQUE (old_email_code),
	CONSTRAINT uq_email_change_confirmations_new_email_code		UNIQUE (new_email_code)
);

CREATE TABLE user_management.password_change_confirmations(
	id						INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id					INT			NOT NULL,
	new_password_hash 		CHAR(60) 	NOT NULL,
	code					CHAR(6)		NOT NULL,
	created_at				TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_password_change_confirmations_id				PRIMARY KEY (id),
	CONSTRAINT fk_password_change_confirmations_user_id			FOREIGN KEY (user_id) REFERENCES auth.users (id) ON DELETE CASCADE,
	CONSTRAINT uq_password_change_confirmations_user_id			UNIQUE (user_id),
	CONSTRAINT uq_password_change_confirmations_code			UNIQUE (code)
);