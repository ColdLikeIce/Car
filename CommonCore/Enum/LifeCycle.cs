using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Enum
{
    public enum LifeCycle
    {
        Scoped = 0x1,
        Singleton = 0x2,
        Transient = 0x3,
    }
}