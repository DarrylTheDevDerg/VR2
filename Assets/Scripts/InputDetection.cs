using UnityEngine;

public class InputDetection : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float trigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float grip = OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger);

        Vector2 leftStick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
        Vector2 rightStick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        if (OVRInput.GetDown(OVRInput.RawButton.A)) print("A Button pressed.");

        if (OVRInput.Get(OVRInput.RawButton.B)) print("B Button held.");

        if (OVRInput.GetUp(OVRInput.RawButton.X)) print("X button released.");

        if (trigger > 0) print("Trigger input detected.");

        if (grip > 0) print("Grip input detected.");

        if (leftStick.magnitude > 0) print("Left thumbstick movement detected.");

        if (rightStick.magnitude > 0) print("Right thumbstick movement detected.");
    }
}
