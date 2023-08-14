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

        public static void CopyTo<TSrc, TDest>(this TSrc src, TDest dest)
        where TSrc : class
        where TDest : class, new()
        {
            MapObject<TSrc, TDest>.GetMapObject().Copy(src, dest);
        }

        public static TDest ConvertTo<TDest>(this object src)
        where TDest : class, new()
        {
            return MapObject<object, TDest>.GetMapObject().Get(src);
        }
    }
}