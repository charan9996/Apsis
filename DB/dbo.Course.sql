CREATE TABLE [dbo].[Course] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (200)   NOT NULL,
    [YorbitCourseId]   NVARCHAR (50)    NOT NULL,
    [BatchType]        NVARCHAR (50)    NOT NULL,
    [CourseType]       NVARCHAR (50)    NOT NULL,
    [Academy]          NVARCHAR (100)   NOT NULL,
    [AssignmentFileId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedDate]      DATETIME         NOT NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Course(AssignmentFileId)_Uploads(Id)] FOREIGN KEY ([AssignmentFileId]) REFERENCES [dbo].[Uploads] ([Id])
);

