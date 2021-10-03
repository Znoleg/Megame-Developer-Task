using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    [SerializeField] private AnimationClip _finishingAniamtion;
    private AnimationClip _currentAnimation;
    private Animator _animator;
    private float _finishingLength;

    public void SetSpeed(float speed)
    {
        _animator.SetFloat("speed", speed);
    }

    public CoroutineObjectBase PlayHitAnimation(out float animationLength)
    {
        animationLength = _finishingLength;
        _animator.SetTrigger("finishing");
        var routine = Utils.CreateWaitCoroutine(this, _finishingLength);
        routine.Start();
        return routine;
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _finishingLength = _finishingAniamtion.length;

    }
}
