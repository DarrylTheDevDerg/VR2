using UnityEngine;

public class AntiSoftlock : MonoBehaviour
{
    private Vector3 _rotation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.eulerAngles != _rotation) 
            transform.rotation = Quaternion.Euler(_rotation);
    }
}
