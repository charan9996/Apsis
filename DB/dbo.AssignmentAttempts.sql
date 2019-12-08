CREATE TABLE [dbo].[AssignmentAttempts] (
    [Id]                      UNIQUEIDENTIFIER NOT NULL,
    [RequestId]               UNIQUEIDENTIFIER NOT NULL,
    [UploadedFileId]          UNIQUEIDENTIFIER NOT NULL,
    [ResultId]                UNIQUEIDENTIFIER NOT NULL,
    [Comments]                NVARCHAR (MAX)   NULL,
    [Score]                   FLOAT (53)       NULL,
    [SubmissionDate]          DATETIME         NULL,
    [EvaluatorDownloadedDate] DATETIME         NULL,
    [ScoreCardId]             UNIQUEIDENTIFIER NULL,
    [ErrorFileId]             UNIQUEIDENTIFIER NULL,
    [CreatedDate]             DATETIME         NOT NULL,
    CONSTRAINT [PK_AssignmentAttempts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AssignmentAttempts(RequestId)_Request(Id)] FOREIGN KEY ([RequestId]) REFERENCES [dbo].[Request] ([Id]),
    CONSTRAINT [FK_AssignmentAttempts(ResultId)_Result(Id)] FOREIGN KEY ([ResultId]) REFERENCES [dbo].[Result] ([Id]),
    CONSTRAINT [FK_AssignmentAttempts(UploadedFileId)_Uploads(Id)] FOREIGN KEY ([UploadedFileId]) REFERENCES [dbo].[Uploads] ([Id]),
    CONSTRAINT [FK_AssignmentAttempts(ScoreCardId)_Uploads(Id)] FOREIGN KEY ([ScoreCardId]) REFERENCES [dbo].[Uploads] ([Id]),
    CONSTRAINT [FK_AssignmentAttempts(ErrorFileId)_Uploads(Id)] FOREIGN KEY ([ErrorFileId]) REFERENCES [dbo].[Uploads] ([Id])
);

