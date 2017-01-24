using UnityEngine;
using System.Collections;

public class TestManager : MonoBehaviour {

    public SuperSquare _SuperSquarePrefab;
    public Color[] _colors;    

    void Awake()
    {
        Cursor.visible = false;
        int i = 0;
        Debug.Log(Input.GetJoystickNames().Length);
       // foreach (var player in Input.GetJoystickNames())
      //  {
           // Debug.Log(player);
            SuperSquare aux = Instantiate(_SuperSquarePrefab, new Vector2(Random.Range(-6,6), Random.Range(-3.5f, 3.5f)), Quaternion.identity) as SuperSquare;
            aux.name = "P1";
            aux.IdPlayer = 0;
            aux.SetColor(_colors[0]);
            aux.transform.parent = GameObject.Find("Test1").transform;
     //   }

        for (i = 1; i < 4; i++)
        {
            aux = Instantiate(_SuperSquarePrefab, new Vector2(Random.Range(-6, 6), Random.Range(-3.5f, 3.5f)), Quaternion.identity) as SuperSquare;
            aux.IdPlayer = i;
            aux.SetColor(_colors[i]);
            aux.GetComponent<ControllerPlayer>().enabled = false;            
            int auxTag = i + 1;
            aux.name = "P" + auxTag;
            aux.tag = "Player"+ auxTag;
            aux.transform.parent = GameObject.Find("Test1").transform;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
