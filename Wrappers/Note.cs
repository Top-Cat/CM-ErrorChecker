using Jint;
using Jint.Native.Object;

class Note : VanillaWrapper<BeatmapNote>
{ 
    public float _time {
        get => wrapped.Time;
        set {
            DeleteObject();
            wrapped.Time = value;
        }
    }

    public int _lineIndex
    {
        get => wrapped.LineIndex;
        set
        {
            DeleteObject();
            wrapped.LineIndex = value;
        }
    }

    public int _lineLayer
    {
        get => wrapped.LineLayer;
        set
        {
            DeleteObject();
            wrapped.LineLayer = value;
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
        get => wrapped.Type;
        set
        {
            DeleteObject();
            wrapped.Type = value;
        }
    }

    public Note(Engine engine, BeatmapNote note) : base(engine, note)
    {
        spawned = true;
    }

    public Note(Engine engine, ObjectInstance o) : base(engine, new BeatmapNote(
        (float) GetJsValue(o, "_time"),
        (int) GetJsValue(o, "_lineIndex"),
        (int) GetJsValue(o, "_lineLayer"),
        (int) GetJsValue(o, "_type"),
        (int) GetJsValue(o, "_cutDirection"),
        GetCustomData(o)
    ), false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
