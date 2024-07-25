using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Mapper
{
    public class MapperManager : IMapper
    {
        public TTarget Map<TSource, TTarget>(TSource source) => source.Map<TSource, TTarget>();

        public List<TTarget> Map<TSource, TTarget>(List<TSource> source) => source.Map<TSource, TTarget>();
    }
}