using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.AzureServices;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Apsis.Models;
using System.Threading.Tasks;
using System.Linq;
using Apsis.Models.Constants;
using Apsis.Models.ViewModel;

namespace Apsis.Business
{
    /// <summary>
    /// Upload Manager Contains the Implementation of the IUploadManager
    /// </summary>
    public class UploadManager : IUploadManager
    {
        private static List<YorbitInput> yorbitInputList = new List<YorbitInput>();
        private static List<YorbitCourseInput> yorbitCourseInputList = new List<YorbitCourseInput>();
        private readonly ICourseManager _courseManager;
        private readonly IRequestManager _requestManager;
        private readonly IUserManager _userManager;
        readonly IUploadRepository _uploadRepository;
        readonly IBlobHelper _blobHelper;
        readonly IFileOperationsManager _fileOperationsManager;
        readonly IRequestRepository _requestRepository;
        public UploadManager(IUploadRepository uploadRepository, IRequestRepository requestRepository, IFileOperationsManager fileOperationsManager, IBlobHelper blobHelper, ICourseManager courseManager, IUserManager userManager, IRequestManager requestManager)
        {
            _uploadRepository = uploadRepository;
            _courseManager = courseManager;
            _requestManager = requestManager;
            _userManager = userManager;
            _fileOperationsManager = fileOperationsManager;
            _blobHelper = blobHelper;
            _requestRepository = requestRepository;
        }

        /// <summary>
        /// Processess Yorbit Input File
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public async Task<Response> ProcessYorbitInput(IFormFile inputFile)
        {
            var response = new Response();
            try
            {
                if (inputFile.Length > 0)
                {
                    var stream = inputFile.OpenReadStream();
                    response = await YorbitExcelParser(stream);

                    if (response.IsSuccess)
                    {
                        response = await PopulateYorbitInputToDB(yorbitInputList);
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        return response;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return response;
            }
        }

        /// <summary>
        /// Parsing through the Excel File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private async Task<Response> YorbitExcelParser(Stream fileStream)
        {
            var response = new Response();

            // Open the excel file and convert to dataset and store to a dataset array
            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                List<string> headers = new List<string> { "Request ID", "MID", "Name", "Email Id", "Location", "Course ID",
                                                           "Course Name","Academy","CourseType","Batch Type",
                                                           "201 Course Status","Sub Status","Remarks",
                                                           "Due Date to submit Assignment / Project","Approved date"};

                for (int col = 1; col <= headers.Count; col++)
                {
                    string headerVal = worksheet.Cells[1, col].Value?.ToString();
                    if (string.IsNullOrEmpty(headerVal) || headers[col - 1].ToLower().Replace(" ", "") != headerVal.ToLower().Replace(" ", ""))
                    {
                        response.Message = "File upload failed : Excel Header mismatch";
                        return response;
                    }
                }

                yorbitInputList = new List<YorbitInput>();
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var listItem = new YorbitInput
                        {
                            RequestId = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Mid = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Name = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            Email = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            Location = worksheet.Cells[row, 5].Value.ToString().Trim(),
                            CourseId = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            CourseName = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            Academy = worksheet.Cells[row, 8].Value.ToString().Trim(),
                            CourseType = worksheet.Cells[row, 9].Value.ToString().Trim(),
                            BatchType = worksheet.Cells[row, 10].Value.ToString().Trim(),
                            CourseStatus = worksheet.Cells[row, 11].Value.ToString().Trim(),
                            SubmissionStatus = worksheet.Cells[row, 12].Value.ToString().Trim(),
                            Remarks = worksheet.Cells[row, 13].Value.ToString().Trim(),
                            SubmissionDate = Convert.ToDateTime(worksheet.Cells[row, 14].Value.ToString().Trim()),
                            ApprovedDate = Convert.ToDateTime(worksheet.Cells[row, 15].Value.ToString().Trim())
                        };
                        yorbitInputList.Add(listItem);
                    }
                    catch (FormatException e)
                    {
                        Logging.Logger.LogException(e);
                        response.IsSuccess = false;
                        response.Message = "File upload failed: Invalid date ( Please use MM/DD/YYYY format)";
                        return response;
                    }
                }

                if (!yorbitInputList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "File upload failed : No rows found";
                    return response;
                }
            }
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// To populate the yorbit input list to Request and Course table
        /// </summary>
        /// <param name="yorbitInputList"></param>
        /// <returns></returns>
        private async Task<Response> PopulateYorbitInputToDB(List<YorbitInput> yorbitInputList)
        {
            var response = new Response();
            var users = new List<User>();
            var courses = new List<Course>();
            var requests = new List<RequestModel>();

            foreach (var row in yorbitInputList)
            {
                Guid userGuid = Guid.NewGuid();
                Guid courseGuid = Guid.NewGuid();

                if (!users.Any(u => u.Mid == row.Mid))
                {
                    var user = new User();
                    user.Id = userGuid;
                    user.Mid = row.Mid;
                    user.Name = row.Name;
                    user.Location = row.Location;
                    user.Email = row.Email;
                    user.RoleId = Constants.LearnerRoleId;
                    users.Add(user);
                }


                if (!requests.Any(r => r.YorbitRequestId == row.RequestId) && await _courseManager.CheckYorbitCourseId(row.CourseId))
                {
                    var request = new RequestModel
                    {
                        Mid = row.Mid,
                        Id = Guid.NewGuid(),
                        LearnerId = userGuid,
                        YorbitRequestId = row.RequestId,
                        YorbitCourseId = row.CourseId,
                        YorbitStatusId = Constants.YetToSubmitYorbitStatusId,
                        AssignmentDueDate = row.SubmissionDate,
                        ApprovedDate = row.ApprovedDate,
                        isPublished = false
                    };
                    requests.Add(request);
                }


            }
            response = await _userManager.AddUsers(users);
            response = await _requestManager.AddRequests(requests);
            if (!response.IsSuccess)
                response.Message = "File upload failed : Invalid Data Provided ";
            return response;
        }
        /// <summary>
        ///  Processess YorbitCourse Input File
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public async Task<Response> ProcessYorbitCourseInput(IFormFile inputFile)
        {
            var response = new Response();
            try
            {
                if (inputFile.Length > 0)
                {
                    var stream = inputFile.OpenReadStream();
                    response = await YorbitCourseExcelParser(stream);

                    if (response.IsSuccess)
                    {
                        response = await PopulateYorbitCourseInputToDB(yorbitCourseInputList);
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        return response;
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return response;
            }
        }
        /// <summary>
        /// parsing through yorbitcourse excel sheet
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private async Task<Response> YorbitCourseExcelParser(Stream fileStream)
        {
            var response = new Response();

            // Open the excel file and convert to dataset and store to a dataset array
            using (ExcelPackage package = new ExcelPackage(fileStream))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                List<string> headers = new List<string> { "Course ID", "Course Name", "Academy", "CourseType", "Batch Type", "EvaluatorMID",
                "Evaluator Name","Evaluator Email"};
                
                for (int col = 1; col <= headers.Count; col++)
                {
                    string headerVal = worksheet.Cells[1, col].Value?.ToString();
                    if (string.IsNullOrEmpty(headerVal) || headers[col - 1].ToLower().Replace(" ", "") != headerVal.ToLower().Replace(" ", ""))
                    {
                        response.Message = "File upload failed : Excel Header mismatch";
                        return response;
                    }
                }

                yorbitCourseInputList = new List<YorbitCourseInput>();
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var listItem = new YorbitCourseInput
                        {
                            CourseId = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            CourseName = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Academy = worksheet.Cells[row, 3].Value.ToString().Trim(),
                            CourseType = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            BatchType = worksheet.Cells[row, 5].Value.ToString().Trim(),
                            EvaluatorMID = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            EvaluatorName = worksheet.Cells[row, 7].Value.ToString().Trim(),
                            EvaluatorEmail=worksheet.Cells[row, 8].Value.ToString().Trim(),
                            CreatedDate = DateTime.Now
                        };
                        yorbitCourseInputList.Add(listItem);
                    }
                    catch (Exception e)
                    {
                        Logging.Logger.LogWarn("[CourseInput] Ignored entry for row " + row);
                        Logging.Logger.LogException(e);
                    }
                }

                if (!yorbitCourseInputList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "File upload failed : No rows found";
                    return response;
                }
            }
            response.IsSuccess = true;
            return response;
        }
        /// <summary>
        /// To populate the yorbit input list to Request and Course table
        /// </summary>
        /// <param name="yorbitInputList"></param>
        /// <returns></returns>
        private async Task<Response> PopulateYorbitCourseInputToDB(List<YorbitCourseInput> yorbitCourseInputList)
        {
            var response = new Response();
            var courses = new List<Course>();
            var users = new List<User>();
            var evaluatorCourses = new List<EvaluatorCourse>();
            foreach (var row in yorbitCourseInputList)
            {
                Guid courseGuid = Guid.NewGuid();
                Guid evaluatorGuid = Guid.NewGuid();
                if (!courses.Any(c => c.YorbitCourseId == row.CourseId))
                {
                    var course = new Course
                    {
                        Id = courseGuid,
                        Name = row.CourseName,
                        YorbitCourseId = row.CourseId,
                        BatchType = row.BatchType,
                        CourseType = row.CourseType,
                        Academy = row.Academy
                    };
                    courses.Add(course);
                }
                if(!users.Any(u=>u.Mid==row.EvaluatorMID))
                {
                    var user = new User
                    {
                        Id = evaluatorGuid,
                        Mid = row.EvaluatorMID,
                        Name = row.EvaluatorName,
                        Location="Bangalore",
                        Email = row.EvaluatorEmail,
                        RoleId = Constants.EvaluatorRoleId
                    };
                    users.Add(user);
                }
                
            }
            response = await _userManager.AddUsersAsEvaluator(users);
            response = await _courseManager.AddCourses(courses);
            if (!response.IsSuccess)
                response.Message = "File upload failed : Invalid Data Provided ";
            else
            {
                foreach(var row in yorbitCourseInputList)
                {
                    Guid evaluatorCourseGuid = Guid.NewGuid();
                   if(!evaluatorCourses.Any(ec => ec.evaluatorMID == row.EvaluatorMID && ec.CourseId == row.CourseId))
                   {
                        var evaluatorCourse = new EvaluatorCourse
                        {
                            id=evaluatorCourseGuid,
                            CourseId=row.CourseId,
                            evaluatorMID=row.EvaluatorMID
                        };
                        evaluatorCourses.Add(evaluatorCourse);
                   }
                }
                response = await _userManager.AddEvaluatorCourse(evaluatorCourses);
            }
            return response;
        }

    }
}
