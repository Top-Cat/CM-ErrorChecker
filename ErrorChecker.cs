using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

[Plugin("Error Checker")]
public class ErrorChecker
{
    private NotesContainer notesContainer;
    private ObstaclesContainer wallsContainer;
    private EventsContainer eventsContainer;
    private List<Check> checks = new List<Check>()
    {
        new VisionBlocks(),
        new StackedNotes()
    };
    private CheckResult errors;
    private UI ui;
    private AudioTimeSyncController atsc;
    private int index = 0;

    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

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
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 3) // Mapper scene
        {
            notesContainer = UnityEngine.Object.FindObjectOfType<NotesContainer>();
            wallsContainer = UnityEngine.Object.FindObjectOfType<ObstaclesContainer>();
            eventsContainer = UnityEngine.Object.FindObjectOfType<EventsContainer>();
            var mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();

            atsc = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).AudioTimeSyncController;

            // Add button to UI
            ui.AddButton(mapEditorUI);
        }
        else if (arg0.buildIndex == 2) // Song Info scene
        {
            var images = UnityEngine.Object.FindObjectOfType<SongInfoEditUI>().GetComponentsInChildren<Image>();
            foreach (var i in images)
            {
                if (i.transform.parent.name != "Revert Button") continue;

                ui.ReloadSprite = i.sprite;
                break;
            }
        }
    }

    public void CheckErrors(Check check)
    {
        var allNotes = notesContainer.LoadedObjects.Cast<BeatmapNote>().OrderBy(it => it._time).ToList();
        var allWalls = wallsContainer.LoadedObjects.Cast<BeatmapObstacle>().OrderBy(it => it._time).ToList();
        var allEvents = eventsContainer.LoadedObjects.Cast<MapEvent>().OrderBy(it => it._time).ToList();

        if (errors != null)
        {
            // Remove error outline from old errors
            foreach (var block in errors.all)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.OutlineVisible = SelectionController.IsObjectSelected(container.objectData);
                    container.SetOutlineColor(SelectionController.SelectedColor, false);
                }
            }
        }

        try
        {
            float[] vals = ui.paramTexts.Select(it => {
                float.TryParse(it.text, out float val);
                return val;
            }).ToArray();
            errors = check.PerformCheck(allNotes, allEvents, allWalls, vals).Commit();

            // Highlight blocks in loaded containers in case we don't scrub far enough with MoveToTimeInBeats to load them
            foreach (var block in errors.errors)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.SetOutlineColor(Color.red);
                }
            }

            foreach (var block in errors.warnings)
            {
                if (BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).LoadedContainers.TryGetValue(block.note, out BeatmapObjectContainer container))
                {
                    container.SetOutlineColor(Color.yellow);
                }
            }

            index = 0;
            NextBlock(0);
        }
        catch (Exception e) { Debug.LogError(e.Message + e.StackTrace); }
    }

    public void NextBlock(int offset = 1)
    {
        if (errors == null || errors.all.Count < 1)
        {
            ui.problemInfoText.text = "No problems found";
            ui.problemInfoText.fontSize = 12;
            ui.problemInfoText.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 50);

            return;
        }

        index = (index + offset) % errors.all.Count;

        if (index < 0)
        {
            index += errors.all.Count;
        }

        float? time = errors.all[index]?.note._time;
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
        if (container.objectData == null || errors == null) return;

        if (errors.errors.Any(it => it.note.Equals(container.objectData)))
        {
            container.SetOutlineColor(Color.red);
        }
        else if (errors.warnings.Any(it => it.note.Equals(container.objectData)))
        {
            container.SetOutlineColor(Color.yellow);
        }
    }

    [Exit]
    private void Exit()
    {
        
    }
}
