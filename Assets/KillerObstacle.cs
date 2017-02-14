using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    public float pushForce = 6f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Polygon targetToKill = null;
        targetToKill = collision.collider.GetComponent<Polygon>();
        if (targetToKill)
            targetToKill.CurrentSuperSquare.ExplodeSquare(targetToKill.Id,collision.contacts[0].normal);
    }
}
