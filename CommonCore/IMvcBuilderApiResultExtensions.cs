using CommonCore.Result;
using LionAir.Core.TimeSecurity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore
{
    public static class IMvcBuilderApiResultExtensions
    {
        /// <summary>
        /// 启用API标准返回值模式
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMvcBuilder AddApiResult(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddMvcOptions(options =>
            {
                options.Filters.Add(typeof(ApiExceptionFilterAttribute));
                options.Filters.Add(typeof(ApiResultFilterAttribute));
            });
        }

        /// <summary>
        /// 启用API签名模式
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMvcBuilder AddApiSignature(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddMvcOptions(options =>
            {
                options.Filters.Add(typeof(ApiTimeSecurityAsyncFilter));
            });
        }
    }
}