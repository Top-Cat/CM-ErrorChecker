using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;
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

    public Func<U, TResult> Bind<T, U, TResult>(Func<T, U, TResult> func, T arg)
    {
        return (file) => func(arg, file);
    }

    private JsValue require(string folder, string file) {
        if (!file.EndsWith(".js"))
        {
            file += ".js";
        }
        string fullPath = Path.Combine(folder, file);
        string jsSource = File.ReadAllText(fullPath);
        string newFolder = new FileInfo(fullPath).DirectoryName;
        try
        {
            var e = new Engine()
                .SetValue("log", new Action<object>(Debug.Log))
                .SetValue("require", new Func<string, JsValue>(Bind<string, string, JsValue>(require, newFolder)))
                .Execute("exports = {}; module = {exports: exports}; console = {log: log};")
                .Execute(jsSource);

            var res = e.GetCompletionValue();

            if (res.IsUndefined())
            {
                res = e.GetValue("exports");
            }

            return res;
        }
        catch (JavaScriptException jse)
        {
            Debug.Log(jse);
            Debug.Log("LINE: " + jse.LineNumber);
            Debug.Log("COLUMN: " + jse.Column);
        }
        return null;
    }

    public ExternalJS(string fileName)
    {
        string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        StreamReader streamReader = new StreamReader(Path.Combine(assemblyFolder, fileName));
        string script = streamReader.ReadToEnd();
        streamReader.Close();

        try
        {
            engine
                .SetValue("require", new Func<string, JsValue>(Bind<string, string, JsValue>(require, assemblyFolder)))
                .SetValue("log", new Action<object>(Debug.Log))
                .Execute("module = {exports: {}}; console = {log: log};")
                .Execute(script)
                .Execute("module.exports.params = JSON.stringify(module.exports.params);");
        }
        catch (JavaScriptException jse)
        {
            Debug.Log(jse);
            Debug.Log("LINE: " + jse.LineNumber);
            Debug.Log("COLUMN: " + jse.Column);
        }

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

    public override CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, params float[] vals)
    {
        result.Clear();

        float currentBeat = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).AudioTimeSyncController.CurrentBeat;

        try {
        engine
            .SetValue("notes", notes.Select(it => new Note(it)).ToArray())
            .SetValue("walls", walls.Select(it => new Wall(it)).ToArray())
            .SetValue("events", events.Select(it => new Event(it)).ToArray())
            .SetValue("cursor", currentBeat)
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
            .Execute("var output = module.exports.run ? module.exports.run(cursor, notes, events, walls) : module.exports.performCheck({notes: notes}" + (vals.Length > 0 ? ", " + string.Join(",", vals.Select(it => it.ToString())) : "") + ");" +
            "if (output && output.notes) { notes = output.notes; };" +
            "if (output && output.events) { events = output.events; };" +
            "if (output && output.walls) { walls = output.walls; };");
        }
        catch (JavaScriptException jse)
        {
            Debug.Log(jse);
            Debug.Log("LINE: " + jse.LineNumber);
            Debug.Log("COLUMN: " + jse.Column);
        }


        Reconcile(engine.GetValue("notes").AsArray(), notes, i => new Note(i), BeatmapObject.Type.NOTE);
        Reconcile(engine.GetValue("walls").AsArray(), walls, i => new Wall(i), BeatmapObject.Type.OBSTACLE);
        Reconcile(engine.GetValue("events").AsArray(), events, i => new Event(i), BeatmapObject.Type.EVENT);

        return result;
    }

    private void Reconcile<T, U>(ArrayInstance noteArr, List<T> notes, Func<ObjectInstance, U> inst, BeatmapObject.Type type) where U : Wrapper<T> where T : BeatmapObject
    {
        List<U> outputNotes = new List<U>();
        foreach (var test in noteArr)
        {
            if (test is ObjectWrapper)
            {
                var o = test as ObjectWrapper;
                var note = o.Target as U;

                outputNotes.Add(note);
            }
            else if (test is ObjectInstance)
            {
                var o = test.AsObject();
                var note = inst(o);

                outputNotes.Add(note);
            }
            else
            {
                Debug.Log("Something else???");
                Debug.Log(test.GetType());
            }
        }

        var outputObjs = outputNotes.Select(it => it.wrapped);
        var toRemove = notes.Where(it => !outputObjs.Contains(it));

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(type);
        foreach (var removeMe in toRemove)
        {
            collection.LoadedContainers.TryGetValue(removeMe, out BeatmapObjectContainer container);
            collection.DeleteObject(removeMe, false);
        }

        foreach (var note in outputNotes)
        {
            note.SpawnObject();
        }

        collection.RefreshPool();
    }
}
