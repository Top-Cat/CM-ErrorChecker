using Jint;
using Jint.Native.Object;

namespace V3
{
    class ColorNote : VanillaWrapper<BeatmapColorNote>
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
            get => wrapped.Color;
            set
            {
                DeleteObject();
                wrapped.Color = value;
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
            (float)GetJsValue(o, new string[] { "b", "_time" }),
            (int)GetJsValue(o, new string[] { "x", "_lineIndex" }),
            (int)GetJsValue(o, new string[] { "y", "_lineLayer" }),
            (int)GetJsValue(o, new string[] { "c", "_type" }),
            (int)GetJsValue(o, new string[] { "d", "_cutDirection" }),
            (int)(GetJsValueOptional(o, "a") ?? 0),
            GetCustomData(o, new string[] { "customData", "_customData" })
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
