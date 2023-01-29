using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class Arc : VanillaWrapper<BaseArc>
{
    public Arc(Engine engine, BaseArc arc) : base(engine, arc)
    {
        spawned = true;
    }

    public Arc(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Arc(
        (float)GetJsValue(o, "b"),
        (int)GetJsValue(o, "c"),
        (int)GetJsValue(o, "x"),
        (int)GetJsValue(o, "y"),
        (int)GetJsValue(o, "d"),
        0,
        (float)GetJsValue(o, "mu"),
        (float)GetJsValue(o, "tb"),
        (int)GetJsValue(o, "tx"),
        (int)GetJsValue(o, "ty"),
        (int)GetJsValue(o, "tc"),
        (float)GetJsValue(o, "tmu"),
        (int)GetJsValue(o, "m"),
        GetCustomData(o, new[] { "customData", "_customData" })
    ), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public float b
    {
        get => wrapped.Time;
        set
        {
            DeleteObject();
            wrapped.Time = value;
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

    public float mu
    {
        get => wrapped.HeadControlPointLengthMultiplier;
        set
        {
            DeleteObject();
            wrapped.HeadControlPointLengthMultiplier = value;
        }
    }

    public float tb
    {
        get => wrapped.TailTime;
        set
        {
            DeleteObject();
            wrapped.TailTime = value;
        }
    }

    public int tx
    {
        get => wrapped.TailPosX;
        set
        {
            DeleteObject();
            wrapped.TailPosX = value;
        }
    }

    public int ty
    {
        get => wrapped.TailPosY;
        set
        {
            DeleteObject();
            wrapped.TailPosY = value;
        }
    }

    public int tc
    {
        get => wrapped.TailCutDirection;
        set
        {
            DeleteObject();
            wrapped.TailCutDirection = value;
        }
    }

    public float tmu
    {
        get => wrapped.TailControlPointLengthMultiplier;
        set
        {
            DeleteObject();
            wrapped.TailControlPointLengthMultiplier = value;
        }
    }

    public int m
    {
        get => wrapped.MidAnchorMode;
        set
        {
            DeleteObject();
            wrapped.MidAnchorMode = value;
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

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Arc);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
