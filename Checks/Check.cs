using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Check
{
    public string Name;
    public List<Param> Params = new List<Param>();
    protected CheckResult result = new CheckResult();

    public class Param
    {
        public string name;
        public float def;

        public Param(string name, float def)
        {
            this.name = name;
            this.def = def;
        }
    }

    public Check(string name)
    {
        Name = name;
    }

    public Check() : this("")
    {
        
    }

    public virtual CheckResult PerformCheck(List<BeatmapNote> notes)
    {
        throw new ArgumentException("Wrong number of parameters");
    }

    public virtual CheckResult PerformCheck(List<BeatmapNote> notes, params float[] vals)
    {
        Debug.Log("Y");
        if (vals.Length == 0 && Params.Count == 0)
        {
            return PerformCheck(notes);
        }
        throw new ArgumentException("Wrong number of parameters");
    }
}
