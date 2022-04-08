using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class keyboardController : MonoBehaviour
{
    public Transform folderSpawn;
    public GameObject folder;
    List<GameObject> keys = new List<GameObject>();
    List<string> fullDirs = new List<string>();

    List<string> possibleFiles = new List<string>();
    int possibleFileIndex = 0;
    public displayController dc;
    public TextMeshPro fileText;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount-2; i++){
            GameObject child = transform.GetChild(i).gameObject;
            GameObject textObj = new GameObject();
            textObj.transform.parent = child.transform;
            TextMeshPro keyText = textObj.AddComponent<TextMeshPro>();
            if(child.name == "enter")
                keyText.SetText("->");
            else if(child.name == "del")
                keyText.SetText("<-");
            else if(child.name == "tab")
                keyText.SetText(">");
            else
                keyText.SetText(child.name);
            RectTransform textTransform = keyText.rectTransform;
            textTransform.localPosition = new Vector3(0, 0, 1.2f);
            textTransform.localEulerAngles = new  Vector3(180, 0, -90);
            textTransform.localScale = new Vector3(2, 2, 2);
            textTransform.sizeDelta = new Vector2(1, 1);
            keyText.color = Color.black;
            keyText.enableAutoSizing = true;
            keyText.fontSizeMin = 10;
            keyText.verticalAlignment = VerticalAlignmentOptions.Geometry;
            keyText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            child.AddComponent<keyController>();
            keys.Add(transform.GetChild(i).gameObject);
        }
    }

    void updatePossibilities(){
        possibleFiles.Clear();
        possibleFileIndex = 0;
        string[] filesAndFolders = filesystemUtils.getImmediateFolders(fullDirs[fullDirs.Count-1]);
        string ExtraText = fileText.text.Substring(fullDirs[fullDirs.Count-1].Length);
        Debug.Log("extra text: " + ExtraText);
        foreach(string fileNames in filesAndFolders){
                string firstCheck = fileNames.Substring(fullDirs[fullDirs.Count - 1].Length);
                if (firstCheck.Length >= ExtraText.Length)
                {
                    string checkTextSub = firstCheck.Substring(0, ExtraText.Length).ToLower();
                    Debug.Log("check text: " + ExtraText);
                    if (ExtraText == checkTextSub)
                    {
                        possibleFiles.Add(fileNames);
                    }
                }
            }
    }

    public void updateText(string s){
        if(s != "enter" && s != "del" && s != "tab")
            fileText.text += s;
        if(s == "enter"){
            if(!filesystemUtils.dirExists(fileText.text))
                return; 
            folderController fc = Instantiate(folder, folderSpawn.position, folderSpawn.rotation).GetComponent<folderController>();
            fc.folderObj = folder;
            fc.setup(fileText.text);
            fileText.text = @"d:\sandbox";
            if(fullDirs.Count > 0)
                fullDirs.Clear();
            possibleFileIndex = 0;
            if(possibleFiles.Count > 0)
                possibleFiles.Clear();
            return;
        }
        if(s == "tab" && possibleFiles.Count > 0)
        {
            fileText.text = possibleFiles[possibleFileIndex];
            possibleFileIndex = possibleFileIndex + 1 % possibleFiles.Count;
            fullDirs.Add(fileText.text);
            return;
        }
        if(s == "del"){
            fileText.text = fileText.text.Substring(0, fileText.text.Length - 1);
            if(fullDirs.Count > 0 && fileText.text.Length < fullDirs[fullDirs.Count-1].Length){
                fullDirs.RemoveAt(fullDirs.Count-1);
            }
            if(fileText.text.Length < @"d:\sandbox".Length)
                fileText.text = @"d:\sandbox";
        }
        string fileOrFolder = filesystemUtils.getFirstFolder(fileText.text);
        if(fileOrFolder != ""){
            Debug.Log("Full dir: " + fileText.text);
            fullDirs.Add(fileText.text);
        }
        if(fullDirs.Count > 0){
            updatePossibilities();
        }
    }
}
