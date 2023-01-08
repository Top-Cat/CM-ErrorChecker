using Jint;
using Jint.Native.Object;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Enums;
using Beatmap.V3;

namespace V3
{
    class Wall : VanillaWrapper<BaseObstacle>
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

        public int _height
        {
            get => wrapped.Height;
            set
            {
                DeleteObject();
                wrapped.Height = value;
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

        public Wall(Engine engine, BaseObstacle wall) : base(engine, wall)
        {
            spawned = true;
        }

        public Wall(Engine engine, ObjectInstance o) : base(engine, new V3Obstacle(JSONWrapper.dictToJSON(new Dictionary<string, object>()
            {
                { "b", (float)GetJsValue(o, new string[] { "b", "_time" }) },
                { "x", (int)GetJsValue(o, new string[] { "x", "_lineIndex" }) },
                { "y", (int)(GetJsExist(o, "_type") ? (GetJsValue(o, "_type") == 0 ? 0 : 2) : GetJsValue(o, "y")) },
                { "d", (float)GetJsValue(o, new string[] { "d", "_duration" }) },
                { "w", (int)GetJsValue(o, new string[] { "w", "_width" }) },
                { "h", (int)(GetJsExist(o, "_type") ? (GetJsValue(o, "_type") == 0 ? 5 : 3) : GetJsValue(o, "h")) },
                { "customData", GetCustomData(o, new string[] { "customData", "_customData" }) }
            })), false, GetJsBool(o, "selected"))
        {
            spawned = false;
            wrapped.CustomData =  GetCustomData(o, new string[] { "customData", "_customData" });

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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.Obstacle);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }
    }
}
