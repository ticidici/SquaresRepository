using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(new Vector3(transform.position.x, transform.position.y), Vector3.one);
    }
}
