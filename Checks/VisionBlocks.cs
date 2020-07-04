using System.Collections.Generic;

class VisionBlocks : Check
{
    public VisionBlocks() : base("Vision Blocks")
    {
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes, float minTime, float maxTime)
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
            if (note._time - visionBlockLeft <= maxTime)
            {
                if (note._lineIndex < 2 && note._time - visionBlockLeft > minTime)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }

                if (note._lineLayer == 1 && note._lineIndex == 1)
                {
                    result.Add(visionBlockLeftNote, "Blocks vision of upcoming note on the left");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note._time - visionBlockRight <= maxTime)
            {
                if (note._lineIndex > 1 && note._time - visionBlockRight > minTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }

                if (note._lineLayer == 1 && note._lineIndex == 2 && note._time - visionBlockLeft <= maxTime)
                {
                    result.Add(visionBlockRightNote, "Blocks vision of upcoming note on the right");
                    result.AddWarning(note, "Is blocked");
                }
            }

            if (note._type != 3 && note._lineLayer == 1)
            {
                if (note._lineIndex == 1)
                {
                    visionBlockLeft = note._time;
                    visionBlockLeftNote = note;
                }
                else if (note._lineIndex == 2)
                {
                    visionBlockRight = note._time;
                    visionBlockRightNote = note;
                }
            }
        }
        return result;
    }
}
