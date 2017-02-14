using UnityEngine;

public class SquareController : MonoBehaviour {

    [Range(1, 4)]
    public int _playerNumber = 1;

    //private Magnet magnet;
    private IControllable _polygonModel; // Canviar nom
    private string _horizontalAxis = "Horizontal ";
    private string _verticalAxis = "Vertical ";
    private string _magnetButton = "Magnet ";

    void Awake()
    {
        //magnet = GetComponent<Magnet>();
        _polygonModel = GetComponent<IControllable>();
        if (_polygonModel == null) { Debug.LogError("No IControllable script found!", this); }
    }

    void Start()
    {       

    }
    
    void Update()
    {
        MovementInputBindings();
        OtherInputBindings();
    }

    public void SetPlayerController(int id)
    {
        switch (id)
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
    
    private void OtherInputBindings()
    {
        if (Input.GetButtonDown(_magnetButton))
        {
            _polygonModel.UseMagnet();
            //Debug.Log("magnet");
        } else if(Input.GetKeyDown(KeyCode.Space))
        {
            transform.parent.GetComponent<Rigidbody2D>().AddForce(new Vector2(0,10), ForceMode2D.Impulse);
            //transform.parent.GetComponent<Rigidbody2D>().MovePosition(new Vector2(0, 10), ForceMode2D.Impulse);
        }
    }

    private void MovementInputBindings()
    {
        float x = Input.GetAxis(_horizontalAxis);
        float y = Input.GetAxis(_verticalAxis);
        _polygonModel.Move(x, y);
    }
}
