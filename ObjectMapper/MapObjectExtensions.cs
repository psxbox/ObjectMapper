using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectMapper
{
    public static class MapObjectExtensions
    {
        public static void CopyTo<TSrc, TDest>(this TSrc src, TDest dest, MapObject<TSrc, TDest> mapper)
        where TSrc : class
        where TDest : class, new()
        {
            mapper.Copy(src, dest);
        }
    }
}