using Jint;
using Jint.Native.Object;

namespace V3
{
    class Wall : VanillaWrapper<BeatmapObstacleV3>
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
            get => wrapped.Height;
            set
            {
                DeleteObject();
                wrapped.Height = value;
            }
        }

        public Wall(Engine engine, BeatmapObstacleV3 wall) : base(engine, wall)
        {
            spawned = true;
        }

        public Wall(Engine engine, ObjectInstance o) : base(engine, new BeatmapObstacleV3(JSONWrapper.castObjToJSON(new {
                b = (float)GetJsValue(o, "b"),
                x = (int)GetJsValue(o, "x"),
                y = (int)GetJsValue(o, "y"),
                d = (float)GetJsValue(o, "d"),
                w = (int)GetJsValue(o, "w"),
                h = (int)GetJsValue(o, "h"),
                customData = GetCustomData(o, "customData")
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Obstacle);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
