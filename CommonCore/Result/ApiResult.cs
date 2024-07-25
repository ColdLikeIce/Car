using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommonCore.Result
{
    public class ApiResult : IApiResult
    {
        /// <summary>
        /// Represents an empty <see cref="IApiResult"/>.
        /// </summary>
        public static readonly IApiResult Empty = new ApiResult
        {
            Code = 0
        };

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>The status code.</value>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult{TData}"/> by the specified result.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The result.</param>
        /// <returns>An instance inherited from <see cref="IApiResult{TResult}"/> interface.</returns>
        public static IApiResult<TData> Succeed<TData>(TData data) => new ApiResult<TData>
        {
            Code = 0,
            Data = data
        };

        /// <summary>
        ///  Creates a new instance of <see cref="IApiResult"/> by the specified error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="code">The status code</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult Failed(string message, int? code = null) => new ApiResult
        {
            Code = code ?? -1,
            Message = message
        };

        /// <summary>
        ///  Creates a new instance of <see cref="IApiResult{TResult}"/> by the specified error message.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The error result.</param>
        /// <param name="message">The message.</param>
        /// <param name="code">The status code.</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult<TData> Failed<TData>(TData data, string message, int? code = null) => new ApiResult<TData>
        {
            Code = code ?? -1,
            Message = message,
            Data = data
        };

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult"/> by the specified status code and message.
        /// </summary>
        /// <param name="code">The status code.</param>
        /// <param name="message">The message.</param>
        /// <returns>An instance inherited from <see cref="IApiResult"/> interface.</returns>
        public static IApiResult From(int code, string message = null) => new ApiResult
        {
            Code = code,
            Message = message
        };

        /// <summary>
        /// Creates a new instance of <see cref="IApiResult{TResult}"/> by the specified result.
        /// </summary>
        /// <typeparam name="TData">The type of the result.</typeparam>
        /// <param name="data">The result.</param>
        /// <param name="code">The status code.</param>
        /// <param name="message">The message.</param>
        /// <returns>An instance inherited from <see cref="IApiResult{TResult}"/> interface.</returns>
        public static IApiResult<TData> From<TData>(TData data, int code, string message) => new ApiResult<TData>
        {
            Code = code,
            Message = message,
            Data = data
        };
    }
}