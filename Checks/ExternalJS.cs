using Jint;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

class ExternalJS : Check
{
    private string fileName;

    public ExternalJS(string fileName) : base("ExternalJS: " + fileName)
    {
        this.fileName = fileName;
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, float minTime, float maxTime)
    {
        result.Clear();

        string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        StreamReader streamReader = new StreamReader(Path.Combine(assemblyFolder, fileName));
        string script = streamReader.ReadToEnd();
        streamReader.Close();

        var engine = new Engine();

        var arr = new JSONArray();
        notes.ForEach(it => arr.Add(it.ConvertToJSON()));

        string r = engine
            .SetValue("log", new Action<object>(Debug.Log))
            .SetValue("notes", arr.ToString())
            .SetValue("minTime", 0.24f)
            .SetValue("maxTime", 0.75f)
            .Execute("notes = JSON.parse(notes); errors = []; warnings = []; " +
                "function addError(note, reason) { note.reason = reason; errors.push(note); }; " +
                "function addWarning(note, reason) { note.reason = reason; warnings.push(note); };")
            .Execute(script)
            .Execute("errors = JSON.stringify(errors); warnings = JSON.stringify(warnings);")
            .GetValue("errors").AsString();
        string w = engine
            .GetValue("warnings").AsString();

        JSONArray errors = JSON.Parse(r).AsArray;
        JSONArray warnings = JSON.Parse(w).AsArray;

        foreach (var err in errors)
        {
            var errObj = err.Value.AsObject;
            var obj = notes.Find(it =>
            {
                return Mathf.Approximately(errObj["_time"].AsFloat, it._time) &&
                    errObj["_lineIndex"].AsInt == it._lineIndex &&
                    errObj["_lineLayer"].AsInt == it._lineLayer &&
                    errObj["_type"].AsInt == it._type &&
                    errObj["_cutDirection"].AsInt == it._cutDirection;
            });

            if (obj != null)
                result.Add(obj, errObj["reason"] ?? "");
        }

        foreach (var err in warnings)
        {
            var errObj = err.Value.AsObject;
            var obj = notes.Find(it =>
            {
                return Mathf.Approximately(errObj["_time"].AsFloat, it._time) &&
                    errObj["_lineIndex"].AsInt == it._lineIndex &&
                    errObj["_lineLayer"].AsInt == it._lineLayer &&
                    errObj["_type"].AsInt == it._type &&
                    errObj["_cutDirection"].AsInt == it._cutDirection;
            });

            if (obj != null)
                result.AddWarning(obj, errObj["reason"] ?? "");
        }

        return result;
    }
}
