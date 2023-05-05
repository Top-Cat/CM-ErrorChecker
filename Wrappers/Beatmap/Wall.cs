using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class Wall : VanillaWrapper<BaseObstacle>
{
    public Wall(Engine engine, BaseObstacle wall) : base(engine, wall)
    {
        spawned = true;
    }

    public Wall(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Obstacle(
        (float)GetJsValue(o, new[] { "b", "_time" }),
        (int)GetJsValue(o, new[] { "x", "_lineIndex" }),
        (int)(GetJsExist(o, "y") ? GetJsValue(o, "y") : GetJsValue(o, "_type") == 0 ? 0 : 2),
        (int)(GetJsExist(o, "_type") ? GetJsValue(o, "_type") : 0),
        (float)GetJsValue(o, new[] { "d", "_duration" }),
        (int)GetJsValue(o, new[] { "w", "_width" }),
        (int)(GetJsExist(o, "h") ? GetJsValue(o, "h") : GetJsValue(o, "_type") == 0 ? 5 : 3),
        GetCustomData(o, new[] { "customData", "_customData" })), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public float _time
    {
        get => wrapped.JsonTime;
        set
        {
            DeleteObject();
            wrapped.JsonTime = value;
        }
    }

    public int _lineIndex
    {
        get => wrapped.PosX;
        set
        {
            DeleteObject();
            wrapped.PosX = value;
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

    public float _duration
    {
        get => wrapped.Duration;
        set
        {
            DeleteObject();
            wrapped.Duration = value;
        }
    }

    public int _width
    {
        get => wrapped.Width;
        set
        {
            DeleteObject();
            wrapped.Width = value;
        }
    }

    public float b
    {
        get => wrapped.JsonTime;
        set
        {
            DeleteObject();
            wrapped.JsonTime = value;
        }
    }

    public int x
    {
        get => wrapped.PosX;
        set
        {
            DeleteObject();
            wrapped.PosX = value;
        }
    }

    public int y
    {
        get => wrapped.PosY;
        set
        {
            DeleteObject();
            wrapped.PosY = value;
        }
    }

    public float d
    {
        get => wrapped.Duration;
        set
        {
            DeleteObject();
            wrapped.Duration = value;
        }
    }

    public int w
    {
        get => wrapped.Width;
        set
        {
            DeleteObject();
            wrapped.Width = value;
        }
    }

    public int h
    {
        get => wrapped.Height;
        set
        {
            DeleteObject();
            wrapped.Height = value;
        }
    }

    public override bool SpawnObject(BeatmapObjectContainerCollection collection)
    {
        if (spawned) return false;

        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Obstacle);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
