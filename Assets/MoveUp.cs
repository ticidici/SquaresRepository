using UnityEngine;
using System.Collections;

public class MoveUp : MonoBehaviour {

    public Vector2 _velocity;
    public bool _killObjectOnTime = true;
    public float _timeBeforeKilling = 10;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<Renderer>().material.color = Color.gray;
    }

    void OnEnable()
    {
        if (_killObjectOnTime)
        {
            Invoke("ReturnToPool", _timeBeforeKilling);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation + 50 * Time.fixedDeltaTime);
    }
}
