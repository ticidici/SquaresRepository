using UnityEngine;
using System.Collections;

public class AttachPoint : MonoBehaviour
{
    private Square _square;

    public Square Square
    {
        get
        {
            return _square;
        }
        set
        {
            _square = value;
        }
    }

    void Awake()
    {
        _square = null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y), .05f);
    }
}
