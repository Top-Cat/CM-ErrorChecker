using Jint;
using Jint.Native.Object;
using SimpleJSON;

abstract class Wrapper<T>
{
    protected bool spawned;
    public T wrapped { get; private set; }

    public Wrapper(T wrapped)
    {
        this.wrapped = wrapped;
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

}
