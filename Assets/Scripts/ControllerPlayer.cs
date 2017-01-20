using UnityEngine;
using System.Collections;

public class ControllerPlayer : MonoBehaviour {

    public float speed = 0.1f;
	
	// Update is called once per frame
	void Update () {
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;       
    }
}
