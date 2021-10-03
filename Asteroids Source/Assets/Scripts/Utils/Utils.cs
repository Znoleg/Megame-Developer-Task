using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Utils
{
    public static T GetRandom<T>() where T : Enum
    {
        var enumArray = Enum.GetValues(typeof(T)).Cast<T>().ToList();
        int count = enumArray.Count();
        return enumArray[Random.Range(0, count)];
    }

    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    public static Vector2 Rotate(this Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        ) * Mathf.Rad2Deg;
    }

    public static float GetZRotation(Vector2 direction)
    {
        Vector2 normalized = direction.normalized;
        return -(Mathf.Atan2(normalized.x, normalized.y) * Mathf.Rad2Deg);
    }

    public static float GetRandomAngle() => Random.Range(0f, 360f);
}

