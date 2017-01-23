using UnityEngine;
using System.Collections;

public class AttachPoint : MonoBehaviour
{
    // De moment no es d'us
    private Square _square;

    //  Nomes interesa saber si esta ocupar (de moment)
    public bool isBusy = false;

    // De moment no es d'us
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
        if(isBusy)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y), .05f);
    }
}
