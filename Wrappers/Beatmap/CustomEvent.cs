using System;
using Beatmap.Base.Customs;
using Beatmap.Enums;
using Beatmap.Helper;
using Jint;
using Jint.Native.Object;

    internal class CustomEvent : Wrapper<BaseCustomEvent>
    {
        private Lazy<JSONWrapper> customData;
        private Action reconcile;

        public CustomEvent(Engine engine, BaseCustomEvent customEvent) : base(engine, customEvent)
        {
            spawned = true;
            InitWrapper();
        }

        public CustomEvent(Engine engine, ObjectInstance o) : base(engine, BeatmapFactory.CustomEvent(
            (float)GetJsValue(o, new [] { "b", "_time" }),
            GetJsString(o, new [] { "t", "_type" }),
            GetCustomData(o, new [] { "d", "_data" })
        ), false, GetJsBool(o, "selected"))
        {
            spawned = false;

            DeleteObject();
            InitWrapper();
        }

        public float _time
        {
            get => wrapped.Time;
            set
            {
                DeleteObject();
                wrapped.Time = value;
            }
        }

        public string _type
        {
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
            }
        }

        public object _data
        {
            get => wrapped.CustomData == null ? null : customData.Value;
            set
            {
                DeleteObject();
                wrapped.CustomData = JSONWrapper.castObjToJSON(value);
                InitWrapper();
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

        public string t
        {
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
            }
        }

        public object d
        {
            get => wrapped.CustomData == null ? null : customData.Value;
            set
            {
                DeleteObject();
                wrapped.CustomData = JSONWrapper.castObjToJSON(value);
                InitWrapper();
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.CustomEvent);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }

        private void InitWrapper()
        {
            reconcile = null;
            customData = new Lazy<JSONWrapper>(() =>
                new JSONWrapper(engine, ref reconcile, wrapped.CustomData, DeleteObject)
            );
        }

        internal override void Reconcile()
        {
            reconcile?.Invoke();
        }
    }
