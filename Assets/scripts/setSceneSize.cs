using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class setSceneSize : MonoBehaviour
{
     private XRInputSubsystem _inputSubSystem;
    private List<Vector3> _boundaries;
    Mesh mesh;
    Vector3[] vertices = new Vector3[4];
    int[] triangles = new int[6];
    // void Start()
    // {
    //     mesh = GetComponent<MeshFilter>().mesh;
    // }
 
    // Update is called once per frame
    void Update()
    {
        if (_inputSubSystem == null)
            VRAwake();
    }
 
    void VRAwake()
    {
        List<XRInputSubsystem> list = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances<XRInputSubsystem>(list);
        foreach (var sSystem in list)
        {
            print(sSystem.ToString());
            _inputSubSystem = sSystem;
            break;
        }
        if (_inputSubSystem != null)
        {
            _inputSubSystem.boundaryChanged += RefreshBoundaries;
        }
    }
 
    private void RefreshBoundaries(XRInputSubsystem inputSubsystem)
    {
        List<Vector3> currentBoundaries = new List<Vector3>();
        //if (UnityEngine.Experimental.XR.Boundary.TryGetGeometry(currentBoundaries))
        if (inputSubsystem.TryGetBoundaryPoints(currentBoundaries))
        {
            //got boundaries, keep only those which didn't change.
            if (currentBoundaries != null && (_boundaries != currentBoundaries || _boundaries.Count != currentBoundaries.Count))
            {
                int counter = 0;
                _boundaries = currentBoundaries;
                // Vector3 boundPoint = _boundaries[0] / 8;
                // this.transform.localScale = new Vector3(Mathf.Abs(boundPoint.x), 1, Mathf.Abs(boundPoint.z));
                foreach(Vector3 point in _boundaries){
                    vertices[counter] = point / 2;
                    Debug.Log("Points are: " + point/2);
                    counter++;
                }
                // mesh.vertices = vertices;
                // mesh.RecalculateBounds();
                resizeMesh();
            }
        }
    }

    private void resizeMesh(){
        mesh = GetComponent<MeshFilter>().mesh;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.Clear ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize ();
        mesh.RecalculateNormals ();
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
