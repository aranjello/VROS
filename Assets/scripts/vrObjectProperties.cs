using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class vrObjData{
    public string path;
    public bool isResizable;
    public bool isFloating;
    public bool isMoveable;
    public Vector3 pos, size;
    public Quaternion rot;
    public bool spawn = false;
    public enum objType{
        folder,
        image,
        video,
        sound
    }

    public objType oType;
}
public class vrObjectProperties : MonoBehaviour
{
    public vrObjData objData;
    public virtual void Start(){
        //objData = new vrObjData();
        saveData.current.objs.Add(objData);
    }
    public IobjectController currController = null;
    public virtual void FixedUpdate(){
        objData.pos = transform.position;
        objData.rot = transform.rotation;
        objData.size = transform.localScale;
    }
}
