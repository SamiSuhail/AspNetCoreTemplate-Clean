CREATE SCHEMA access;

CREATE TABLE access.scopes(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	name		VARCHAR(61)	NOT NULL,
	created_at	TIMESTAMP	NOT NULL,

	CONSTRAINT pk_scopes_id					PRIMARY KEY (id),
	CONSTRAINT uq_scopes_name				UNIQUE (name)
);

CREATE TABLE access.user_scopes(
	user_id		INT			NOT NULL,
	scope_id	INT			NOT NULL,
	created_at	TIMESTAMP	NOT NULL,

	CONSTRAINT pk_user_scopes_user_id_scope_id	PRIMARY KEY (user_id, scope_id),
	CONSTRAINT fk_user_scopes_user_id			FOREIGN KEY (user_id)		REFERENCES auth.users (id) ON DELETE CASCADE,
	CONSTRAINT fk_user_scopes_scope_id			FOREIGN KEY (scope_id)		REFERENCES access.scopes (id) ON DELETE CASCADE
);