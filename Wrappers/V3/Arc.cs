using Jint;
using Jint.Native.Object;
using SimpleJSON;
using System.Collections.Generic;

namespace V3
{
    class Arc : VanillaWrapper<BeatmapArc>
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

        public float mu
        {
            get => wrapped.HeadControlPointLengthMultiplier;
            set
            {
                DeleteObject();
                wrapped.HeadControlPointLengthMultiplier = value;
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

        public int tc
        {
            get => wrapped.TailCutDirection;
            set
            {
                DeleteObject();
                wrapped.TailCutDirection = value;
            }
        }

        public float tmu
        {
            get => wrapped.TailControlPointLengthMultiplier;
            set
            {
                DeleteObject();
                wrapped.TailControlPointLengthMultiplier = value;
            }
        }

        public int m
        {
            get => wrapped.ArcMidAnchorMode;
            set
            {
                DeleteObject();
                wrapped.ArcMidAnchorMode = value;
            }
        }

        public Arc(Engine engine, BeatmapArc arc) : base(engine, arc)
        {
            spawned = true;
        }

        public Arc(Engine engine, ObjectInstance o) : base(engine, new BeatmapArc(JSONWrapper.dictToJSON(new Dictionary<string, object>()
        {
            { "b", (float)GetJsValue(o, "b") },
            { "c", (int)GetJsValue(o, "c") },
            { "x", (int) GetJsValue(o, "x") },
            { "y", (int)GetJsValue(o, "y") },
            { "d", (int)GetJsValue(o, "d") },
            { "mu", (float)GetJsValue(o, "mu") },
            { "tb", (float)GetJsValue(o, "tb") },
            { "tx", (int)GetJsValue(o, "tx") },
            { "ty", (int)GetJsValue(o, "ty") },
            { "tc", (int)GetJsValue(o, "tc") },
            { "tmu", (float)GetJsValue(o, "tmu") },
            { "m", (int)GetJsValue(o, "m") },
            { "customData", GetCustomData(o, "customData") }
        })), false, GetJsBool(o, "selected"))
        {
            spawned = false;
            wrapped.CustomData = GetCustomData(o, new string[] { "customData", "_customData" });

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
