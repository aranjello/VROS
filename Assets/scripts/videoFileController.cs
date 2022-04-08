using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class videoFileController : genericFilObj, IVRAction
{
    public override void setup(string filePath){
        objData.path = filePath;
        objData.oType = vrObjData.objType.video;
        this.GetComponent<UnityEngine.Video.VideoPlayer>().url = filePath;
        this.GetComponent<UnityEngine.Video.VideoPlayer>().Prepare();
        //this.GetComponent<UnityEngine.Video.VideoPlayer>().Play();
    }
    public void triggerPull()
    {
        UnityEngine.Video.VideoPlayer v = this.GetComponent<UnityEngine.Video.VideoPlayer>();
        if(v.isPlaying)
            v.Pause();
        else
            v.Play();
    }

    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { objData.isFloating = !objData.isFloating; }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }
}
