using UnityEngine;

public class ControllerTracking : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 leftPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion leftRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        Vector3 rightPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rightRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
    }
}
