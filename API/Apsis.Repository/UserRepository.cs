using Apsis.Abstractions.Repository;
using Apsis.Models;
using Apsis.Models.Entities;
using Apsis.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apsis.Models.Constants;

namespace Apsis.Repository
{
    public class UserRepository : IUserRepository
    {
        readonly IRepository _dataRepository;
        public UserRepository (IRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        /// <summary>
        /// To add yobit input to User table
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public async Task<Response> AddUsers(List<User> users)
        {
            Response response = new Response();
            response.IsSuccess = false;
            try
            {
                foreach (User user in users)
                {
                    await _dataRepository.ExecuteAsync(SqlQueries.AddToUser, user);
                }
                response.IsSuccess = true;
                response.Message = "Successfully posted to Yorbit Status Table";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "File upload failed : Insert query to User not working" ;
                return response;
            }
        }
        
        public async Task<User> GetUser(string mid)
        {
            try
            {
                return (await _dataRepository.QueryAsync<User>(SqlQueries.GetUserDetailByMid, new { id = mid })).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<User> GetUser(Guid id)
        {
            try
            {
                return (await _dataRepository.QueryAsync<User>(SqlQueries.GetUserDetailById, new { id })).First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Response> AddEvaluatorCourse(List<EvaluatorCourse> evaluatorCourses)
        {
            int count = 0; bool isFail = false;
            Response response = new Response();
            response.IsSuccess = false;
            try
            {
                foreach (EvaluatorCourse evaluatorCourse in evaluatorCourses)
                {
                  count = await _dataRepository.ExecuteAsync(SqlQueries.AddEvaluatorCourse, evaluatorCourse);
                    if (count < 1)
                    {
                        isFail = true;
                    }
                }
                if (!isFail)
                {
                    response.IsSuccess = true;
                    response.Message = "Successfully posted";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Successfully Partially posted";

                }
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "File upload failed : Insert query to Table not working";
                return response;
            }

        }
        public async Task<Response> AddUsersAsEvaluators(List<User> users)
        {
            Response response = new Response();
            response.IsSuccess = false;
            try
            {
                foreach (User user in users)
                {
                    await _dataRepository.ExecuteAsync(SqlQueries.AddToUserEvaluators, user);
                }
                response.IsSuccess = true;
                response.Message = "Successfully posted Table";
                return response;
            }
            catch (Exception ex)
            {
                response.Message = "File upload failed : Insert query to User not working";
                return response;
            }
        }
    }
}
