using UnityEngine;
 
public static class QuaternionExtensions
{
    public static Quaternion Diff(this Quaternion from, Quaternion to)
    {
        return Quaternion.Inverse(from) * to;
    }
    public static Quaternion Add(this Quaternion from, Quaternion diff)
    {
        return from * diff;
    }
}
 