using Beatmap.Base.Customs;
using Beatmap.Enums;
using Beatmap.V2.Customs;
using Jint;
using Jint.Native.Object;

namespace V2
{
    class BpmChange : Wrapper<BaseBpmChange>
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

        public float _BPM
        {
            get => wrapped.Bpm;
            set
            {
                DeleteObject();
                wrapped.Bpm = value;
            }
        }

        public float _beatsPerBar
        {
            get => wrapped.BeatsPerBar;
            set
            {
                DeleteObject();
                wrapped.BeatsPerBar = value;
            }
        }

        public float _metronomeOffset
        {
            get => wrapped.MetronomeOffset;
            set
            {
                DeleteObject();
                wrapped.MetronomeOffset = value;
            }
        }

        public BpmChange(Engine engine, BaseBpmChange bpmChange) : base(engine, bpmChange)
        {
            spawned = true;
        }

        public BpmChange(Engine engine, ObjectInstance o) : base(engine, new V2BpmChange(
                (float)GetJsValue(o, "_BPM"),
                (float)GetJsValue(o, "_time")
        )
        {
            BeatsPerBar = (float)GetJsValue(o, "_beatsPerBar"),
            MetronomeOffset = (float)GetJsValue(o, "_metronomeOffset")
        }, false, GetJsBool(o, "selected"))
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

            var collection = BeatmapObjectContainerCollection.GetCollectionForType(ObjectType.BpmChange);
            collection.DeleteObject(wrapped, false);

            spawned = false;
            return true;
        }

        internal override void Reconcile()
        {
            // Nothing :)
        }
    }
}
