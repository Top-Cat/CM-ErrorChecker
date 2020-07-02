using System.Collections.Generic;
using UnityEngine;

namespace ErrorChecker
{
    class VisionBlocks
    {
        public static List<BeatmapNote> Check(List<BeatmapNote> notes)
        {
            float visionBlockLeft = -1f;
            float visionBlockRight = -1f;

            float minTime = 0;
            float maxTime = 1;

            List<BeatmapNote> result = new List<BeatmapNote>();

            foreach(var note in notes) {
                if (note._lineIndex < 2 && note._time - visionBlockLeft > minTime && note._time - visionBlockLeft <= maxTime)
                {
                    result.Add(note);
                    Debug.Log("Left error " + note);
                }

                if (note._lineLayer == 1 && note._lineIndex == 1 && note._time - visionBlockLeft <= maxTime)
                {
                    result.Add(note);
                    Debug.Log("Left error 2 " + note);
                }

                if (note._lineIndex > 1 && note._time - visionBlockRight > minTime && note._time - visionBlockRight <= maxTime)
                {
                    result.Add(note);
                    Debug.Log("Right error " + note);
                }

                if (note._lineLayer == 1 && note._lineIndex == 2 && note._time - visionBlockLeft <= maxTime)
                {
                    result.Add(note);
                    Debug.Log("Right error 2 " + note);
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
}
