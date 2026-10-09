using UnityEngine;

public class ProximidadControl : MonoBehaviour
{
    public Light luz;
    public float distancia = 0.2f;
    private OVRCameraRig rig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rig = FindFirstObjectByType<OVRCameraRig>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rig == null || luz == null) return;

        Vector3 localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        Vector3 worldPosition = rig.trackingSpace.TransformPoint(localPosition);

        luz.enabled = Vector3.Distance(worldPosition, transform.position) < distancia;
    }
}
