using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public MoveUp _obvs;

    [Range(0.05f,1f)]
    public float _timebetweenSpawns = 1f;
    private float _timeSinceLastSpawn;

    void FixedUpdate()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn >= _timebetweenSpawns)
        {
            _timeSinceLastSpawn -= _timebetweenSpawns;
            SpawnStuff();
        }
    }
    
    void SpawnStuff()
    {
        //MoveUp spawn = _obvs.GetPooledInstance<MoveUp>();
        //spawn.transform.position = new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f);
    }
}

