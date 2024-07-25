using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LionAir.Core.TimeSecurity
{
    /// <summary>
    /// 安全API
    /// </summary>
    public class ApiTimeSecurityAttribute : Attribute, IFilterMetadata
    {
        public ApiTimeSecurityAttribute()
        {
        }
    }
}