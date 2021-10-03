using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Animations
{
    public static CoroutineObjectBase PlayFadeAnimation(MonoBehaviour owner, SpriteRenderer rendererToAnimate, float alpha, float time)
    {
        var routine = new CoroutineObject(owner, () => PlayFadeAnimationInternal(rendererToAnimate, alpha, time));
        routine.Start();
        return routine;
    }

    private static IEnumerator PlayFadeAnimationInternal(SpriteRenderer rendererToAnimate, float alpha, float time)
    {
        float startAlpha = rendererToAnimate.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(startAlpha, alpha, t));
            rendererToAnimate.material.color = newColor;
            yield return null;
        }
    }
}

