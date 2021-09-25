using System;
using System.Collections.Generic;

class VisionBlocks : Check
{
    public VisionBlocks() : base("Vision Blocks")
    {
        Params.Add(new FloatParam("Min Time", 0.24f));
        Params.Add(new FloatParam("Max Time", 0.75f));
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, List<MapEvent> events, List<BeatmapObstacle> walls, List<BeatmapCustomEvent> customEvents, List<BeatmapBPMChange> bpmChanges, params IParamValue[] vals)
    {
        if (vals.Length > 1)
        {
            return PerformCheck(notes, ((ParamValue<float>) vals[0]).value, ((ParamValue<float>) vals[1]).value);
        }
        throw new ArgumentException("Wrong number of parameters");
    }

    public CheckResult PerformCheck(List<BeatmapNote> notes, float minTime, float maxTime)
    {
        result.Clear();

        float visionBlockLeft = -1f;
        float visionBlockRight = -1f;

        BeatmapNote visionBlockLeftNote = null;
        BeatmapNote visionBlockRightNote = null;
        if (notes.Count > 0)
        {
            visionBlockLeftNote = notes[0];
            visionBlockRightNote = notes[0];
        }

        foreach (var note in notes) {
            if (note.Time - visionBlockLeft <= maxTime)
            {
                if (note.LineIndex < 2 && note.Time - visionBlockLeft > minTime)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }

                if (note.LineLayer == 1 && note.LineIndex == 1)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note.Time - visionBlockRight <= maxTime)
            {
                if (note.LineIndex > 1 && note.Time - visionBlockRight > minTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }

                if (note.LineLayer == 1 && note.LineIndex == 2 && note.Time - visionBlockLeft <= maxTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note.Type != 3 && note.LineLayer == 1)
            {
                if (note.LineIndex == 1)
                {
                    visionBlockLeft = note.Time;
                    visionBlockLeftNote = note;
                }
                else if (note.LineIndex == 2)
                {
                    visionBlockRight = note.Time;
                    visionBlockRightNote = note;
                }
            }
        }
        return result;
    }
}
