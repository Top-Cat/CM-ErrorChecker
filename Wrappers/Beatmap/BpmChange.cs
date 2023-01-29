using Beatmap.Base.Customs;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class BpmChange : Wrapper<BaseBpmChange>
{
    public BpmChange(Engine engine, BaseBpmChange bpmChange) : base(engine, bpmChange)
    {
        spawned = true;
    }

    public BpmChange(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.BpmChange(
        (float)GetJsValue(o, new[] { "b", "_time" }),
        (float)GetJsValue(o, new[] { "m", "_BPM" })
    ), false, GetJsBool(o, "selected"))
    {
        spawned = false;
        wrapped.BeatsPerBar = (float)(GetJsValue(o, new[] { "p", "_beatsPerBar" }) ?? 4f);
        wrapped.MetronomeOffset = (float)(GetJsValue(o, new[] { "o", "_metronomeOffset" }) ?? 4f);

        DeleteObject();
    }

    public float _time
    {
        get => wrapped.Time;
        set
        {
            DeleteObject();
            wrapped.Time = value;
        }
    }

    public float _BPM
    {
        get => wrapped.Bpm;
        set
        {
            DeleteObject();
            wrapped.Bpm = value;
        }
    }

    public float _beatsPerBar
    {
        get => wrapped.BeatsPerBar;
        set
        {
            DeleteObject();
            wrapped.BeatsPerBar = value;
        }
    }

    public float _metronomeOffset
    {
        get => wrapped.MetronomeOffset;
        set
        {
            DeleteObject();
            wrapped.MetronomeOffset = value;
        }
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

    public float m
    {
        get => wrapped.Bpm;
        set
        {
            DeleteObject();
            wrapped.Bpm = value;
        }
    }

    public float p
    {
        get => wrapped.BeatsPerBar;
        set
        {
            DeleteObject();
            wrapped.BeatsPerBar = value;
        }
    }

    public float o
    {
        get => wrapped.MetronomeOffset;
        set
        {
            DeleteObject();
            wrapped.MetronomeOffset = value;
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

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.BpmChange);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }

    internal override void Reconcile()
    {
        // Nothing :)
    }
}
