using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision)
    {
        Square targetToKill = null;
        targetToKill = collision.collider.GetComponent<Square>();
        if (targetToKill)
        {
            targetToKill.KillThisSquare();
        }
    }
}
