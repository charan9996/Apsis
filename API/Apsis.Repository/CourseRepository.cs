using Apsis.Abstractions.Repository;
using Apsis.Models.Common;
using Apsis.Models.Constants;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Repository
{
    public class CourseRepository : ICourseRepository
    {
        readonly IRepository _dataRepository;
        public CourseRepository(IRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// To add Yorbit input to Course table
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        public async Task<Response> AddCourses(List<Course> courses)
        {
            var response = new Response();
            try
            {
                foreach (var course in courses)
                {
                    await _dataRepository.ExecuteAsync(SqlQueries.AddToCourse, course);
                }
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                response.Message = "File upload failed : Insert query to Course not working";
            }
            return response;
        }

        public async Task<IEnumerable<CourseModel>> GetAllCoursesAsync(CourseFilter courseFilter)
        {
            try
            {
                StringBuilder dynamicQuery = new StringBuilder();
                dynamicQuery.Append(SqlQueries.GetAllCourses);
                if (courseFilter.Keyword != null && courseFilter.Keyword.Length > 0)
                {
                    StringBuilder subKeywordSearchConditions = new StringBuilder();
                    string keyword = $"'%{courseFilter.Keyword.Trim()}%'";

                    subKeywordSearchConditions.Append($"[C].[BatchType] LIKE {keyword} OR");
                    subKeywordSearchConditions.Append($"[C].[CourseType] LIKE {keyword} OR");
                    subKeywordSearchConditions.Append($"[C].[Academy] LIKE {keyword} OR");
                    subKeywordSearchConditions.Append($"[C].[Name] LIKE {keyword} OR");
                    subKeywordSearchConditions.Append($"[C].[YorbitCourseId] LIKE {keyword}");

                    dynamicQuery.Append(" ");
                    dynamicQuery.Append($"AND ({subKeywordSearchConditions.ToString()})");
                }
                string order;
                switch (courseFilter.Sort)
                {
                    case E_COURSE_SORT.ACADEMY_NAME_ASC:
                        order = "[C].[Academy] ASC";
                        break;

                    case E_COURSE_SORT.ACADEMY_NAME_DESC:
                        order = "[C].[Academy] DESC";
                        break;

                    case E_COURSE_SORT.COURSE_NAME_DESC:
                        order = "[C].[Name] DESC";
                        break;

                    case E_COURSE_SORT.BATCH_TYPE_ASC:
                        order = "[C].[BatchType] ASC";
                        break;
                    case E_COURSE_SORT.BATCH_TYPE_DESC:
                        order = "[C].[BatchType] DESC";
                        break;

                    case E_COURSE_SORT.COURSE_TYPE_ASC:
                        order = "[C].[CourseType] ASC";
                        break;
                    case E_COURSE_SORT.COURSE_TYPE_DESC:
                        order = "[C].[CourseType] DESC";
                        break;

                    case E_COURSE_SORT.YORBIT_COURSE_ID_ASC:
                        order = "[C].[YorbitCourseId] ASC";
                        break;
                    case E_COURSE_SORT.YORBIT_COURSE_ID_DESC:
                        order = "[C].[YorbitCourseId] DESC";
                        break;
                    default:
                        order = "[C].[Name] ASC";
                        break;
                }
                order = "ORDER BY " + order;
                dynamicQuery.Append(" ");
                dynamicQuery.Append(order);

                //Pagination
                dynamicQuery.Append($" offset {courseFilter.CurrentPage} rows fetch next {courseFilter.PageSize} rows only;");


                return await _dataRepository.QueryAsync<CourseModel>(dynamicQuery.ToString());
            }
            catch (Exception ex)
            {

                return null;
            }

        }
        public async Task<CourseModel> GetCourseDetails(Guid id)
        {
            try
            {
                return (await _dataRepository.QueryAsync<CourseModel>(SqlQueries.GetCourseDetails, new { id })).First();
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<User>> GetCourseEvaluatorsList(Guid id)
        {
            try
            {
                return (await _dataRepository.QueryAsync<User>(SqlQueries.GetEvaluators, new { id }))?.ToList();
            }
            catch (Exception ex)
            {
                // Log
                return null;
            }
        }

        public async Task<int> AddEvaluator(Guid id, Guid evaluatorId, Guid courseId)
        {
            try
            {
                return (await _dataRepository.ExecuteAsync(SqlQueries.AddEvaluator, new { pId = id, eId = evaluatorId, cId = courseId }));
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
        public async Task<int> DeleteEvaluator(Guid courseId, Guid evaluatorId)
        {
            try
            {
                return (await _dataRepository.ExecuteAsync(SqlQueries.DeleteEvaluator, new { cId = courseId, eId = evaluatorId }));
            }
            catch(Exception ex)
            {
                return -1;
            }
        }
        /// <summary>
        /// Method Implementation to Update the Course Repository 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="uploadId"></param>
        /// <returns></returns>
        public async Task<Response> UpdateCourseAsync(Guid courseId, Guid uploadId)
        {
            var response = new Response();
            try
            {
                var result = await _dataRepository.ExecuteAsync(SqlQueries.UpdateCourse, new { courseId, uploadId });
                response.IsSuccess = result == 1;
            }
            catch (Exception ex)
            {
                // Log
            }
            return response;
        }

        /// <summary>
        /// File Details of Problem Statement of Course
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public async Task<Upload> GetProblemStatementDetails(Guid id)
        {
            try
            {
                return (await _dataRepository.QueryAsync<Upload>(SqlQueries.GetProblemStatementDetails, new { id }))?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> CheckYorbitCourseId(string yorbitCourseId)
        {
            try
            {
                var checkYorbitCourseId = (await _dataRepository.QueryAsync<string>(SqlQueries.checkYorbitCourseId, new { yorbitCourseId })).FirstOrDefault();
                if(checkYorbitCourseId == null)
                {
                    return false;
                }
                else if (checkYorbitCourseId.ToLower() == yorbitCourseId.ToLower())
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                Logging.Logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
