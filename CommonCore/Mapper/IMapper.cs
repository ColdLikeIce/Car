using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Mapper
{
    public interface IMapper
    {
        TTarget Map<TSource, TTarget>(TSource source);

        List<TTarget> Map<TSource, TTarget>(List<TSource> source);
    }
}