DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'surveillance') THEN
        CREATE SCHEMA surveillance;
    END IF;
END $EF$;


CREATE TABLE surveillance.devicelog (
    id text NOT NULL,
    deviceid text NOT NULL,
    value double precision NOT NULL,
    timestamp timestamp with time zone NOT NULL,
    unit text NOT NULL,
    CONSTRAINT devicelog_pkey PRIMARY KEY (id)
);


CREATE TABLE surveillance."user" (
    id text NOT NULL,
    email text NOT NULL,
    hash text NOT NULL,
    salt text NOT NULL,
    role text NOT NULL,
    CONSTRAINT user_pkey PRIMARY KEY (id)
);


