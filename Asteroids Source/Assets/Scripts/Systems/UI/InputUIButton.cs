using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InputUIButton : EventButton
{
    [SerializeField][Tooltip("List of inputs changable by this button. The first input element will set as default.")] 
    private List<InputTypeName> _inputs = new List<InputTypeName>();
    [SerializeField] private TextMeshProUGUI _inputText;
    private int _currentInputIndex = 0;
    private const string _standartText = "Input: ";

    public InputBehaviourBase CurrentInput { get; private set; }
    public event Action<InputBehaviourBase> OnInputChange;

    private void Start()
    {
        InputTypeName inputTypePair = _inputs.First();
        CurrentInput = inputTypePair.InputBehaviour;
        ChangeText(inputTypePair);

        Button.onClick.AddListener(ChangeInput);
    }

    private void ChangeInput()
    {
        if (_currentInputIndex == _inputs.Count - 1) _currentInputIndex = 0;
        else _currentInputIndex++;
        InputTypeName inputTypePair = _inputs[_currentInputIndex];
        CurrentInput = inputTypePair.InputBehaviour;
        ChangeText(inputTypePair);

        OnInputChange?.Invoke(CurrentInput);
    }

    private void ChangeText(InputTypeName inputTypeName)
    {
        _inputText.text = _standartText + inputTypeName.InputName;
    }

    [Serializable]
    private class InputTypeName
    {
        [SerializeField] private InputBehaviourBase _inputBehaviour;
        [SerializeField] private string _inputName;

        public InputBehaviourBase InputBehaviour => _inputBehaviour;
        public string InputName => _inputName;
    }

}
