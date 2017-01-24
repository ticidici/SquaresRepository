using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SquareController))]
public class Square : MonoBehaviour
{
    public float _length = 1f;
    public AttachPoint _AttachPointPrefab;
    public float _xSpeed = 7.1f;
    public float _ySpeed = 3.8f;
    public float _magnetWaitTime = 1.5f;

    private AttachPoint[] _attachPoints;

    private GameObject _firstParent;
    private bool _isMagnetEnabled = true;
    private int _id;

    public int Id                         // Test Input
    {
        set
        {
            _id = value;
        }
    }

    #region Unity Methods
    void Awake()
    {
        // ATTACH POINTS
        _attachPoints = new AttachPoint[4];
        _firstParent = transform.parent.gameObject;

        SetAttachPoints();        
    }

    void Start()
    {
        ResetColor();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        ResetColor();
    }
    #endregion

    #region Private Methods

    private void SetAttachPoints()
    {
        _attachPoints[0] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x - _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Left
        _attachPoints[1] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x + _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Right
        _attachPoints[2] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y + _length), Quaternion.identity) as AttachPoint;  // Top
        _attachPoints[3] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y - _length), Quaternion.identity) as AttachPoint;  // Bottom

        foreach (var item in _attachPoints)
        {
            item.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    // Retorna el AttachPoint mes proper al point donat
    private AttachPoint GetAttachPointClosestTo(Vector3 point)
    {
        AttachPoint closest = _attachPoints[0];
        float minimumDistance = Vector3.Distance(point,closest.transform.position);

        for (int i = 1; i < _attachPoints.Length; i++)
        {
            float currentDistance = Vector3.Distance(point, _attachPoints[i].transform.position);
            if (minimumDistance > currentDistance)
            {
                minimumDistance = currentDistance;
                closest = _attachPoints[i];
            }
        }
        return closest;
    }

    private void ResetColor()
    {
        GetComponent<Renderer>().material.color = TestManager.instance._colors[_id];
    }

    private void StartTimer()
    {
        _isMagnetEnabled = false;
        Debug.Log("Magnet disabled");
        StartCoroutine(WaitActiveTime());
    }

    private IEnumerator WaitActiveTime()
    {
        yield return new WaitForSeconds(_magnetWaitTime);
        _isMagnetEnabled = true;
        Debug.Log("Magnet enabled");
    }

    #endregion

    #region Public Methods
    public void Detach() // Poder hi ha algun bug per no desactivar els colliders? De moment no hi ha res sospitos
    {
        foreach (AttachPoint item in _attachPoints)
        {
            item.isBusy = false;
        }
        transform.parent = gameObject.transform.root;
        _firstParent.SendMessage("Reset", this);
        ResetColor();
    }

    public void AttachTo(Square target) // TODO: Passar-ho a net/Millorar
    {
        target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        ResetColor();
        target.ResetColor();
        
        transform.rotation = target.transform.parent.rotation;

        Vector3 distanceBetweenSquares = target.gameObject.transform.position - transform.position;
        Vector3 dir = distanceBetweenSquares.normalized;

        Vector3 initPoint = transform.position + dir * _length;
        Vector3 finalPoint = target.gameObject.transform.position - dir * _length;

        AttachPoint a = GetAttachPointClosestTo(initPoint);
        a.isBusy = true;
        AttachPoint b = target.GetAttachPointClosestTo(finalPoint);
        b.isBusy = true;
        transform.position = b.transform.position;

        target.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public float GetDistance(Square target)
    {
        return Vector3.Distance(target.transform.position, transform.position);        
    }

    public void Move(float x, float y)
    {
        Vector2 velocityVector = new Vector2(x, y);
        velocityVector.Normalize();
        velocityVector.x *= Time.deltaTime * _xSpeed;
        velocityVector.y *= Time.deltaTime * _ySpeed;
        //Debug.Log(""+velocityVector.x + "  " + velocityVector.y);
        GetComponentInParent<SuperSquare>().MovementInput(this, velocityVector);//Canviar la manera de pillar referencia
    }

    public void UseMagnet()
    {
        if (_isMagnetEnabled)
        {
            GetComponentInParent<SuperSquare>().MagnetInput(this);//Canviar la manera de pillar referencia
            StartTimer();
        }
    }
    #endregion
}
