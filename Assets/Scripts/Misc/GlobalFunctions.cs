using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    private static GlobalFunctions instance;

    private void Awake()
    {
        instance = this;
    }

    public static T RandomEnumValue<T>()
    {
        // https://discussions.unity.com/t/using-random-range-to-pick-a-random-value-out-of-an-enum/119639/3
        var values = Enum.GetValues(typeof(T));
        int random = UnityEngine.Random.Range(0, values.Length);
        return (T)values.GetValue(random);
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
}
