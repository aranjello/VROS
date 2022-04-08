using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerTest : MonoBehaviour, IVRAction
{
    public void triggerSqueeze(float triggerVal){
        print("my name is: " + this.name + " and the val is: " + triggerVal);
    }

    public void triggerPull()
    {
        print("trigger pulled");
    }

    public void gripGrab()
    {
        print("grip grabbed");
    }

    public void menuPress()
    {
        print("menu pressed");
    }

    public void touchPadClick()
    {
        print("touch pad clicked");
    }

    public void touchPadPosition(Vector2 touchPosition)
    {
        print("touchpad at: " + touchPosition);
    }
}
