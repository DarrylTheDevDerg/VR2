using UnityEngine;
using System;
using System.Collections.Generic;

public class Cuttable : MonoBehaviour
{
    [Serializable]
    public class CutStage
    {
        public GameObject piece;
        public bool disappear;
    }
    
    public GameObject start;
    public List<CutStage> steps;
    
    private int _currentCut;
    private float _sizeReduction;

    public void ChangeObj()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knife"))
        {
            ChangeObj();
        }
    }

    public void Advance()
    {
        _currentCut++;
        _sizeReduction = 1f / steps.Count;
        
        steps[_currentCut].piece.SetActive(true);
    }
}
