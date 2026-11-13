using UnityEngine;

// Added automatically by CookingSimulator to every trigger zone that lives on another GameObject.
public class CookingTriggerRelay : MonoBehaviour
{
    public CookingSimulator target;

    void OnTriggerEnter(Collider other)
    {
        if (target) target.HandleEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (target) target.HandleExit(other);
    }
}
