using System.Collections.Generic;
using System.Linq;
using Beatmap.Base;

public class CheckResult {
    public class Problem
    {
        public BaseNote note;
        public string reason;

        public Problem(BaseNote note, string reason)
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

    public void Clear()
    {
        errors.Clear();
        warnings.Clear();
        all = null;
    }

    public void Add(BaseNote note, string reason = "")
    {
        AddError(note, reason);
    }

    public void AddError(BaseNote note, string reason = "")
    {
        errors.Add(new Problem(note, reason));
    }

    public void AddWarning(BaseNote note, string reason = "")
    {
        warnings.Add(new Problem(note, reason));
    }

    public CheckResult Commit()
    {
        all = errors.Union(warnings).OrderBy(it => it.note.JsonTime).ToList();
        return this;
    }
}
