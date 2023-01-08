using System;
using Beatmap.Base;
using Jint;

abstract class VanillaWrapper<T> : Wrapper<T> where T : BaseObject
{
    private Lazy<JSONWrapper> pCustomData;
    private Action reconcile;

    public object customData
    {
        get => wrapped.CustomData == null ? null : pCustomData.Value;
        set
        {
            DeleteObject();
            wrapped.CustomData = JSONWrapper.castObjToJSON(value);
            wrapped.RefreshCustom();
            InitWrapper();
        }
    }

    public object _customData
    {
        get => wrapped.CustomData == null ? null : pCustomData.Value;
        set
        {
            DeleteObject();
            wrapped.CustomData = JSONWrapper.castObjToJSON(value);
            wrapped.RefreshCustom();
            InitWrapper();
        }
    }

    protected VanillaWrapper(Engine engine, T wrapped, bool hasOriginal = true, bool? selected = null) : base(engine, wrapped, hasOriginal, selected)
    {
        InitWrapper();
    }

    private void InitWrapper()
    {
        reconcile = null;
        pCustomData = new Lazy<JSONWrapper>(() =>
            new JSONWrapper(engine, ref reconcile, wrapped.CustomData, DeleteObject)
        );
    }

    internal override void Reconcile()
    {
        reconcile?.Invoke();

        if (wrapped.CustomData != null && wrapped.CustomData.Count == 0)
        {
            wrapped.CustomData = null;
        }
        
        wrapped.RefreshCustom();
    }
}
