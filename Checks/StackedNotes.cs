using System;
using System.Collections.Generic;

class StackedNotes : Check
{
    public StackedNotes() : base("Stacked Notes")
    {
    }

    protected override CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, List<BeatmapCustomEvent> customEvents, List<BeatmapBPMChange> bpmChanges)
    {
        result.Clear();

        for (int i = 0; i < notes.Count - 1; i++)
        {
            for (int j = i + 1; j < notes.Count; j++)
            {
                var noteA = notes[i];
                var noteB = notes[j];

                if (noteB.Time - noteA.Time > 0.1)
                {
                    break;
                }

                if (noteA.LineIndex == noteB.LineIndex && noteA.LineLayer == noteB.LineLayer)
                {
                    result.Add(noteA);
                    result.Add(noteB);
                }
            }
        }
        return result;
    }
}
