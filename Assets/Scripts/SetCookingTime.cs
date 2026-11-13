using System;
using System.Collections.Generic;
using UnityEngine;

public class SetCookingTime : MonoBehaviour
{
    [Serializable]
    public class WhichFood
    {
        public string foodName;
        public float timeToCook;
    }

    public List<WhichFood> valid;
    
    private CookingSimulator _c;

    void Awake()
    {
        _c = FindFirstObjectByType<CookingSimulator>();
    }

    void OnTriggerEnter(Collider other)
    {
        print(other.name);
        
        foreach (WhichFood food in valid)
        {
            if (food.foodName.Equals(other.name) && other.CompareTag("Food"))
            {
                if(!_c.timeNeeded.Equals(food.timeToCook)) _c.timeNeeded = food.timeToCook;
            }
        }
    }
}
