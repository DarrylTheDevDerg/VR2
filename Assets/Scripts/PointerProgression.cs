using UnityEngine;
using UnityEngine.Rendering;

public class PointerProgression : MonoBehaviour
{
    [Header("Circle")]
    [ColorUsage(true, true)]
    public Color circleColor = Color.red;

    public bool autoRadius = true;
    
    public float radiusPadding, manualRadius;
    public float ringThickness, groundOffset;

    public float pulseSpeed, fadeSpeed;
    
    [Header("Arrow"), ColorUsage(true, true)]
    public Color arrowColor = Color.red;

    public float arrowLength, heightHover;
    public bool beBillboard = true;
    
    public float bounceSpeed, bounceLength;
    
    public Material circleOverride, arrowOverride;
    
    private Transform _root, _circle, _arrow;
    private Material _cMat, _aMat;
    private Mesh _cMesh, _aMesh;
    
    private float _r, _bOffset, _tOffset;
    private Vector2 _cOffset;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void MeasureTarget()
    {
        Renderer r = GetComponent<Renderer>();
        Bounds b;
        
        if (r) b = r.bounds;
        else b = new Bounds(transform.position + Vector3.up * 0.5f, Vector3.one);
        
        Vector3 p = transform.position;
        _bOffset = b.min.y - p.y;
        _tOffset = b.max.y - p.y;
        _cOffset = new Vector2(b.center.x - p.x, b.center.z - p.z);
        
        _r = autoRadius ? Mathf.Max(b.extents.x, b.extents.z) + radiusPadding : manualRadius;
    }

    private void BuildMarker()
    {
        _root = new GameObject($"{name} + Marker").transform;
    }

    private Transform CreatePart(string partName, Mesh mesh, Color color, Material oMaterial, out Material matInstance)
    {
        var go = new GameObject(partName);
        go.transform.SetParent(_root, false);
        
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        
        var mr = go.AddComponent<MeshRenderer>();
        mr.shadowCastingMode = ShadowCastingMode.Off;
        mr.receiveShadows = false;
        
        matInstance = oMaterial != null ? new Material(oMaterial) : new Material(Shader.Find("Sprites/Default"));
        
        return go.transform;
    }
}
