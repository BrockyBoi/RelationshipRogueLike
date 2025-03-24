using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods 
{
    public enum VectorAxis
    {
        X, Y, Z
    }
    public static T GetRandomElement<T>(this IEnumerable<T> elements)
    {
        return elements.ElementAt(Random.Range(0, elements.Count()));
    }

    public static Vector3 ChangeAxis(this Vector3 vector, VectorAxis axis, float value)
    {
        float xValue = axis == VectorAxis.X ? value : vector.x;
        float yValue = axis == VectorAxis.Y ? value : vector.y;
        float zValue = axis == VectorAxis.Z ? value : vector.z;
        return new Vector3(xValue, yValue, zValue);
    }

    public static bool IsValidIndex<T>(this IEnumerable<T> elements, int index)
    {
        return index >= 0 && index < elements.Count();
    }
}
