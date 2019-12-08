using System;
using System.Collections.Generic;
using System.Text;
using Apsis.Models.Constants;

namespace Apsis.Repository
{
    internal static class SqlQueries
    {
        /// <summary>
        /// Query to get all the samples
        /// </summary>
        internal const string GetAllSamples =
            @"SELECT
				[S].[Id],
				[S].[Name]
			FROM [dbo].[Sample] S";

        /// <summary>
        /// Query to get sample by given id
        /// </summary>
        internal const string GetSampleById =
            @"SELECT
				[S].[Id],
				[S].[Name]
			FROM [dbo].[Sample] S
			WHERE [S].[Id] = @Id";



        internal const string GetLearnerRequests =
            @"SELECT               
				     R.[Id]
	                ,R.[YorbitRequestId]
	                ,R.[YorbitStatusId]
	                ,R.[AssignmentDueDate]
	                ,A.[SubmissionDate]
	                ,AL.[ResultId]
	                ,C.[YorbitCourseId]
                    ,C.[Name] AS CourseName
	                ,Up.[FilePath]
	                ,U.[Mid]
				    ,CNT.[Attempts]
              FROM
              [dbo].[Request] R
              LEFT JOIN
              (SELECT
                    [RequestId] 
                   ,MAX([SubmissionDate]) AS SubmissionDate
              FROM [dbo].[AssignmentAttempt] 
              GROUP BY [RequestId]) A
              ON R.[Id] = A.[RequestId]
			  LEFT JOIN
			  (SELECT
			  COUNT([RequestId]) AS Attempts
		     ,[RequestId]
		     ,[ResultId]
			  FROM [dbo].[AssignmentAttempt] 
			  GROUP BY [RequestId], [ResultId]
			  HAVING [ResultId] = @notClearedId OR [ResultId] = @clearedId) CNT
			  ON R.[Id] = CNT.[RequestId]
              LEFT JOIN
              [dbo].[AssignmentAttempt] AL
              ON A.[SubmissionDate] = AL.[SubmissionDate]
              JOIN 
              [dbo].[Course] C
              ON C.[Id] = R.[CourseId]
              LEFT JOIN 
              [dbo].[Upload] Up
              ON Up.[Id] = C.[AssignmentFileId]
              JOIN 
              [dbo].[User] U
              ON U.[Id] = R.[LearnerId]
              WHERE ([AL].[ResultId] != '6BE2995B-4E48-4F5A-B281-52759F284462' OR [AL].[ResultId] IS NULL) AND U.[Id]=@learnerId
              ORDER BY AL.[CreatedDate] DESC";

        /// <summary>
        /// Query to get ProblemStatement by given request ids
        /// </summary>
        internal const string GetProblemStatementbyRequestId =
            @"SELECT
				[c].[AssignmentFileId]
			FROM [dbo].[Course] c
			INNER JOIN
			[dbo].[Request] r
			ON
			[c].[Id]=[r].[CourseId]
			WHERE [r].[Id]= @id
			AND
			[r].[IsActive] = 'TRUE'";


        /// <summary>
        /// Query to get Uploaded File ID by given request ids
        /// </summary>
        internal const string GetAssignmentAttemptsByRequestId =
            @"SELECT
				[UploadedFileId] 
			FROM [dbo].[AssignmentAttempt]
			INNER JOIN
				(
					SELECT 
						[RequestId], 
						MAX(SubmissionDate) as maxSubmissionDate 
					FROM [dbo].[AssignmentAttempt]
					WHERE 
						[RequestId] in @id
						AND 
						IsActive = 1 
					GROUP BY [RequestId]
				) s
			ON
				[dbo].[AssignmentAttempt].RequestId = s.RequestId 
				AND
				[dbo].[AssignmentAttempt].SubmissionDate = s.maxSubmissionDate";

        /// <summary>
        /// Query to get Uploads File Path by given upload File Id
        /// </summary>
        internal const string GetMultipleUploadsByUploadedFileId =
            @"SELECT 
				[Id],
				[FileName],
				[FilePath],
				[CreatedDate],
				[IsActive]
			FROM [dbo].[Upload] 
			WHERE 
				[Id] IN @id 
				AND 
				IsActive = 1";

        /// <summary>
        /// Query to get Uploads File Path by given upload File Id
        /// </summary>
        internal const string GetUploadByUploadedFileId =
            @"SELECT 
				[Id],
				[FileName],
				[FilePath],
				[CreatedDate],
				[IsActive]
			FROM [dbo].[Upload] 
			WHERE 
				[Id] = @id 
				AND 
				IsActive = 1";

        /// <summary>
        /// Query to add yorbit input to request 
        /// </summary>
        internal const string AddToRequest =
            @"DECLARE @evaluator uniqueidentifier = (SELECT TOP 1 [EC].[EvaluatorId]
														 FROM [dbo].[EvaluatorCourse] AS [EC]
														 WHERE [EC].[CourseId] =(Select [C].[Id] from [dbo].[Course] C where [C].[YorbitCourseId] = @YorbitCourseId)
                                                        ORDER BY [EC].[Id])

			DECLARE @learner uniqueidentifier =
			(SELECT [U].[Id]
			 FROM [dbo].[User] AS [U]
			 WHERE [U].[Mid] = @Mid)
			
			DECLARE @courseFetchedId uniqueidentifier =
			(SELECT [C].[Id]
			FROM [dbo].[Course] AS [C]
			WHERE [C].[YorbitCourseId]= @YorbitCourseId)

			IF NOT EXISTS(SELECT * FROM [dbo].[Request] AS [R]
					  WHERE [R].[YorbitRequestId]= @YorbitRequestId)
				BEGIN

					INSERT INTO [dbo].[Request] 
					([Id], 
					[YorbitRequestId], 
					[CourseId],
					[EvaluatorId],
					[YorbitStatusId],
					[AssignmentDueDate],
					[isPublished],
					[ApprovedDate],
					[LearnerId])

					VALUES 

					(@Id , 
					 @YorbitRequestId,
					 @courseFetchedId, 
					 @evaluator, 
					 @YorbitStatusId,
					 @AssignmentDueDate , 
					 @isPublished,
					 @ApprovedDate,
					 @learner)
				END";


        /// <summary>
        /// Query to add yorbit input to course 
        /// </summary>
        internal const string AddToCourse =
           @"IF NOT EXISTS(SELECT* FROM [dbo].[Course] AS [C]
					  WHERE [C].[YorbitCourseId]= @YorbitCourseId)
				BEGIN
					INSERT INTO [dbo].[Course] 
					([Id],
					 [Name],
					 [YorbitCourseId],
					 [BatchType],
					 [CourseType],
					 [Academy])

					VALUES

					(@Id,
					 @Name,
					 @YorbitCourseId,
					 @BatchType,
					 @CourseType,
					 @Academy)
				END";

        /// <summary>
        /// Query to add yorbit input to user
        /// </summary>
        internal const string AddToUser =
                @"IF EXISTS(SELECT* FROM [dbo].[User] AS [U]
					  WHERE [U].[Mid]= @Mid)
                BEGIN
                     UPDATE [dbo].[User]
                     SET  [dbo].[User].[Email]=@Email,
                           [dbo].[User].[Location]=@Location
                     WHERE [dbo].[User].[Mid]=@Mid
                END
                ELSE
				BEGIN
					INSERT INTO [dbo].[User] 
					([Id],
					 [Mid],
					 [Name],
					 [Location],
					 [Email],
					 [RoleId])

					VALUES

					(@Id,
					 @Mid,
					 @Name,
					 @Location,
					 @Email,
					 @RoleId)
				END
   ";

        internal const string GetAllCourses =
            @"SELECT COUNT(*) OVER() AS RowsCount,
				[C].[YorbitCourseId] AS YorbitCourseId,
				[C].[Id] AS Id,
				[C].[Name] AS CourseName,
				[C].[Academy] AS Academy,
				[C].[BatchType] AS BatchType,
				[C].[CourseType] AS CourseType,
                [C].[AssignmentFileId] AS AssignmentFileId
			FROM [dbo].[Course] C
			WHERE [C].[IsActive] != 0";

        internal const string GetCourseDetails =
             @"SELECT 
                [C].[YorbitCourseId] AS YorbitCourseId,
                [C].[Id] AS Id,
                [C].[Name] AS CourseName,
                [C].[Academy] AS Academy,
                [C].[BatchType] AS BatchType,
                [C].[CourseType] AS CourseType
            FROM [dbo].[Course] C
            WHERE 
                [C].[IsActive] != 0
                AND
                [C].[Id] = @id";

        internal const string GetRequestCourseDetailsForEmail =
             @"SELECT
                [R].[Id] AS [RequestId],
                [R].[YorbitRequestId],
                [R].[AssignmentDueDate],
                [C].[YorbitCourseId],
                [C].[Name] AS CourseName,
                [C].[Academy] AS Academy,
                [C].[BatchType] AS BatchType,
                [C].[CourseType] AS CourseType,
                [L].[Name] AS LearnerName,
                [L].[MID] AS LearnerMID,
                [L].[Email] AS LearnerEmail,
                [E].[Name] AS EvaluatorName,
                [E].[Email] AS EvaluatorEmail,
                [temp].[AssignmentAttemptId],
                [temp].[ResultId],
                [temp].[Score],
                [temp].[Comments],
                [temp].[AssignmentFile],
                [temp].[ScoreCardFile],
                [temp].[ErrorFile]
            FROM [dbo].[Request] R
            INNER JOIN [dbo].[Course] C ON [C].[Id]=[R].[CourseId]
            INNER JOIN [dbo].[User] L ON [L].[Id]=[R].[LearnerId]
            LEFT JOIN [dbo].[User] E ON [E].[Id]=[R].[EvaluatorId]
            LEFT JOIN
            (
                SELECT
                    [A].[Id] AS [AssignmentAttemptId],
                    [A].[RequestId],
                    [A].[ResultId],
                    [A].[Score],
                    [A].[Comments],
                    [AF].[FileName] AS [AssignmentFile],
                    [SC].[FileName] AS [ScoreCardFile],
                    [EF].[FileName] AS [ErrorFile]
                FROM [dbo].[AssignmentAttempt] A
                INNER JOIN
                (
                    SELECT
                        RequestId,
                        MAX(SubmissionDate) as maxSubmissionDate
                    FROM [dbo].[AssignmentAttempt]
                    WHERE [RequestId] IN @ids AND
                        IsActive = 1
                    GROUP BY [RequestId]
                ) A1 ON A1.[RequestId] = [A].[RequestId] AND A1.maxSubmissionDate =[A].[SubmissionDate]
                LEFT JOIN [dbo].[Upload] AF ON [AF].[Id]=[A].[UploadedFileId]
                LEFT JOIN [dbo].[Upload] EF ON [EF].[Id]=[A].[ErrorFileId]
                LEFT JOIN [dbo].[Upload] SC ON [SC].[Id]=[A].[ScoreCardId]
            ) temp ON [temp].[RequestId]= [R].[Id]
            WHERE [R].[Id] IN @ids";

        /// <summary>
        /// Query to get the list of all the Evaluators Details for the particular course ID
        /// </summary>
        internal const string GetEvaluators =
            @"SELECT
                [Id],
                [Mid],
                [Name],
                [Location],
                [Email],
                [RoleId],
                [CreatedDate],
                [Vendor],
                [IsExternal],
                [IsActive]
            FROM [dbo].[User] 
			INNER JOIN
                    (
                        SELECT
                            [EvaluatorID]
                        FROM [dbo].[EvaluatorCourse]
                        WHERE
                            [CourseId] = @id
                    ) s
                ON
				[dbo].[User].Id = s.EvaluatorId 
				AND
				[dbo].[User].IsActive = 1;";

        /// <summary>
        /// Query to get the user details by MID
        /// </summary>
        internal const string GetUserDetailByMid =
            @"SELECT
                [Id],
                [Mid],
                [Name],
                [Location],
                [Email],
                [RoleId],
                [CreatedDate],
                [Vendor],
                [IsExternal],
                [IsActive]
            FROM [dbo].[User] 
            WHERE
                [Mid] = @id
                AND
				[IsActive] = 1;";

        /// <summary>
        /// Query to get the user details by ID
        /// </summary>
        internal const string GetUserDetailById =
            @"SELECT
                [Id],
                [Mid],
                [Name],
                [Location],
                [Email],
                [RoleId],
                [CreatedDate],
                [Vendor],
                [IsExternal],
                [IsActive]
            FROM [dbo].[User] 
            WHERE
                [Id] = @id
                AND
				[IsActive] = 1;";

        internal const string AddEvaluator =
            @"IF NOT EXISTS(SELECT
                [Id]
            FROM [dbo].[EvaluatorCourse]
            WHERE
                [EvaluatorId] = @eId
                AND
                [CourseId] = @cId)

		BEGIN
				INSERT into [dbo].[EvaluatorCourse]
                (
                    [Id],
                    [EvaluatorId],
                    [CourseId]
                )
            VALUES
                (
                    @pId,
                    @eId,
                    @cId
                )
		END";

        /// <summary>
        /// Query to Insert File(Assignment Question) by Learing OPM 
        /// </summary>
        internal const string AddUpload =
        @"
			INSERT INTO [dbo].[Upload]
			(
				[Id],
				[FileName],
				[FilePath]
			) 
			VALUES
			(
				@id,
				@filename,
				@filepath
			)";

        /// <summary>
        /// Query to Get Request details 
        /// </summary>
        internal const string GetRequestDetails =
         @"
            SELECT *
            FROM [dbo].[Request]
            WHERE [Id]= @id";

        /// <summary>
        /// Query to Get AssignmentAttempts 
        /// </summary>
        internal const string GetAssignmentAttempts =
         @"
            SELECT
	            A.[Id]
               ,A.[RequestId]
               ,A.[UploadedFileId]
               ,A.[ResultId]
               ,R.[YorbitRequestId]
               ,U.[Mid]
               ,A.[CreatedDate]
            FROM [dbo].[Request] R
            LEFT JOIN
            [dbo].[AssignmentAttempt] A               
            ON [A].[RequestId] = [R].[Id]
            LEFT JOIN
               [dbo].[User] U
            ON U.[Id] = R.[LearnerId]
            WHERE [R].[Id]= @id
            ORDER BY [A].[CreatedDate] DESC";

        /// <summary>
        /// Query to Update Course Table
        /// </summary>
        internal const string UpdateCourse =
            @"UPDATE [dbo].[Course]
			SET [AssignmentFileId] = @uploadId
			WHERE [Id]=@courseid AND [IsActive] = 1";

        /// <summary>
        /// query To update AssignmentDuedate by learning Opm
        /// </summary>
        internal const string UpdateAssignmentDueDate =
            @"UPDATE [dbo].[Request] 
			 SET [dbo].[Request].[AssignmentDueDate]=@NewAssignmentDueDate
			 WHERE [dbo].[Request].[Id] = @id";

        internal const string GetRequestList =
           @"
            SELECT COUNT(*) OVER() AS [TotalRows],
                            [R].[Id] as [RequestId], [A].[SubmissionDate], [A].[ResultId], 
                            [R].[AssignmentDueDate], [R].[YorbitRequestId],
                            [U].[Name], [U].[MID],
                            [C].[YorbitCourseId], [C].[Academy], [C].[Name] as [CourseName],
                            [R].[isPublished]
            FROM [Request] as [R]  
            LEFT JOIN 
                         (SELECT [A1].* FROM [AssignmentAttempt] as [A1] INNER JOIN
                                (SELECT [A2].[RequestId], MAX([A2].SubmissionDate) 
                                       as [maxdate] FROM [AssignmentAttempt] as [A2] WHERE [A2].[IsActive] = 1 
                                       GROUP BY [A2].RequestId 
                                ) [tempA] 
                                ON [A1].[SubmissionDate] = [tempA].[maxdate] 
                                       AND [A1].[RequestId] = [tempA].[RequestId]
                         )[A]
                   ON [R].[Id] = [A].[RequestId] AND [R].[IsActive] = 1 
            INNER JOIN [User] AS [U] ON [R].[LearnerId] = [U].[Id] AND [U].[IsActive] = 1 
            INNER JOIN [Course] AS [C] ON [R].[CourseId] = [C].[Id] AND [C].[IsActive] = 1 
            WHERE [R].[IsActive] = 1";

        internal const string DeleteEvaluator =
            @"DELETE
                FROM [dbo].[EvaluatorCourse]
                WHERE
                    (
                        [CourseId] = @cId
                        AND
                        [EvaluatorId] = @eId
                    );";

        internal const string GetProblemStatementDetails =
            @"SELECT
				[u].[FileName]
			FROM [dbo].[upload] u
			INNER JOIN
			[dbo].[Course] c
			ON
			[c].[AssignmentFileId]=[u].[Id]
			WHERE [c].[Id]= @id
			AND
			[c].[IsActive] = 'TRUE';";

        internal const string AddAssignmentAttempt =
         @"INSERT INTO [dbo].[AssignmentAttempt]
           ([Id]
           ,[RequestId]
           ,[UploadedFileId]
           ,[ResultId]
           ,[SubmissionDate]
            ) 
            VALUES
            (
                @id,
                @requestId,
                @uploadedFileId,
                @resultId,
                getdate()
            )";

        internal static string UpdateAssignmentAttemptForUploadedFile =
         @"UPDATE[dbo].[AssignmentAttempt]
            SET 
                [ResultId] = @resultId,
                [UploadedFileId] = @uploadedFileId,
                [SubmissionDate] = getdate()
            WHERE [RequestId] = @id AND [ResultId] = @latestResultId";
        internal static string GetYorbitStatusIdByRequestId =
            @"SELECT 
                     [dbo].[Request].[YorbitStatusId]
              FROM  [dbo].[Request]
              WHERE [dbo].[Request].[Id]=@id";
        /// <summary>
        /// query to get details from database to pass as parameter to email manager for result publish by opm
        /// </summary>
        internal const string GetRequestId_ResultPublish =
                   @"SELECT
                   [dbo].[Request].[Id]
                  FROM 
                   [dbo].[Request]
                  INNER JOIN (SELECT
				      [dbo].[AssignmentAttempt].[RequestId]
			    FROM [dbo].[AssignmentAttempt]
			    INNER JOIN
		           (SELECT  RequestId,
						MAX(SubmissionDate) as maxSubmissionDate 
					FROM [dbo].[AssignmentAttempt]
					WHERE 
						[RequestId]IN @ids
						AND 
						IsActive = 1 
					GROUP BY [RequestId]
				) s
				ON s.maxSubmissionDate =[dbo].[AssignmentAttempt].[SubmissionDate] 
                 AND s.RequestId=[dbo].[AssignmentAttempt].[RequestId] 
				 WHERE
				 [dbo].[AssignmentAttempt].[ResultId] in(@Clear,@notClear)) T
               ON T.RequestId=[dbo].[Request].[Id] 
                WHERE
                  [dbo].[Request].[Id] IN @ids
                AND 
                   [dbo].[Request].[isPublished]=0";
        /// <summary>
        /// query to update isPUBLISHED in RequestTable
        /// </summary>
        internal const string UpdatePublishedStatus =
                   @"UPDATE [dbo].[Request]
                     SET [isPublished]=@isPublished
                     WHERE
                        [Id] in @ids";

        internal const string GetAllRequestDetails =
            @" SELECT
                Top 1
                    [r].[LearnerId],
                    [r].[CourseId],
					[r].[YorbitRequestId],
                    [u].[Mid] AS LearnerMid,
                    [u].[Name] AS LearnerName,
                    [u].[Location] AS Location,
                    [u].[Email] AS Email,
                    [c].[YorbitCourseId] AS YorbitCourseId,
                    [c].[Name] AS CourseName,
                    [c].[Academy] AS Academy,
                    [a].[Id] AS AssignmentAttemptId,
                    [a].[Score] AS Score,
                    [a].[Comments] AS Comments,
                    [a].[ResultId] AS ResultId,
                    [result].[Name] AS ResultName,
                    [a].[SubmissionDate] AS SubmissionDate,
                    [a].[UploadedFileId] AS UploadedFileId,
                    [a].[ScoreCardId] AS ScoreCardId,
                    [u1].[FileName] AS AssignmentSolutionFileName,
                    [u2].[FileName] AS ScoreCardFileName,
                    [u3].[FileName] AS ErrorFileName,
                    [r].[EvaluatorId] AS EvaluatorId
                FROM dbo.[Request] r
                    INNER JOIN dbo.[User] AS [u] on [r].[LearnerId] = [u].[Id]
                    INNER JOIN dbo.[Course] AS [c] on [r].[CourseId] = [c].[Id]
                    LEFT JOIN dbo.[AssignmentAttempt] AS [a] on [r].[Id] = [a].[RequestId]
                    LEFT JOIN dbo.[Result] AS [result] on [a].[ResultId] = [result].[Id]
                    LEFT JOIN dbo.[Upload] AS [u1] on UploadedFileId = [u1].[Id]
                    LEFT JOIN dbo.[Upload] AS [u2] on ScoreCardId = [u2].[Id]
                    LEFT JOIN dbo.[Upload] AS [u3] on ErrorFileId = [u3].[Id]
                WHERE
                    [r].[Id] = @id
                    ORDER BY
                        SubmissionDate desc";


        /// <summary>
        /// Updating Comment and ErrorFileId in AssignmentAttempt
        /// </summary>
        internal static string UpdateAssignmenAttemptForErrorFileUpload =
            @"UPDATE [dbo].[AssignmentAttempt]
                SET 
                    [ErrorFileId] = @fileId,
                    [ResultId] = (SELECT 
                                    [R].[Id]
                                  FROM [dbo].[Result] R
                                  WHERE [R].[Name] like '%Error%'),
                    [Comments] = @comment,
                    [EvaluationDate] = @evaluationDate
                WHERE 
                    [Id] = @id";

        /// <summary>
        /// Get current AssignmentAttempt
        /// </summary>
        internal static string GetCurrentAssignmentAttempts =
            @"SELECT 
                TOP 1 
                A.[Id],
                A.[RequestId],
                A.[UploadedFileId],
                A.[ResultId],
                R.[YorbitRequestId],
                U.[Mid],
                A.[CreatedDate]
				 FROM [dbo].[AssignmentAttempt] A
				 inner join [dbo].[Request] R
				  ON [A].[RequestId] = [R].[Id]
				  inner join [dbo].[User] U
            ON U.[Id] = R.[LearnerId]
            WHERE [A].[RequestId] = @id
            AND [A].[IsActive] = '1'
            ORDER BY
            [A].[SubmissionDate] DESC";

        /// <summary>
        /// Get current User
        /// </summary>
        internal static string GetCurrentUser =
            @"SELECT
                * 
              FROM [dbo].[User] U
                INNER JOIN 
                [dbo].[Request] R
                ON 
                    [U].[Id]=[R].[LearnerId]
                WHERE [R].[Id] = @Id";
        internal const string GetRecentAssignmentAttemptDetails =
            @"SELECT
                [Id],
				[ResultId] 
			FROM [dbo].[AssignmentAttempt]
			INNER JOIN
				(
					SELECT 
						[RequestId], 
						MAX(SubmissionDate) as maxSubmissionDate 
					FROM [dbo].[AssignmentAttempt]
					WHERE 
						[RequestId] = @id
						AND 
						IsActive = 1 
					GROUP BY [RequestId]
				) s
			ON
				[dbo].[AssignmentAttempt].RequestId = s.RequestId 
				AND
				[dbo].[AssignmentAttempt].SubmissionDate = s.maxSubmissionDate";

        internal const string UpdateEvaluationResult =
            @"UPDATE
                dbo.[AssignmentAttempt]
                SET
                    [Comments] = @comments,
                    [Score] = @score,
                    [ResultId] = @resultId,
                    [EvaluationDate] = @evaluationDate,
                    [ScoreCardId] = @attemptScoreCardId
                WHERE
                    [Id] = @aId;";

        internal const string UpdateEvaluationResultNonFile =
            @"UPDATE
                dbo.[AssignmentAttempt]
                SET
                    [Comments] = @comments,
                    [Score] = @score,
                    [ResultId] = @resultId,
                    [EvaluationDate] = @evaluationDate
                WHERE
                    [Id] = @aId;";

        internal const string GetAttemptsLog =
            @"SELECT
	            [a].[SubmissionDate],
	            [a].[EvaluationDate],
	            [r].[Name] AS ResultName
	        FROM 
                dbo.[AssignmentAttempt] AS [a]
	            INNER JOIN dbo.[Result] AS [r] 
                    ON [a].[ResultId] = [r].[Id]
	            WHERE 
                    [a].[RequestId] = @id order by [a].[SubmissionDate] desc";

        internal const string AddEvaluatorCourse =
             @"
            IF NOT EXISTS( SELECT * FROM [dbo].[EvaluatorCourse] EC 
                          WHERE
                         [EC].[EvaluatorId]=(Select [U].[Id] from [dbo].[User] U where [U].[MID] = @evaluatorMID) 
                         AND
                         [EC].[CourseId]=(Select [C].[Id] from [dbo].[Course] C where [C].[YorbitCourseId] = @CourseId)
                       )
   
            BEGIN

            INSERT INTO [dbo].[EvaluatorCourse]
            ([Id],
            [EvaluatorId],
            [CourseId]
             ) 
            VALUES
             (
                @id,
                (Select [U].[Id] from [dbo].[User] U where [U].[MID] = @evaluatorMID),
              (Select [C].[Id] from [dbo].[Course] C where [C].[YorbitCourseId] = @courseId)
             )

           END";

        internal const string checkYorbitCourseId =
            @"SELECT [C].[YorbitCourseId] 
                FROM [dbo].[Course] C
                WHERE [C].[YorbitCourseId]  = @yorbitCourseId ";
        internal const string AddToUserEvaluators =
                @"IF NOT EXISTS(SELECT* FROM [dbo].[User] AS [U]
					  WHERE [U].[Mid]= @Mid)
				BEGIN
					INSERT INTO [dbo].[User] 
					([Id],
					 [Mid],
					 [Name],
					 [Location],
					 [Email],
					 [RoleId])

					VALUES

					(@Id,
					 @Mid,
					 @Name,
					 @Location,
					 @Email,
					 @RoleId)
				END 
                   ELSE
                 BEGIN
                    UPDATE [dbo].[User]
                    SET [dbo].[User].[RoleId]= @RoleId,
                         [dbo].[User].[Email]=@Email,
                         [dbo].[User].[Location]=@Location
                     WHERE
                        [dbo].[User].[Mid]=@Mid
                      AND
                        [dbo].[User].[RoleId]='30cda44a-6b05-4091-9dee-31419120b22b'
                   END";          
        internal const string GetRequestDetailsForReport =
          @"SELECT
              [R].[YorbitRequestId] AS RequestId,
              [U].[MID], 
              [U].[Name],
              [U].[Email] as [EmailId],
              [C].[Name] as [CourseName] ,
              [C].[YorbitCourseId] as CourseId, 
              [C].[Academy], 
              [R].[ApprovedDate],
              [R].[AssignmentDueDate],
              [dbo].[User].[Email] as [EvaluatorMailId],
              [U].IsExternal,
              [U].[Vendor],
              [A].[SubmissionDate],
              DATEDIFF(DAY, [A].SubmissionDate,[R].AssignmentDueDate) as LearnerTAT,
              [A].EvaluatorDownloadedDate as [EvaluatorAssignmentDownloadDate],
              [A].EvaluationDate as [EvaluationResultDate],
              DATEDIFF(DAY, [A].EvaluationDate , [A].EvaluatorDownloadedDate) as EvaluatorTAT,
              [A].Score,
              [Res].[Name] as [RequestStatus],
              [A].[Comments] as [EvaluatorComments]

                     FROM[Request] as [R]

              FULL OUTER JOIN

                     [AssignmentAttempt] as [A] ON [R].[Id] = [A].[RequestId] AND [R].[IsActive] = 1 

              FULL OUTER JOIN

                     [Result] as [Res] ON [A].[ResultId] = [Res].[Id]

              INNER JOIN

                     [User] AS[U] ON[R].[LearnerId] = [U].[Id] AND[U].[IsActive] = 1

              INNER JOIN

                     [Course] AS[C] ON[R].[CourseId] = [C].[Id] AND[C].[IsActive] = 1

              FULL OUTER JOIN
                 
                     [dbo].[User] ON [R].EvaluatorId = [dbo].[User].Id

              WHERE

                     [R].[IsActive] = 1 ";

        internal const string GetEvaluatorId =
            @"SELECT [Id] 
              FROM [dbo].[User]
             WHERE
                [MID]=@id
             AND 
                 [RoleId]=@evaluatorRoleId";

        internal const string UpdateAssignmentDownloadDate =
            @"UPDATE 
                [AssignmentAttempt] 
                SET 
                    [EvaluatorDownloadedDate] = getdate() 
                    WHERE 
                        [Id] IN 
                            (
                                SELECT 
                                    [a].[Id] 
                                FROM [AssignmentAttempt] as [a] 
                                join 
                                    (
                                        SELECT 
                                            requestId,
                                            MAX(SubmissionDate) AS latestdate 
                                        FROM [AssignmentAttempt] 
                                        WHERE 
                                            [requestId] IN @id
                                        GROUP BY 
                                            [requestId]
                                    ) [tempA]
                                ON 
                                    [a].requestId = [tempA].requestId 
                                    AND 
                                    [a].submissiondate = [tempA].latestdate 
                                    AND 
                                    [a].evaluatorDownloadedDate IS null
                                    AND
                                    [a].[ResultId] = '7002ED9E-47D4-4C68-86FF-39EEE95ACEB4'
                                INNER JOIN
                                        dbo.[Request] AS [r] 
                                        ON
                                            [a].[requestId] = [r].[id]
                                        INNER JOIN 
                                            [user] as [u]
                                        ON
                                            [r].[LearnerId] = [u].[Id] 
                                        WHERE 
                                            [u].[mid] != @mid
                            )";
    }
}


