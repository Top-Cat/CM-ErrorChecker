using Jint;
using Jint.Native.Object;

class BpmChange : Wrapper<BeatmapBPMChange>
{
    public float _time {
        get => wrapped._time;
        set {
            DeleteObject();
            wrapped._time = value;
        }
    }

    public float _BPM
    {
        get => wrapped._BPM;
        set
        {
            DeleteObject();
            wrapped._BPM = value;
        }
    }

    public float _beatsPerBar
    {
        get => wrapped._beatsPerBar;
        set
        {
            DeleteObject();
            wrapped._beatsPerBar = value;
        }
    }

    public float _metronomeOffset
    {
        get => wrapped._metronomeOffset;
        set
        {
            DeleteObject();
            wrapped._metronomeOffset = value;
        }
    }

    public BpmChange(Engine engine, BeatmapBPMChange bpmChange) : base(engine, bpmChange)
    {
        spawned = true;
    }

    public BpmChange(Engine engine, ObjectInstance o) : base(engine, new BeatmapBPMChange(
            (float) GetJsValue(o, "_BPM"),
            (float) GetJsValue(o, "_time")
    ) {
        _beatsPerBar = (float) GetJsValue(o, "_beatsPerBar"),
        _metronomeOffset = (float) GetJsValue(o, "_metronomeOffset")
    }, false, GetJsBool(o, "selected"))
    {
        spawned = false;

        DeleteObject();
    }

    public override bool SpawnObject()
    {
        if (spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.BPM_CHANGE);
        collection.SpawnObject(wrapped, false, false);

        spawned = true;
        return true;
    }

    internal override bool DeleteObject()
    {
        if (!spawned) return false;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.BPM_CHANGE);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }

    internal override void Reconcile()
    {
        // Nothing :)
    }
}
