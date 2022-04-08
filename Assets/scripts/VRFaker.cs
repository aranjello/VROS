using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRFaker : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    // Start is called before the first frame update
    float rotationX = 0;
    float rotationY = 90;
    public float lookSpeed = 2.0f;

    Vector3 grabPoint = Vector3.zero;
    bool grabbed = false;

    GameObject grabbedObj = null, pointedObject = null;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 nonvertForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 nonvertRight = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        transform.position += nonvertForward * moveSpeed * Input.GetAxis("Vertical");
        transform.position += nonvertRight * moveSpeed * Input.GetAxis("Horizontal");
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY += Input.GetAxis("Mouse X") * lookSpeed;
        transform.localRotation = Quaternion.Euler(rotationX,rotationY, 0); 
        if(Input.GetMouseButton(0) && !grabbed && pointedObject != null){
            grabbed = true;
            pointedObject.GetComponent<Rigidbody>().isKinematic = true;
            pointedObject.transform.parent = this.transform;
            grabbedObj = pointedObject;
        }else if(!Input.GetMouseButton(0) && grabbed){
            grabbed = false;
            grabbedObj.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObj.transform.parent = null;
            grabbedObj = null;
        }

        if(grabbedObj != null && Input.mouseScrollDelta.y != 0){
            grabbedObj.transform.localPosition += Vector3.Normalize(grabbedObj.transform.localPosition) * Input.mouseScrollDelta.y;
        }

        if(grabbedObj != null){
            IVRAction va = grabbedObj.GetComponent<IVRAction>();
            if( va != null){
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    va.triggerPull();
                    if (grabbedObj.name.Contains("folder"))
                        remove();
                }
                if(Input.GetKeyDown(KeyCode.LeftShift)){
                    va.menuPress();
                }
            }
        }
    }

    public void remove(){
        Destroy(grabbedObj);
        grabbedObj = null;
        grabbed = false;
    }

     void FixedUpdate()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity) 
            && hit.collider.CompareTag("VRInteractable")
            && !grabbed)
        {
            pointedObject = hit.collider.gameObject;
        }else{
            pointedObject = null;
        }
    }
}
