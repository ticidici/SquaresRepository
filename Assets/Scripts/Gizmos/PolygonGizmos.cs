using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PolygonGizmos : MonoBehaviour {

    void Awake()
    {
    }

    void Update()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 aux = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector3.up;
        Gizmos.DrawRay(transform.position, aux);
    }
}