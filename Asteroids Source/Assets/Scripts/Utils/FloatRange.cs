using System;

[Serializable]
public class FloatRange : Range<float>
{
    public float RandomValue => UnityEngine.Random.Range(Min, Max);

    public FloatRange(float min, float max) : base(min, max)
    {

    }
}

