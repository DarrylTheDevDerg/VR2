using UnityEngine;

public class VRMenu : MonoBehaviour
{
    public Transform head;
    public float distance, followSpeed, yOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!head) return;
        
        Vector3 target = head.position + head.forward * distance;
        target.y += yOffset;
        
        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
        
        Quaternion targetRot = Quaternion.LookRotation(transform.position - head.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, followSpeed * Time.deltaTime);
    }
}
