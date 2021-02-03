using System.Collections.Generic;
using System.Linq;
using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using SimpleJSON;
using UnityEngine;

abstract class Wrapper<T> : ObjectInstance where T : BeatmapObject
{
    private readonly Engine engine;
    protected bool spawned;
    internal readonly T wrapped;

    public Wrapper(Engine engine, T wrapped) : base(engine)
    {
        this.engine = engine;
        this.wrapped = wrapped;
    }

    public object _customData
    {
        get => new JSONWraper(engine, wrapped._customData, DeleteObject);
        set
        {
            DeleteObject();
            wrapped._customData = JSONWraper.castObjToJSON(value);
        }
    }

    public override IEnumerable<KeyValuePair<JsValue, PropertyDescriptor>> GetOwnProperties()
    {
        return new JSONWraper(engine, wrapped.ConvertToJSON(), DeleteObject).GetOwnProperties();
    }
    
    public override JsValue Get(JsValue property, JsValue receiver)
    {
        return new JSONWraper(engine, wrapped.ConvertToJSON(), DeleteObject).Get(property, receiver);
    }
    
    protected static double GetJsValue(ObjectInstance o, string key)
    {
        o.TryGetValue(key, out var value);
        return (double)value.ToObject();
    }

    protected static JSONNode GetCustomData(ObjectInstance o)
    {
        var engine = new Engine();

        var customData = engine
            .SetValue("data", o)
            .Execute("JSON.stringify(data._customData);")
            .GetCompletionValue();

        if (customData.IsUndefined())
        {
            return null;
        }

        return JSON.Parse(customData.AsString());
    }

    public abstract void SpawnObject();

    protected abstract void DeleteObject();

    public override string ToString()
    {
        return wrapped.ToString();
    }
    
    public JsValue ToJSON(JsValue receiver)
    {
        return this;
    }
}
