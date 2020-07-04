using Jint;
using Jint.Native.Array;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

class ExternalJS : Check
{
    private Engine engine = new Engine();

    public ExternalJS(string fileName)
    {
        string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        StreamReader streamReader = new StreamReader(Path.Combine(assemblyFolder, fileName));
        string script = streamReader.ReadToEnd();
        streamReader.Close();

        engine
            .SetValue("log", new Action<object>(Debug.Log))
            .Execute("module = {exports: {}};")
            .Execute(script)
            .Execute("module.exports.params = JSON.stringify(module.exports.params);");

        var exports = engine.GetValue(engine.GetValue("module"), "exports");
        JSONObject ps = JSON.Parse(engine.GetValue(exports, "params").AsString()).AsObject;
        foreach (var p in ps)
        {
            Debug.Log(fileName + " has param " + p);
            float.TryParse(p.Value.Value, out float def);
            Params.Add(new Param(p.Key, def));
        }
        string name = engine.GetValue(exports, "name").AsString();
        Name = "ExternalJS: " + name;
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, params float[] vals)
    {
        result.Clear();

        var arr = new JSONArray();
        notes.ForEach(it => arr.Add(it.ConvertToJSON()));

        string r = engine
            .SetValue("notes", arr.ToString())
            .SetValue("minTime", 0.24f)
            .SetValue("maxTime", 0.75f)
            .Execute("notes = JSON.parse(notes); errors = []; warnings = []; " +
                "function addError(note, reason) { note.reason = reason; errors.push(note); }; " +
                "function addWarning(note, reason) { note.reason = reason; warnings.push(note); };" +
                "module.exports.performCheck({notes: notes}" + (vals.Length > 0 ? ", " + string.Join(",", vals.Select(it => it.ToString())) : "") + ");" +
                "errors = JSON.stringify(errors); warnings = JSON.stringify(warnings);")
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
