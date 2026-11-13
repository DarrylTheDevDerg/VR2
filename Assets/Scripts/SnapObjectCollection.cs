using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Oculus.Interaction;

[RequireComponent(typeof(Collider))]
public class SnapObjectCollection : MonoBehaviour
{
    [Serializable]
    public class ProgresionConsistency
    {
        public int indexId;
        public string[] taskNames;
        public List<GameObject> objectsNeeded;
    }

    public List<ProgresionConsistency> taskValidation =  new List<ProgresionConsistency>();
    public List<GameObject> itemList = new List<GameObject>();
    public float itemLimit, itemSpacing;
    public bool requireGrab;
    public string tagTarget;

    public float fadeDuration = 1f;
    public bool shouldGuide = false;
    public GameObject guideObject;
    
    [CanBeNull] public UnityEvent afterFinish;
    
    private bool _done, _f, _sameList;
    private List<Vector3> _validPoints = new List<Vector3>();
    private const int MaxPosAttempts = 20;
    private Collider _c;
    private ProgressionControl _pG;
    private bool _isFull => itemList.Count > itemLimit;
    
    private Material _mat;
    private float _oA;

    private int _p, _current;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _c = GetComponent<Collider>();
        if (!_c.isTrigger) _c.isTrigger = true;

        if (guideObject)
        {
            if (guideObject.TryGetComponent(out MeshRenderer r))
            {
                _mat = r.material;
                _oA = _mat.color.a;
            }
            
            if (shouldGuide) guideObject.SetActive(true);
        }

        _pG = FindFirstObjectByType<ProgressionControl>();
        
        if (shouldGuide && !_f && guideObject.activeSelf)
        {
            StartCoroutine(FadeLoop());
            _f = true;
        }
    }

    void OnTriggerEnter(Collider other) => TryGrab(other);
    void OnTriggerStay(Collider other) => TryGrab(other);

    void TryGrab(Collider other)
    {
        if (_done || _isFull) return;
        
        Rigidbody r = other.attachedRigidbody;
        if (!r) return;
        
        GameObject g = r.gameObject;
        
        if (itemList.Contains(g)) return;
        
        g.transform.SetParent(transform);
        if (transform.IsChildOf(g.transform)) return;
        
        if (!g.CompareTag(tagTarget)) return;
        
        Grabbable grab = g.GetComponentInParent<Grabbable>();
        
        if (grab) if (!grab.enabled) return;
        else if (r.isKinematic) return;
        
        itemList.Add(g);
        
        GameObject root = grab != null ? grab.gameObject : g;
        DisableGrabbing(root, grab);

        _p++;
        StartCoroutine(PlaceRoutine(g, r));
        
        _sameList = new HashSet<GameObject>(itemList).SetEquals(taskValidation[_current].objectsNeeded);
        
    }

    static void DisableGrabbing(GameObject g, Grabbable gr)
    {
        foreach (IInteractable i in g.GetComponentsInChildren<IInteractable>(true))
        {
            if (i is Behaviour b) b.enabled = false;
        }
        
        if (gr) gr.enabled = false;
    }
    
    static void EnableGrabbing(GameObject g, Grabbable gr)
    {
        foreach (IInteractable i in g.GetComponentsInChildren<IInteractable>(true))
        {
            if (i is Behaviour b) b.enabled = true;
        }
        
        if (gr) gr.enabled = true;
    }

    IEnumerator PlaceRoutine(GameObject g, Rigidbody r)
    {
        yield return null;
        yield return new WaitForFixedUpdate();

        if (!g)
        {
            _p--;
            yield break;
        }
        
        float bPivot = PivotToBottom(g);
        g.transform.SetParent(transform);

        Vector3 p = GetFreePoint();
        
        g.transform.position = new Vector3(p.x, p.y + bPivot, p.z);

        if (r && r.isKinematic)
        {
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
        }
        
        _validPoints.Add(p);
        _p--;
        
        if (!_done && _p <= 0 && _sameList) Ending();
    }
    
    Vector3 GetFreePoint()
    {
        Bounds b = _c.bounds;
        Vector3 c = Vector3.zero;

        for (int i = 0; i < MaxPosAttempts; i++)
        {
            c = new Vector3(
                Random.Range(b.min.x, b.max.x), 
                b.min.y, 
                Random.Range(b.min.z, b.max.z));
            
            if (isFree(c)) break;
        }
        
        return c;
    }

    bool isFree(Vector3 pos)
    {
        foreach (Vector3 p in _validPoints)
            if (Vector3.Distance(p, pos) < itemSpacing) return false;
        
        return true;
    }

    static float PivotToBottom(GameObject g)
    {
        Collider[] col = g.GetComponentsInChildren<Collider>();
        if (col.Length == 0) return 0;
        
        Bounds b = col[0].bounds;

        for (int i = 0; i < col.Length; i++) b.Encapsulate(col[i].bounds.center);
        
        return g.transform.position.y - b.min.y;
    }

    private void Ending()
    {
        if (_done) return;
        if (_pG) MoveOn();
        
        _done = true;
        
        
        StopCoroutine(FadeLoop());
        if (guideObject) guideObject.SetActive(false);

        foreach (GameObject g in itemList)
        {
            g.transform.parent = null;
            EnableGrabbing(g, g.GetComponent<Grabbable>());
        }
        
        afterFinish?.Invoke();
        enabled = false;
        _c.enabled = false;
    }
    
    public void ResetCollection(bool next)
    {
        if (next) _current++;
        
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
        if (!_done)
            foreach (string taskName in taskValidation[_current].taskNames)
            {
                _pG.IncreaseTaskCompletion(taskName);
            }
    }
}
