using UnityEngine;
using UnityEditor;

public static class Extensions
{
   public static float X(this Transform t)
    {
        return t.position.x;
    }
    public static float Y(this Transform t)
    {
        return t.position.y;
    }
}