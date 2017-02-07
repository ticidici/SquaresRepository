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

	// Use this for initialization
	void Start () {
        Destroy(gameObject,10);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + 50 * Time.fixedDeltaTime);
    }
}
