using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class PlateZone : MonoBehaviour
{
    [SerializeField] private int itemsNeeded = 1;

    public UnityEvent<int> onItemServed;   // passes the running count
    public UnityEvent onPlateReady;

    private int count;

    private void Reset() => GetComponent<Collider>().isTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        FoodItem item = other.GetComponentInParent<FoodItem>();
        if (item == null || item.served) return;

        item.served = true;
        count++;
        onItemServed?.Invoke(count);

        if (count == itemsNeeded) onPlateReady?.Invoke();
    }
}