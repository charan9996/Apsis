CREATE TABLE [dbo].[Result] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (50)    NOT NULL,
    [Description] NVARCHAR (250)   NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_Result] PRIMARY KEY CLUSTERED ([Id] ASC)
);

