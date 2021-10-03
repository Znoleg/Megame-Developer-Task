using System.Collections.Generic;
using UnityEngine;

public class InputReciever : Singleton<InputReciever>
{
    [SerializeField] private InputBehaviourBase _inputBehaviour;

    public IReadOnlyList<InputActions> CurrentInputs { get; private set; } = new List<InputActions>();
    public InputEvents InputEvents => _inputBehaviour; // KEYBOARD INPUT keyboard+mouse

    public void ChangeInput(InputBehaviourBase inputSystem)
    {
        inputSystem.Initialize(PlayerMovement.Instance.transform);
        _inputBehaviour = inputSystem;
    }

    private void Start()
    {
        _inputBehaviour.Initialize(PlayerMovement.Instance.transform);
    }

    private void Update()
    {
        CurrentInputs = _inputBehaviour.GetInputs();
    }
}

