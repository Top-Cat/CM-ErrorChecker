using Jint;
using Jint.Native.Object;

class CustomEvent : Wrapper<BeatmapCustomEvent>
{ 
    public float _time {
        get { return wrapped._time; }
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public string _type
    {
        get { return wrapped._type; }
        set
        {
            DeleteObject();
            wrapped._type = value;
        }
    }

    public object _data
    {
        get => new JSONWraper(engine, wrapped._customData, DeleteObject);
        set
        {
            DeleteObject();
            wrapped._customData = JSONWraper.castObjToJSON(value);
        }
    }

    public CustomEvent(Engine engine, BeatmapCustomEvent customEvent) : base(engine, customEvent)
    {
        spawned = true;
    }

    public CustomEvent(Engine engine, ObjectInstance o) : base(engine, new BeatmapCustomEvent(
            (float) GetJsValue(o, "_time"),
            GetJsString(o, "_type"),
            GetCustomData(o, "_data")
        ), false)
    {
        spawned = false;

        DeleteObject();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    protected override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
