using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    public float pushForce = 6f;

    void Awake()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(GetComponent<Renderer>().material.color, 0.0f), new GradientColorKey(GetComponent<Renderer>().material.color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .17f), new GradientAlphaKey(0.0f, 1.0f) });
        col.color = grad;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Polygon targetToKill = null;
        targetToKill = collision.collider.GetComponent<Polygon>();
        if (targetToKill)
            targetToKill.CurrentSuperSquare.ExplodeSquare(targetToKill.Id,collision.contacts[0].normal);
    }
}
