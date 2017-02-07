using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject _obvs;
    public float _waitTimebetweenSpawns = 1f;

    private GameObject _backgroundSquares;

	// Use this for initialization
	void Start () {
        _backgroundSquares = new GameObject("Test Background");
        InvokeRepeating("StartTimer", 2.0f,0.3f);
    }

    // Cooldown del magnet
    private void StartTimer()
    {
        GameObject backgroundSquare = Instantiate(_obvs, new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f), Quaternion.identity) as GameObject;
        backgroundSquare.transform.SetParent(_backgroundSquares.transform);
    }
}
