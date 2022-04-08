using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVRAction
{
    void triggerPull();
    void triggerSqueeze(float triggerValue);
    void gripGrab();
    void touchPadClick();
    void touchPadPosition(Vector2 touchPosition);
    void menuPress();

}
