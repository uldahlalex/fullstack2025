drop schema if exists surveillance cascade;
create schema if not exists surveillance;

create table surveillance.device
(
    id text primary key not null,
    name text not null
);

create table surveillance.devicelog
(
    id text primary key not null,
    deviceid text references surveillance.device(id) not null,
    value double precision not null,
    timestamp  timestamp with time zone not null,
    unit text not null
);

create table surveillance.user
(
    id text primary key not null,
    email text not null,
    hash text not null,
    salt text not null,
    role text not null
)