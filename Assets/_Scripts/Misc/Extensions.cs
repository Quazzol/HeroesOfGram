using UnityEngine;

public static class Extensions
{
    public static bool AreEqual(this float value, float compare)
    {
        return UnityEngine.Mathf.Abs(value - compare) < float.Epsilon;
    }
    public static float ZeroIfNegative(this float value)
    {
        return value < float.Epsilon ? 0 : value;
    }

    public static bool AreEqual(this Vector3 first, Vector3 second)
    {
        return Vector3.Distance(first, second) < float.Epsilon;
    }
}
