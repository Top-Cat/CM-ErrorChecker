using System.Collections;
using System.Collections.Generic;

class BeatmapObjectArray<T, U> : IEnumerable<T> where T : Wrapper<U> where U : BeatmapObject
{
    private T[] objects;
    
    public BeatmapObjectArray(T[] objects)
    {
        this.objects = objects;
    }

    public override string ToString() => string.Join<T>(", ", objects);

    public IEnumerator<T> GetEnumerator() => objects.GetEnumerator() as IEnumerator<T>;

    IEnumerator IEnumerable.GetEnumerator() => objects.GetEnumerator();
}
