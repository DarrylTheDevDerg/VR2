using UnityEngine;

public class VisualPointer : MonoBehaviour
{
    public GameObject circle, arrow;
    private Bounds _b;

    Bounds GetMeasure(Transform orig)
    {
        var b = orig.gameObject.GetComponent<Renderer>().bounds;
        
        return b;
    }

    public void PointerVisualization(GameObject o)
    {
        _b = GetMeasure(o.transform);
        
        if (o.transform.childCount < 1)
        {
            Instantiate(circle, new Vector3(o.transform.position.x, _b.min.y, o.transform.position.z), circle.transform.localRotation, o.transform).transform.localScale = new Vector3(1f, 1f, 1f) * _b.size.magnitude;
            Instantiate(arrow, new Vector3(o.transform.position.x, _b.max.y * 2, o.transform.position.z), transform.rotation, o.transform).transform.localScale = new Vector3(1f, 1f, 1f) * _b.size.magnitude;
        }
    }
}
