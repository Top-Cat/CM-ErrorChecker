using Jint;
using Jint.Native.Object;

class Event : VanillaWrapper<MapEvent>
{ 
    public float _time {
        get => wrapped._time;
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public int _type
    {
        get => wrapped._type;
        set
        {
            DeleteObject();
            wrapped._type = value;
        }
    }

    public int _value
    {
        get => wrapped._value;
        set
        {
            DeleteObject();
            wrapped._value = value;
        }
    }

    public Event(Engine engine, MapEvent mapEvent) : base(engine, mapEvent)
    {
        spawned = true;
    }

    public Event(Engine engine, ObjectInstance o) : base(engine, new MapEvent(
            (float) GetJsValue(o, "_time"),
            (int) GetJsValue(o, "_type"),
            (int) GetJsValue(o, "_value"),
            GetCustomData(o)
        ), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        if (wrapped._customData["_lightGradient"] != null)
        {
            wrapped._lightGradient = new MapEvent.ChromaGradient(wrapped._customData["_lightGradient"]);
        }

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
}
