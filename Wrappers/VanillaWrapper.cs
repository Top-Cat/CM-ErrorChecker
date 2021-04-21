using Jint;

abstract class VanillaWrapper<T> : Wrapper<T> where T : BeatmapObject
{
    public object _customData
    {
        get => new JSONWraper(engine, wrapped._customData, DeleteObject);
        set
        {
            DeleteObject();
            wrapped._customData = JSONWraper.castObjToJSON(value);
        }
    }

    protected VanillaWrapper(Engine engine, T wrapped, bool hasOriginal = true, bool? selected = null) : base(engine, wrapped, hasOriginal, selected)
    {
    }
}
