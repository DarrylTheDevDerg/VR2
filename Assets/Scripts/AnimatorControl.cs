using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControl : MonoBehaviour
{
    public enum PropertyType {INT, FLOAT, BOOL, TRIGGER}
    public enum TriggerAction {SET, RESET}
    
    [Serializable]
    public class Asset
    {
        public string name;
        public PropertyType type;

        public float numMod;
        public bool boolMod;
        public TriggerAction triggerMod;

        public bool advance;
    }

    [Serializable]
    public class MultiEdit
    {
        public List<Asset> assets;
    }
    
    public List<MultiEdit> param;
    public bool shouldBegin;

    private Animator _a;
    private int _current = 0;
    private bool _working;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _a = GetComponent<Animator>();
        
        if (shouldBegin) UpdateAnimatorParams(_current);
    }

    public void UpdateAnimatorParams(int i)
    {
        if (_working) return;
        
        _working = true;
        
        foreach (Asset a in param[i].assets)
        {
            switch (a.type)
            {
                case PropertyType.INT:
                    _a.SetInteger(a.name, (int)a.numMod);
                    break;
                
                case PropertyType.FLOAT:
                    _a.SetFloat(a.name, a.numMod);
                    break;
                
                case PropertyType.BOOL:
                    _a.SetBool(a.name, a.boolMod);
                    break;
                
                case PropertyType.TRIGGER:
                    if (a.triggerMod == TriggerAction.SET) _a.SetTrigger(a.name);
                    else _a.ResetTrigger(a.name);
                    break;
            }
            
            if (a.advance) _current++;
        
            if (_current > param.Count + 1) _current = 0;
        }
        
        _working = false;
    }

    public void TriggerSet(string tName)
    {
        _a.SetTrigger(tName);
    }

    public void TriggerReset(string tName)
    {
        _a.ResetTrigger(tName);
    }

    public void DemandChange(int index)
    {
        UpdateAnimatorParams(index);
    }

    public void OnDemandChange()
    {
        UpdateAnimatorParams(_current);
    }
}
