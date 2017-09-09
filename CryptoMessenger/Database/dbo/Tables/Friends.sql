CREATE TABLE [dbo].[Friends] (
    [friend_one] INT NOT NULL,
    [friend_two] INT NOT NULL,
    [accepted]   BIT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([friend_one] ASC, [friend_two] ASC),
    FOREIGN KEY ([friend_one]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([friend_two]) REFERENCES [dbo].[Users] ([user_id])
);

