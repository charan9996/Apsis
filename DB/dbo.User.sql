CREATE TABLE [dbo].[User] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Mid]         NVARCHAR (10)    NOT NULL,
    [Name]        NVARCHAR (10)    NOT NULL,
    [Location]    NVARCHAR (10)    NULL,
    [Email]       NVARCHAR (50)    NOT NULL,
    [RoleId]      UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate] DATETIME         NOT NULL,
    [Vendor]      NVARCHAR (100)   NULL,
    [IsExternal]  BIT              NULL,
    [CourseId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User(RoleId)_Role(RoleId)] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]),
    CONSTRAINT [FK_User(CourseId)_Course(Id)] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id])
);

