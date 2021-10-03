using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class EventButton : MonoBehaviour
{
    public event Action OnClick;

    protected Button Button { get; private set; }

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(() => OnClick?.Invoke());
    }
}
