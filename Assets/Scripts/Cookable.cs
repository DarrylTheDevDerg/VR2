using UnityEngine;

// Put this on any food prefab to decide what it becomes and how long it takes.
public class Cookable : MonoBehaviour
{
    [Tooltip("What this food turns into. Empty = use the simulator's default.")]
    public GameObject cookedVersion;

    [Min(0f), Tooltip("Cook time in game seconds. 0 = use the simulator's default.")]
    public float cookTime;

    // Set by the simulator once the food has been cooked
    [HideInInspector] public bool isCooked;
}
