using UnityEngine;
using UnityEngine.Events;

public class PanPourer : MonoBehaviour
{
    [Header("Contents area (a box inside the pan)")]
    [SerializeField] private Transform contentsCenter;   // empty child, sits just above the pan floor
    [SerializeField] private Vector3 contentsHalfExtents = new Vector3(0.12f, 0.05f, 0.12f);
    [SerializeField] private LayerMask foodLayers = ~0;

    [Header("Pouring")]
    [SerializeField] private float startPourAngle = 35f;     // tilt before we start helpin'
    [SerializeField] private float fullForceAngle = 90f;     // tilt at which help is maxed out
    [SerializeField] private float slideAcceleration = 3f;   // m/s², the gentle shove

    [Header("Events")]
    public UnityEvent onPourStarted;
    public UnityEvent onPourStopped;

    public bool IsPouring { get; private set; }

    private readonly Collider[] buffer = new Collider[32];

    private void FixedUpdate()
    {
        float tilt = Vector3.Angle(transform.up, Vector3.up);
        int count = 0;

        if (tilt > startPourAngle)
        {
            count = Physics.OverlapBoxNonAlloc(
                contentsCenter.position, contentsHalfExtents, buffer,
                contentsCenter.rotation, foodLayers, QueryTriggerInteraction.Ignore);

            // The floor slopes toward wherever the pan's "up" leans
            Vector3 downhill = Vector3.ProjectOnPlane(transform.up, Vector3.up);
            float strength = Mathf.InverseLerp(startPourAngle, fullForceAngle, tilt);

            if (downhill.sqrMagnitude > 0.0001f)
            {
                downhill.Normalize();
                for (int i = 0; i < count; i++)
                {
                    Rigidbody rb = buffer[i].attachedRigidbody;
                    if (rb || rb.isKinematic) continue;
                    if (!buffer[i].GetComponentInParent<FoodItem>()) continue;

                    rb.WakeUp();
                    rb.AddForce(downhill * slideAcceleration * strength, ForceMode.Acceleration);
                }
            }
        }

        bool pouringNow = count > 0;
        if (pouringNow != IsPouring)
        {
            IsPouring = pouringNow;
            if (IsPouring) onPourStarted?.Invoke(); else onPourStopped?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (contentsCenter == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(contentsCenter.position, contentsCenter.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, contentsHalfExtents * 2f);
    }
}
