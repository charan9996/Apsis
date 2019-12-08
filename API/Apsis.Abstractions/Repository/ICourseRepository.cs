using Apsis.Models;
using Apsis.Models.Common;
using Apsis.Models.ViewModel;
using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apsis.Models.Entities;
using System.Text;

namespace Apsis.Abstractions.Repository
{
    public interface ICourseRepository
    {
        /// <summary>
        /// Add yorbit input 
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        Task<Response> AddCourses(List<Course> courses);
        /// <summary>
        /// Update the course Repository
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="uploadId"></param>
        /// <returns></returns>
        Task<Response> UpdateCourseAsync(Guid courseId, Guid uploadId);
        /// <summary>
        /// Return Courses based on search, sort and pagination parameters
        /// </summary>
        /// <param name="courseFilter"></param>
        /// <returns></returns>
        Task<IEnumerable<CourseModel>> GetAllCoursesAsync(CourseFilter courseFilter);

        Task<IEnumerable<User>> GetCourseEvaluatorsList(Guid id);

        Task<CourseModel> GetCourseDetails(Guid id);


        Task<Upload> GetProblemStatementDetails(Guid id);

        Task<int> AddEvaluator(Guid id, Guid evaluatorId, Guid courseId);

        Task<int> DeleteEvaluator(Guid courseId, Guid evaluatorId);

        Task<bool> CheckYorbitCourseId(string yorbitCourseId);
    }
}
