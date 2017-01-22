using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Square : MonoBehaviour
{
    public float _length = 1f;
    public AttachPoint _AttachPointPrefab;

    private List<Vector2> _vertices;
    private AttachPoint[] _attachPoints;

    private GameObject _parent;

    void Awake()
    {
        // ATTACH POINTS
        _attachPoints = new AttachPoint[4];
        _vertices = new List<Vector2>();
        _parent = transform.parent.gameObject;

        SetAttachPoints();
        UpdateVertices();
    }

    void Start()
    {        
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

    private void UpdateVertices()
    {
        float halfLength = _length / 2;

        _vertices.Add(new Vector2(transform.position.x - halfLength, transform.position.y + halfLength));     // Top-Left
        _vertices.Add(new Vector2(transform.position.x + halfLength, transform.position.y + halfLength));     // Top-Right
        _vertices.Add(new Vector2(transform.position.x + halfLength, transform.position.y - halfLength));     // Bottom-Right
        _vertices.Add(new Vector2(transform.position.x - halfLength, transform.position.y - halfLength));     // Bottom-Left
    }

    public Vector2[] GetVertices()
    {
        return _vertices.ToArray();
    }

    public void Detach()
    {
        //_parent.gameObject.SetActive(true);
        //GetComponent<BoxCollider2D>().enabled = false;
        transform.parent = gameObject.transform.root;
        Debug.Log(_parent.name);
        _parent.SendMessage("Reset");
        

    }

    public void AttachTo(Square target) // Tenir en compte les rotacions?
    {
        target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        Vector3 distanceBetweenSquares = target.gameObject.transform.position - transform.position;     

        if (Mathf.Abs(distanceBetweenSquares.x) < Mathf.Abs(distanceBetweenSquares.y))
        {
            transform.position += new Vector3(distanceBetweenSquares.x, 0, 0);
            if (distanceBetweenSquares.y < 0)
                transform.position += new Vector3(0, _length + distanceBetweenSquares.y, 0); // TOP
            else
                transform.position += new Vector3(0, -_length + distanceBetweenSquares.y, 0); // BOTTOM
        }
        else
        {
            transform.position += new Vector3(0, distanceBetweenSquares.y, 0);
            if (distanceBetweenSquares.x < 0)
                transform.position += new Vector3(_length + distanceBetweenSquares.x, 0, 0);  // RIGHT
            else
                transform.position += new Vector3(-_length + distanceBetweenSquares.x, 0, 0); // LEFT
        }

        target.UpdateVertices();
        UpdateVertices();

        target.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public float GetDistance(Square target)
    {
        return Vector3.Magnitude(target.transform.position - transform.position);
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
