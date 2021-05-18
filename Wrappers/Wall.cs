using Jint;
using Jint.Native.Object;

class Wall : VanillaWrapper<BeatmapObstacle>
{ 
    public float _time {
        get => wrapped._time;
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public int _lineIndex
    {
        get => wrapped._lineIndex;
        set
        {
            DeleteObject();
            wrapped._lineIndex = value;
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

    public float _duration
    {
        get => wrapped._duration;
        set
        {
            DeleteObject();
            wrapped._duration = value;
        }
    }

    public int _width
    {
        get => wrapped._width;
        set
        {
            DeleteObject();
            wrapped._width = value;
        }
    }

    public Wall(Engine engine, BeatmapObstacle wall) : base(engine, wall)
    {
        spawned = true;
    }

    public Wall(Engine engine, ObjectInstance o) : base(engine, new BeatmapObstacle(
            (float)GetJsValue(o, "_time"),
            (int)GetJsValue(o, "_lineIndex"),
            (int)GetJsValue(o, "_type"),
            (float)GetJsValue(o, "_duration"),
            (int)GetJsValue(o, "_width"),
            GetCustomData(o)
        ), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.OBSTACLE);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.OBSTACLE);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
