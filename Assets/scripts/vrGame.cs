using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class vrGame : genericFilObj, IVRAction
{
    public TextMeshPro tmp;
    private string gameName;
    // Start is called before the first frame update
    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
    }

    public override void setup(string name){
        objData.path = name;
        this.gameName = name;
        tmp.SetText(Path.GetFileNameWithoutExtension(name));
    }

    public void triggerPull()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
        Application.OpenURL(gameName);
    }
    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { objData.isFloating = !objData.isFloating; }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }
}
