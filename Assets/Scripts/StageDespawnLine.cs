using UnityEngine;
using System.Collections;

public class StageDespawnLine : MonoBehaviour {

    private StageSpawnPoint _spawnPoint;

	// Use this for initialization
	void Awake () {
        _spawnPoint = transform.parent.GetComponentInChildren<StageSpawnPoint>();
        if (!_spawnPoint) { Debug.LogError("Falta un spawn point compartint pare amb la despawn line", this); }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Entering Trigger");
        GameObject incomingObject = collider.gameObject;
        incomingObject.SetActive(false);
        _spawnPoint.RespawnGameObject(incomingObject);
    }
}
