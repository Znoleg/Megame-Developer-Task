using System;
using System.Collections.Generic;
using UnityEngine;

public enum InputActions
{
    Move, RotateLeft, RotateRight, Shoot, Pause
}

public enum Direction
{
    Left = -1, None = 0, Right = 1, 
}

public class KeyData
{
    private readonly KeyCode[] _keyCodes;

    public IReadOnlyList<KeyCode> KeyCodes => _keyCodes;
    public InputActions InputAction { get; private set; }

    public KeyData(ref KeyCode[] keyCodes, InputActions inputAction)
    {
        _keyCodes = keyCodes;
        InputAction = inputAction;
    }
}

public abstract class InputBehaviourBase : InputEvents
{
    protected readonly List<KeyData> _keyDatas = new List<KeyData>();
    protected readonly List<InputActions> _inputs = new List<InputActions>();

    protected Transform PlayerTransform { get; private set; }
    
    public virtual void Initialize(Transform player) => PlayerTransform = player;
    public abstract IReadOnlyList<InputActions> GetInputs();
}
