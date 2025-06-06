using MemoryGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations;

public static class ExtensionMethods
{

    public static T GetRandomElement<T>(this IEnumerable<T> elements)
    {
        return elements.ElementAt(UnityEngine.Random.Range(0, elements.Count()));
    }

    public enum EVectorAxis
    {
        X, Y, Z
    }
    public static Vector3 ChangeAxis(this Vector3 vector, EVectorAxis axis, float value)
    {
        float xValue = axis == EVectorAxis.X ? value : vector.x;
        float yValue = axis == EVectorAxis.Y ? value : vector.y;
        float zValue = axis == EVectorAxis.Z ? value : vector.z;
        return new Vector3(xValue, yValue, zValue);
    }

    public enum EColorAxis
    {
        R, G, B, A
    }
    public static Color ChangeColorAxis(this Color color, EColorAxis colorAxis, float value) 
    {
        float rValue = colorAxis == EColorAxis.R ? value : color.r;
        float gValue = colorAxis == EColorAxis.G ? value : color.g;
        float bValue = colorAxis == EColorAxis .B ? value : color.b;
        float aValue = colorAxis == EColorAxis.A ? value : color.a;

        return new Color(rValue, gValue, bValue, aValue);
    }

    public static bool Has<T>(this Enum type , T value)
    {
        //https://stackoverflow.com/questions/93744/most-common-c-sharp-bitwise-operations-on-enums
        return (((int)(object)type & (int)(object)value) == (int)(object)value);
    }

    public static bool IsValidIndex<T>(this IEnumerable<T> elements, int index)
    {
        return index >= 0 && index < elements.Count();
    }

    public static bool IsNearlyZero(this float value)
    {
        return Mathf.Approximately(value, 0);
    }
}
