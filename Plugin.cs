using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ErrorChecker
{
    [Plugin("Error Checker")]
    public class Plugin
    {
        NotesContainer notesContainer;

        [Init]
        private void Init()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            Debug.Log("Plugin has loaded! 2");
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == 3)
            {
                foreach (GameObject rootObj in arg0.GetRootGameObjects())
                {
                    if (rootObj.name.Equals("Editor"))
                    {
                        notesContainer = rootObj.GetComponentInChildren<NotesContainer>();
                    } else if (rootObj.GetComponent<MapEditorUI>() != null)
                    {
                        // Add button to UI
                        UI.AddButton(rootObj, CheckErrors);
                    }
                }
            }
        }

        private void CheckErrors()
        {
            SelectionController.DeselectAll();
            var allNotes = notesContainer.LoadedObjects.Cast<BeatmapNote>().OrderBy(it => it._time).ToList();

            var errors = VisionBlocks.Check(allNotes).Union(
                StackedNotes.Check(allNotes)
            );

            foreach (var block in errors)
            {
                SelectionController.Select(block, true, false, false);
            }
        }

        [Exit]
        private void Exit()
        {
            Debug.Log("Application has closed!");
        }
    }
}
