using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Plugin("CM JS")]
public class CMJS
{
    private NotesContainer notesContainer;
    private ChainsContainer chainsContainer;
    private ArcsContainer arcsContainer;
    private ObstaclesContainer wallsContainer;
    private EventsContainer eventsContainer;
    private CustomEventsContainer customEventsContainer;
    private BPMChangesContainer bpmChangesContainer;
    private List<Check> checks = new List<Check>()
    {
        new VisionBlocks(),
        new StackedNotes()
    };
    private CheckResult errors;
    private UI ui;
    private AudioTimeSyncController atsc;
    private int index = 0;
    private bool movedAfterRun = false;

    [Init]
    private void Init()
    {
        SceneManager.sceneLoaded += SceneLoaded;

        string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        foreach (string file in Directory.GetFiles(assemblyFolder, "*.js"))
        {
            checks.Add(new ExternalJS(Path.GetFileName(file)));
        }

        ui = new UI(this, checks);

        try
        {
            JintPatch.DoPatching();
        }
        catch (HarmonyException e)
        {
            Debug.LogError("Failed to patch Jint during CM-JS init");
            Debug.LogException(e);
            Debug.LogException(e.InnerException);
        }
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 3) // Mapper scene
        {
            notesContainer = UnityEngine.Object.FindObjectOfType<NotesContainer>();
            arcsContainer = UnityEngine.Object.FindObjectOfType<ArcsContainer>();
            chainsContainer = UnityEngine.Object.FindObjectOfType<ChainsContainer>();
            wallsContainer = UnityEngine.Object.FindObjectOfType<ObstaclesContainer>();
            eventsContainer = UnityEngine.Object.FindObjectOfType<EventsContainer>();
            customEventsContainer = UnityEngine.Object.FindObjectOfType<CustomEventsContainer>();
            bpmChangesContainer = UnityEngine.Object.FindObjectOfType<BPMChangesContainer>();
            var mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();

            atsc = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).AudioTimeSyncController;

            // Add button to UI
            ui.AddButton(mapEditorUI);
        }
    }

    public void CheckErrors(Check check)
    {
        bool isV3 = BeatSaberSongContainer.Instance.Map.Version.StartsWith("3");

        if (errors != null)
        {
            // Remove error outline from old errors
            foreach (var block in errors.all)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.OutlineVisible = SelectionController.IsObjectSelected(container.ObjectData);
                    container.SetOutlineColor(SelectionController.SelectedColor, false);
                }
            }
        }

        try
        {
            var vals = ui.paramTexts.Select((it, idx) =>
            {
                switch (it)
                {
                    case UITextInput textInput:
                        return check.Params[idx].Parse(textInput.InputField.text);
                    case UIDropdown dropdown:
                        return check.Params[idx].Parse(dropdown.Dropdown.value.ToString());
                    case Toggle toggle:
                        return check.Params[idx].Parse(toggle.isOn.ToString());
                    default:
                        return new ParamValue<string>(null); // IDK
                }
            }).ToArray();

            if (isV3)
            {
                var allNotes = notesContainer.LoadedObjects.Where(it => it is BeatmapColorNote).Cast<BeatmapColorNote>().OrderBy(it => it.Time).ToList();
                var allBombs = notesContainer.LoadedObjects.Where(it => it is BeatmapBombNote).Cast<BeatmapBombNote>().OrderBy(it => it.Time).ToList();
                var allArcs = arcsContainer.LoadedObjects.Cast<BeatmapArc>().OrderBy(it => it.Time).ToList();
                var allChains = chainsContainer.LoadedObjects.Cast<BeatmapChain>().OrderBy(it => it.Time).ToList();
                var allWalls = wallsContainer.LoadedObjects.Where(it => it is BeatmapObstacleV3).Cast<BeatmapObstacleV3>().OrderBy(it => it.Time).ToList();
                var allEvents = eventsContainer.LoadedObjects.Where(it => it is MapEventV3).Cast<MapEventV3>().OrderBy(it => it.Time).ToList();
                var allCustomEvents = customEventsContainer.LoadedObjects.Cast<BeatmapCustomEvent>().OrderBy(it => it.Time).ToList();
                var allBpmChanges = bpmChangesContainer.LoadedObjects.Cast<BeatmapBPMChange>().OrderBy(it => it.Time).ToList();
                errors = check.PerformCheck(allNotes, allBombs, allArcs, allChains, allEvents, allWalls, allCustomEvents, allBpmChanges, vals).Commit();
            } else
            {
                var allNotes = notesContainer.LoadedObjects.Cast<BeatmapNote>().OrderBy(it => it.Time).ToList();
                var allWalls = wallsContainer.LoadedObjects.Cast<BeatmapObstacle>().OrderBy(it => it.Time).ToList();
                var allEvents = eventsContainer.LoadedObjects.Cast<MapEvent>().OrderBy(it => it.Time).ToList();
                var allCustomEvents = customEventsContainer.LoadedObjects.Cast<BeatmapCustomEvent>().OrderBy(it => it.Time).ToList();
                var allBpmChanges = bpmChangesContainer.LoadedObjects.Cast<BeatmapBPMChange>().OrderBy(it => it.Time).ToList();
                errors = check.PerformCheck(allNotes, allEvents, allWalls, allCustomEvents, allBpmChanges, vals).Commit();
            }

            // Highlight blocks in loaded containers in case we don't scrub far enough with MoveToTimeInBeats to load them
            foreach (var block in errors.errors)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.SetOutlineColor(Color.red);
                }
            }

            foreach (var block in errors.warnings)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.ObjectType.Note).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.SetOutlineColor(Color.yellow);
                }
            }

            index = 0;
            movedAfterRun = false;
            
            if (errors == null || errors.all.Count < 1)
            {
                ui.problemInfoText.text = "No problems found";
            }
            else
            {
                ui.problemInfoText.text = $"{errors.all.Count} problems found";
            }
            ui.problemInfoText.fontSize = 12;
            ui.problemInfoText.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 50);
            //NextBlock(0);
        }
        catch (Exception e) { Debug.LogError(e.Message + e.StackTrace); }
    }

    public void NextBlock(int offset = 1)
    {
        if (!movedAfterRun)
        {
            movedAfterRun = true;
            if (offset > 0) offset = 0;
        }
        
        if (errors == null || errors.all.Count < 1)
        {
            return;
        }

        index = (index + offset) % errors.all.Count;

        if (index < 0)
        {
            index += errors.all.Count;
        }

        float? time = errors.all[index]?.note.Time;
        if (time != null)
        {
            atsc.MoveToTimeInBeats(time ?? 0);
        }

        if (ui.problemInfoText != null)
        {
            ui.problemInfoText.text = errors.all[index]?.reason ?? "...";
            ui.problemInfoText.fontSize = 12;
            ui.problemInfoText.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 50);
        }
    }

    [ObjectLoaded]
    private void ObjectLoaded(BeatmapObjectContainer container)
    {
        if (container.ObjectData == null || errors == null) return;

        if (errors.errors.Any(it => it.note.Equals(container.ObjectData)))
        {
            container.SetOutlineColor(Color.red);
        }
        else if (errors.warnings.Any(it => it.note.Equals(container.ObjectData)))
        {
            container.SetOutlineColor(Color.yellow);
        }
    }

    [Exit]
    private void Exit()
    {
        
    }
}
