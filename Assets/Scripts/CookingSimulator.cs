using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CookingSimulator : MonoBehaviour
{
    // One cooking spot in the oven: holds one dish and keeps that dish's own timer.
    private class CookSlot
    {
        public GameObject food;
        public readonly HashSet<Collider> contacts = new HashSet<Collider>();
        public float time;
        public float required;

        public bool IsFree => !food;

        public void Begin(GameObject obj, Collider first, float requiredTime)
        {
            food = obj;
            contacts.Clear();
            contacts.Add(first);
            time = 0f;
            required = requiredTime;
        }

        public void Clear()
        {
            food = null;
            contacts.Clear();
            time = 0f;
        }
    }

    [Min(0f), Tooltip("Default cook time. Used when the food has no Cookable (or its cook time is 0).")]
    public float timeNeeded;
    [Min(1f), Header("Time multiplier"), Tooltip("Checks how fast 'time' goes in the game. 1 is IRL time (absolutely not recommended, btw), 10+ is recommended.")]
    public float timeMultiplier = 10f;

    [Header("Visual indicators if UI exists for them. One per cooking slot, in order.")]
    [Tooltip("Indicator for the first dish.")]
    public Image uiIndicator;
    [Tooltip("Indicators for the second dish, the third, and so on.")]
    public Image[] extraIndicators;

    [Range(0f, 1f)]
    public float minCookPower;

    public bool speedScalesWithPower;
    public bool showTimeRemaining;

    [Header("Trigger zones: food is detected by any of these")]
    [Tooltip("Put as many trigger colliders here as you like (two, for example). If empty, the collider on this GameObject is used, like before.")]
    public Collider[] triggerZones;

    [Header("How many dishes can cook at the same time")]
    [Min(1)] public int maxFoods = 2;

    [Header("Cooked result")]
    [FormerlySerializedAs("cookedFood")]
    [Tooltip("Default result. A Cookable on the food overrides it. Empty = the food itself counts as cooked.")]
    public GameObject cookedFoodPrefab;
    [Tooltip("When a result prefab is spawned, remove the raw food from the scene.")]
    public bool destroyRawFood;

    [Tooltip("Invoked once for each dish that finishes cooking.")]
    public UnityEvent<GameObject> uponFinishCooking;
    public OvenKnob knob;

    // The last thing that came out of the oven
    public GameObject lastCooked { get; private set; }

    // Every dish that's cooking (or waiting for the heat) right now
    public IEnumerable<GameObject> Foods
    {
        get
        {
            foreach (CookSlot slot in _slots)
                if (!slot.IsFree) yield return slot.food;
        }
    }

    private readonly List<CookSlot> _slots = new List<CookSlot>();
    private float _power;

    void Awake()
    {
        int count = Mathf.Max(1, maxFoods);
        for (int i = 0; i < count; i++) _slots.Add(new CookSlot());
    }

    void OnEnable()
    {
        if (knob) knob.onValueChange.AddListener(SetOvenPower);
    }

    void OnDisable()
    {
        if (knob) knob.onValueChange.RemoveListener(SetOvenPower);
    }

    void Start()
    {
        for (int i = 0; i < _slots.Count; i++)
            ConfigureIndicator(IndicatorFor(i));

        SetupZones();
    }

    void Update()
    {
        float spd = timeMultiplier * (speedScalesWithPower ? _power : 1f);

        for (int i = 0; i < _slots.Count; i++)
        {
            CookSlot slot = _slots[i];
            bool hasFood = !slot.IsFree;

            if (hasFood && _power > minCookPower)
            {
                slot.time += Time.deltaTime * spd;

                if (slot.time >= slot.required) FinishCooking(slot);
            }
            else if (!hasFood) slot.Clear();

            UpdateUI(i, slot);
        }
    }

    // ---- Public setters: call these from code or from a UnityEvent ----

    public void SetOvenPower(float value)
    {
        _power = Mathf.Clamp01(value);
    }

    // Sets the default cooked result. Pass null so the food itself counts as cooked.
    public void SetCookedFood(GameObject prefab)
    {
        cookedFoodPrefab = prefab;
    }

    // ---- Trigger handling ----

    // A trigger collider on a child object can't tell us about its events on its own,
    // so each zone gets a small relay that forwards them here.
    private void SetupZones()
    {
        if (triggerZones == null) return;

        foreach (Collider zone in triggerZones)
        {
            if (!zone) continue;

            zone.isTrigger = true;

            // A collider on this very GameObject reaches OnTriggerEnter/Exit below by itself
            if (zone.gameObject == gameObject) continue;

            CookingTriggerRelay relay = zone.GetComponent<CookingTriggerRelay>();
            if (!relay) relay = zone.gameObject.AddComponent<CookingTriggerRelay>();
            relay.target = this;
        }
    }

    void OnTriggerEnter(Collider other) => HandleEnter(other);
    void OnTriggerExit(Collider other) => HandleExit(other);

    public void HandleEnter(Collider other)
    {
        GameObject root = GetRoot(other);

        // Already cooking this dish? Then it's just another of its colliders (or a second zone).
        CookSlot slot = FindSlot(root);
        if (slot != null)
        {
            slot.contacts.Add(other);
            return;
        }

        if (!other.CompareTag("Food") && !root.CompareTag("Food")) return;

        Cookable cookable = root.GetComponentInChildren<Cookable>();
        if (cookable && cookable.isCooked) return;

        slot = FindFreeSlot();
        if (slot == null) return; // oven is full

        slot.Begin(root, other, ResolveTime(root, cookable));
    }

    public void HandleExit(Collider other)
    {
        CookSlot slot = FindSlot(GetRoot(other));
        if (slot == null) return;

        // A dish only leaves once it's out of every zone
        slot.contacts.Remove(other);
        if (slot.contacts.Count == 0) slot.Clear();
    }

    private CookSlot FindSlot(GameObject root)
    {
        foreach (CookSlot slot in _slots)
            if (!slot.IsFree && slot.food == root) return slot;

        return null;
    }

    private CookSlot FindFreeSlot()
    {
        foreach (CookSlot slot in _slots)
            if (slot.IsFree) return slot;

        return null;
    }

    static GameObject GetRoot(Collider col)
    {
        return col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
    }

    // ---- Cooking ----

    private float ResolveTime(GameObject obj, Cookable cookable)
    {
        if (cookable && cookable.cookTime > 0f) return cookable.cookTime;

        // Old name-based times. Drop these once your prefabs carry a Cookable.
        if (obj.name.Contains("CRice")) return 15f;
        if (obj.name.Contains("CSteak")) return 30f;

        return timeNeeded;
    }

    void FinishCooking(CookSlot slot)
    {
        GameObject raw = slot.food;
        Cookable rawCookable = raw.GetComponentInChildren<Cookable>();
        GameObject prefab = rawCookable && rawCookable.cookedVersion ? rawCookable.cookedVersion : cookedFoodPrefab;

        GameObject result = raw;
        if (prefab)
        {
            result = Instantiate(prefab, raw.transform.position, raw.transform.rotation);
            if (destroyRawFood) Destroy(raw);
        }

        // Mark it so it can't be cooked a second time if it goes back in
        Cookable resultCookable = result.GetComponentInChildren<Cookable>();
        if (!resultCookable) resultCookable = result.AddComponent<Cookable>();
        resultCookable.isCooked = true;

        lastCooked = result;
        slot.Clear();

        uponFinishCooking?.Invoke(result);
    }

    // ---- UI ----

    private Image IndicatorFor(int slotIndex)
    {
        if (slotIndex == 0) return uiIndicator;

        int extra = slotIndex - 1;
        if (extraIndicators != null && extra < extraIndicators.Length) return extraIndicators[extra];

        return null;
    }

    private static void ConfigureIndicator(Image img)
    {
        // Because I don't trust my co-workers or my own faulty memory.
        if (!img || img.type == Image.Type.Filled) return;

        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillClockwise = true;
    }

    void UpdateUI(int slotIndex, CookSlot slot)
    {
        Image img = IndicatorFor(slotIndex);
        if (!img) return;

        float prog = slot.required > 0f ? Mathf.Clamp01(slot.time / slot.required) : 0f;
        img.fillAmount = showTimeRemaining ? 1f - prog : prog;
    }

    // True while at least one dish is actually cooking
    public bool Inquiry()
    {
        if (_power <= minCookPower) return false;

        foreach (CookSlot slot in _slots)
            if (!slot.IsFree) return true;

        return false;
    }
}
