using UnityEngine;

public class SquareController : MonoBehaviour {

    [Range(1, 4)]
    public int _playerNumber = 1;

    //private Magnet magnet;
    private Square _squareModel;
    private string _horizontalAxis = "Horizontal ";
    private string _verticalAxis = "Vertical ";
    private string _magnetButton = "Magnet ";

    void Awake()
    {
        //magnet = GetComponent<Magnet>();
        _squareModel = GetComponent<Square>();
        if (!_squareModel) { Debug.LogError("No Square script found!", this); }
    }

    void Start()
    {
        switch (_playerNumber)
        {
            case 1:
                _horizontalAxis = string.Concat(_horizontalAxis, "1");
                _verticalAxis = string.Concat(_verticalAxis, "1");
                _magnetButton = string.Concat(_magnetButton, "1");
                break;
            case 2:
                _horizontalAxis = string.Concat(_horizontalAxis, "2");
                _verticalAxis = string.Concat(_verticalAxis, "2");
                _magnetButton = string.Concat(_magnetButton, "2");
                break;
            case 3:
                _horizontalAxis = string.Concat(_horizontalAxis, "3");
                _verticalAxis = string.Concat(_verticalAxis, "3");
                _magnetButton = string.Concat(_magnetButton, "3");
                break;
            case 4:
                _horizontalAxis = string.Concat(_horizontalAxis, "4");
                _verticalAxis = string.Concat(_verticalAxis, "4");
                _magnetButton = string.Concat(_magnetButton, "4");
                break;
        }

    }

    void Update()
    {
        MovementInputBindings();
        OtherInputBindings();
    }

    private void OtherInputBindings()
    {
        if (Input.GetButtonDown(_magnetButton))
        {
            _squareModel.UseMagnet();
            //Debug.Log("magnet");
        }
    }

    private void MovementInputBindings()
    {
        float x = Input.GetAxis(_horizontalAxis);
        float y = Input.GetAxis(_verticalAxis);
        _squareModel.Move(x, y);
    }
}
