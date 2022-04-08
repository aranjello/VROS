using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class handController : MonoBehaviour, IobjectController
{
    public InputActionAsset InputActions;
    private InputActionMap _handMap;
    [SerializeField]
    private handOptions hand;
    private List<GameObject> objectsEntered = new List<GameObject>();
    private GameObject currObj = null, lastRayCast = null;
    private GameObject otherHand = null;
    private float origDistance = 0;
    private Vector3 origSize;
    Vector3 grabDir, origOtherHandPos;
    Quaternion origOtherHandRot;
    Rigidbody rb;
    private bool grabbed = false;
    private Vector3 grabPos;
    private Quaternion grabRot, origGrabRot;
    [Range(0f,1f)] public float positionStrength = 1f;
	[Range(0f,1f)] public float rotationStrength = 1f;
    private enum handOptions
    {
        left,
        right
    }
    void Awake(){
        if(hand == handOptions.left)
            _handMap = InputActions.actionMaps[0];
        else
            _handMap = InputActions.actionMaps[1];
        
        foreach(InputAction a in _handMap){
            a.started += actionPhaseHandler;
            a.performed += actionPhaseHandler;
            a.canceled += actionPhaseHandler;
            a.Enable();
        }
        // _handMap[gripStr].started += grip;
        // _handMap[gripStr].canceled += grip;
        // _handMap[triggerStr].performed += trigger;
        // _handMap[gripStr].Enable();
        // _handMap[triggerStr].Enable();
    }
    void actionPhaseHandler(InputAction.CallbackContext context){
        if(currObj == null){
            if (lastRayCast != null && context.started)
            {
                IVRAction action = lastRayCast.GetComponent<IVRAction>();
                switch (context.action.name)
                {
                    case "triggerPress":
                        action.triggerPull();
                        break;
                    case "triggerSqueeze":
                        action.triggerSqueeze(context.ReadValue<float>());
                        break;
                    case "touchPadTouch":
                        action.touchPadClick();
                        break;
                    case "touchPadPos":
                        action.touchPadPosition(context.ReadValue<Vector2>());
                        break;
                    case "menuButton":
                        action.menuPress();
                        break;
                    case "gripGrab":
                        action.gripGrab();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return;
        }
            
        vrObjectProperties props = currObj.GetComponent<vrObjectProperties>();
        if(props != null && props.objData.isMoveable){
            if (context.action.name == "gripGrab")
            {
                grip(context);
                if (grabbed == false || context.canceled)
                    return;
            }
        }
        IVRAction a = currObj.GetComponent<IVRAction>();
        if(a == null || !context.performed)
            return;
        switch (context.action.name)
        {
            case "triggerPress":
                a.triggerPull();
                break;
            case "triggerSqueeze":
                a.triggerSqueeze(context.ReadValue<float>());
                break;
            case "touchPadTouch":
                a.touchPadClick();
                break;
            case "touchPadPos":
                a.touchPadPosition(context.ReadValue<Vector2>());
                break;
            case "menuButton":
                a.menuPress();
                break;
            case "gripGrab":
                a.gripGrab();
                break;
            default:
                 throw new NotImplementedException();
        }
    }

    void OnTriggerEnter(Collider c){
        GameObject collidedObj = c.gameObject;
        while(collidedObj.transform.parent != null && (collidedObj.GetComponent<vrObjectProperties>() == null)){
            //Debug.Log("collided object: " + collidedObj.name + " parent: " + collidedObj.transform.parent.name);
            collidedObj = collidedObj.transform.parent.gameObject;
        }
        vrObjectProperties props = collidedObj.GetComponent<vrObjectProperties>();
        if (props != null)
        {
            //Debug.Log("making currObj: " + collidedObj.name);
            if (!objectsEntered.Contains(collidedObj))
                objectsEntered.Add(collidedObj);
            if (!grabbed)
            {   
                currObj = collidedObj;
            }
        }
    }

    //refactor for better multi object drop
    void OnTriggerExit(Collider c){
        vrObjectProperties props = c.gameObject.GetComponent<vrObjectProperties>();
        if (props != null)
        {
            if (objectsEntered.Contains(c.gameObject))
                objectsEntered.Remove(c.gameObject);
            if (c.gameObject == currObj)
            {
                if (!grabbed)
                {
                    checkOtherObjects();
                }
            }
        }
    }
    void checkOtherObjects(){
        objectsEntered.RemoveAll(obj => obj == null);
        if (!objectsEntered.Contains(currObj))
        {
            if (objectsEntered.Count > 0)
            {
                currObj = objectsEntered[objectsEntered.Count - 1];
            }
        }
        currObj = null;
    }
    //refactor for safer pickup
    void grip(InputAction.CallbackContext context){
        if(currObj != null){
            if (!grabbed && context.started)
            {
                Debug.Log("starting grab on " + currObj.name);
                startGrab();
            }
            else if(grabbed && context.canceled)
            {
                endGrab();
            }
        }
    }

    void setForSizing(GameObject g){
        otherHand = g;
        origDistance = Vector3.Distance(this.transform.position, otherHand.transform.position);
        origSize = currObj.transform.localScale;
        grabDir = this.transform.position-otherHand.transform.position;
        origOtherHandPos = otherHand.GetComponent<handController>().grabPos;
        origOtherHandRot = otherHand.GetComponent<handController>().grabRot;
    }

    void startGrab(){
        Debug.Log("attempting grab on " + currObj.name);
        grabbed = true;
        vrObjectProperties props = currObj.GetComponent<vrObjectProperties>();
        if(props != null){
            IobjectController controller = props.currController;
            if(controller != null){
                if(props.objData.isResizable && controller.getGameObject().GetComponent<handController>() != null){
                    setForSizing(controller.getGameObject());
                    return;
                }else{
                    controller.remove(currObj);
                }
            }   
        }
        props.currController = this;
        rb = currObj.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
        grabPos = currObj.transform.position - this.transform.position;
        grabRot = Quaternion.Inverse(this.transform.rotation) * currObj.transform.rotation;
        origGrabRot = this.transform.rotation;
        
    }

    void endGrab(){
        vrObjectProperties props = currObj.GetComponent<vrObjectProperties>();
        if(props != null){
            if(otherHand == null)
                props.currController = null;
        }
        otherHand = null;
        grabbed = false;
        checkOtherObjects();
        if(!props.objData.isFloating && rb != null){
            
            rb.useGravity = true;
        }else if(rb != null){
            rb.useGravity = false;
            rb.isKinematic = true;
        }
        rb = null;
        grabPos = Vector3.zero;
        grabRot = new Quaternion(0, 0, 0, 0);
    }

    
    //refactor to enusre constent pick up and drop
    void FixedUpdate(){
        if(grabbed && currObj != null){
            if(otherHand == null)
                moveCurrObj();
            else
                resizeObj();
        }

        if(currObj == null){
            checkForPointObj();
        }
    }

    void checkForPointObj(){
         RaycastHit hit;
        // You can use Raycast in many ways, this is a way to use it
        // We take our Camera Position, then we "shoot" it forward, save infos in hit, and specify a range
        if (Physics.SphereCast(this.transform.position,.1f,this.transform.forward, out hit, 1.5f,1<<7))
        {
            if(lastRayCast && lastRayCast != hit.transform.gameObject){
                lastRayCast.GetComponent<Renderer>().material.color = Color.white;
            }
            lastRayCast = hit.transform.gameObject;
            lastRayCast.GetComponent<Renderer>().material.color = Color.blue;
        }else if(lastRayCast != null){
            lastRayCast.GetComponent<Renderer>().material.color = Color.white;
        }
    }
    void resizeObj(){
        otherHand.GetComponent<handController>().grabPos = origOtherHandPos * (float)Math.Pow(1.0f + Vector3.Distance(transform.position, otherHand.transform.position) - origDistance,2.0f);
        // Quaternion q;
        // Vector3 v1 = this.transform.position - otherHand.transform.position;
        // Vector3 v2 = grabDir;
        // Vector3 a = Vector3.Cross(v1, v2);
        // q.x = a.x;
        // q.y = a.y;
        // q.z = a.z;
        // q.w = (float)Math.Sqrt(Math.Pow(v1.magnitude,2) * Math.Pow(v2.magnitude,2)) + Vector3.Dot(v1, v2);
        // otherHand.GetComponent<handController>().grabRot = origOtherHandRot * Quaternion.Inverse(q);
        currObj.transform.localScale = origSize * (float)Math.Pow(1.0f + Vector3.Distance(transform.position, otherHand.transform.position) - origDistance,2.0f);
    }

        public GameObject getGameObject()
        {
            return this.gameObject;
        }

        public void remove(GameObject g)
        {
            if (g == currObj)
            {
                endGrab();
                currObj = null;
            }
        }

        void moveCurrObj()
        {
            Debug.Log("attempting to move: " + currObj.name);
            //this.transform.rotation * Quaternion.Inverse(origGrabRot) is basically what is the delta of roation from when we first grabbed to now
            //multiplying this delta by the orginal vector from our hand to the center of the object rotaes the vector to piint where the center should
            //be after rotating adding the current hand positon gives us the point our new center should be subtracyting the objects current position
            //tells us how far where our object should be is from where it is.
            Vector3 deltaPos = this.transform.rotation * Quaternion.Inverse(origGrabRot) * grabPos + this.transform.position - currObj.transform.position;
            rb.velocity = 1f / Time.fixedDeltaTime * deltaPos * Mathf.Pow(positionStrength, 90f * Time.fixedDeltaTime);

            Quaternion deltaRot = (this.transform.rotation * grabRot) * Quaternion.Inverse(currObj.transform.rotation);

            float angle;
            Vector3 axis;

            deltaRot.ToAngleAxis(out angle, out axis);

            if (angle > 180.0f) angle -= 360.0f;

            if (angle != 0) rb.angularVelocity = (1f / Time.fixedDeltaTime * angle * axis * 0.01745329251994f * Mathf.Pow(rotationStrength, 90f * Time.fixedDeltaTime));
        }
    }
