using UnityEngine;
using UnityEngine.Events;

public class SpoonMixer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform spoonTip;     // empty child at the spoon's bowl end
    [SerializeField] private Transform bowlCenter;   // empty child at the center of the bowl (Y axis = up)

    [Header("Bowl shape (local units, meters)")]
    [SerializeField] private float bowlRadius = 0.15f;
    [SerializeField] private float innerDeadZone = 0.02f;  // ignore the dead center, where angles go haywire
    [SerializeField] private float rimHeight = 0.0f;       // tip must be below this local Y
    [SerializeField] private float bowlDepth = 0.12f;      // ...and above -depth

    [Header("Mixing")]
    [SerializeField] private float turnsToFinish = 5f;     // full laps needed to call it mixed
    [SerializeField] private float minSpeedDegPerSec = 30f; // ignore lazy dawdlin'

    [Header("Events")]
    public UnityEvent<float> onProgressChanged;  // 0..1
    public UnityEvent onMixed;

    private float lastAngle;
    private bool hasLastAngle;
    private float progress;
    private bool finished;

    private float totalAmount;   // everything in the bowl
    private float mixedAmount;   // how much of it is blended
    public float Progress => totalAmount <= 0f ? 0f : mixedAmount / totalAmount;
    
    public event System.Action<IngredientData, float> OnIngredientAdded;

    private void Start()
    {
        spoonTip.transform.Find("Tip");
    }
    
    private void Update()
    {
        if (finished) return;
        
        spoonTip.transform.Find("Tip");

        // Tip position relative to the bowl, so moving/rotating the bowl doesn't matter
        Vector3 local = bowlCenter.InverseTransformPoint(spoonTip.position);
        Vector2 flat = new Vector2(local.x, local.z);
        float dist = flat.magnitude;

        bool inside = dist > innerDeadZone && dist < bowlRadius
                      && local.y < rimHeight && local.y > -bowlDepth;

        if (!inside)
        {
            hasLastAngle = false; // spoon left the bowl, restart tracking
            return;
        }

        float angle = Mathf.Atan2(flat.y, flat.x) * Mathf.Rad2Deg;

        if (hasLastAngle)
        {
            float delta = Mathf.Abs(Mathf.DeltaAngle(lastAngle, angle));
            float speed = delta / Time.deltaTime;

            if (speed >= minSpeedDegPerSec && totalAmount > 0f)
            {
                mixedAmount = Mathf.Min(totalAmount,
                    mixedAmount + totalAmount * delta / (360f * turnsToFinish));
                onProgressChanged?.Invoke(Progress);

                if (Progress >= 1f)
                {
                    finished = true;
                    onMixed?.Invoke();
                }
            }
        }

        lastAngle = angle;
        hasLastAngle = true;
    }

    public void ResetMixing()
    {
        progress = 0f;
        finished = false;
        hasLastAngle = false;
        onProgressChanged?.Invoke(0f);
    }
    
    public void AddIngredient(IngredientData data, float amount)
    {
        totalAmount += amount;
        finished = false;                 // fresh stuff, so it needs mixing again
        OnIngredientAdded?.Invoke(data, amount);
        onProgressChanged?.Invoke(Progress);
    }

// Lets a bottle ask "am I pouring over the bowl?"
    public bool IsOverBowl(Vector3 worldPoint, float maxHeightAbove = 0.4f)
    {
        Vector3 local = bowlCenter.InverseTransformPoint(worldPoint);
        return new Vector2(local.x, local.z).magnitude < bowlRadius
               && local.y > -bowlDepth && local.y < maxHeightAbove;
    }
}
