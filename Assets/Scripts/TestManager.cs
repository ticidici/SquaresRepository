using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Classe temporal per fer les proves
public class TestManager : MonoBehaviour {
    public GameObject _squarePolygon;
    public Color[] _colors;

    public List<SpawnPoint> _spawnPoints = new List<SpawnPoint>(1);

    public static TestManager instance = null;

    public void AddNew()
    {
        SpawnPoint aux = Instantiate(Resources.Load("SpawnPoint", typeof(SpawnPoint)) as SpawnPoint, Vector3.zero, Quaternion.identity) as SpawnPoint;
        aux.transform.parent = transform;
        _spawnPoints.Add(aux);
    }

    public void RemoveLast()
    {
        DestroyImmediate(_spawnPoints[_spawnPoints.Count - 1].gameObject);
        _spawnPoints.RemoveAt(_spawnPoints.Count-1);
    }

    void Awake()
    {
        //Cursor.visible = false;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        for (int i = 0; i < _spawnPoints.Count; i++)        
            PoolManager.SpawnObject(_squarePolygon.gameObject,_spawnPoints[i].transform.position,Quaternion.identity);

        // Destroy SpawnPoint
        foreach (SpawnPoint item in _spawnPoints)
        {
            //item.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}