using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CookingSimulator : MonoBehaviour
{
    [Min(0f)]
    public float timeNeeded;
    [Min(1f), Header("Time multiplier"), Tooltip("Checks how fast 'time' goes in the game. 1 is IRL time (absolutely not recommended, btw), 10+ is recommended.")]
    public float timeMultiplier;

    [Header("Visual indicator if UI exists for it.")]
    public Image uiIndicator;

    [Range(0f, 1f)]
    public float minCookPower;

    public bool speedScalesWithPower;
    public bool showTimeRemaining;

    public GameObject food, cookedFood;
    public OvenKnob knob;
    public UnityEvent<GameObject> uponFinishCooking;
    
    private float _cTime;
    private float _power;
    private bool _active;

    void OnEnable()
    {
        if (knob) knob.onValueChange.AddListener(SetOvenPower);
    }

    void OnDisable()
    {
        if (knob) knob.onValueChange.RemoveListener(SetOvenPower);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Because I don't trust my co-workers and my own faulty memory. Fuck me for being so naive.
        if (uiIndicator.type != Image.Type.Filled && uiIndicator)
        {
            uiIndicator.type = Image.Type.Filled;
            uiIndicator.fillMethod = Image.FillMethod.Radial360;
            uiIndicator.fillClockwise = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool hasFood = food;
        _active = hasFood && _power > minCookPower;
        
        if (_active)
        {
            float spd = timeMultiplier * (speedScalesWithPower ? _power : 1f);
            _cTime += Time.deltaTime * spd;

            if (_cTime >= timeNeeded) FinishCooking();
        }
        else if (!hasFood) _cTime = 0f;
        
        UpdateUI();
    }

    void SetOvenPower(float value)
    {
        _power = Mathf.Clamp01(value);
    }
    
    void FinishCooking()
    {
        _cTime = 0f;

        if (cookedFood)
        {
            cookedFood = Instantiate(cookedFood, food.transform.position, food.transform.rotation);
        }
        else cookedFood = food;

        food = null;
        uponFinishCooking?.Invoke(cookedFood);
    }

    void UpdateUI()
    {
        if (!uiIndicator) return;
        float prog = timeNeeded > 0f ? Mathf.Clamp01(_cTime / timeNeeded) : 0f;
        uiIndicator.fillAmount = showTimeRemaining ? 1f - prog : prog;
    }

    static GameObject GetRoot(Collider col)
    {
        return col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        if (food) return;
        if (other.CompareTag("Food")) return;
        
        GameObject f = GetRoot(other);
        if (f.Equals(cookedFood)) return;
        
        food = f;
    }

    void OnTriggerExit(Collider other)
    {
        if (food && GetRoot(other).Equals(food)) food = null;
    }
}
