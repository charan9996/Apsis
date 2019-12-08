using System;
using System.Collections.Generic;
using Apsis.Models.Entities;
using Apsis.Abstractions.Business;
using Apsis.Abstractions.Repository;
using Apsis.Models.Response;
using Apsis.Models;
using System.Threading.Tasks;

namespace Apsis.Business
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        
        public UserManager(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Add Yorbit input to User table 
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<Response> AddUsers(List<User> users)
        {
            return await _userRepository.AddUsers(users);
        }

        public async Task<User> GetUser(string mid)
        {
            return await _userRepository.GetUser(mid);
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await _userRepository.GetUser(userId);
        }
        public async Task<Response> AddEvaluatorCourse(List<EvaluatorCourse> evaluatorCourses)
        {
           return await _userRepository.AddEvaluatorCourse(evaluatorCourses);

        }
        public async Task<Response> AddUsersAsEvaluator(List<User> users)
        {
            return await _userRepository.AddUsersAsEvaluators(users);
        }
    }
}
