using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public GameObject _obvs;
    public float _waitTimebetweenSpawns = 1f;

	// Use this for initialization
	void Start () {
        InvokeRepeating("StartTimer", 2.0f,0.3f);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    // Cooldown del magnet
    private void StartTimer()
    {
        Instantiate(_obvs, new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f), Quaternion.identity);
        //StartCoroutine(WaitActiveTime());
    }

    private IEnumerator WaitActiveTime()
    {
        Instantiate(_obvs, new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f), Quaternion.identity);
        yield return new WaitForSeconds(_waitTimebetweenSpawns);
    }
}
