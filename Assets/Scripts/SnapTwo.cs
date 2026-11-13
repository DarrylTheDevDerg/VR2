using System;
using System.Collections.Generic;
using Oculus.Interaction;
using OIGrabbable = Oculus.Interaction.Grabbable;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SnapTwo : MonoBehaviour
{
    [Serializable]
    public class ProgressEquivalent
    {
        public int progressValue;
        public GameObject[] objectsNeeded4Progress;
    }
    
    public string targetTag;
    public List<ProgressEquivalent> progressEquivalent;
    
    private ProgressionControl _pG;
    private List<GameObject> _c = new List<GameObject>();
    private int _i;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pG = FindFirstObjectByType<ProgressionControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_c.Equals(progressEquivalent[_i].objectsNeeded4Progress))
        {
            CheckProgress();
            _c.Clear();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag) && _c.Count < progressEquivalent[_i].objectsNeeded4Progress.Length && other.GetComponent<Grabbable>())
        {
            print(other.name);
            
            foreach (GameObject g in progressEquivalent[_i].objectsNeeded4Progress)
            {
                if (g.Equals(other.gameObject) && !_c.Contains(other.gameObject)) _c.Add(other.gameObject);
                
                other.GetComponent<Grabbable>().enabled = false;
            }
        }
    }

    void CheckProgress()
    {
        foreach (ProgressionControl.Task t in _pG.steps[progressEquivalent[_i].progressValue].groceryList)
        {
            t.isDone = true;
        }

        _i++;
    }

    void ResetItems()
    {
        foreach (GameObject g in _c)
        {
            g.GetComponent<OVRGrabbable>().enabled = true;
        }
    }

    static void FuckYourGrabbing(GameObject g, Grabbable grab)
    {
        foreach (IInteractable i in g.GetComponents<IInteractable>())
        {
            if (i is Behaviour b) b.enabled = false;
        }
        
        if (grab) grab.enabled = false;
    }
}
