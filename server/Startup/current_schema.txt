DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'jerneif') THEN
        CREATE SCHEMA jerneif;
    END IF;
END $EF$;


CREATE TABLE jerneif.game (
    id uuid NOT NULL,
    weeknumber integer NOT NULL,
    yearnumber integer NOT NULL,
    CONSTRAINT "PK_game" PRIMARY KEY (id)
);


CREATE TABLE jerneif.player (
    id uuid NOT NULL,
    created_at timestamp with time zone,
    activated boolean,
    CONSTRAINT "PK_player" PRIMARY KEY (id)
);


CREATE TABLE jerneif.winnersequence (
    id uuid NOT NULL,
    gameid uuid NOT NULL,
    created_at timestamp with time zone,
    sequence integer[] NOT NULL,
    CONSTRAINT "PK_winnersequence" PRIMARY KEY (id),
    CONSTRAINT "FK_winnersequence_game_gameid" FOREIGN KEY (gameid) REFERENCES jerneif.game (id) ON DELETE CASCADE
);


CREATE TABLE jerneif.board (
    id uuid NOT NULL,
    userid uuid NOT NULL,
    gameid uuid NOT NULL,
    created_at timestamp with time zone NOT NULL,
    sortednumbers integer[] NOT NULL,
    afviklet boolean NOT NULL,
    won boolean NOT NULL,
    CONSTRAINT "PK_board" PRIMARY KEY (id),
    CONSTRAINT "FK_board_game_gameid" FOREIGN KEY (gameid) REFERENCES jerneif.game (id) ON DELETE CASCADE,
    CONSTRAINT "FK_board_player_userid" FOREIGN KEY (userid) REFERENCES jerneif.player (id) ON DELETE CASCADE
);


CREATE INDEX "IX_board_gameid" ON jerneif.board (gameid);


CREATE INDEX "IX_board_userid_gameid" ON jerneif.board (userid, gameid);


CREATE UNIQUE INDEX winnersequence_gameid_key ON jerneif.winnersequence (gameid);


