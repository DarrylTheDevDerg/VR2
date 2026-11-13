using JetBrains.Annotations;
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Collider))]
public class AlchemyFromTriggers : MonoBehaviour
{
    public GameObject targetObject, newObject;
    
    [CanBeNull] public string targetTag;
    public bool shouldDestroy;

    private bool _done;

    void OnTriggerEnter(Collider other)
    {
        if (!newObject) return;
        
        if (other.gameObject.Equals(targetObject)) 
            Alakazam(targetObject);
        
        else if (!targetObject && other.CompareTag(targetTag) && !string.IsNullOrEmpty(targetTag))
            Alakazam(other.gameObject);
    }

    void Alakazam(GameObject obj)
    {
        if (_done) return;
        
        if (obj.GetComponent<Grabbable>()) DisableGrabbing(obj, obj.GetComponent<Grabbable>());
        
        Instantiate(newObject, obj.transform.position, Quaternion.identity);
        
        if (shouldDestroy)
        {
            Destroy(obj);
            Destroy(this);
        }
        else
        {
            obj.SetActive(false);
            this.gameObject.SetActive(false);
        }
        
        enabled = false;
        _done = true;
    }
    
    static void DisableGrabbing(GameObject g, Grabbable gr)
    {
        foreach (IInteractable i in g.GetComponentsInChildren<IInteractable>(true))
        {
            if (i is Behaviour b) b.enabled = false;
        }
        
        if (gr) gr.enabled = false;
    }
}
