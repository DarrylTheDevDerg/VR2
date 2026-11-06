using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class SnapObjectCollection : MonoBehaviour
{
    [Serializable]
    public class ProgresionConsistency
    {
        public int indexId;
        public string[] taskNames;
    }

    public List<ProgresionConsistency> taskValidation;
    public List<GameObject> itemList;
    public float itemLimit;

    public float fadeDuration = 1f;
    
    public bool shouldGuide = false;
    public GameObject guideObject;
    
    [CanBeNull] public UnityEvent afterFinish;

    private Collider _c;
    private Vector3 _measuring;
    private Bounds _b;
    
    private Material _mat;
    private float _oA;

    private bool _done, _f;

    private List<Vector3> _validPoints = new List<Vector3>();

    private ProgressionControl _pG;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _c = GetComponent<Collider>();
        if (!_c.isTrigger) _c.isTrigger = true;
        
        _measuring = _c.ClosestPointOnBounds(transform.position);
        _b = _c.bounds;
        
        _mat = guideObject.GetComponent<MeshRenderer>().material;
        _oA = _mat.color.a;

        _pG = FindFirstObjectByType<ProgressionControl>();

        if (shouldGuide) guideObject.SetActive(true);
        
        if (shouldGuide && !_f && guideObject.activeSelf)
        {
            StartCoroutine(FadeLoop());
            _f = true;
        }
    }

    void Update()
    {
        if (itemList.Count > itemLimit)
        {
            if (_pG) MoveOn();
            afterFinish?.Invoke();
            
            guideObject.SetActive(false);

            this.enabled = false;
            _done = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (itemList.Count <= itemLimit)
        {
            if (itemList.Contains(other.gameObject)) return;
            
            other.transform.SetParent(transform);
            
            if (other.GetComponent<OVRGrabbable>()) other.GetComponent<OVRGrabbable>().GrabEnd(Vector3.zero, Vector3.zero);
            if (other.GetComponent<OVRGrabbable>()) other.GetComponent<OVRGrabbable>().enabled = false;
        
            Vector3 pos = new Vector3(Mathf.PerlinNoise1D(Random.Range(_b.min.x, _b.max.x)), _b.min.y, Mathf.PerlinNoise1D(Random.Range(_c.bounds.min.z, _c.bounds.max.z)));

            if (_validPoints is { Count: > 0 })
            {
                foreach (Vector3 o in _validPoints)
                {
                    if (o.Equals(pos)) pos = new Vector3(Mathf.InverseLerp(Mathf.PerlinNoise1D(Random.Range(_b.min.x, _b.max.x)), o.x, pos.x), _b.min.y, Mathf.InverseLerp(Mathf.PerlinNoise1D(Random.Range(_b.min.z, _b.max.z)), o.z, pos.z));
                }
            }

            other.transform.position = pos;
            _validPoints.Add(pos);
        
            itemList.Add(other.gameObject);
        }
    }

    public void Reset()
    {
        itemList.Clear();
        _validPoints.Clear();
        
        guideObject.SetActive(true);
        
        _done = false;
    }

    IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return Fade(_oA, 0f);
            yield return Fade(0f, _oA);
        }
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = _mat.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            _mat.color = c;
            
            yield return null;
        }
    }

    public void MoveOn()
    {
        foreach (ProgresionConsistency p in taskValidation)
        {
            foreach (string taskName in p.taskNames)
            {
                foreach (ProgressionControl.Task t in _pG.steps[p.indexId].groceryList)
                {
                    if (t.name.Equals(taskName)) t.isDone = true;
                }
            }
        }
    }
}
