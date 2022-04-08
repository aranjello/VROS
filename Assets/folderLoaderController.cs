using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class folderLoaderController : MonoBehaviour
{

    fileReaderController frc;
    // Start is called before the first frame update
    void Start()
    {
        frc = GetComponentInParent<fileReaderController>();
    }

    void OnCollisionEnter(Collision c){
        if(c.gameObject.GetComponent<vrObjectProperties>()){
            frc.addFile(c.gameObject);
        }
    }
}
