using Jint;
using Jint.Native.Object;

namespace V2
{
    class Wall : VanillaWrapper<BeatmapObstacle>
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

        public int _type
        {
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
            }
        }

        public float _duration
        {
            get => wrapped.Duration;
            set
            {
                DeleteObject();
                wrapped.Duration = value;
            }
        }

        public int _width
        {
            get => wrapped.Width;
            set
            {
                DeleteObject();
                wrapped.Width = value;
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
            get => 0;
            set
            {
            }
        }

        public float d
        {
            get => wrapped.Duration;
            set
            {
                DeleteObject();
                wrapped.Duration = value;
            }
        }

        public int w
        {
            get => wrapped.Width;
            set
            {
                DeleteObject();
                wrapped.Width = value;
            }
        }

        public int h
        {
            get => 0;
            set
            {
            }
        }

        public Wall(Engine engine, BeatmapObstacle wall) : base(engine, wall)
        {
            spawned = true;
        }

        public Wall(Engine engine, ObjectInstance o) : base(engine, new BeatmapObstacle(
                (float)GetJsValue(o, new string[] { "_time", "b" }),
                (int)GetJsValue(o, new string[] { "_lineIndex", "x" }),
                (int)(GetJsValueOptional(o, "_type") ?? (GetJsValue(o, "y") < 2 ? 0 : 1)),
                (float)GetJsValue(o, new string[] { "_duration", "d" }),
                (int)GetJsValue(o, new string[] { "_width", "w" }),
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Obstacle);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
