using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonCore.Mapper
{
    public static class MapperExtendsions
    {
        /// <summary>
        /// 映射到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TTarget Map<TSource, TTarget>(this TSource source) => Mapper.Instance.Map<TSource, TTarget>(source);

        /// <summary>
        /// 映射到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static List<TTarget> Map<TSource, TTarget>(this List<TSource> sources) => Mapper.Instance.Map<TSource, TTarget>(sources);

        /// <summary>
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>因为重名了，所以对方法取别名，同 MapTo</remarks>
        public static TTarget Replicate<TSource, TTarget>(this TSource source) => source.Map<TSource, TTarget>();

        /// <summary>
        /// 复制到
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="sources"></param>
        /// <returns></returns>
        /// <remarks>因为重名了，所以对方法取别名，同 MapTo</remarks>
        public static List<TTarget> Replicate<TSource, TTarget>(this List<TSource> sources) => sources.Map<TSource, TTarget>();
    }
}