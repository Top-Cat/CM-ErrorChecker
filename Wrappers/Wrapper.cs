using Jint;
using Jint.Native.Object;
using SimpleJSON;

abstract class Wrapper<T> where T : BeatmapObject
{
    protected readonly Engine engine;
    protected bool spawned;
    internal readonly T wrapped;
    internal readonly T original;

    public Wrapper(Engine engine, T wrapped, bool hasOriginal = true)
    {
        this.engine = engine;
        this.wrapped = wrapped;
        if (hasOriginal) original = BeatmapObject.GenerateCopy(wrapped);
    }

    protected static double GetJsValue(ObjectInstance o, string key)
    {
        o.TryGetValue(key, out var value);
        return (double)value.ToObject();
    }

    protected static string GetJsString(ObjectInstance o, string key)
    {
        o.TryGetValue(key, out var value);
        return (string)value.ToObject();
    }

    protected static JSONNode GetCustomData(ObjectInstance o, string key = "_customData")
    {
        var engine = new Engine();

        var customData = engine
            .SetValue("data", o)
            .Execute($"JSON.stringify(data.{key});")
            .GetCompletionValue();

        if (customData.IsUndefined())
        {
            return null;
        }

        return JSON.Parse(customData.AsString());
    }

    public abstract bool SpawnObject();

    protected abstract bool DeleteObject();

    public override string ToString()
    {
        return wrapped.ToString();
    }
}
