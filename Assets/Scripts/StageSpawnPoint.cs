using UnityEngine;
using System.Collections;

public class StageSpawnPoint : MonoBehaviour {

    public void RespawnGameObject(GameObject objectToRespawn)
    {
        objectToRespawn.transform.position = transform.position;
        objectToRespawn.SetActive(true);
    }
}
