using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour
{
    public float _length = 1f;
    public AttachPoint _AttachPointPrefab;
    public float xSpeed = 7.1f;
    public float ySpeed = 3.8f;

    private Vector2[] _vertices;
    private AttachPoint[] _attachPoints;

    

    void Awake()
    {
        // ATTACH POINTS
        _attachPoints = new AttachPoint[4];
        _vertices = new Vector2[4];
    }

    void Start(){
        SetAttachPoints();
        UpdateVertices();
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
        
        _vertices[0] = new Vector2(transform.position.x - halfLength, transform.position.y + halfLength);     // Top-Left
        _vertices[1] = new Vector2(transform.position.x + halfLength, transform.position.y + halfLength);     // Top-Right
        _vertices[2] = new Vector2(transform.position.x + halfLength, transform.position.y - halfLength);     // Bottom-Right
        _vertices[3] = new Vector2(transform.position.x - halfLength, transform.position.y - halfLength);     // Bottom-Left
    }

    public Vector2[] GetVertices()
    {
        return _vertices;
    }

    public void AttachSquare(Square target) // Venen a mi
    {
        target.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        Debug.Log("BEGIN " + target.gameObject.name + "   " + target.gameObject.transform.position);
        Vector3 aux = transform.position - target.gameObject.transform.position;
        if (Mathf.Abs(aux.x) < Mathf.Abs(aux.y))
        {
            target.gameObject.transform.position += new Vector3(aux.x, 0, 0);
            if (aux.y < 0)
                target.gameObject.transform.position += new Vector3(0, _length + aux.y, 0); // TOP
            else
                target.gameObject.transform.position += new Vector3(0, -_length + aux.y, 0); // BOTTOM
        }
        else
        {
            target.gameObject.transform.position += new Vector3(0, aux.y, 0);
            if (aux.x < 0)
                target.gameObject.transform.position += new Vector3(_length + aux.x, 0, 0);  // RIGHT
            else
                target.gameObject.transform.position += new Vector3(-_length + aux.x, 0, 0); // LEFT
        }
        Debug.Log("END " + target.gameObject.name + "   " + target.gameObject.transform.position);
        target.UpdateVertices();
        UpdateVertices();

        target.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public float GetDistance(Square target)
    {
        return Vector3.Magnitude(target.transform.position - transform.position);
    }

    public void Move(float x, float y)
    {
        Vector2 velocityVector = new Vector2(x, y);
        velocityVector.Normalize();
        velocityVector.x *= Time.deltaTime * xSpeed;
        velocityVector.y *= Time.deltaTime * ySpeed;
        Debug.Log(""+velocityVector.x + "  " + velocityVector.y);
        GetComponentInParent<SuperSquare>().MovementInput(this, velocityVector);//Canviar la manera de pillar referencia
    }
}
