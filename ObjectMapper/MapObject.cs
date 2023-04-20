using System.Linq.Expressions;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObjectMapper;

public class MapObject<TSrc, TDest>
    where TSrc : class
    where TDest : class
{
    private readonly Dictionary<string, Delegate> _maps;
    private readonly List<string> _ignoreList;

    private MapObject()
    {
        _maps = new();
        _ignoreList = new();
    }

    public static MapObject<TSrc, TDest> GetMapObject() => new();

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
                value = srcProp?.GetValue(source, null);
            }
            toProp?.SetValue(destination, value, null);
        }
    }
}