using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class BpmEvent : Wrapper<BaseBpmEvent>
{
    public BpmEvent(Engine engine, BaseBpmEvent bpmEvent) : base(engine, bpmEvent)
    {
        spawned = true;
    }

    public BpmEvent(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.BpmEvent(
        (float)GetJsValue(o, new[] { "b", "_time" }),
        (float)GetJsValue(o, new[] { "m", "_BPM" })
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

    public float _BPM
    {
        get => wrapped.FloatValue;
        set
        {
            DeleteObject();
            wrapped.FloatValue = value;
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

    public float m
    {
        get => wrapped.Bpm;
        set
        {
            DeleteObject();
            wrapped.Bpm = value;
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
