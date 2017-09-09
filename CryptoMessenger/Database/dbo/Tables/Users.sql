CREATE TABLE [dbo].[Users] (
    [user_id]  INT          IDENTITY (1, 1) NOT NULL,
    [login]    VARCHAR (30) NOT NULL,
    [password] VARCHAR (70) NOT NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC),
    UNIQUE NONCLUSTERED ([login] ASC)
);

