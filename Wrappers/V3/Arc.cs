using Jint;
using Jint.Native.Object;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;

namespace V3
{
    class Arc : VanillaWrapper<BaseArc>
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
            get => wrapped.PosX;
            set
            {
                DeleteObject();
                wrapped.PosX = value;
            }
        }

        public int y
        {
            get => wrapped.PosY;
            set
            {
                DeleteObject();
                wrapped.PosY = value;
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
            get => wrapped.TailPosX;
            set
            {
                DeleteObject();
                wrapped.TailPosX = value;
            }
        }

        public int ty
        {
            get => wrapped.TailPosY;
            set
            {
                DeleteObject();
                wrapped.TailPosY = value;
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
            get => wrapped.MidAnchorMode;
            set
            {
                DeleteObject();
                wrapped.MidAnchorMode = value;
            }
        }

        public Arc(Engine engine, BaseArc arc) : base(engine, arc)
        {
            spawned = true;
        }

        public Arc(Engine engine, ObjectInstance o) : base(engine, new V3Arc(JSONWrapper.dictToJSON(new Dictionary<string, object>()
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
            { "customData", GetCustomData(o, new string[] { "customData", "_customData" }) }
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Arc);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
