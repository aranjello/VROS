using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fileReaderController : MonoBehaviour
{
    public Transform objTransform;
    Transform objLastTransform;
    int objIndex = 0;
    int lastVal = 0;
    public fileReaderFolderHolder fh;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateFloater(){
        if(lastVal != objIndex){
            fh.currObj.GetComponent<folderController>().objectsInside[lastVal].SetActive(false);
        }
        GameObject wantedObj = fh.currObj.GetComponent<folderController>().objectsInside[objIndex];
        objLastTransform = wantedObj.transform;
        wantedObj.transform.position = objTransform.position;
        wantedObj.transform.rotation = objTransform.rotation;
        //wantedObj.transform.localScale = Vector3.one;
        wantedObj.SetActive(true);
    }

    public void folderEntered(){
        objIndex = 0;
        lastVal = objIndex;
        if(fh.currObj.GetComponent<folderController>().objectsInside.Count > 0)
            updateFloater();
        fh.currObj.GetComponent<folderController>().frc = this;
    }

    public void folderExited(){
        fh.currObj.GetComponent<folderController>().objectsInside[objIndex].SetActive(false);
    }

    public void removeLastElem(){
        objIndex -= 1;
        if(objIndex < 0){
            objIndex = fh.currObj.GetComponent<folderController>().objectsInside.Count - 1;
        }
        lastVal = objIndex;
        Debug.LogWarning("LastVal: " + lastVal + " objIndex: " + objIndex);
        if(fh.currObj.GetComponent<folderController>().objectsInside.Count > 0 && lastVal >= 0 && objIndex >= 0)
            updateFloater();
    }

    public void move(bool left){
        if(fh.currObj != null){
            lastVal = objIndex;
            if(left){
                
                objIndex = objIndex - 1;
                if(objIndex < 0)
                    objIndex = fh.currObj.GetComponent<folderController>().objectsInside.Count-1;
            }
            else
            {
                objIndex += 1;
                if(objIndex > fh.currObj.GetComponent<folderController>().objectsInside.Count-1){
                    objIndex = 0;
                }
            }
            if(fh.currObj.GetComponent<folderController>().objectsInside.Count > 0 && lastVal >= 0 && objIndex >= 0)
                updateFloater();
        }
    }

    public void addFile(GameObject g){
        if(fh.currObj != null){
            fh.currObj.GetComponent<folderController>().setupInternalFiles(g);
            removeLastElem();
        }
    }

    public void removedFolder(){

    }
}
