using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public MoveUp _obvs;
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

        //Instantiate(_obvs, new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f), Quaternion.identity);

        //GameObject prefab = stuffPrefabs[Random.Range(0, stuffPrefabs.Length)];
        MoveUp spawn = _obvs.GetPooledInstance<MoveUp>();
        spawn.transform.position = new Vector3(Random.Range(-9, 9), transform.position.y - 25, 0.5f);
         /*spawn.transform.localPosition = transform.position;
         spawn.transform.localScale = Vector3.one * scale.RandomInRange;
         spawn.transform.localRotation = Random.rotation;

         spawn.Body.velocity = transform.up * velocity +
             Random.onUnitSphere * randomVelocity.RandomInRange;
         spawn.Body.angularVelocity =
             Random.onUnitSphere * angularVelocity.RandomInRange;

         spawn.SetMaterial(stuffMaterial);*/
    }
}

