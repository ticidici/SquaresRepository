using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class ControllerAssigner : MonoBehaviour {

    List<InputDevice> _assignedDevices = new List<InputDevice>();
	
	void Update () {
        InputDevice device = InputManager.ActiveDevice;

        if (device.AnyButton.WasPressed)
        {
            if (device.Action2.WasPressed)
            {
                Debug.Log("Removing controller: " + device.Name);
                _assignedDevices.Remove(device);
                return;
            }
            foreach (InputDevice assignedDevice in _assignedDevices)
            {

                if (device == assignedDevice)
                {
                    Debug.Log("Already assigned device: " + device.Name);
                    return;
                } 
            }
            if (device.Action1.WasPressed)
            {
                Debug.Log("A button pressed. Adding device: " + device.Name);
                _assignedDevices.Add(device);
            }
            else
            {
                Debug.Log("Other button pressed or unknown button");
            }
        }

        
	}
}
