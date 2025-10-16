// CatmullRom.cs
using UnityEngine;

public static class CatmullRom
{
    // p1~p2 구간에서 t(0~1) 샘플. p0,p3는 양 끝 보조점.
    public static Vector3 Sample(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}
