DO
$EF$
BEGIN
    IF
NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'jerneif') THEN
CREATE SCHEMA jerneif;
END IF;
END $EF$;


CREATE TABLE "AspNetRoles"
(
    "Id"               text NOT NULL,
    "Name"             character varying(256),
    "NormalizedName"   character varying(256),
    "ConcurrencyStamp" text,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);


CREATE TABLE "AspNetUsers"
(
    "Id"                   text    NOT NULL,
    "UserName"             character varying(256),
    "NormalizedUserName"   character varying(256),
    "Email"                character varying(256),
    "NormalizedEmail"      character varying(256),
    "EmailConfirmed"       boolean NOT NULL,
    "PasswordHash"         text,
    "SecurityStamp"        text,
    "ConcurrencyStamp"     text,
    "PhoneNumber"          text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled"     boolean NOT NULL,
    "LockoutEnd"           timestamp with time zone,
    "LockoutEnabled"       boolean NOT NULL,
    "AccessFailedCount"    integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);


CREATE TABLE jerneif.game
(
    id         uuid    NOT NULL,
    weeknumber integer NOT NULL,
    yearnumber integer NOT NULL,
    CONSTRAINT "PK_game" PRIMARY KEY (id)
);


CREATE TABLE jerneif.player
(
    id         uuid NOT NULL,
    created_at timestamp with time zone,
    activated  boolean,
    CONSTRAINT "PK_player" PRIMARY KEY (id)
);


CREATE TABLE "AspNetRoleClaims"
(
    "Id"         integer GENERATED BY DEFAULT AS IDENTITY,
    "RoleId"     text NOT NULL,
    "ClaimType"  text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserClaims"
(
    "Id"         integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId"     text NOT NULL,
    "ClaimType"  text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserLogins"
(
    "LoginProvider"       text NOT NULL,
    "ProviderKey"         text NOT NULL,
    "ProviderDisplayName" text,
    "UserId"              text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserRoles"
(
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserTokens"
(
    "UserId"        text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name"          text NOT NULL,
    "Value"         text,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE jerneif.winnersequence
(
    id         uuid NOT NULL,
    gameid     uuid NOT NULL,
    created_at timestamp with time zone,
    sequence   integer[] NOT NULL,
    CONSTRAINT "PK_winnersequence" PRIMARY KEY (id),
    CONSTRAINT "FK_winnersequence_game_gameid" FOREIGN KEY (gameid) REFERENCES jerneif.game (id) ON DELETE CASCADE
);


CREATE TABLE jerneif.board
(
    id            uuid                     NOT NULL,
    userid        uuid                     NOT NULL,
    gameid        uuid                     NOT NULL,
    created_at    timestamp with time zone NOT NULL,
    sortednumbers integer[] NOT NULL,
    afviklet      boolean                  NOT NULL,
    won           boolean                  NOT NULL,
    CONSTRAINT "PK_board" PRIMARY KEY (id),
    CONSTRAINT "FK_board_game_gameid" FOREIGN KEY (gameid) REFERENCES jerneif.game (id) ON DELETE CASCADE,
    CONSTRAINT "FK_board_player_userid" FOREIGN KEY (userid) REFERENCES jerneif.player (id) ON DELETE CASCADE
);


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");


CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");


CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");


CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");


CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");


CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");


CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");


CREATE INDEX "IX_board_gameid" ON jerneif.board (gameid);


CREATE INDEX "IX_board_userid_gameid" ON jerneif.board (userid, gameid);


CREATE UNIQUE INDEX winnersequence_gameid_key ON jerneif.winnersequence (gameid);

