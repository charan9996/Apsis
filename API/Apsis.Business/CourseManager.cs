using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.AzureServices;
using Apsis.Models.Common;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apsis.Business
{
    /// <summary>
    /// Course Manager which contain implementation of IcourseManager
    /// </summary>
    public class CourseManager : ICourseManager
    {
        readonly ICourseRepository _courseRepository;
        readonly IUserManager _userManager;
        readonly IUploadHelper _uploadHelper;
        readonly IBlobHelper _blobHelper;
        public CourseManager(ICourseRepository courseRepository, IUploadHelper uploadHelper, IUserManager userManager, IBlobHelper blobHelper)
        {
            _courseRepository = courseRepository;
            _uploadHelper = uploadHelper;
            _userManager = userManager;
            _blobHelper = blobHelper;
        }

        public async Task<CourseModel> GetCourseEvaluatorDetails(Guid id)
        {
            var courseModel = new CourseModel();
            try
            {
                courseModel = await _courseRepository.GetCourseDetails(id);
                courseModel.Evaluators = (await _courseRepository.GetCourseEvaluatorsList(courseModel.Id))?.ToList();
                var problemStatemnentDetails = await _courseRepository.GetProblemStatementDetails(id);
                if (problemStatemnentDetails == null)
                {
                    courseModel.CourseProblemStatementUrl = null;
                }
                else
                {

                    var blobFileUrl = await _blobHelper.GetBlobUriAsync("assignments", problemStatemnentDetails.FileName.Trim());
                    courseModel.CourseProblemStatementUrl = blobFileUrl;
                }
                courseModel.IsSuccess = true;
            }
            catch(Exception ex)
            {
                courseModel.Message = "Error Getting Evaluator Details";
            }
            return courseModel;
        }

        public async Task<User> AddEvaluator(Guid courseId, string mid)
        {
            try
            {
                var userDetail = await _userManager.GetUser(mid);
                if (userDetail != null)
                {
                    var addEvaluator = await _courseRepository.AddEvaluator(Guid.NewGuid(), userDetail.Id, courseId);
                    if(addEvaluator > 0)
                    {
                        return userDetail;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        public async Task<CourseModel> DeleteEvaluator(Guid courseId, Guid evaluatorId)
        {
            CourseModel courseModel = new CourseModel();
            try
            {
                var deleteEvaluator = await _courseRepository.DeleteEvaluator(courseId, evaluatorId);
                if(deleteEvaluator > 0)
                {
                    courseModel.IsSuccess = true;
                }
                else
                {
                    courseModel.IsSuccess = false;
                    courseModel.Message = "User Not Exist to Delete";
                }
                return courseModel;
            }
            catch (Exception ex)
            {
                courseModel.Message = "Error Deleting the User";
                return courseModel;
            }
        }
        /// <summary>
        /// To Add Yorbit Input 
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        public async Task<Response> AddCourses(List<Course> courses)
        {
            return await _courseRepository.AddCourses(courses);
        }

        /// <summary>
        /// Update the Assignment in the Course Repository
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FileDownloadResponse> UpdateAssigmentAsync(Guid courseId, IFormFile file)
        {
            var response = new FileDownloadResponse();
            var courseDetails = await _courseRepository.GetCourseDetails(courseId);
            var fileName = (courseDetails.YorbitCourseId + "_" + courseDetails.CourseName + ".zip").Replace(" ", "");
            var upload = await _uploadHelper.UploadFileAsync(file, fileName);
            if (upload != null)
            {
                var result = await _courseRepository.UpdateCourseAsync(courseId, upload.Id);
                response.IsSuccess = result.IsSuccess;
                response.FileUrl = upload.FilePath;
            }
            response.Message = response.IsSuccess ? "Assignment uploaded successfully" : "Error uploading assignment.";
            return response;
        }
        
        public async Task<IEnumerable<CourseModel>> GetAllCoursesAsync(CourseFilter courseFilter)
        {
            courseFilter.CurrentPage = (courseFilter.CurrentPage - 1) * courseFilter.PageSize;
            if (courseFilter.CurrentPage <=0)
                courseFilter.CurrentPage = 0;
            return await (_courseRepository.GetAllCoursesAsync(courseFilter));
        }

        public async Task<bool> CheckYorbitCourseId(string yorbitCourseId)
        {
            try
            {
                return await _courseRepository.CheckYorbitCourseId(yorbitCourseId);
            }
            catch (Exception ex)
            {

                Logging.Logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
