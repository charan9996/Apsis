using Apsis.Models;
using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apsis.Models.Entities;

namespace Apsis.Abstractions.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Add yorbit input to User table
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<Response> AddUsers(List<User> users);

        Task<User> GetUser(string mid);
        Task<User> GetUser(Guid id);
        Task<Response> AddEvaluatorCourse(List<EvaluatorCourse> evaluatorCourses);
        Task<Response> AddUsersAsEvaluators(List<User> users);
    }
}
