using System;
using Jint;
using Jint.Native.Object;

namespace V2
{
    class CustomEvent : Wrapper<BeatmapCustomEvent>
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

        public string _type
        {
            get => wrapped.Type;
            set
            {
                DeleteObject();
                wrapped.Type = value;
            }
        }

        private Lazy<JSONWrapper> customData;
        private Action reconcile;
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

        public CustomEvent(Engine engine, BeatmapCustomEvent customEvent) : base(engine, customEvent)
        {
            spawned = true;
            InitWrapper();
        }

        public CustomEvent(Engine engine, ObjectInstance o) : base(engine, new BeatmapCustomEvent(
                (float)GetJsValue(o, "_time"),
                GetJsString(o, "_type"),
                GetCustomData(o, "_data")
        ), false, GetJsBool(o, "selected"))
        {
            spawned = false;

            DeleteObject();
            InitWrapper();
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Event);
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
}
