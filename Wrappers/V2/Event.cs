using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Shared;
using Beatmap.V2;
using Jint;
using Jint.Native.Object;

namespace V2
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

        public Event(Engine engine, ObjectInstance o) : base(engine, new V2Event(
                (float)GetJsValue(o, new string[] { "_time", "b" }),
                (int)GetJsValue(o, new string[] { "_type", "et" }),
                (int)GetJsValue(o, new string[] { "_value", "i" }),
                GetCustomData(o, new string[] { "_customData", "customData" }),
                (float)GetJsValue(o, new string[] { "_floatValue", "f" })
            ), false, GetJsBool(o, "selected"))
        {
            spawned = false;

            DeleteObject();
        }

        public override bool SpawnObject(BeatmapObjectContainerCollection collection)
        {
            if (spawned) return false;

            if (wrapped.CustomData != null && wrapped.CustomData["_lightGradient"] != null)
            {
                wrapped.CustomLightGradient = new ChromaLightGradient(wrapped.CustomData["_lightGradient"]);
            }

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
