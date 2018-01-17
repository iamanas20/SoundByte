using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SoundByte.Core.Items;

namespace SoundByte.Core.Services
{
    /// <summary>
    ///     A generic HTTP service that uses <see cref="System.Net.Http.HttpClient"/> for handling requests.
    /// </summary>
    public interface IHttpService
    {
        #region GET
        /// <summary>
        ///     Performs a GET request at the specified url. This method returns the response as 
        ///     an object T.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to get</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        Task<HttpReponse<T>> GetAsync<T>(string url, CancellationTokenSource cancellationTokenSource = null);
        #endregion

        #region POST
        /// <summary>
        ///     Performs a POST request at a specified url. This version takes a string-string
        ///     dictionary as the body.        
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to post</param>
        /// <param name="bodyParamaters">string-string dictionary as the body</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        Task<HttpReponse<T>> PostAsync<T>(string url, [NotNull] Dictionary<string, string> bodyParamaters, CancellationTokenSource cancellationTokenSource = null);

        /// <summary>
        ///     Performs a POST request at a specified url. This version takes a JSON string
        ///     as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to post</param>
        /// <param name="body">json string as the body</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        Task<HttpReponse<T>> PostAsync<T>(string url, string body, CancellationTokenSource cancellationTokenSource = null);
        #endregion

        #region PUT
        /// <summary>
        ///     Performs a PUT request at a specified url. This version takes a string-string
        ///     dictionary as the body.        
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to put</param>
        /// <param name="bodyParamaters">string-string dictionary as the body</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        Task<HttpReponse<T>> PutAsync<T>(string url, [NotNull] Dictionary<string, string> bodyParamaters, CancellationTokenSource cancellationTokenSource = null);

        /// <summary>
        ///     Performs a PUT request at a specified url. This version takes a JSON string
        ///     as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to put</param>
        /// <param name="body">json string as the body</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns></returns>
        Task<HttpReponse<T>> PutAsync<T>(string url, string body, CancellationTokenSource cancellationTokenSource = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        Task<HttpReponse<bool>> PutAsync(string url, string body, CancellationTokenSource cancellationTokenSource = null);
        #endregion

        #region DELETE
        /// <summary>
        ///     Deletes a specified resource. Will return true if the resource was deleted, else
        ///     false is the resource was not deleted.
        /// </summary>
        /// <param name="url">The url resource to delete</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>True if the resource was deleted, false if not.</returns>
        Task<HttpReponse<bool>> DeleteAsync(string url, CancellationTokenSource cancellationTokenSource = null);
        #endregion

        #region EXISTS
        /// <summary>
        ///     Checks to see if a resource is at the specified location. Returns true if the 
        ///     resource exists, returns false if it does not. Will throw an exception incase of an
        ///     unknown error.
        /// </summary>
        /// <param name="url">The url to check</param>
        /// <param name="cancellationTokenSource">Allows the ability to cancel the current task.</param>
        /// <returns>True if resource exists, false if not.</returns>
        Task<HttpReponse<bool>> ExistsAsync(string url, CancellationTokenSource cancellationTokenSource = null);
        #endregion
    }
}