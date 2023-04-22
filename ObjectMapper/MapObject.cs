using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectMapper;

public class MapObject<TSrc, TDest>
    where TSrc : class
    where TDest : class, new() 
{
    private readonly Dictionary<string, Delegate> _maps;
    private readonly List<string> _ignoreList;

    private MapObject()
    {
        _maps = new();
        _ignoreList = new();
    }

    /// <summary>
    /// Get the <see cref="this">MapObject</see>
    /// </summary>
    /// <returns><see cref="this">MapObject</see></returns>
    public static MapObject<TSrc, TDest> GetMapObject() => new();

    /// <summary>
    /// Custom map object members
    /// </summary>
    /// <param name="destination">Expression of destination object</param>
    /// <param name="source">Expression of source object</param>
    /// <typeparam name="TDestMember">Type of destination object member(property)</typeparam>
    /// <returns></returns>
    public MapObject<TSrc, TDest> CustomMap<TDestMember>(Expression<Func<TDest, TDestMember>> destination, Expression<Func<TSrc, TDestMember>> source)
    {
        var destMember = destination.Body as MemberExpression;
        if (destMember is not null)
        {
            string destPropName = destMember.Member.Name;
            _maps.Add(destPropName, source.Compile());
        }
        return this;
    }

    /// <summary>
    /// Set ignoring property
    /// </summary>
    /// <param name="destObj">Expression of destination object</param>
    /// <typeparam name="TDestMember">Type of destination object member(property)</typeparam>
    /// <returns><see cref="this"/></returns>
    public MapObject<TSrc, TDest> Ignore<TDestMember>(Expression<Func<TDest, TDestMember>> destObj)
    {
        var destMember = destObj.Body as MemberExpression;
        if (destMember is not null)
        {
            string destPropName = destMember.Member.Name;
            _ignoreList.Add(destPropName);
        }
        return this;
    }

    /// <summary>
    /// Copy properies from <see cref="TSrc"/> to <see cref="TDest"/>
    /// </summary>
    /// <param name="source">Source object</param>
    /// <param name="destination">Destination object</param>
    public void Copy(TSrc source, TDest destination)
    {
        var props = destination.GetType().GetProperties();
        foreach (var prop in props)
        {
            if (_ignoreList.Contains(prop.Name)) continue;
            var toProp = destination.GetType().GetProperty(prop.Name);
            object? value;
            if (_maps.TryGetValue(prop.Name, out Delegate? valueDelegate))
            {
                value = valueDelegate?.DynamicInvoke(source);
            }
            else
            {
                var srcProp = source.GetType().GetProperty(prop.Name);
                if (srcProp is null) continue;
                value = srcProp?.GetValue(source, null);
            }
            toProp?.SetValue(destination, value, null);
        }
    }

    ///<summary>
    /// Get the new <see cref="TDest">destination</see> object
    ///</summary>
    public TDest Get(TSrc src)
    {
        var dest = new TDest();
        Copy(src, dest);
        return dest;
    }

}