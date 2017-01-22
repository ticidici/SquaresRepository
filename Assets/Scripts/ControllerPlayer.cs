using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class ControllerPlayer : MonoBehaviour {

    public float _speed = 12f;

    private Rigidbody2D _Rigidbody;
    private float _MovementInputValueX;
    private float _MovementInputValueY;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update () {
        _MovementInputValueX = Input.GetAxis("Horizontal");
        _MovementInputValueY = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 movement = Vector2.right * _MovementInputValueX * _speed * Time.deltaTime;
        movement += Vector2.up * _MovementInputValueY * _speed * Time.deltaTime;

        _Rigidbody.AddForceAtPosition(movement, transform.position + new Vector3(0,0.1f,0), ForceMode2D.Impulse);
    }
}
