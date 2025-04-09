using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    private static GlobalFunctions instance;

    private void Awake()
    {
        instance = this;
    }

    public static T RandomEnumValue<T>(params T[] elementsToAvoid)
    {
        // https://discussions.unity.com/t/using-random-range-to-pick-a-random-value-out-of-an-enum/119639/3
        var values = Enum.GetValues(typeof(T));
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

    public static void LerpObjectToLocation(GameObject objectToMove, Vector3 finalPosition,  float duration)
    {
        instance.StartCoroutine(instance.RunLerpObjectToLocation(objectToMove, finalPosition, duration));
    }

    private IEnumerator RunLerpObjectToLocation(GameObject objectToMove, Vector3 finalPosition, float duration)
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
}
