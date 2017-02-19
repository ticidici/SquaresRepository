using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Classe temporal per fer les proves
public class TestManager : MonoBehaviour {

    private GameObject _squarePolygon;

    public Color[] _colors = new Color[4] { new Color(242f / 255, 95 / 255f, 92f / 255, 1), new Color(36f / 255, 123f / 255, 160f / 255, 1),
                                            new Color(244f / 255, 162f / 255, 97f / 255, 1), new Color(255f / 255, 224f / 255, 102f / 255, 1) };

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

        _squarePolygon = Resources.Load("Prefab/Square", typeof(GameObject)) as GameObject;

        for (int i = 0; i < _spawnPoints.Count; i++)        
            PoolManager.SpawnObject(_squarePolygon.gameObject,_spawnPoints[i].transform.position,Quaternion.identity);

        // Destroy SpawnPoint
        foreach (SpawnPoint item in _spawnPoints)
        {
            item.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}

    void Reset()
    {
        var tempList = transform.Cast<Transform>().ToList();
        foreach (var item in tempList)
        {
            if(item.GetComponent<SpawnPoint>())
                DestroyImmediate(item.gameObject);
        }
    }
}