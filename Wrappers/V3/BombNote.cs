using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;
using Jint;
using Jint.Native.Object;

namespace V3
{
    class BombNote : VanillaWrapper<BaseBombNote>
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

        public BombNote(Engine engine, BaseBombNote bomb) : base(engine, bomb)
        {
            spawned = true;
        }

        public BombNote(Engine engine, ObjectInstance o) : base(engine, new V3BombNote(
            (float)GetJsValue(o, "b"),
            (int)GetJsValue(o, "x"),
            (int)GetJsValue(o, "y"),
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Note);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
