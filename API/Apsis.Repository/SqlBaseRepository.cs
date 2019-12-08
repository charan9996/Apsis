using Apsis.Abstractions.Repository;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Repository
{
    public class SqlBaseRepository : IRepository
    {
        string _connectionString { get; set; }
        public SqlBaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:SQLConnectionString"];
        }

        /// <summary>
        /// Method to execute the SELECT query to fetch the rows of generic type 'T'.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a list of type 'T'</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, dynamic parameters = null)
        {
            IEnumerable<T> result = null;
            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                result = await connection.QueryAsync<T>(sqlQuery, (object)parameters);
            }

            return result;
        }

        /// <summary>
        /// Method to execute a UPDATE/DELETE query on the database
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Integer value indicating the query result</returns>
        public async Task<int> ExecuteAsync(string sqlQuery, dynamic parameters = null)
        {
            int result;

            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                result = await connection.ExecuteAsync(sqlQuery, (object)parameters);
            }

            return result;
        }

        /// <summary>
        /// Method to fetch a single value. Ideally used for INSERT query to return inserted id.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>An object which is the query result</returns>
        public async Task<T> ExecuteScalarAsync<T>(string sqlQuery, dynamic parameters = null)
        {
            T result;

            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                result = await connection.ExecuteScalarAsync<T>(sqlQuery, (object)parameters);
            }

            return result;
        }


        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from two different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with two result sets</returns>
        public async Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string sqlQuery, dynamic parameters = null)
        {
            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                using (var reader = await connection.QueryMultipleAsync(sqlQuery, param: (object)parameters))
                {
                    var tFirst = reader.Read<TFirst>();
                    var tSecond = reader.Read<TSecond>();
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>(tFirst, tSecond);
                }
            }
        }

        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from three different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with three result sets</returns>
        public async Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(string sqlQuery, dynamic parameters = null)
        {
            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                using (var reader = await connection.QueryMultipleAsync(sqlQuery, param: (object)parameters))
                {
                    var tFirst = reader.Read<TFirst>();
                    var tSecond = reader.Read<TSecond>();
                    var tThird = reader.Read<TThird>();
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>(tFirst, tSecond, tThird);
                }
            }
        }

        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from four different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with four result sets</returns>
        public async Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(string sqlQuery, dynamic parameters = null)
        {
            using (IDbConnection connection = SqlConnectionFactory.CreateConnection(_connectionString))
            {
                connection.Open();
                using (var reader = await connection.QueryMultipleAsync(sqlQuery, param: (object)parameters))
                {
                    var tFirst = reader.Read<TFirst>();
                    var tSecond = reader.Read<TSecond>();
                    var tThird = reader.Read<TThird>();
                    var tFourth = reader.Read<TFourth>();
                    return new Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>(tFirst, tSecond, tThird, tFourth);
                }
            }
        }
    }
}
