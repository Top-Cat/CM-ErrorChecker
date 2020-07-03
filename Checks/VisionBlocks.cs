using System.Collections.Generic;

class VisionBlocks : Check
{
    public VisionBlocks() : base("Vision Blocks")
    {
    }

    public override CheckResult PerformCheck(List<BeatmapNote> notes)
    {
        result.Clear();

        float visionBlockLeft = -1f;
        float visionBlockRight = -1f;

        float minTime = 0.24f;
        float maxTime = 0.75f;

        foreach(var note in notes) {
            if (note._time - visionBlockLeft <= maxTime)
            {
                if (note._lineIndex < 2 && note._time - visionBlockLeft > minTime)
                {
                    result.Add(note);
                }

                if (note._lineLayer == 1 && note._lineIndex == 1)
                {
                    result.Add(note);
                }
            }

            if (note._time - visionBlockRight <= maxTime)
            {
                if (note._lineIndex > 1 && note._time - visionBlockRight > minTime)
                {
                    result.Add(note);
                }

                if (note._lineLayer == 1 && note._lineIndex == 2 && note._time - visionBlockLeft <= maxTime)
                {
                    result.Add(note);
                }
            }

            if (note._type != 3 && note._lineLayer == 1)
            {
                if (note._lineIndex == 1)
                {
                    visionBlockLeft = note._time;
                }
                else if (note._lineIndex == 2)
                {
                    visionBlockRight = note._time;
                }
            }
        }
        return result;
    }
}
