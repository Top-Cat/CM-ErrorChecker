using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Esprima;
using UnityEngine;

class ExternalJS : Check
{
    private Engine engine;
    private readonly Constraint timeConstraint = new TimeConstraint2(TimeSpan.FromSeconds(30L));
    private readonly string fileName;
    private bool valid;

    private const bool DebugMode = false;
    private static void TimeLog(string msg) {
        if (!DebugMode) return;

        var time = DateTime.Now;
        Debug.Log($"[{time}.{time.Millisecond}] [CM-JS] {msg}");
    }

    private class TimeConstraint2 : Constraint
    {
        private readonly TimeSpan _timeout;
        private CancellationTokenSource cts;

        public TimeConstraint2(TimeSpan timeout) => _timeout = timeout;

        public override void Check()
        {
            if (!cts.IsCancellationRequested)
                return;
            throw new TimeoutException();
        }

        public override void Reset()
        {
            cts?.Dispose();
            cts = new CancellationTokenSource(this._timeout);
        }
    }

    public Func<U, TResult> Bind<T, U, TResult>(Func<T, U, TResult> func, T arg)
    {
        return (file) => func(arg, file);
    }

    private static void LogIt(object o)
    {
        if (o is ExpandoObject ex)
        {
            Debug.Log(JSONWraper.dictToJSON(ex));
        }
        else
        {
            Debug.Log(o);
        }
    }

    private static void Alert(string o)
    {
        PersistentUI.Instance.ShowDialogBox(o, r =>
        {
            // Callback?
        }, PersistentUI.DialogBoxPresetType.Ok);
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
            var e = new Engine(options =>
                {
                    options.Constraint(timeConstraint).LimitRecursion(200);
                })
                .SetValue("log", new Action<object>(LogIt))
                .SetValue("alert", new Action<string>(Alert))
                .SetValue("require", new Func<string, JsValue>(Bind<string, string, JsValue>(require, newFolder)))
                .Execute("exports = {}; module = {exports: exports}; console = {log: log};");

            var res = e.Evaluate(jsSource);

            if (res.IsUndefined())
            {
                res = e.GetValue("exports");
            }

            return res;
        }
        catch (JavaScriptException jse)
        {
            Debug.Log(jse);
        }
        return null;
    }

    public ExternalJS(string fileName)
    {
        this.fileName = fileName;
        LoadJS();
    }

    public override void Reload()
    {
        LoadJS();
    }

    private void LoadJS()
    {
        engine = new Engine(options =>
        {
            options.Constraint(timeConstraint).LimitRecursion(200);
        });

        var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var streamReader = new StreamReader(Path.Combine(assemblyFolder, fileName));
        var script = streamReader.ReadToEnd();
        streamReader.Close();

        try
        {
            engine
                .SetValue("require", new Func<string, JsValue>(Bind<string, string, JsValue>(require, assemblyFolder)))
                .SetValue("log", new Action<object>(LogIt))
                .SetValue("alert", new Action<string>(Alert))
                .Execute("module = {exports: {}}; console = {log: log}; var global = {};")
                .Execute(script)
                .Execute("module.exports.params = JSON.stringify(module.exports.params);");

            var exports = engine.GetValue(engine.GetValue("module"), "exports");

            var isErrorCheck = engine.GetValue(exports, "errorCheck");
            errorCheck = !isErrorCheck.IsBoolean() || isErrorCheck.AsBoolean();

            var @params = engine.GetValue(exports, "params");
            Params.Clear();
            if (@params.IsString())
            {
                var ps = JSON.Parse(@params.AsString()).AsObject;
                foreach (var p in ps)
                {
                    if (p.Value.IsBoolean)
                    {
                        Params.Add(new BoolParam(p.Key, p.Value.AsBool));
                    }
                    else if (p.Value.IsString)
                    {
                        Params.Add(new StringParam(p.Key, p.Value.Value));
                    }
                    else if (p.Value.IsNumber)
                    {
                        Params.Add(new FloatParam(p.Key, p.Value.AsFloat));
                    }
                    else if (p.Value.IsArray)
                    {
                        Params.Add(new ListParam(p.Key, p.Value.AsArray.Children.Select(it => it.Value).ToList()));
                    }
                }
            }

            var nameObj = engine.GetValue(exports, "name");
            if (nameObj.IsString())
            {
                var name = nameObj.AsString();
                Name = "ExternalJS: " + name;
            }
            else
            {
                Name = $"ExternalJS: {fileName}";
            }

            valid = true;
        }
        catch (JavaScriptException jse)
        {
            Name = $"ExternalJS: [{fileName}]";
            Debug.LogWarning($"Error loading {fileName}\n{jse.Message}");
        }
        catch (ParserException jse)
        {
            Name = $"ExternalJS: [{fileName}]";
            Debug.LogWarning($"Error loading {fileName}\n{jse.Message}");
        }
    }

    public override void OnSelected()
    {
        if (!valid) LoadJS();
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
            return Mathf.Approximately(_time, it.Time) &&
                _lineIndex == it.LineIndex &&
                _lineLayer == it.LineLayer &&
                _type == it.Type &&
                _cutDirection == it.CutDirection;
        });
    }

    class MapData {
        public float currentBPM { get; private set; }
        public float songBPM { get; private set; }
        public float NJS { get; private set; }
        public float offset { get; private set; }

        public MapData(float currentBPM, float songBPM, float NJS, float offset)
        {
            this.currentBPM = currentBPM;
            this.songBPM = songBPM;
            this.NJS = NJS;
            this.offset = offset;
        }
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, List<BeatmapCustomEvent> customEvents, List<BeatmapBPMChange> bpmChanges, params IParamValue[] vals)
    {
        result.Clear();

        var atsc = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).AudioTimeSyncController;
        var currentBeat = atsc.CurrentBeat;

        var collection = BeatmapObjectContainerCollection.GetCollectionForType<BPMChangesContainer>(BeatmapObject.ObjectType.BpmChange);
        var lastBPMChange = collection.FindLastBpm(atsc.CurrentBeat);
        var currentBPM = lastBPMChange?.Bpm ?? atsc.Song.BeatsPerMinute;

        var originalNotes = notes.Select(it => new Note(engine, it)).ToArray();
        var originalWalls = walls.Select(it => new Wall(engine, it)).ToArray();
        var originalEvents = events.Select(it => new Event(engine, it)).ToArray();
        var originalCustomEvents = customEvents.Select(it => new CustomEvent(engine, it)).ToArray();
        var originalBpmChanges = bpmChanges.Select(it => new BpmChange(engine, it)).ToArray();

        try
        {
            var valsToString = vals.Select(paramValue =>
            {
                switch (paramValue)
                {
                    case ParamValue<float> pvf:
                        return pvf.value.ToString();
                    case ParamValue<string> pvs:
                        return $"\"{pvs.value}\"";
                    case ParamValue<bool> pvb:
                        return pvb.value ? "true" : "false";
                    default:
                        return "null";
                }
            });

            var valsCombined = string.Join(",", valsToString);

            TimeLog("Init");

            var tmp = engine
            .SetValue("notes", originalNotes)
            .SetValue("walls", originalWalls)
            .SetValue("events", originalEvents)
            .SetValue("customEvents", originalCustomEvents)
            .SetValue("bpmChanges", originalBpmChanges)
            .SetValue("data", new MapData(
                currentBPM,
                atsc.Song.BeatsPerMinute,
                BeatSaberSongContainer.Instance.DifficultyData.NoteJumpMovementSpeed,
                BeatSaberSongContainer.Instance.DifficultyData.NoteJumpStartBeatOffset
            ))
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
            }));

            TimeLog("Run");

            tmp.Execute("global.params = [" + valsCombined + "];" +
            "var output = module.exports.run ? module.exports.run(cursor, notes, events, walls, {}, global, data, customEvents, bpmChanges) : module.exports.performCheck({notes: notes}" + (vals.Length > 0 ? ", " + valsCombined : "") + ");" +
            "if (output && output.notes) { notes = output.notes; };" +
            "if (output && output.events) { events = output.events; };" +
            "if (output && output.customEvents) { customEvents = output.customEvents; };" +
            "if (output && output.bpmChanges) { bpmChanges = output.bpmChanges; };" +
            "if (output && output.walls) { walls = output.walls; };");
        }
        catch (JavaScriptException jse)
        {
            Debug.LogWarning($"Error running {fileName}\n{jse.Message}");
        }

        TimeLog("Reconcile");

        SelectionController.DeselectAll();
        var actions = new List<BeatmapAction>();
        actions.AddRange(Reconcile(originalNotes, engine.GetValue("notes").AsArray(), notes, i => new Note(engine, i), BeatmapObject.ObjectType.Note));
        actions.AddRange(Reconcile(originalWalls, engine.GetValue("walls").AsArray(), walls, i => new Wall(engine, i), BeatmapObject.ObjectType.Obstacle));
        actions.AddRange(Reconcile(originalEvents, engine.GetValue("events").AsArray(), events, i => new Event(engine, i), BeatmapObject.ObjectType.Event));
        actions.AddRange(Reconcile(originalCustomEvents, engine.GetValue("customEvents").AsArray(), customEvents, i => new CustomEvent(engine, i), BeatmapObject.ObjectType.CustomEvent));
        actions.AddRange(Reconcile(originalBpmChanges, engine.GetValue("bpmChanges").AsArray(), bpmChanges, i => new BpmChange(engine, i), BeatmapObject.ObjectType.BpmChange));

        SelectionController.SelectionChangedEvent?.Invoke();

        TimeLog("Registering undo actions");

        if (actions.Count > 0)
        {
            var allAction = new ActionCollectionAction(actions, true, true, "External Script");
            BeatmapActionContainer.AddAction(allAction);
        }

        TimeLog("Fin");

        return result;
    }

    private List<BeatmapAction> Reconcile<T, U>(IEnumerable<U> original, ArrayInstance noteArr, List<T> notes, Func<ObjectInstance, U> inst, BeatmapObject.ObjectType type) where U : Wrapper<T> where T : BeatmapObject
    {
        TimeLog("Reconcile " + original.GetType());

        var beatmapActions = new List<BeatmapAction>();
        var outputNotes = new List<U>();
        foreach (var test in noteArr)
        {
            if (test is U a)
            {
                outputNotes.Add(a);
            }
            else if (test is ObjectWrapper b)
            {
                var note = b.Target as U;

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

        var lookup = original.ToDictionary(x => x.wrapped, x => x);

        TimeLog("Call reconcile on output");

        var outputObjs = outputNotes.Select(it =>
        {
            it.Reconcile();
            return it.wrapped;
        }).ToHashSet();
        TimeLog("Remove objects");
        var toRemove = notes.Where(it => !outputObjs.Contains(it));

        var collection = BeatmapObjectContainerCollection.GetCollectionForType(type);
        foreach (var removeMe in toRemove)
        {
            var wrapper = lookup[removeMe];

            //collection.LoadedContainers.TryGetValue(removeMe, out BeatmapObjectContainer container); // Does this do something?
            beatmapActions.Add(new BeatmapObjectDeletionAction(wrapper.original, "Script deleted object"));
            wrapper.DeleteObject();
        }

        TimeLog("Spawn objects");

        var toAction = new List<U>();
        foreach (var note in outputNotes)
        {
            if (!note.SpawnObject(collection)) continue;

            toAction.Add(note);
        }

        TimeLog("Update selection");

        foreach (var note in outputNotes.Where(note => note.selected))
        {
            SelectionController.Select(note.wrapped, true, false, false);
        }

        TimeLog("Create actions");

        foreach (var note in toAction) {
            if (note.original != null)
            {
                beatmapActions.Add(new BeatmapObjectModifiedAction(note.wrapped, note.wrapped, note.original, "Script edited object"));
            }
            else
            {
                beatmapActions.Add(new BeatmapObjectPlacementAction(note.wrapped, Enumerable.Empty<BeatmapObject>(), "Script spawned object"));
            }
        }

        collection.RefreshPool();
        return beatmapActions;
    }
}
