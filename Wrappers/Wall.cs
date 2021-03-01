using Jint;
using Jint.Native.Object;
using UnityEngine;

class Wall : VanillaWrapper<BeatmapObstacle>
{ 
    public float _time {
        get { return wrapped._time; }
        set {
            Debug.Log("Set wall time to " + value);
            DeleteObject();
            wrapped._time = value;
        }
    }

    public int _lineIndex
    {
        get { return wrapped._lineIndex; }
        set
        {
            Debug.Log("Set wall index to " + value);
            DeleteObject();
            wrapped._lineIndex = value;
        }
    }

    public int _type
    {
        get { return wrapped._type; }
        set
        {
            Debug.Log("Set wall type to " + value);
            DeleteObject();
            wrapped._type = value;
        }
    }

    public float _duration
    {
        get { return wrapped._duration; }
        set
        {
            Debug.Log("Set wall duration to " + value);
            DeleteObject();
            wrapped._duration = value;
        }
    }

    public int _width
    {
        get { return wrapped._width; }
        set
        {
            Debug.Log("Set wall width to " + value);
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
        ), false)
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

    protected override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.OBSTACLE);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
