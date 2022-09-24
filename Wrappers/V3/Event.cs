using Jint;
using Jint.Native.Object;

namespace V3
{
    class Event : VanillaWrapper<MapEventV3>
    {
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

        public Event(Engine engine, MapEventV3 mapEvent) : base(engine, mapEvent)
        {
            spawned = true;
        }

        public Event(Engine engine, ObjectInstance o) : base(engine, new MapEventV3(JSONWraper.castObjToJSON(new
        {
            b = (float)GetJsValue(o, "b"),
            et = (int)GetJsValue(o, "et"),
            i = (int)GetJsValue(o, "i"),
            f = (float)GetJsValue(o, "f"),
            customData = GetCustomData(o, "customData")
        })
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Event);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
