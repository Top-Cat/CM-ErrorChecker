using Jint.Native.Object;

class Event : Wrapper<MapEvent>
{ 
    public float _time {
        get { return wrapped._time; }
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public int _type
    {
        get { return wrapped._type; }
        set
        {
            DeleteObject();
            wrapped._type = value;
        }
    }

    public int _value
    {
        get { return wrapped._value; }
        set
        {
            DeleteObject();
            wrapped._value = value;
        }
    }

    public Event(MapEvent mapEvent) : base(mapEvent)
    {
        spawned = true;
    }

    public Event(ObjectInstance o) : base(new MapEvent(
            (float) GetJsValue(o, "_time"),
            (int) GetJsValue(o, "_type"),
            (int) GetJsValue(o, "_value")
        ))
    {
        spawned = false;

        DeleteObject();
    }

    public override void SpawnObject()
    {
        if (spawned) return;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
    }

    protected override void DeleteObject()
    {
        if (!spawned) return;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.EVENT);
        collection.DeleteObject(wrapped, false);

        spawned = false;
    }
}
