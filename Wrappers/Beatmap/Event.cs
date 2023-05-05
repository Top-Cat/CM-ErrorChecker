using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

internal class Event : VanillaWrapper<BaseEvent>
{
    public Event(Engine engine, BaseEvent mapEvent) : base(engine, mapEvent)
    {
        spawned = true;
    }

    public Event(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Event(
        (float)GetJsValue(o, new[] { "b", "_time" }),
        (int)GetJsValue(o, new[] { "et", "_type" }),
        (int)GetJsValue(o, new[] { "i", "_value" }),
        (float)GetJsValue(o, new[] { "f", "_floatValue" }),
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

    public int _type
    {
        get => wrapped.Type;
        set
        {
            DeleteObject();
            wrapped.Type = value;
        }
    }

    public int _value
    {
        get => wrapped.Value;
        set
        {
            DeleteObject();
            wrapped.Value = value;
        }
    }

    public float _floatValue
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

    public int et
    {
        get => wrapped.Type;
        set
        {
            DeleteObject();
            wrapped.Type = value;
        }
    }

    public int i
    {
        get => wrapped.Value;
        set
        {
            DeleteObject();
            wrapped.Value = value;
        }
    }

    public float f
    {
        get => wrapped.FloatValue;
        set
        {
            DeleteObject();
            wrapped.FloatValue = value;
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

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Event);
        collection.DeleteObject(wrapped, false);

        spawned = false;
        return true;
    }
}
