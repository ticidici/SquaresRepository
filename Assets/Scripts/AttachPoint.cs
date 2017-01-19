using UnityEngine;
using System.Collections;

public class AttachPoint : MonoBehaviour
{   
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y), .05f);
    }
}
