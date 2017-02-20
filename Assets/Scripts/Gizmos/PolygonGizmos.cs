using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PolygonGizmos : MonoBehaviour {

    //SuperPolygon _polygon; 

    void Awake()
    {
        
    }

    void Update()
    {
        //_polygon = GetComponentInParent<SuperPolygon>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 aux = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector3.up;
        Gizmos.DrawRay(transform.position, aux);
        /*
        if(_polygon != null)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.back, _polygon._radiusForMerge);
        }*/
    }
}