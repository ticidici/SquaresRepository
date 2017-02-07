using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {

    public SuperSquare _SuperSquarePrefab;
    public Color[] _colors;

    [Range(1,4)]
    public int _numPlayers = 1;

    public static TestManager instance = null;

    void Awake()
    {
        Cursor.visible = false;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        for (int i = 0; i < _numPlayers; i++)
        {
            SuperSquare aux = Instantiate(_SuperSquarePrefab, new Vector2(Random.Range(-9, 9), Random.Range(-2f, 2f)), Quaternion.identity) as SuperSquare;
            aux.Id = i;           
            int auxTag = i + 1;
            aux.name = "Player " + auxTag;
            aux.tag = "Player"+ auxTag;
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
