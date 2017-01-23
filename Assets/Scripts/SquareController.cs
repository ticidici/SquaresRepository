using UnityEngine;
using System.Collections;

public class SquareController : MonoBehaviour {

    public int _playerNumber = 1;

    //private Magnet magnet;
    private Square _squareModel;

    void Awake()
    {
        //magnet = GetComponent<Magnet>();
        _squareModel = GetComponent<Square>();
    }

    void Update()
    {
        MovementInputBindings();
        OtherInputBindings();
    }

    private void OtherInputBindings()
    {
        if (Input.GetButtonDown("Magnet 1"))
        {
            //magnet.EnableMagnet();
        }

        /*if (Input.GetButtonUp("Magnet 1"))
        {
            magnet.DisableMagnet();
        }
        */
    }

    private void MovementInputBindings()
    {
        float x = Input.GetAxis("Horizontal 1");
        float y = Input.GetAxis("Vertical 1");
        _squareModel.Move(x, y);
    }
}
