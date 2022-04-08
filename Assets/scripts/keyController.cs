using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class keyController : MonoBehaviour, IVRAction
{
    keyboardController controller;
    void Start()
    {
        controller = GetComponentInParent<keyboardController>();
    }

    public void triggerPull() { 
        controller.updateText(this.name);
    }
    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }

}