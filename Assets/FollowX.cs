using UnityEngine;
using System.Collections;

public class FollowX : MonoBehaviour {
        
    public LayerMask _layerMaskToSearch;

    // Update is called once per frame
    void FixedUpdate() {
        Collider2D[] found = Physics2D.OverlapCircleAll(transform.position, 300, _layerMaskToSearch);        

        foreach (var item in found)
        {
            float dir = transform.position.y - item.transform.position.y;// / item.transform.position.y + transform.position.y;
            item.GetComponent<Rigidbody2D>().AddForce(new Vector3(0,dir,0));
        }
    }
}
