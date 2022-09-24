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

        public Wall(Engine engine, BeatmapObstacle wall) : base(engine, wall)
        {
            spawned = true;
        }

        public Wall(Engine engine, ObjectInstance o) : base(engine, new BeatmapObstacle(
                (float)GetJsValue(o, "_time"),
                (int)GetJsValue(o, "_lineIndex"),
                (int)GetJsValue(o, "_type"),
                (float)GetJsValue(o, "_duration"),
                (int)GetJsValue(o, "_width"),
                GetCustomData(o)
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
