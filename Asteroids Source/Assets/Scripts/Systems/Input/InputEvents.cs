using System;
using UnityEngine;

public abstract class InputEvents : ScriptableObject
{
    public static event Action OnShootKeyDown;

    public static event Action OnMoveKeyUp;
    public static event Action OnMoveKeyDown;

    public static event Action OnPauseKeyDown;

    public abstract bool GetFaceDirection(out Vector2 direction);
    protected static void InvokeShootKeyDown() => OnShootKeyDown?.Invoke();
    protected static void InvokeMoveKeyUp() => OnMoveKeyUp?.Invoke();
    protected static void InvokeMoveKeyDown() => OnMoveKeyDown?.Invoke();
    protected static void InvokePauseKeyDown() => OnPauseKeyDown?.Invoke();
}

