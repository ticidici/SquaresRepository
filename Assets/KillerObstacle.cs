using UnityEngine;
using System.Collections;

public class KillerObstacle : MonoBehaviour {

    public float pushForce = 6f;
    Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps)
        {
            var col = ps.colorOverLifetime;
            col.enabled = true;
            Gradient grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] { new GradientColorKey(GetComponent<Renderer>().material.color, 0.0f), new GradientColorKey(GetComponent<Renderer>().material.color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, .17f), new GradientAlphaKey(0.0f, 1.0f) });
            col.color = grad;
        }
        //_rb.AddTorque(1, ForceMode2D.Impulse);
        
    }

    void Start()
    {
        //_rb.AddForce(Vector3.left * 100, ForceMode2D.Force);
    }
    
    void FixedUpdate()
    {
        
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(gameObject);
        Polygon targetToKill = null;
        targetToKill = collision.collider.GetComponent<Polygon>();
        if (targetToKill)
        {
            targetToKill.CurrentSuperSquare.ExplodeSquare(targetToKill.Id, collision.contacts[0].normal);
            CameraShake.ShakeCameraDefault();
        }
    }
}
