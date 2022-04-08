using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class imgFileController : genericFilObj, IVRAction
{
    public override void setup(string pathToImage){
        objData.path = pathToImage;
        objData.oType = vrObjData.objType.image;
        Texture2D newTex = null;
        byte[] fileData;
        fileData = File.ReadAllBytes(pathToImage);
        newTex = new Texture2D(2, 2);
        newTex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        
        this.GetComponent<Renderer>().materials[1].SetTexture("_MainTex", newTex);
        //this.GetComponent<Renderer>().materials[1].SetFloat("_Mode", 1.0f);
    }
    public void triggerPull() { }
    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { objData.isFloating = !objData.isFloating; }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }
}
