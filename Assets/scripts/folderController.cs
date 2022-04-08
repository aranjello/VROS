using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Threading;
using System.IO;
using UnityEngine;
using TMPro;
using TriLibCore;


public class folderController : genericFilObj, IobjectController, IVRAction
{
    [SerializeField]
    public GameObject imgObj, vidObj, soundObj, folderObj, portal;
    public fileReaderController frc;
    bool open = false;

    private Vector3 objectPlacement = new Vector3(0, 1, 0);
    public List<GameObject> objectsInside = new List<GameObject>();
    public void Start(){
        if (path != "")
        {
            setup(path);
            loadFiles();
        }
    }

    public override void setup(string s){
        objData.path = s;
        print(s);
        path = s;
        this.GetComponentInChildren<TextMeshPro>().SetText(path);
        Debug.Log("folder: " + s + " has #" + objectsInside.Count + "objects in it");
    }

    public string getName(){
        return path;
    }

    private async void loadFiles(){
        string[] containedFiles = filesystemUtils.getAllFiles(path);
        List<string> containedFolders = await filesystemUtils.getAllDirectories(path);
        foreach(string s in containedFiles){
            if (File.Exists(s)){
                switch (Path.GetExtension(s).ToLower())
                {
                    case ".png":
                    case ".jpeg":
                    case ".jpg":
                    case ".tif":
                    case ".webp":
                        print($"{s} is image");
                        loadImage(s);
                        break;
                    case ".mp4":
                    case ".mov":
                    case ".avi":
                        print($"{s} is vid");
                        loadVideo(s);
                        break;
                    case ".wav":
                    case ".ogg":
                        print($"{s} is sound");
                        LoadClip(s);
                        break;
                    // case ".stl":
                    // case ".fbx":
                    // case ".obj":
                    // case ".gltf2":
                    // case ".ply":
                    //     print($"{s} is model");
                    //     loadModel(s);
                    //     break;
                    default:
                        Debug.LogWarning($"unhandled file {s} on extension {Path.GetExtension(s)}");
                        break;
                }
            }
            await System.Threading.Tasks.Task.Yield();
            
        }
        foreach(string s in containedFolders){
            GameObject g = Instantiate(folderObj);
            g.GetComponent<folderController>().folderObj = folderObj;
            g.GetComponent<folderController>().setup(s);
            g.GetComponent<folderController>().loadFiles();
            g.GetComponent<genericFilObj>().path = s;
            setupInternalFiles(g);
            //g.transform.localScale *= 2;
        }
        //triggerPull();
    }
    public void triggerPull()
    {
        // rearrangeFiles();
        // bool currentFolderState = !GetComponent<Animator>().GetBool("open");
        // GetComponent<Animator>().SetBool("open",currentFolderState);
        // setFilesActive(currentFolderState);
        // await System.Threading.Tasks.Task.Yield();
    }

    public void triggerSqueeze(float triggerValue) { }
    public void gripGrab() { }
    public void touchPadClick() { objData.isFloating = !objData.isFloating; }
    public void touchPadPosition(Vector2 touchPosition) { }
    public void menuPress() { }

    public void setFilesActive(bool folderState){
        if(folderState == false){
            for(int i = 0; i < objectsInside.Count; i++){
                GameObject g = objectsInside[i];
                g.SetActive(folderState);
            }
        }else{
            for(int i = 0; i < Mathf.Min(9,objectsInside.Count); i++){
                GameObject g = objectsInside[i];
                g.SetActive(folderState);
            }
        }
        portal.SetActive(folderState);
    }
    void loadImage(string path){
        GameObject g = Instantiate(imgObj,objectPlacement,Quaternion.identity);
        g.GetComponent<genericFilObj>().path = path;
        g.GetComponentInChildren<TextMeshPro>().SetText(Path.GetFileName(path));
        //objectPlacement += new Vector3(0, 1, 0);
        g.GetComponent<imgFileController>().setup(path);
        setupInternalFiles(g);
    }

    void loadVideo(string path){
        GameObject g = Instantiate(vidObj,objectPlacement,Quaternion.identity);
        g.GetComponent<genericFilObj>().path = path;
        g.GetComponentInChildren<TextMeshPro>().SetText(Path.GetFileName(path));
        //objectPlacement += new Vector3(0, 1, 0);
        g.GetComponent<videoFileController>().setup(path);
        setupInternalFiles(g);
    }
    void LoadClip(string path)
    {
        GameObject g = Instantiate(soundObj,objectPlacement,Quaternion.identity);
        g.GetComponent<genericFilObj>().path = path;
        g.GetComponentInChildren<TextMeshPro>().SetText(Path.GetFileName(path));
        //objectPlacement += new Vector3(0, 1, 0);
        g.GetComponent<audioFileController>().setup(path);
        setupInternalFiles(g);
    }

    async void loadModel(string s){
        AssetLoaderContext ac =  AssetLoader.LoadModelFromFile(s);
        if(ac.Task == null)
            return;
        await ac.Task;
        GameObject g = Instantiate(ac.RootGameObject);
        g.AddComponent<genericFilObj>().path = path;
        g.AddComponent<MeshCollider>();
        g.AddComponent<Rigidbody>();
        //objectPlacement += new Vector3(0, 1, 0);
        setupInternalFiles(g);
    }

    public void setupInternalFiles(GameObject g){
        vrObjectProperties objProps = g.GetComponent<vrObjectProperties>();
        if (objProps != null && objProps.currController != this && objProps.currController != null)
        {
            objProps.currController.remove(g);
        }
        objProps.currController = this;
        //g.transform.localScale = Vector3.one*.025f;
        g.transform.parent = this.transform;
        g.GetComponent<Rigidbody>().isKinematic = true;
        foreach(Collider c in g.GetComponentsInChildren<Collider>()){
                c.isTrigger = true;
            }
        if(g.GetComponent<Collider>())
            g.GetComponent<Collider>().isTrigger = true;
        objectsInside.Add(g);
        g.gameObject.SetActive(false);
    }

    public GameObject getGameObject(){
        return this.gameObject;
    }

    public void remove(GameObject g){
        Debug.Log("removing " + g.name);
        //g.transform.localScale = Vector3.one;
        g.transform.parent = null;
        g.GetComponent<Rigidbody>().isKinematic = false;
        foreach(Collider c in g.GetComponentsInChildren<Collider>()){
                c.isTrigger = false;
            }
        if(g.GetComponent<Collider>())
            g.GetComponent<Collider>().isTrigger = false;
        // string currFilePath = g.GetComponent<genericFilObj>().path;
    //     string newFilePath = @"D:\loosefiles\" + SceneManager.GetActiveScene().name + @"\";
    //     if (Directory.Exists(currFilePath))
    //     {
    //         newFilePath += Path.GetFileName(currFilePath);
    //         Debug.LogWarning(newFilePath);
    //         Directory.Move(currFilePath, newFilePath);
    //         g.GetComponent<folderController>().setup(newFilePath);
    //     }
    //     else if (File.Exists(currFilePath))
    //     {
    //         newFilePath += Path.GetFileName(currFilePath);
    //         //File.Move(currFilePath, newFilePath);
            
    //     }
    //     g.GetComponent<genericFilObj>().objData.path = newFilePath;
    //     g.GetComponent<genericFilObj>().path = newFilePath;
        objectsInside.Remove(g);
        frc.removeLastElem();
        //     //setFilesActive(true);
        //     //rearrangeFiles();
    }
    // public void rearrangeFiles()
    // {
    //     float spacing = .5f;
    //     int gridSize = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Min(objectsInside.Count,9)));
    //     for (int i = 0; i < objectsInside.Count; i++){
    //         GameObject g = objectsInside[i];
    //         int x = i % gridSize;
    //         int y = i / gridSize;
    //         g.transform.localRotation = Quaternion.Euler(180,0,0);
    //         //g.transform.right = this.transform.right;
    //         g.transform.localPosition = new Vector3(-.1f, (gridSize-1) / 2.0f * -spacing + spacing * x, (gridSize-1) / 2.0f * -spacing + spacing * y);
    //     }
    // }
}
