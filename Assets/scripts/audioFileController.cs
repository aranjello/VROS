using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class audioFileController : genericFilObj, IVRAction
{
    public override async void setup(string filePath){
        objData.path = filePath;
        objData.oType = vrObjData.objType.sound;
        AudioClip clip = null;
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(Path.Combine(@"file:\\",filePath), AudioType.WAV))
        {
            uwr.SendWebRequest();
    
            // wrap tasks in try/catch, otherwise it'll fail silently
            try
            {
                while (!uwr.isDone) { await Task.Delay(5); print("attempting load"); }
                if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError) Debug.Log($"{uwr.error}");
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (System.Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }
        this.GetComponent<AudioSource>().clip = clip;
    }

    public void triggerPull()
    {
        AudioSource a = this.GetComponent<AudioSource>();
        if(a.isPlaying)
            a.Pause();
        else
            a.Play();
    }

    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { objData.isFloating = !objData.isFloating; }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }
}
