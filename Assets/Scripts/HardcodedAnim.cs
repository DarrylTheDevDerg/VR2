using UnityEngine;

public class HardcodedAnim : MonoBehaviour
{
    public enum AnimationType
    {
        Pulse,
        Bounce
    };
    
    public float speed, modHeight;
    public AnimationType type;

    private Vector3 _origScale;
    private float _sprWldHgt, _lastY;
    private Renderer _r;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _r = GetComponent<Renderer>();
        
        _origScale = transform.localScale;
        _sprWldHgt = _r.bounds.size.y;
        _lastY = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case AnimationType.Bounce:
                BounceEffect();
                break;
            
            case AnimationType.Pulse:
                PulseEffect();
                break;
        }
    }

    private void PulseEffect()
    {
        float t = Time.time;
        float sX = 1f;
        float sY = 1f;

        float p = 1f + Mathf.Sin(t * speed * Mathf.PI * 2f);
        sX *= p;
        sY *= p;

        transform.localScale = new Vector3(_origScale.x * sX, _origScale.y * sY, _origScale.z);
    }

    private void BounceEffect()
    {
        float t = Time.time;
        float offY = 0f;

        float b = Mathf.Abs(Mathf.Sin(t * speed * Mathf.PI));
        offY = b * modHeight * _sprWldHgt;
        
        transform.position += Vector3.up * (offY - _lastY);
        _lastY = offY;
    }
}
