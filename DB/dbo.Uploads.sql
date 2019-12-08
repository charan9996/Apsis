CREATE TABLE [dbo].[Uploads] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [FileName]    NVARCHAR (250)   NOT NULL,
    [FilePath]    NVARCHAR (MAX)   NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    CONSTRAINT [PK_Uploads] PRIMARY KEY CLUSTERED ([Id] ASC)
);

