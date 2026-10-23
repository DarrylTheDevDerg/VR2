using UnityEngine;
using UnityEngine.Events;

public class OvenKnob : MonoBehaviour
{
    public enum Axis {X,Y,Z}
    public Axis axis;
    public float minAngle = 0f, maxAngle = 270f;

    public int steps = 0;
    
    public UnityEvent<float> onValueChange;
    public float Value;

    private float _lValue = -1f;

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
        Vector3 eulers = transform.localEulerAngles;
        float raw = axis == Axis.X ? eulers.x : axis == Axis.Y ? eulers.y : eulers.z;
        
        return Mathf.DeltaAngle(0f, raw);
    }
}
