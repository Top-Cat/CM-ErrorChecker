using System;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Base.Customs;

public abstract class Check
{
    public string Name;
    public readonly List<IParam> Params = new List<IParam>();
    protected readonly CheckResult result = new CheckResult();
    public bool errorCheck = true;

    protected Check(string name)
    {
        Name = name;
    }

    protected Check() : this("")
    {
        
    }

    protected virtual CheckResult PerformCheck(List<BaseNote> notes, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges)
    {
        throw new ArgumentException("Wrong number of parameters");
    }

    public virtual CheckResult PerformCheck(List<BaseNote> notes, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges, params IParamValue[] vals)
    {
        if (vals.Length == 0 && Params.Count == 0)
        {
            return PerformCheck(notes, events, walls, customEvents, bpmChanges);
        }
        throw new ArgumentException("Wrong number of parameters");
    }

    protected virtual CheckResult PerformCheck(List<BaseNote> notes, List<BaseNote> bombs, List<BaseArc> arcs, List<BaseChain> chains, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges)
    {
        throw new ArgumentException("Wrong number of parameters");
    }

    public virtual CheckResult PerformCheck(List<BaseNote> notes, List<BaseNote> bombs, List<BaseArc> arcs, List<BaseChain> chains, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges, params IParamValue[] vals)
    {
        if (vals.Length == 0 && Params.Count == 0)
        {
            return PerformCheck(notes, bombs, arcs, chains, events, walls, customEvents, bpmChanges);
        }
        throw new ArgumentException("Wrong number of parameters");
    }

    public virtual void OnSelected()
    {

    }

    public virtual void Reload()
    {

    }
}
