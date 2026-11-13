using UnityEngine;

public class IngredientContainer : MonoBehaviour
{
    [SerializeField] private IngredientData ingredient;
    [SerializeField] private SpoonMixer bowl;
    [SerializeField] private Transform spout;            // empty child at the opening
    [SerializeField] private ParticleSystem pourEffect;  // optional

    [SerializeField] private float capacity = 1f;
    [SerializeField] private float flowPerSecond = 0.1f;
    [SerializeField] private float pourAngle = 100f;     // degrees away from upright
    [SerializeField] private float maxHeightAboveBowl = 0.4f;

    private float amount;

    private void Awake() => amount = capacity;

    private void Update()
    {
        bool tilted = Vector3.Angle(transform.up, Vector3.up) > pourAngle;
        bool pouring = amount > 0f && tilted && bowl.IsOverBowl(spout.position, maxHeightAboveBowl);

        if (pouring)
        {
            float poured = Mathf.Min(amount, flowPerSecond * Time.deltaTime);
            amount -= poured;
            bowl.AddIngredient(ingredient, poured);
        }

        if (pourEffect)
        {
            if (pouring && !pourEffect.isPlaying) pourEffect.Play();
            else if (!pouring && pourEffect.isPlaying) pourEffect.Stop();
        }
    }
}