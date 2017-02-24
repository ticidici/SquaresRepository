using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

    private Rigidbody2D _rb;
    GameObject _target;

    public AttachPoint _attachPoint;
    public float toAngle;
    public bool _isActive = false;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void FixedUpdate() {
        if(_isActive)
        {
            //Debug.DrawLine(transform.position, _target.transform.position, Color.yellow);
            //Debug.DrawLine(transform.position, _attachPoint.transform.position, Color.yellow);

            float AngleRad = Mathf.Atan2(_target.transform.position.y - _rb.position.y, _target.transform.position.x - _rb.position.x);
            toAngle = Mathf.Rad2Deg * AngleRad - _attachPoint.angle;
            _rb.MoveRotation(Mathf.LerpAngle(_rb.rotation, toAngle, Time.time));
        }
    }

    public void Setup(GameObject target, AttachPoint attachPoint)
    {
        _target = target;
        _attachPoint = attachPoint;
    }
}
