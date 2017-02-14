using UnityEngine;
using System.Collections;

// Classe temporal per fer les proves
public class TestManager : MonoBehaviour {
    public GameObject _squarePolygon;
    public Color[] _colors;

    [Range(1,4)]
    public int _numPlayers = 1;
    public SpawnPoint[] _spawnPoints;

    public static TestManager instance = null;

    void Awake()
    {
        //Cursor.visible = false;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //PoolManager.WarmPool(_squarePolygon.gameObject, _numPlayers);

        for (int i = 0; i < _numPlayers; i++)        
            PoolManager.SpawnObject(_squarePolygon.gameObject,_spawnPoints[i].transform.position,Quaternion.identity);

        // Destroy SpawnPoint
        for (int i = 0; i < _numPlayers; i++)
            Destroy(_spawnPoints[i].gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
