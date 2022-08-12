using Jint;
using Jint.Native.Object;

class Event : VanillaWrapper<MapEvent>
{ 
    public float _time {
        get => wrapped.Time;
        set {
            DeleteObject();
            wrapped.Time = value;
        }
    }

    public int _type
    {
        get => wrapped.Type;
        set
        {
            DeleteObject();
            wrapped.Type = value;
        }
    }

    public int _value
    {
        get => wrapped.Value;
        set
        {
            DeleteObject();
            wrapped.Value = value;
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

    public override bool SpawnObject(BeatmapObjectContainerCollection collection)
    {
        if (spawned) return false;

        if (wrapped.CustomData != null && wrapped.CustomData["_lightGradient"] != null)
        {
            wrapped.LightGradient = new MapEvent.ChromaGradient(wrapped.CustomData["_lightGradient"]);
        }

        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Event);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
