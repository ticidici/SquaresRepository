using UnityEngine;
using InControl;

public class SquareController : MonoBehaviour {

    [Range(1, 4)]
    public int _playerNumber = 1;
    public int _deviceIndex;//no hacer esto, tener una referencia a InputDevice

    private IControllable _polygonModel; // Canviar nom (tots sabem que aquest comentari mai es borrarà ni es farà res al respecte)
    private string _horizontalAxis = "Horizontal ";
    private string _verticalAxis = "Vertical ";
    private string _magnetButton = "Magnet ";

    void Awake()
    {
        _polygonModel = GetComponent<IControllable>();
        if (_polygonModel == null) { Debug.LogError("No IControllable script found!", this); }
    }
    
    void Update()
    {
        MovementInputBindings();
        OtherInputBindings();
    }

    public void SetPlayerController(int id)
    {
        _playerNumber = id;
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
        //if (Input.GetButtonDown(_magnetButton))
        if(InputManager.Devices[_deviceIndex].Action1)
        {
            _polygonModel.UseMagnet();
        }
    }

    private void MovementInputBindings()
    {
        float x = 0;
        float y = 0;
        //x = Input.GetAxis(_horizontalAxis);
        //y = Input.GetAxis(_verticalAxis);
        x = InputManager.Devices[_deviceIndex].LeftStickX;
        y = InputManager.Devices[_deviceIndex].LeftStickY;
 
        x = InputManager.ActiveDevice.LeftStickX;
        y = InputManager.ActiveDevice.LeftStickY;

        _polygonModel.Move(x, y);
    }
}
