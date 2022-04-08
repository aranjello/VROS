using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class folderEntrace : MonoBehaviour
{
    void OnTriggerEnter(Collider c){
        folderController f = GetComponentInParent<folderController>();
        GameObject currObj = c.gameObject;
        while(currObj.GetComponent<genericFilObj>() == null && currObj.transform.parent != null){
            currObj = currObj.transform.parent.gameObject;
        }
        if(!f.objectsInside.Contains(currObj) && currObj.GetComponent<genericFilObj>() != null){
            string currFilePath = currObj.GetComponent<genericFilObj>().path;
            string newFilePath = f.getName() + @"\";
            if (Directory.Exists(currFilePath))
            {
                newFilePath += Path.GetFileName(currFilePath);
                Debug.LogWarning(newFilePath);
                Directory.Move(currFilePath, newFilePath);
                currObj.GetComponent<folderController>().setup(newFilePath);
            }
            else if (File.Exists(currFilePath))
            {
                newFilePath += Path.GetFileName(currFilePath);
                File.Move(currFilePath, newFilePath);
            }   
            currObj.GetComponent<genericFilObj>().path = newFilePath;
            f.setupInternalFiles(currObj);
            // f.rearrangeFiles();
            f.setFilesActive(true);
        }
    }
}
