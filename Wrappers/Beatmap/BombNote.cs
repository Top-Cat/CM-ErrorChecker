using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

    internal class BombNote : VanillaWrapper<BaseNote>
    {
        public BombNote(Engine engine, BaseNote bomb) : base(engine, bomb)
        {
            spawned = true;
        }

        public BombNote(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.Bomb(
            (float)GetJsValue(o, "b"),
            (int)GetJsValue(o, "x"),
            (int)GetJsValue(o, "y"),
            GetCustomData(o, new[] { "customData", "_customData" })
        ), false, GetJsBool(o, "selected"))
        {
            spawned = false;

            DeleteObject();
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
