using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class filesytemTest : MonoBehaviour
{
    public string root;
    private CancellationTokenSource cancelToken;
    private Task<List<string>> read;
    long starTime;
    private List<string> fullFileList;
    // Start is called before the first frame update
    void Start()
    {
        cancelToken = new CancellationTokenSource();
        starTime = DateTime.Now.Ticks;
        read = filesystemUtils.getAllFilesAndDirs(root);
        read.ContinueWith(fileReadDone);
    }

    void fileReadDone(Task<List<string>> t){
        if(t.Status == TaskStatus.RanToCompletion && !cancelToken.IsCancellationRequested)
            Debug.LogWarning("task finished successfully");
        else if(t.Status == TaskStatus.Faulted)
            print(t.Exception?.Message);
        else
            Debug.LogWarning("task canceled");
        
        fullFileList = t.Result;
        foreach(string s in fullFileList){
            print(s);
        }
    }

    void Update(){
        // if(DateTime.Now.Ticks - starTime > 1000000 && read.Status == TaskStatus.WaitingForActivation){
        //     endTask();
        // }
    }

    public void endTask(){
        cancelToken.Cancel();
    }
}
