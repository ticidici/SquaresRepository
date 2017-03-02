using UnityEngine;
using System.Collections;

public class ScorePoints : MonoBehaviour {

    public int _pointsWorth = 50;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Polygon polygon = collider.GetComponent<Polygon>();
        if (polygon)
        {
            polygon.ReceivePoints(_pointsWorth);
            gameObject.SetActive(false);
        }
    }
}
