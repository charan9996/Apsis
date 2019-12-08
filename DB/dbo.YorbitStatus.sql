CREATE TABLE [dbo].[YorbitStatus] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (250)   NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_YorbitStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);

