using System.Collections.Generic;
using System.Linq;

public class CheckResult {
    public class Problem
    {
        public BeatmapNote note;
        public string reason;

        public Problem(BeatmapNote note, string reason)
        {
            this.note = note;
            this.reason = reason;
        }
    }

    public List<Problem> errors = new List<Problem>();
    public List<Problem> warnings = new List<Problem>();
    public List<Problem> all;

    public CheckResult()
    {

    }

    public void Add(BeatmapNote note, string reason = "")
    {
        AddError(note, reason);
    }

    public void AddError(BeatmapNote note, string reason = "")
    {
        errors.Add(new Problem(note, reason));
    }

    public void AddWarning(BeatmapNote note, string reason = "")
    {
        warnings.Add(new Problem(note, reason));
    }

    public CheckResult Commit()
    {
        all = errors.Union(warnings).OrderBy(it => it.note._time).ToList();
        return this;
    }
}
