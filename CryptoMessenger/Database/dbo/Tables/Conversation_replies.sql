CREATE TABLE [dbo].[Conversation_replies] (
    [reply_id]        INT            IDENTITY (1, 1) NOT NULL,
    [reply]           NVARCHAR (MAX) NOT NULL,
    [conversation_id] INT            NOT NULL,
    [user_id]         INT            NOT NULL,
    [time]            DATETIME2 (1)  NOT NULL,
    PRIMARY KEY CLUSTERED ([reply_id] ASC),
    FOREIGN KEY ([conversation_id]) REFERENCES [dbo].[Conversations] ([conversation_id]),
    FOREIGN KEY ([user_id]) REFERENCES [dbo].[Users] ([user_id])
);

