using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class Chain : VanillaWrapper<BaseChain>
{
    public Chain(Engine engine, BaseChain chain) : base(engine, chain)
    {
        spawned = true;
    }

    public Chain(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Chain(
        (int)GetJsValue(o, "b"),
        (int)GetJsValue(o, "x"),
        (int)GetJsValue(o, "y"),
        (int)GetJsValue(o, "c"),
        (int)GetJsValue(o, "d"),
        0,
        (float)GetJsValue(o, "tb"),
        (int)GetJsValue(o, "tx"),
        (int)GetJsValue(o, "ty"),
        (int)GetJsValue(o, "sc"),
        (float)GetJsValue(o, "s"),
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

    public int sc
    {
        get => wrapped.SliceCount;
        set
        {
            DeleteObject();
            wrapped.SliceCount = value;
        }
    }

    public float s
    {
        get => wrapped.Squish;
        set
        {
            DeleteObject();
            wrapped.Squish = value;
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

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Chain);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
