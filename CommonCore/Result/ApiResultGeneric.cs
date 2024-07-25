using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CommonCore.Result
{
    public class ApiResult<TData> : ApiResult, IApiResult<TData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}"/> class.
        /// </summary>
        public ApiResult()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResult{TResult}" /> class.
        /// </summary>
        /// <param name="data">The result.</param>
        /// <param name="code">The status code.</param>
        public ApiResult(TData data, int? code)
        {
            Code = code ?? 0;
            Data = data;
        }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>The result.</value>
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
}