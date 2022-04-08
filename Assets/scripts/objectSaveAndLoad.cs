using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class objectSaveAndLoad : MonoBehaviour
{
    string savePath;

    public GameObject imageObj, vidObj, soundObj, folderObj;
    public void Awake(){
        if(!Directory.Exists(@"D:\loosefiles\"+SceneManager.GetActiveScene().name)){
            Directory.CreateDirectory(@"D:\loosefiles\"+SceneManager.GetActiveScene().name);
        }
        savePath = Application.persistentDataPath + "/userRoomData/" + "test.mcn";
        if (File.Exists(savePath))
        {
            saveData.current = (saveData)Serializationmanager.load(savePath);
            if(saveData.current.objs == null || saveData.current.objs.Count == 0)
                return;
            foreach (vrObjData obj in saveData.current.objs)
            {
                
                if (obj.spawn)
                {
                    Debug.LogWarning("Spawning from path: " + obj.path);
                    GameObject g = null;
                    switch (obj.oType)
                    {
                        case vrObjData.objType.folder:
                            g = Instantiate(folderObj, obj.pos, obj.rot);
                            g.GetComponent<folderController>().folderObj = folderObj;
                            break;
                        case vrObjData.objType.image:
                            g = Instantiate(imageObj, obj.pos, obj.rot);
                            break;
                        case vrObjData.objType.video:
                            g = Instantiate(vidObj, obj.pos, obj.rot);
                            break;
                        case vrObjData.objType.sound:
                            g = Instantiate(soundObj, obj.pos, obj.rot);
                            break;
                    }
                    g.transform.localScale = obj.size;
                    g.GetComponent<genericFilObj>().setup(obj.path);
                    g.GetComponent<vrObjectProperties>().objData = obj;
                    g.GetComponent<Rigidbody>().useGravity = !obj.isFloating;
                    g.GetComponent<Rigidbody>().isKinematic = obj.isFloating;
                }
            }
        }
    }

    public void OnApplicationQuit(){
        Serializationmanager.Save("test", saveData.current);
    }
}
