using Jint;
using Jint.Native.Object;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;

namespace V3
{
    class Event : VanillaWrapper<BaseEvent>
    {
        public float _time
        {
            get => wrapped.Time;
            set
            {
                DeleteObject();
                wrapped.Time = value;
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
            get => wrapped.Time;
            set
            {
                DeleteObject();
                wrapped.Time = value;
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

        public Event(Engine engine, BaseEvent mapEvent) : base(engine, mapEvent)
        {
            spawned = true;
        }

        public Event(Engine engine, ObjectInstance o) : base(engine, new V3BasicEvent(
                (float)GetJsValue(o, new string[] { "b", "_time" }),
                (int)GetJsValue(o, new string[] { "et", "_type" }),
                (int)GetJsValue(o, new string[] { "i", "_value" }),
                GetCustomData(o, new string[] { "customData", "_customData" }),
                (float)GetJsValue(o, new string[] { "f", "_floatValue" })
            ), false, GetJsBool(o, "selected"))
        {
            spawned = false;

            DeleteObject();
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
}
