CREATE TABLE [dbo].[Conversations] (
    [conversation_id] INT IDENTITY (1, 1) NOT NULL,
    [user_one]        INT NOT NULL,
    [user_two]        INT NOT NULL,
    PRIMARY KEY CLUSTERED ([conversation_id] ASC),
    FOREIGN KEY ([user_one]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([user_two]) REFERENCES [dbo].[Users] ([user_id])
);

