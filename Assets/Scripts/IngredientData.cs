using UnityEngine;

[CreateAssetMenu(menuName = "Cooking/Ingredient")]
public class IngredientData : ScriptableObject
{
    public string displayName;
    
    [ColorUsage(false, false)]
    public Color color = Color.white;
}