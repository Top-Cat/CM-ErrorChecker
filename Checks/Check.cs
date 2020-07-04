using System.Collections.Generic;

public abstract class Check
{
    public string Name;
    protected CheckResult result = new CheckResult();

    public Check(string name)
    {
        Name = name;
    }

    public abstract CheckResult PerformCheck(List<BeatmapNote> notes, float minTime, float maxTime);
}
