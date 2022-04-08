using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;

public class triLibTest : MonoBehaviour
{
    public string tempPath = @"D:\VROSRegular\Assets\Models\room.blend";
    // Start is called before the first frame update
    async void Start()
    {
        AssetLoaderContext ac =  AssetLoader.LoadModelFromFile(tempPath);
        await ac.Task;
        Instantiate(ac.RootGameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
