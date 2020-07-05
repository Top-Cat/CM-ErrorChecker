using Jint;
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
            float.TryParse(p.Value.Value, out float def);
            Params.Add(new Param(p.Key, def));
        }
        string name = engine.GetValue(exports, "name").AsString();
        Name = "ExternalJS: " + name;
    }

    private BeatmapNote FromDynamic(dynamic note, List<BeatmapNote> notes)
    {
        float _time = Convert.ChangeType(note._time, typeof(float));
        int _lineIndex = Convert.ChangeType(note._lineIndex, typeof(int));
        int _lineLayer = Convert.ChangeType(note._lineLayer, typeof(int));
        int _type = Convert.ChangeType(note._type, typeof(int));
        int _cutDirection = Convert.ChangeType(note._cutDirection, typeof(int));

        return notes.Find(it =>
        {
            return Mathf.Approximately(_time, it._time) &&
                _lineIndex == it._lineIndex &&
                _lineLayer == it._lineLayer &&
                _type == it._type &&
                _cutDirection == it._cutDirection;
        });
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, params float[] vals)
    {
        result.Clear();

        var arr = new JSONArray();
        notes.ForEach(it => arr.Add(it.ConvertToJSON()));

        engine
            .SetValue("notes", arr.ToString())
            .SetValue("minTime", 0.24f)
            .SetValue("maxTime", 0.75f)
            .SetValue("addError", new Action<object, string>((dynamic note, string str) =>
            {
                var obj = FromDynamic(note, notes);

                if (obj != null)
                    result.Add(obj, str ?? "");
            }))
            .SetValue("addWarning", new Action<object, string>((dynamic note, string str) =>
            {
                var obj = FromDynamic(note, notes);

                if (obj != null)
                    result.AddWarning(obj, str ?? "");
            }))
            .Execute("notes = JSON.parse(notes); module.exports.performCheck({notes: notes}" + (vals.Length > 0 ? ", " + string.Join(",", vals.Select(it => it.ToString())) : "") + ");");

        return result;
    }
}
