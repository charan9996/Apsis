using Apsis.Models;
using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apsis.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace Apsis.Abstractions.Business
{
    public interface IUserManager
    {
        /// <summary>
        /// Add Yorbit input to User table
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        Task<Response> AddUsers(List<User> users);
        Task<User> GetUser(string mid);
        Task<User> GetUser(Guid userId);
        Task<Response> AddEvaluatorCourse(List<EvaluatorCourse> evaluatorCourse);
        Task<Response> AddUsersAsEvaluator(List<User> users);
    }
}
