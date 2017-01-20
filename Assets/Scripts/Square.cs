using UnityEngine;
using System.Collections;

public class Square : MonoBehaviour
{


    public float _length = 1f;
    public AttachPoint _AttachPointPrefab;

    private AttachPoint[] _attachPoints;
    private Vector2[] _vertex;

    void Awake()
    {
        // ATTACH POINTS
        _attachPoints = new AttachPoint[4];

        _attachPoints[0] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x - _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Left
        _attachPoints[1] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x + _length, transform.position.y), Quaternion.identity) as AttachPoint;  // Right
        _attachPoints[2] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y + _length), Quaternion.identity) as AttachPoint;  // Top
        _attachPoints[3] = Instantiate(_AttachPointPrefab, new Vector2(transform.position.x, transform.position.y - _length), Quaternion.identity) as AttachPoint;  // Bottom

        foreach (var item in _attachPoints)
        {
            item.gameObject.transform.SetParent(gameObject.transform);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    public Vector2[] GetVertex()
    {
        if (_vertex == null)
            _vertex = new Vector2[4];

        float halfLength = _length / 2;

        _vertex[0] = new Vector2(transform.position.x - halfLength, transform.position.y + halfLength);     // Top-Left
        _vertex[1] = new Vector2(transform.position.x + halfLength, transform.position.y + halfLength);     // Top-Right
        _vertex[2] = new Vector2(transform.position.x + halfLength, transform.position.y - halfLength);     // Bottom-Right
        _vertex[3] = new Vector2(transform.position.x - halfLength, transform.position.y - halfLength);     // Bottom-Left        

        return _vertex;
    }

    public void AttachSquares(Square target) // Venen a mi
    {
        Debug.Log(target.transform.name);
        Vector3 aux = transform.position - target.gameObject.transform.position;//canviar nom de aux (prova per pull request)
        //Debug.Log(aux);
        if (Mathf.Abs(aux.x) < Mathf.Abs(aux.y))
        {
            target.gameObject.transform.position += new Vector3(aux.x, 0, 0);
            if (aux.y < 0)
                target.gameObject.transform.position += new Vector3(0, _length + aux.y, 0);
            else
                target.gameObject.transform.position += new Vector3(0, -_length + aux.y, 0);
        }
        else
        {
            target.gameObject.transform.position += new Vector3(0, aux.y, 0);
            if (aux.x < 0)
                target.gameObject.transform.position += new Vector3(_length + aux.x, 0, 0);
            else
                target.gameObject.transform.position += new Vector3(-_length + aux.x, 0, 0);
        }
    }

    public float GetDistance(Square target)
    {
        return Vector3.Magnitude(target.transform.position - transform.position);
    }
}
