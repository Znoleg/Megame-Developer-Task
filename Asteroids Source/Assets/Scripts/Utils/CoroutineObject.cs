﻿using System;
using System.Collections;
using UnityEngine;

public sealed class CoroutineObject : CoroutineObjectBase
{
    public Func<IEnumerator> Routine { get; private set; }

    public override event Action Finished;

    public CoroutineObject(MonoBehaviour owner, Func<IEnumerator> routine)
    {
        Owner = owner;
        Routine = routine;
    }

    private IEnumerator Process()
    {
        yield return Routine.Invoke();

        Coroutine = null;

        Finished?.Invoke();
    }

    public void Start()
    {
        Stop();

        Coroutine = Owner.StartCoroutine(Process());
    }

    public void Stop()
    {
        if (IsProcessing)
        {
            Owner.StopCoroutine(Coroutine);
            
            Coroutine = null;
        }
    }

    public void Stop(IEnumerator coroutine)
    {
        Stop();

        coroutine = null;
    }
}

public abstract class CoroutineObjectBase
{
    public MonoBehaviour Owner { get; protected set; }
    public Coroutine Coroutine { get; protected set; }

    public bool IsProcessing => Coroutine != null;

    public abstract event Action Finished;
}
