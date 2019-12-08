using Apsis.Models.Common;
using Apsis.Models.Response;
using Apsis.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Apsis.Models.Entities;
using System.Threading.Tasks;
using Apsis.Models.Response;
namespace Apsis.Abstractions.Business
{
    /// <summary>
    /// IcourseManager that contains method definition 
    /// </summary>
    public interface ICourseManager
    {
        /// <summary>
        /// Add Yorbit Input list to Course table 
        /// </summary>
        /// <param name="courses"></param>
        /// <returns></returns>
        Task<Response> AddCourses(List<Course> courses);
        /// <summary>
        /// Method Declaration to implement UpdateAssignment
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
         Task<FileDownloadResponse> UpdateAssigmentAsync( Guid courseId,IFormFile file);
       

        Task<IEnumerable<CourseModel>> GetAllCoursesAsync(CourseFilter courseFilter);

        Task<CourseModel> GetCourseEvaluatorDetails(Guid id);

        Task<User> AddEvaluator(Guid courseId, string mid);

        Task<CourseModel> DeleteEvaluator(Guid courseId, Guid evaluatorId);

        Task<bool> CheckYorbitCourseId(string yorbitCourseId);
    }
}