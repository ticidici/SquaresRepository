using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {

    public SuperSquare _SuperSquarePrefab;
    public Color[] _colors;

    public static TestManager instance = null;

    void Awake()
    {
        Cursor.visible = false;

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        for (int i = 0; i < 4; i++)
        {
            SuperSquare aux = Instantiate(_SuperSquarePrefab, new Vector2(Random.Range(-6, 6), Random.Range(-3.5f, 3.5f)), Quaternion.identity) as SuperSquare;
            aux.Id = i;
            //aux.GetComponent<ControllerPlayer>().enabled = false;            
            int auxTag = i + 1;
            aux.name = "Player " + auxTag;
            aux.tag = "Player"+ auxTag;
            aux.transform.parent = GameObject.Find("Test1").transform;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
