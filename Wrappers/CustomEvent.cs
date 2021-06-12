using System;
using Jint;
using Jint.Native.Object;

class CustomEvent : Wrapper<BeatmapCustomEvent>
{
    public float _time {
        get => wrapped._time;
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public string _type
    {
        get => wrapped._type;
        set
        {
            DeleteObject();
            wrapped._type = value;
        }
    }

    private Lazy<JSONWraper> customData;
    private Action reconcile;
    public object _data
    {
        get => wrapped._customData == null ? null : customData.Value;
        set
        {
            DeleteObject();
            wrapped._customData = JSONWraper.castObjToJSON(value);
            InitWrapper();
        }
    }

    public CustomEvent(Engine engine, BeatmapCustomEvent customEvent) : base(engine, customEvent)
    {
        spawned = true;
        InitWrapper();
    }

    public CustomEvent(Engine engine, ObjectInstance o) : base(engine, new BeatmapCustomEvent(
            (float) GetJsValue(o, "_time"),
            GetJsString(o, "_type"),
            GetCustomData(o, "_data")
    ), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
        InitWrapper();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }

    private void InitWrapper()
    {
        reconcile = null;
        customData = new Lazy<JSONWraper>(() =>
            new JSONWraper(engine, ref reconcile, wrapped._customData, DeleteObject)
        );
    }

    internal override void Reconcile()
    {
        reconcile?.Invoke();
    }
}
