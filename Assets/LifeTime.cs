using UnityEngine;
using System.Collections;

public class LifeTime : MonoBehaviour {
	void Start () {
        Destroy(gameObject,1f);
	}
}
