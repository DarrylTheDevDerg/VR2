using UnityEngine; 
using UnityEngine.Events;

public class CuttingSimulation : MonoBehaviour
{
    public Transform knife;
    public string target = "Food";
    public Collider contact;
    
    public Vector3 edgeDirection = Vector3.down;

    public float minCutSpeed = 1f, minEdgeAlignment = 0.5f, cooldown = 0.2f, velocitySmoothing = 0.5f;
    
    public UnityEvent<GameObject> onCutting;

    private bool _shouldDestroy = true;
    private Vector3 _lastPosition;
    private Vector3 _velocity;
    private float _lastCutTime = -999f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (knife == null) knife = transform;
        _lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rawVelocity = (knife.position - _lastPosition) / Time.fixedDeltaTime;
        _velocity = Vector3.Lerp(_velocity, rawVelocity, velocitySmoothing);
        _lastPosition = knife.position;
    }

    void OnTriggerEnter(Collider other) => TryCut(other);
    void OnTriggerStay(Collider other) => TryCut(other);

    void TryCut(Collider other)
    {
        if (!other.CompareTag(target)) return;
        
        if (Time.time - _lastCutTime < cooldown) return;
        
        float s = _velocity.magnitude;
        if (s < minCutSpeed) return;
        
        Vector3 edgeInWorld = knife.TransformDirection(edgeDirection).normalized;
        float align = Vector3.Dot(_velocity.normalized, edgeInWorld);
        
        if (align < minEdgeAlignment) return;
        
        _lastCutTime = Time.time;
        Cut(other.gameObject);
    }

    void Cut(GameObject fud)
    {
        onCutting?.Invoke(fud);
        
        if (_shouldDestroy) Destroy(fud);
    }
}
