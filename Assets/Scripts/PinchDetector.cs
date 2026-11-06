using UnityEngine;
using UnityEngine.Events;

public class PinchDetector : MonoBehaviour
{
    public OVRHand hand;
    public float maxDistance;
    public UnityEvent onPinch;

    private bool _wasActive;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!hand) hand = FindFirstObjectByType<OVRHand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hand.IsTracked) return;
        
        bool pinching = hand.GetFingerIsPinching(OVRHand.HandFinger.Index);
        bool near = Vector3.Distance(hand.PointerPose.position, transform.position) < maxDistance;
        bool active = pinching && near;
        
        if (active && !_wasActive) onPinch.Invoke();
        
        _wasActive = active;
    }
}
