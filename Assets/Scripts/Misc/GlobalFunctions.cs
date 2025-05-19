using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GlobalFunctions
{
    public static T RandomEnumValue<T>(params T[] elementsToAvoid)
    {
        // https://discussions.unity.com/t/using-random-range-to-pick-a-random-value-out-of-an-enum/119639/3
        Array values = Enum.GetValues(typeof(T));
        T randomElement;
        int iterations = 0;
        do
        { 
            randomElement = (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));

            if (++iterations >= 100)
            {
                Debug.LogError("Stuck in RandomEnumValue");
                break;
            }

        } while (elementsToAvoid.Contains(randomElement));
        return randomElement;
    }

    public static void LerpObjectToLocation(MonoBehaviour coroutineParent, GameObject objectToMove, Vector3 finalPosition,  float duration)
    {
        if (coroutineParent)
        {
            coroutineParent.StartCoroutine(RunLerpObjectToLocation(objectToMove, finalPosition, duration));
        }
        else
        {
            Debug.LogError("Null object provided");
        }
    }

    private static IEnumerator RunLerpObjectToLocation(GameObject objectToMove, Vector3 finalPosition, float duration)
    {
        Vector3 objectStartPos = objectToMove.transform.position;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            objectToMove.transform.position = Vector3.Lerp(objectStartPos, finalPosition, time / duration);
            yield return null;
        }

        objectToMove.transform.position = finalPosition;
    }

    public static void LerpObjectScale(MonoBehaviour coroutineParent, GameObject objectToScale, Vector3 finalScale, float duration)
    {
        if (coroutineParent)
        {
            coroutineParent.StartCoroutine(RunLerpObjectScale(objectToScale, finalScale, duration));
        }
        else
        {
            Debug.LogError("Null object provided");
        }
    }

    private static IEnumerator RunLerpObjectScale(GameObject objectToScale, Vector3 finalScale, float duration)
    {
        Vector3 startScale = objectToScale.transform.localScale;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            objectToScale.transform.localScale = Vector3.Lerp(startScale, finalScale, time / duration);
            yield return null;
        }

        objectToScale.transform.localScale = finalScale;
    }

    public static void LerpRectTransform(MonoBehaviour coroutineParent, RectTransform rectToTransform, RectTransform finalRectValues, float duration)
    {
        if (coroutineParent)
        {
            coroutineParent.StartCoroutine(RunLerpORectTransform(rectToTransform, finalRectValues, duration));
        }
        else
        {
            Debug.LogError("Null object provided");
        }
    }

    private static IEnumerator RunLerpORectTransform(RectTransform rectToTransform, RectTransform finalRectValues, float duration)
    {
        //rectToTransform.SetParent(finalRectValues.parent);
        Vector2 startMin = rectToTransform.offsetMin;
        Vector2 startMax = rectToTransform.offsetMax;
        Vector3 startPos = rectToTransform.position;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            rectToTransform.offsetMin = Vector2.Lerp(startMin, finalRectValues.offsetMin, time / duration);
            rectToTransform.offsetMax = Vector2.Lerp(startMax, finalRectValues.offsetMax, time / duration);
            rectToTransform.position = Vector3.Lerp(startPos, finalRectValues.position, time / duration);
            yield return null;
        }

        rectToTransform.offsetMin = finalRectValues.offsetMin;
        rectToTransform.offsetMax = finalRectValues.offsetMax;
    }

    public static List<T> EnumToList<T>(params T[] restrictedTypes) where T : Enum
    {
        List<T> enumList = new List<T>();
        foreach (T mem in Enum.GetValues(typeof(T)))
        {
            if (!restrictedTypes.Contains(mem))
            {
                enumList.Add(mem);
            }
        }

        return enumList;
    }

    public static Vector3 GetRandomWorldPosOnScreen(float minX, float maxX, float minY, float maxY)
    {
        float x = UnityEngine.Random.Range(minX, maxX);
        float y = UnityEngine.Random.Range(minY, maxY);

        return Camera.main.ViewportToWorldPoint(new Vector3(x, y, 0));
    }

    public static bool ensure(bool condition, string message = "")
    {
        if (!condition)
        {
            Debug.LogError(message != string.Empty ? message : "Ensure is fired");
        }

        return condition;
    }
}
