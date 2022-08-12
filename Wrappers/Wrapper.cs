using Jint;
using Jint.Native.Object;
using SimpleJSON;

abstract class Wrapper<T> where T : BeatmapObject
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
        if (hasOriginal) original = BeatmapObject.GenerateCopy(wrapped);
        _selected = selected.GetValueOrDefault(SelectionController.IsObjectSelected(wrapped));
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

    protected static bool? GetJsBool(ObjectInstance o, string key)
    {
        if (o.TryGetValue(key, out var value))
        {
            return (bool) value.ToObject();
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

    public abstract bool SpawnObject(BeatmapObjectContainerCollection collection);

    internal abstract bool DeleteObject();

    internal abstract void Reconcile();

    public override string ToString()
    {
        return wrapped.ToString();
    }
}
