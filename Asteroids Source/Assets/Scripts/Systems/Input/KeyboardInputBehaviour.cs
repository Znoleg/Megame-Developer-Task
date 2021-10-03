using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Keyboard Input", menuName = "ScriptableObjects/Inputs/Keyboard")]
public sealed class KeyboardInputBehaviour : InputBehaviourBase
{
    [SerializeField] private KeyCode[] _rotateLeftKeys = new KeyCode[] { KeyCode.A, KeyCode.LeftArrow };
    [SerializeField] private KeyCode[] _rotateRightKeys = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };
    [SerializeField] private KeyCode[] _moveKeys = new KeyCode[] { KeyCode.W, KeyCode.UpArrow };
    [SerializeField] private KeyCode[] _shootKeys = new KeyCode[] { KeyCode.Space };
    [SerializeField] private KeyCode[] _pauseKeys = new KeyCode[] { KeyCode.Escape };
    private readonly Dictionary<Direction, bool> _rotationDictionary = new Dictionary<Direction, bool>();

    public override bool GetFaceDirection(out Vector2 direction)
    {
        direction = Vector2.up;
        if (_rotationDictionary.Values.All(val => val == false) 
            || (_rotationDictionary[Direction.Left] && _rotationDictionary[Direction.Right]))
        {
            return false;
        }
        if (_rotationDictionary[Direction.Left])
        {
            direction = Utils.Rotate(PlayerTransform.up, -30f);
            return true;
        }
        if (_rotationDictionary[Direction.Right])
        {
            direction = Utils.Rotate(PlayerTransform.up, 30f);
            return true;
        }
        return false;
    }

    public override IReadOnlyList<InputActions> GetInputs()
    {
        _inputs.Clear();
        foreach (KeyData keyData in _keyDatas)
        {
            foreach (KeyCode key in keyData.KeyCodes)
            {
                if (Input.GetKey(key))
                {
                    _inputs.Add(keyData.InputAction);
                }
                if (Input.GetKeyDown(key))
                {
                    if (keyData.InputAction == InputActions.Shoot)
                        InvokeShootKeyDown();
                    else if (keyData.InputAction == InputActions.Move)
                        InvokeMoveKeyDown();
                    else if (keyData.InputAction == InputActions.RotateLeft)
                        SetRotationStatus(Direction.Left, true);
                    else if (keyData.InputAction == InputActions.RotateRight)
                        SetRotationStatus(Direction.Right, true);
                    else if (keyData.InputAction == InputActions.Pause)
                        InvokePauseKeyDown();
                }
                else if (Input.GetKeyUp(key))
                {
                    if (keyData.InputAction == InputActions.Move)
                        InvokeMoveKeyUp();
                    else if (keyData.InputAction == InputActions.RotateLeft)
                        SetRotationStatus(Direction.Left, false);
                    else if (keyData.InputAction == InputActions.RotateRight)
                        SetRotationStatus(Direction.Right, false);
                }
            }
        }

        return _inputs;
    }

    private void SetRotationStatus(Direction rotation, bool status) => _rotationDictionary[rotation] = status;

    private void OnEnable()
    {
        _keyDatas.Add(new KeyData(ref _moveKeys, InputActions.Move));
        _keyDatas.Add(new KeyData(ref _shootKeys, InputActions.Shoot));
        _keyDatas.Add(new KeyData(ref _rotateLeftKeys, InputActions.RotateLeft));
        _keyDatas.Add(new KeyData(ref _rotateRightKeys, InputActions.RotateRight));
        _keyDatas.Add(new KeyData(ref _pauseKeys, InputActions.Pause));

        _rotationDictionary.Add(Direction.Left, false);
        _rotationDictionary.Add(Direction.Right, false);
    }
}

