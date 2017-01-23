using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
    public float _length = 1f;
    public AttachPoint _AttachPointPrefab;

    private AttachPoint[] _attachPoints;

    private GameObject _parent;

    void Awake()
    {
        // ATTACH POINTS
        _attachPoints = new AttachPoint[4];
        _parent = transform.parent.gameObject;

        SetAttachPoints();
    }

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

    public void Detach() // Poder hi ha algun bug per no desactivar els colliders? De moment no hi ha res sospitos
    {
        foreach (AttachPoint item in _attachPoints)
        {
            item.isBusy = false;
        }
        transform.parent = gameObject.transform.root;
        _parent.SendMessage("Reset",this);
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

    // Ha de retornar si el attach s'ha pogut fer
    public void AttachTo(Square target) // TODO: Passar-ho a net
    {
        target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        
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

    void OnCollisionEnter2D(Collision2D coll)
    {
        GetComponent<Renderer>().material.color = Color.red;
        //GetComponent<SpriteRenderer>().color = Color.red;   Quan te sprite
    }
    void OnCollisionExit2D(Collision2D collisionInfo)
    {
        ResetColor();
        //GetComponent<SpriteRenderer>().color = Color.white; Quan te sprite
    }

    private void ResetColor()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
