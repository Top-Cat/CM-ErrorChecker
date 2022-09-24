using Jint;
using Jint.Native.Object;
using System.Collections.Generic;

namespace V3
{
    class Chain : VanillaWrapper<BeatmapChain>
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

        public int c
        {
            get => wrapped.Color;
            set
            {
                DeleteObject();
                wrapped.Color = value;
            }
        }

        public int x
        {
            get => wrapped.X;
            set
            {
                DeleteObject();
                wrapped.X = value;
            }
        }

        public int y
        {
            get => wrapped.Y;
            set
            {
                DeleteObject();
                wrapped.Y = value;
            }
        }

        public int d
        {
            get => wrapped.Direction;
            set
            {
                DeleteObject();
                wrapped.Direction = value;
            }
        }

        public float tb
        {
            get => wrapped.TailTime;
            set
            {
                DeleteObject();
                wrapped.TailTime = value;
            }
        }

        public int tx
        {
            get => wrapped.TailX;
            set
            {
                DeleteObject();
                wrapped.TailX = value;
            }
        }

        public int ty
        {
            get => wrapped.TailX;
            set
            {
                DeleteObject();
                wrapped.TailX = value;
            }
        }

        public int sc
        {
            get => wrapped.SliceCount;
            set
            {
                DeleteObject();
                wrapped.SliceCount = value;
            }
        }

        public float s
        {
            get => wrapped.SquishAmount;
            set
            {
                DeleteObject();
                wrapped.SquishAmount = value;
            }
        }

        public Chain(Engine engine, BeatmapChain chain) : base(engine, chain)
        {
            spawned = true;
        }

        public Chain(Engine engine, ObjectInstance o) : base(engine, new BeatmapChain(JSONWrapper.dictToJSON(new Dictionary<string, dynamic>()
        {
            { "b", (float)GetJsValue(o, "b") },
            { "c", (int)GetJsValue(o, "c") },
            { "x", (int) GetJsValue(o, "x") },
            { "y", (int)GetJsValue(o, "y") },
            { "d", (int)GetJsValue(o, "d") },
            { "tb", (float)GetJsValue(o, "tb") },
            { "tx", (int)GetJsValue(o, "tx") },
            { "ty", (int)GetJsValue(o, "ty") },
            { "tc", (int)GetJsValue(o, "tc") },
            { "sc", (int)GetJsValue(o, "sc") },
            { "s", (float)GetJsValue(o, "s") },
            { "customData", GetCustomData(o, "customData") }
        })), false, GetJsBool(o, "selected"))
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
