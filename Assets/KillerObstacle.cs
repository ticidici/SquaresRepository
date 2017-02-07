using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    public float pushForce = 6f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Square targetToKill = null;
        targetToKill = collision.collider.GetComponent<Square>();
        if (targetToKill)
        {
            targetToKill.KillThisSquare(collision.contacts[0].normal, pushForce);
        }
    }
}
