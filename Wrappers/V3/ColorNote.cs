using Jint;
using Jint.Native.Object;

namespace V3
{
    class ColorNote : VanillaWrapper<BeatmapColorNote>
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

        public int a
        {
            get => wrapped.AngleOffset;
            set
            {
                DeleteObject();
                wrapped.AngleOffset = value;
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

        public ColorNote(Engine engine, BeatmapColorNote note) : base(engine, note)
        {
            spawned = true;
        }

        public ColorNote(Engine engine, ObjectInstance o) : base(engine, new BeatmapColorNote(
            (float)GetJsValue(o, "b"),
            (int)GetJsValue(o, "x"),
            (int)GetJsValue(o, "y"),
            (int)GetJsValue(o, "c"),
            (int)GetJsValue(o, "d"),
            (int)GetJsValue(o, "a"),
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
