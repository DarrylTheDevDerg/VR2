using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Collider))]
public class AlchemyFromTriggers : MonoBehaviour
{
    public GameObject targetObject, newObject;
    
    public string targetTag;
    public bool shouldDestroy;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Equals(targetObject)) Alakazam();
        if (other.CompareTag(targetTag) && !string.IsNullOrEmpty(targetTag)) Alakazam();
    }

    void Alakazam()
    {
        if (targetObject.GetComponent<Grabbable>() != null) DisableGrabbing(targetObject, targetObject.GetComponent<Grabbable>());
        
        Instantiate(newObject, targetObject.transform.position, Quaternion.identity);
        
        if (shouldDestroy) Destroy(targetObject);
        else targetObject.SetActive(false);
        
        enabled = false;
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
