using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V2;
using Jint;
using Jint.Native.Object;

namespace V2
{
    class Note : VanillaWrapper<BaseNote>
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
            get => wrapped.PosX;
            set
            {
                DeleteObject();
                wrapped.PosX = value;
            }
        }

        public int _lineLayer
        {
            get => wrapped.PosY;
            set
            {
                DeleteObject();
                wrapped.PosY = value;
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
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
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

        public int a
        {
            get => 0;
            set
            {
            }
        }

        public int c
        {
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
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

        public Note(Engine engine, BaseNote note) : base(engine, note)
        {
            spawned = true;
        }

        public Note(Engine engine, ObjectInstance o) : base(engine, new V2Note(
            (float)GetJsValue(o, new string[] { "_time", "b" }),
            (int)GetJsValue(o, new string[] { "_lineIndex", "x" }),
            (int)GetJsValue(o, new string[] { "_lineLayer", "y" }),
            (int)GetJsValue(o, new string[] { "_type", "c" }),
            (int)GetJsValue(o, new string[] { "_cutDirection", "d" }),
            GetCustomData(o, new string[] { "_customData", "customData" })
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
