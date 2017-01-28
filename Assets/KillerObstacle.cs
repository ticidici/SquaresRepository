using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("kill");
        Square targetToKill = null;
        targetToKill = collision.collider.GetComponent<Square>();
        if (targetToKill)
        {
            Debug.Log("kill for real");
            targetToKill.KillThisSquare();
        }
    }
}
