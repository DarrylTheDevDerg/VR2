using UnityEngine;
using UnityEngine.Events;

public class OvenKnob : MonoBehaviour
{
    public enum Axis {X,Y,Z}
    public Axis axis;
    public float minAngle = 0f, maxAngle = 270f;
    public bool invert;

    public int steps = 0;
    
    public UnityEvent<float> onValueChange;
    public float Value;

    private float _lValue = -1f;
    private Quaternion _rest;

    void Start()
    {
        _rest = transform.localRotation;
    }
    
    // Update is called once per frame
    void Update()
    {
        StepCalculation();
    }

    void StepCalculation()
    {
        float ang = GetLocalAngle();
        float t = Mathf.InverseLerp(minAngle, maxAngle, ang);
        
        if (steps > 0) t = Mathf.Round(t * steps) / steps;
        
        Value = t;

        if (!Mathf.Approximately(t, _lValue))
        {
            _lValue = t;
            onValueChange?.Invoke(t);
        }
    }

    float GetLocalAngle()
    {
        Vector3 axisVec, refVec;

        switch (axis)
        {
            case Axis.X: axisVec = Vector3.right; refVec = Vector3.up; break;
            case Axis.Y: axisVec = Vector3.up; refVec = Vector3.forward; break;
            default: axisVec = Vector3.forward; refVec = Vector3.right; break;
        }
        
        Quaternion delta = Quaternion.Inverse(_rest) * transform.localRotation;
        Vector3 rotated = delta * refVec;
        
        float signed = Vector3.SignedAngle(refVec, rotated, axisVec);
        if (invert) signed = -signed;
        
        float mid = (minAngle + maxAngle) * 0.5f;
        return mid + Mathf.DeltaAngle(mid, signed);
    }
}
