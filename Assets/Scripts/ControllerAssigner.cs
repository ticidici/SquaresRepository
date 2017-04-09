using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerAssigner : MonoBehaviour {

    public GameObject _previewPolygonPrefab;

    private List<InputDevice> _assignedDevices = new List<InputDevice>();
    private List<PreviewPolygon> _previewPolygons = new List<PreviewPolygon>();
    private List<Color> _colorsToAssign = new List<Color>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _colorsToAssign.Add(Color.red);
        _colorsToAssign.Add(Color.blue);
        _colorsToAssign.Add(Color.green);
        _colorsToAssign.Add(Color.yellow);
        _colorsToAssign.Add(Color.cyan);
    }

	void Update () {
        //InputDevice device = InputManager.ActiveDevice;
        foreach (InputDevice device in InputManager.Devices)
        {
            if (device.AnyButton.WasPressed)
            {
                foreach (InputDevice assignedDevice in _assignedDevices)
                {
                    if (device == assignedDevice)
                    {
                        int index = _assignedDevices.IndexOf(device);
                        PreviewPolygon polygon = _previewPolygons[index];
                        if (device.Action2.WasPressed)
                        {

                            _colorsToAssign.Add(polygon.ReturnColor());
                            _previewPolygons.Remove(polygon);
                            Destroy(polygon.gameObject);
                            Debug.Log("Removing controller: " + device.Name);
                            _assignedDevices.Remove(device);
                            return;
                        }
                        if (device.Action3.WasPressed)
                        {
                            _colorsToAssign.Add(polygon.ReturnColor());
                            polygon.AssignColor(GetNewPreviewColor());
                        }
                        if (device.Action4.WasPressed)
                        {
                            polygon.ChangeShape();
                        }
                        Debug.Log("Already assigned device: " + device.Name);
                        return;
                    }
                }
                if (device.Action1.WasPressed)
                {
                    Debug.Log("A button pressed. Adding device: " + device.Name);
                    _assignedDevices.Add(device);

                    GameObject polygon = Instantiate(_previewPolygonPrefab);
                    PreviewPolygon script = polygon.GetComponent<PreviewPolygon>();
                    script.AssignController(device);
                    script.AssignColor(GetNewPreviewColor());
                    _previewPolygons.Add(script);
                }
                else
                {
                    Debug.Log("Other button pressed on unassigned device or unknown button");
                }
            }
        }  
	}

    public InputDevice GetDevice(int playerId)
    {
        if (playerId <= _assignedDevices.Count)
        {
            return _assignedDevices[playerId - 1];
        }
        else
        {
            Debug.LogWarning("There's more players than controllers.");
            return null;
        }
    }

    private Color GetNewPreviewColor()
    {
        Color color = _colorsToAssign[0];
        _colorsToAssign.RemoveAt(0);
        return color;
    }
}
