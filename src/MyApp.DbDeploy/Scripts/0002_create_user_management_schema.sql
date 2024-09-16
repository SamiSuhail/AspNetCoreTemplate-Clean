﻿CREATE SCHEMA user_management;

CREATE TABLE user_management.email_change_confirmations(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	user_id		INT			NOT NULL,
	code		CHAR(6)		NOT NULL,
	created_at	TIMESTAMP	NOT NULL,
	
	CONSTRAINT pk_email_change_confirmations_id			PRIMARY KEY (id),
	CONSTRAINT fk_email_change_confirmations_user_id	FOREIGN KEY (user_id) REFERENCES auth.users (id),
	CONSTRAINT uq_email_change_confirmations_user_id	UNIQUE (user_id),
	CONSTRAINT uq_email_change_confirmations_code		UNIQUE (code)
)