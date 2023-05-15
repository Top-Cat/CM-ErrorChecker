using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class Note : VanillaWrapper<BaseNote>
{
    public Note(Engine engine, BaseNote note) : base(engine, note)
    {
        spawned = true;
    }

    public Note(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Note(
        (float)GetJsValue(o, new[] { "b", "_time" }),
        (int)GetJsValue(o, new[] { "x", "_lineIndex" }),
        (int)GetJsValue(o, new[] { "y", "_lineLayer" }),
        (int)GetJsValue(o, new[] { "c", "_type" }),
        (int)GetJsValue(o, new[] { "d", "_cutDirection" }),
        (int)(GetJsValueOptional(o, "a") ?? 0),
        GetCustomData(o, new[] { "customData", "_customData" })
    ), false, GetJsBool(o, "selected"))
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

    public int _lineLayer
    {
        get => wrapped.PosY;
        set
        {
            DeleteObject();
            wrapped.PosY = value;
        }
    }

    public int _cutDirection
    {
        get => wrapped.CutDirection;
        set
        {
            DeleteObject();
            wrapped.CutDirection = value;
        }
    }

    public int _type
    {
        get => wrapped.Color;
        set
        {
            DeleteObject();
            wrapped.Color = value;
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

    public int c
    {
        get => wrapped.Color;
        set
        {
            DeleteObject();
            wrapped.Color = value;
        }
    }

    public int d
    {
        get => wrapped.CutDirection;
        set
        {
            DeleteObject();
            wrapped.CutDirection = value;
        }
    }

    public int a
    {
        get => wrapped.AngleOffset;
        set
        {
            DeleteObject();
            wrapped.AngleOffset = value;
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

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Note);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
