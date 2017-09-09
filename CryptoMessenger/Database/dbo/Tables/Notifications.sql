CREATE TABLE [dbo].[Notifications] (
    [notification_id]   INT           IDENTITY (1, 1) NOT NULL,
    [user_one]          INT           NOT NULL,
    [user_two]          INT           NOT NULL,
    [time]              DATETIME2 (1) NOT NULL,
    [accept_friendship] BIT           DEFAULT ((0)) NOT NULL,
    [reject_friendship] BIT           DEFAULT ((0)) NOT NULL,
    [send_friendship]   BIT           DEFAULT ((0)) NOT NULL,
    [cancel_friendship] BIT           DEFAULT ((0)) NOT NULL,
    [remove_friend]     BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([notification_id] ASC),
    FOREIGN KEY ([user_one]) REFERENCES [dbo].[Users] ([user_id]),
    FOREIGN KEY ([user_two]) REFERENCES [dbo].[Users] ([user_id])
);

