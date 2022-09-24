using Jint;
using Jint.Native.Object;

namespace V3
{
    class BombNote : VanillaWrapper<BeatmapBombNote>
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

        public int x
        {
            get => wrapped.LineIndex;
            set
            {
                DeleteObject();
                wrapped.LineIndex = value;
            }
        }

        public int y
        {
            get => wrapped.LineLayer;
            set
            {
                DeleteObject();
                wrapped.LineLayer = value;
            }
        }

        public BombNote(Engine engine, BeatmapBombNote bomb) : base(engine, bomb)
        {
            spawned = true;
        }

        public BombNote(Engine engine, ObjectInstance o) : base(engine, new BeatmapBombNote(
            (float)GetJsValue(o, "b"),
            (int)GetJsValue(o, "x"),
            (int)GetJsValue(o, "y"),
            GetCustomData(o, "customData")
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
