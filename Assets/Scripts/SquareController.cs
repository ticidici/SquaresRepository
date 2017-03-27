using UnityEngine;
using InControl;

public class SquareController : MonoBehaviour {

    [Range(1, 4)]
    public int _playerNumber = 1;

    private ControllerAssigner _controllerAssigner;
    private InputDevice _thisController;
    private IControllable _polygonModel; // Canviar nom (tots sabem que aquest comentari mai es borrarà ni es farà res al respecte)

    void Awake()
    {
        _polygonModel = GetComponent<IControllable>();
        if (_polygonModel == null) { Debug.LogError("No IControllable script found!", this); }
    }
    
    void Update()
    {
        if (_thisController == null) { return; }
        MovementInputBindings();
        OtherInputBindings();
    }

    public void SetPlayerController(int id)
    {
        _controllerAssigner = FindObjectOfType<ControllerAssigner>();
        if (_controllerAssigner == null) { Debug.LogError("No Controller assigner found!"); }
        _playerNumber = id;
        _thisController = _controllerAssigner.GetDevice(id);
    }
    
    private void OtherInputBindings()
    {
        if(_thisController.Action1)
        {
            _polygonModel.UseMagnet();
        }
    }

    private void MovementInputBindings()
    {
        float x = 0;
        float y = 0;
 
        x = _thisController.LeftStickX;
        y = _thisController.LeftStickY;

        _polygonModel.Move(x, y);
    }
}
