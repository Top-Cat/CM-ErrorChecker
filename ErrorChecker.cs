﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

[Plugin("Error Checker")]
public class ErrorChecker
{
    private NotesContainer notesContainer;
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
        Debug.Log("Plugin has loaded! 4");

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
            var mapEditorUI = UnityEngine.Object.FindObjectOfType<MapEditorUI>();

            atsc = BeatmapObjectContainerCollection.GetCollectionForType(BeatmapObject.Type.NOTE).AudioTimeSyncController;

            // Add button to UI
            ui.AddButton(mapEditorUI);
        }
    }

    public void CheckErrors(Check check)
    {
        var allNotes = notesContainer.LoadedObjects.Cast<BeatmapNote>().OrderBy(it => it._time).ToList();

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
            errors = check.PerformCheck(allNotes).Commit();

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

            NextBlock(0);
        }
        catch (Exception) { }
    }

    public void NextBlock(int offset = 1)
    {
        if (errors.all.Count < 1) return;

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
        Debug.Log("Application has closed!");
    }
}