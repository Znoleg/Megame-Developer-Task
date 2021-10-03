using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public static float GetVectorAngle(Vector3 vector1, Vector3 vector2, bool ignoreY)
    {
        if (ignoreY) vector1.y = vector2.y;
        float angle = Vector3.Angle(vector1, vector2);
        Vector3 cross = Vector3.Cross(vector1, vector2);
        if (cross.y < 0) angle = -angle;
        return angle;
    }

    public static Vector3 GetLocalEulerRotation(Vector3 direction, Vector3 forward, bool ignoreY)
    {
        float angle = GetVectorAngle(direction, forward, ignoreY);
        return new Vector3(angle, 0f, 0f);
    }

    public static Vector3 GetGlobalEulerRotation(Vector3 direction, Vector3 forward, bool ignoreY)
    {
        float angle = GetVectorAngle(direction, forward, ignoreY);
        return new Vector3(0f, angle, 0f);
    }

    public static float MapValue(float x, float xLeft, float xRight, float resLeft, float resRight)
    {
        if (xRight == xLeft) return xLeft;
        return (x - xLeft) / (xRight - xLeft) * (resRight - resLeft) + resLeft;
    }

    public static CoroutineObject CreateWaitCoroutine(MonoBehaviour owner, float waitTime)
    {
        var routine = new CoroutineObject(owner, () => WaitCoroutine(waitTime));
        return routine;
    }

    private static IEnumerator WaitCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        yield break;
    }
}
