using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButton : EventButton
{
    public void SetInteractable(bool status)
    {
        Button.interactable = status;
    }
}
