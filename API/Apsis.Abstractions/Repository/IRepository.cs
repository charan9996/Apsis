using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Apsis.Abstractions.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Method to execute the SELECT query to fetch the rows of generic type 'T'.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a list of type 'T'</returns>
        Task<IEnumerable<T>> QueryAsync<T>(string sqlQuery, dynamic parameters = null);

        /// <summary>
        /// Method to execute a UPDATE/DELETE query on the database
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Integer value indicating the query result</returns>
        Task<int> ExecuteAsync(string sqlQuery, dynamic parameters = null);

        /// <summary>
        /// Method to fetch a single object of simple type (ideally used for INSERT query to return inserted id).
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>An object which is the query result</returns>
        Task<T> ExecuteScalarAsync<T>(string sqlQuery, dynamic parameters = null);

        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from two different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with two result sets</returns>
        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> QueryMultipleAsync<TFirst, TSecond>(string sqlQuery, dynamic parameters = null);

        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from three different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with three result sets</returns>
        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> QueryMultipleAsync<TFirst, TSecond, TThird>(string sqlQuery, dynamic parameters = null);

        /// <summary>
        /// Method to execute multiple SELECT queries that return result sets from four different tables.
        /// </summary>
        /// <param name="sqlQuery">SQL query to execute</param>
        /// <param name="parameters">Parameters to be associated with SQL query</param>
        /// <returns>Returns a tuple with four result sets</returns>
        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>> QueryMultipleAsync<TFirst, TSecond, TThird, TFourth>(string sqlQuery, dynamic parameters = null);
    }
}
