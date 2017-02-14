using UnityEngine;
using System.Collections;

public class AttachPoint : MonoBehaviour
{
    // De moment no es d'us
    private Polygon Square { get; set; }

    //  Nomes interesa saber si esta ocupar (de moment)
    public bool isBusy = false;

    void Awake()
    {
        Square = null;
    }

    void OnDrawGizmos()
    {
        if(isBusy)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y), .05f);
    }

    // On Collider es fica en busy
    // Nomes respon a altres instancies que tinguin la classe shape
    // Aquesta classe pot ser un quadrat, un triangle etc
}
