using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowController : MonoBehaviour, IVRAction
{
    public bool left;
    fileReaderController frc;
    // Start is called before the first frame update
    void Start()
    {
        frc = GetComponentInParent<fileReaderController>();
    }

    // Update is called once per frame
    public void triggerPull() { 
        frc.move(left);
    }
    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }

}
