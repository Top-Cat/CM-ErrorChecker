using System;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Base.Customs;

class StackedNotes : Check
{
    public StackedNotes() : base("Stacked Notes")
    {
    }

    protected override CheckResult PerformCheck(List<BaseNote> notes, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges)
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

                if (noteA.PosX == noteB.PosX && noteA.PosY == noteB.PosY)
                {
                    result.Add(noteA);
                    result.Add(noteB);
                }
            }
        }
        return result;
    }
}
