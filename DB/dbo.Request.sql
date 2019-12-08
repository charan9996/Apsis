CREATE TABLE [dbo].[Request] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [YorbitRequestId]   INT              NOT NULL,
    [CourseId]          UNIQUEIDENTIFIER NOT NULL,
    [EvaluatorId]       UNIQUEIDENTIFIER NOT NULL,
    [YorbitStatusId]    UNIQUEIDENTIFIER NOT NULL,
    [AssignmentDueDate] DATETIME         NOT NULL,
    [isPublished]       BIT              DEFAULT ('FALSE') NOT NULL,
    [ApprovedDate]      DATETIME         NOT NULL,
    [CreatedDate]       DATETIME         NOT NULL,
    [LearnerId]         UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [PK_Request] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Request(CourseId)_Course(Id)] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Course] ([Id]),
	CONSTRAINT [FK_Request(LearnerId)_User(Id)] FOREIGN KEY ([LearnerId]) REFERENCES [dbo].[User] ([Id]),
	CONSTRAINT [FK_Request(EvaluatorId)_User(Id)] FOREIGN KEY ([EvaluatorId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Request(YorbitStatusId)_YorbitStatus(Id)] FOREIGN KEY ([YorbitStatusId]) REFERENCES [dbo].[YorbitStatus] ([Id])
);

