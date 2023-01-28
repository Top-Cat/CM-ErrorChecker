using System;
using System.Collections.Generic;
using Beatmap.Base;
using Beatmap.Base.Customs;

class VisionBlocks : Check
{
    public VisionBlocks() : base("Vision Blocks")
    {
        Params.Add(new FloatParam("Min Time", 0.24f));
        Params.Add(new FloatParam("Max Time", 0.75f));
    }

    public override CheckResult PerformCheck(List<BaseNote> notes, List<BaseNote> bombs, List<BaseArc> arcs, List<BaseChain> chains, List<BaseEvent> events, List<BaseObstacle> walls, List<BaseCustomEvent> customEvents, List<BaseBpmChange> bpmChanges, params IParamValue[] vals)
    {
        if (vals.Length > 1)
        {
            return PerformCheck(notes, ((ParamValue<float>) vals[0]).value, ((ParamValue<float>) vals[1]).value);
        }
        throw new ArgumentException("Wrong number of parameters");
    }

    public CheckResult PerformCheck(List<BaseNote> notes, float minTime, float maxTime)
    {
        result.Clear();

        float visionBlockLeft = -1f;
        float visionBlockRight = -1f;

        BaseNote visionBlockLeftNote = null;
        BaseNote visionBlockRightNote = null;
        if (notes.Count > 0)
        {
            visionBlockLeftNote = notes[0];
            visionBlockRightNote = notes[0];
        }

        foreach (var note in notes) {
            if (note.Time - visionBlockLeft <= maxTime)
            {
                if (note.PosX < 2 && note.Time - visionBlockLeft > minTime)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }

                if (note.PosY == 1 && note.PosX == 1)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note.Time - visionBlockRight <= maxTime)
            {
                if (note.PosX > 1 && note.Time - visionBlockRight > minTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }

                if (note.PosY == 1 && note.PosX == 2 && note.Time - visionBlockLeft <= maxTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note.Type != 3 && note.PosY == 1)
            {
                if (note.PosX == 1)
                {
                    visionBlockLeft = note.Time;
                    visionBlockLeftNote = note;
                }
                else if (note.PosX == 2)
                {
                    visionBlockRight = note.Time;
                    visionBlockRightNote = note;
                }
            }
        }
        return result;
    }
}
