using UnityEngine;
using System.Collections;

public class Shredder : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Tampoc hauria de matar el walls, ja ho faré
        Destroy(collider.gameObject);//Segons quines coses s'hauria de fer object pooling
    }
}
