using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;
using SimpleJSON;

abstract class Wrapper<T> where T : BaseObject
{
    protected readonly Engine engine;
    protected bool spawned;
    internal readonly T wrapped;
    internal readonly T original;

    private bool _selected;
    public bool selected {
        get => _selected;
        set {
            DeleteObject();
            _selected = selected;
        }
    }

    public Wrapper(Engine engine, T wrapped, bool hasOriginal = true, bool? selected = null)
    {
        this.engine = engine;
        this.wrapped = wrapped;
        if (hasOriginal) original = BeatmapFactory.Clone(wrapped);
        _selected = selected.GetValueOrDefault(SelectionController.IsObjectSelected(wrapped));
    }

    protected static double GetJsValue(ObjectInstance o, string key)
    {
        o.TryGetValue(key, out var value);
        return (double)value.ToObject();
    }

    protected static double? GetJsValue(ObjectInstance o, IEnumerable<string> key)
    {
        foreach (string k in key)
        {
            if (o.TryGetValue(k, out var value))
            {
                return (double)value.ToObject();
            }
        }

        return null;
    }

    protected static double? GetJsValueOptional(ObjectInstance o, string key)
    {
        if (o.TryGetValue(key, out var value))
        {
            return (double)value.ToObject();
        }

        return null;
    }

    protected static bool GetJsExist(ObjectInstance o, string key)
    {
        return o.IsPrimitive();
    }

    protected static string GetJsString(ObjectInstance o, string key)
    {
        o.TryGetValue(key, out var value);
        return (string)value.ToObject();
    }

    protected static bool? GetJsBool(ObjectInstance o, string key)
    {
        if (o.TryGetValue(key, out var value))
        {
            return (bool)value.ToObject();
        }

        return null;
    }

    protected static JSONNode GetCustomData(ObjectInstance o, string key = "_customData")
    {
        var engine = new Engine();

        var customData = engine
            .SetValue("data", o)
            .Evaluate($"JSON.stringify(data.{key});");

        if (customData.IsUndefined())
        {
            return null;
        }

        return JSON.Parse(customData.AsString());
    }

    protected static JSONNode GetCustomData(ObjectInstance o, IEnumerable<string> key)
    {
        foreach(var k in key)
        {
            var engine = new Engine();

            var customData = engine
                .SetValue("data", o)
                .Evaluate($"JSON.stringify(data.{k});");

            if (customData.IsUndefined())
            {
                continue;
            }

            return JSON.Parse(customData.AsString());
        }

        return null;
    }

    public abstract bool SpawnObject(BeatmapObjectContainerCollection collection);

    internal abstract bool DeleteObject();

    internal abstract void Reconcile();

    public override string ToString()
    {
        return wrapped.ToString();
    }
}
