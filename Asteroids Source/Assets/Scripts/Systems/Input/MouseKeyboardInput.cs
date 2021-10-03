using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Mouse+Keyboard Input", menuName = "ScriptableObjects/Inputs/Mouse + Keyboard")]
public class MouseKeyboardInput : InputBehaviourBase
{
    [SerializeField] private KeyCode[] _moveKeys = new KeyCode[] { KeyCode.W, KeyCode.Mouse1 };
    [SerializeField] private KeyCode[] _shootKeys = new KeyCode[] { KeyCode.Space, KeyCode.Mouse0 };
    [SerializeField] private KeyCode[] _pauseKeys = new KeyCode[] { KeyCode.Escape };

    public override bool GetFaceDirection(out Vector2 direction)
    {
        direction = Vector2.up;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        Vector2 toMouseDirection = ((Vector2)mouseWorldPosition - (Vector2)PlayerTransform.position).normalized;
        
        if ((Vector2)PlayerTransform.up == toMouseDirection) return false;
        direction = toMouseDirection;
        return true;
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
                    else if (keyData.InputAction == InputActions.Pause)
                        InvokePauseKeyDown();
                }
                else if (Input.GetKeyUp(key))
                {
                    if (keyData.InputAction == InputActions.Move)
                        InvokeMoveKeyUp();
                }
            }
        }

        return _inputs;
    }

    private void OnEnable()
    {
        _keyDatas.Add(new KeyData(ref _moveKeys, InputActions.Move));
        _keyDatas.Add(new KeyData(ref _shootKeys, InputActions.Shoot));
        _keyDatas.Add(new KeyData(ref _pauseKeys, InputActions.Pause));
    }
}

