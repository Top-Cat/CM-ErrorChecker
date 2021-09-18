using System;
using System.Collections.Generic;

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

    protected virtual CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, List<BeatmapCustomEvent> customEvents, List<BeatmapBPMChange> bpmChanges)
    {
        throw new ArgumentException("Wrong number of parameters");
    }

    public virtual CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, List<BeatmapCustomEvent> customEvents, List<BeatmapBPMChange> bpmChanges, params IParamValue[] vals)
    {
        if (vals.Length == 0 && Params.Count == 0)
        {
            return PerformCheck(notes, events, walls, customEvents, bpmChanges);
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
