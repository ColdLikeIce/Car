using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Mapper
{
    internal class Mapper
    {
        public static readonly Mapper Instance = new Mapper();

        private Mapper()
        {
        }

        public TTarget Map<TSource, TTarget>(TSource source) => CacheModel<TSource, TTarget>.Invoke(source);

        public List<TTarget> Map<TSource, TTarget>(List<TSource> sources) => sources.AsParallel().Select(CacheModel<TSource, TTarget>.Invoke).ToList();

        internal class CacheModel<TSource, TTarget>
        {
            private static readonly Func<TSource, TTarget> Func;

            static CacheModel()
            {
                var parameterExpression = Expression.Parameter(typeof(TSource), "x");
                var sourcePropNames = typeof(TSource).GetProperties()
                    .Where(x => !x.IsDefined(typeof(NotMapAttribute), true))
                    .Select(x => x.Name)
                    .ToArray();

                var memberBindings = typeof(TTarget).GetProperties()
                    .Where(x => x.CanWrite && sourcePropNames.Any(y => y.ToUpper() == x.Name.ToUpper()))
                    .Select(x => Expression.Bind(typeof(TTarget).GetProperty(x.Name),
                    Expression.Property(parameterExpression,
                    typeof(TSource).GetProperty(sourcePropNames.FirstOrDefault(y => y.ToUpper() == x.Name.ToUpper())))));

                Func = Expression.Lambda<Func<TSource, TTarget>>(Expression.MemberInit(Expression.New(typeof(TTarget)), memberBindings), parameterExpression).Compile();
            }

            public static TTarget Invoke(TSource source) => Func(source);
        }
    }
}