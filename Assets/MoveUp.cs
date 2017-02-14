using UnityEngine;
using System.Collections;

public class MoveUp : MonoBehaviour {

    public Vector2 velocity;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Renderer>().material.color = Color.gray;
    }

    void OnEnable()
    {
        Invoke("ReturnToPool", 10);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + 50 * Time.fixedDeltaTime);
    }
}
