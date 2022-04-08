using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fileReaderFolderHolder : MonoBehaviour, IobjectController
{
    public GameObject currObj;
    bool fullExit = true;

    
    fileReaderController frc;
    // Start is called before the first frame update
    void Start()
    {
        frc = GetComponentInParent<fileReaderController>();
    }

    void OnCollisionEnter(Collision collider){
        if(!fullExit)
            return;
        GameObject g = collider.gameObject;
        while(g.transform.parent != null)
            g = g.transform.parent.gameObject;
        vrObjectProperties op = g.GetComponent<vrObjectProperties>();
        if(!currObj && op != null && op.GetComponent<folderController>()){
            currObj = g;
            op.currController.remove(g);
            op.currController = this;
            g.transform.position = this.transform.position;
            g.transform.rotation = Quaternion.Euler(0,0,-45f);
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if(rb != null){
                rb.isKinematic = true;
            }
            g.GetComponent<Rigidbody>().isKinematic = true;
            foreach(Collider c in g.GetComponentsInChildren<Collider>()){
                c.isTrigger = true;
            }
            if(g.GetComponent<Collider>())
                g.GetComponent<Collider>().isTrigger = true;
            this.gameObject.SetActive(false);
            fullExit = false;
            frc.folderEntered();
        }
    }

    void OnCollisionExit(Collision c){
        if (c.gameObject.GetComponent<folderController>())
        {
            fullExit = true;
            
            c.gameObject.GetComponent<folderController>().frc = null;
        }
    }

    void OnTriggerExit(Collider c){
        this.GetComponent<Collider>().isTrigger = false;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    public void remove(GameObject g){
        if(g == currObj){
            Debug.Log("removing " + currObj.name);
            currObj.GetComponent<Rigidbody>().isKinematic = false;
            foreach(Collider c in currObj.GetComponentsInChildren<Collider>()){
                    c.isTrigger = false;
                }
            if(currObj.GetComponent<Collider>())
                currObj.GetComponent<Collider>().isTrigger = false;
            }
            frc.folderExited();
            currObj = null;
        this.GetComponent<Collider>().isTrigger = true;
        this.gameObject.SetActive(true);
    }
}
