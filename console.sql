CREATE TYPE ELEMENT AS ENUM ('Fire', 'Water', 'Nature', 'Wind');

CREATE TABLE Player (
    PlayerID        SERIAL PRIMARY KEY,
    Sector          INT,
    Name            TEXT,
    Currency        INT
);

CREATE TABLE Account(
    AccountID       SERIAL PRIMARY KEY,
    Username        varchar(100),
    Password        varchar(100),
    EMail           varchar(100)
);

CREATE TABLE Settings (
    SettingsID      SERIAL PRIMARY KEY ,
    PlayerID        INT REFERENCES Player(PlayerID),
    Sound           INT,
    Gameplay        INT,
    Skill           INT,
    Battle          INT
);

CREATE TABLE Monster (
    MonsterID       SERIAL PRIMARY KEY,
    Name            TEXT,
    Element         ELEMENT,
    Level           INT,
    Experience      INT,
    Health          INT,
    Attack          INT,
    Defense         INT
);

CREATE TABLE PlayerMonster (
    OwnershipID     SERIAL PRIMARY KEY,
    PlayerID        INT REFERENCES Player (PlayerID),
    MonsterID       INT REFERENCES Monster (MonsterID),
    LoadOutPos      INT DEFAULT NULL,
    Level           INT,
    CurrentHealth   INT,
    Experience      INT
);

