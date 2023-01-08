using Jint;
using Jint.Native.Object;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;

namespace V3
{
    class Chain : VanillaWrapper<BaseChain>
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
            get => wrapped.Squish;
            set
            {
                DeleteObject();
                wrapped.Squish = value;
            }
        }

        public Chain(Engine engine, BaseChain chain) : base(engine, chain)
        {
            spawned = true;
        }

        public Chain(Engine engine, ObjectInstance o) : base(engine, new V3Chain(JSONWrapper.dictToJSON(new Dictionary<string, object>()
        {
            { "b", (float)GetJsValue(o, "b") },
            { "c", (int)GetJsValue(o, "c") },
            { "x", (int) GetJsValue(o, "x") },
            { "y", (int)GetJsValue(o, "y") },
            { "d", (int)GetJsValue(o, "d") },
            { "tb", (float)GetJsValue(o, "tb") },
            { "tx", (int)GetJsValue(o, "tx") },
            { "ty", (int)GetJsValue(o, "ty") },
            { "sc", (int)GetJsValue(o, "sc") },
            { "s", (float)GetJsValue(o, "s") },
            { "customData", GetCustomData(o, new string[] { "customData", "_customData" }) }
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Chain);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
