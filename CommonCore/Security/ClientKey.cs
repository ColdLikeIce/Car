using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Security
{
    public class ClientKey
    {
        public ClientKey(string appId, string accessKey)
        {
            AppId = appId;
            AccessKey = accessKey;
        }

        public ClientKey()
        {
        }

        public string AppId { get; set; }

        public string AccessKey { get; set; }
    }
}