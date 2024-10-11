CREATE SCHEMA access;

CREATE TABLE access.scopes(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	key			VARCHAR(30)	NOT NULL,
	value		VARCHAR(30) NOT NULL,

	CONSTRAINT pk_scopes_id					PRIMARY KEY (id),
	CONSTRAINT uq_scopes_key_value			UNIQUE (key, value)
);

CREATE TABLE access.groups(
	id			INT			NOT NULL GENERATED ALWAYS AS IDENTITY,
	
	CONSTRAINT pk_groups_id					PRIMARY KEY (id)
);

CREATE TABLE access.group_users(
	group_id	INT		NOT NULL,
	user_id		INT		NOT NULL,

	CONSTRAINT pk_group_users_group_id_user_id	PRIMARY KEY (group_id, user_id),
	CONSTRAINT fk_group_users_group_id			FOREIGN KEY (group_id) REFERENCES access.groups (id) ON DELETE CASCADE,
	CONSTRAINT fk_group_users_user_id			FOREIGN KEY (user_id) REFERENCES auth.users (id) ON DELETE CASCADE
);

CREATE TABLE access.group_scopes(
	group_id	INT		NOT NULL,
	scope_id	INT		NOT NULL,

	CONSTRAINT pk_group_scopes_group_id_scope_id	PRIMARY KEY (group_id, scope_id),
	CONSTRAINT fk_group_scopes_group_id			FOREIGN KEY (group_id) REFERENCES access.groups (id) ON DELETE CASCADE,
	CONSTRAINT fk_group_scopes_scope_id			FOREIGN KEY (scope_id) REFERENCES access.scopes (id) ON DELETE CASCADE
);