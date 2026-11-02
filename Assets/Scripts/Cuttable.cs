using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class Cuttable : MonoBehaviour
{
    [Serializable]
    public class CutStage
    {
        [Header("This must contain the 'piece' to display, it HAS to be\nan inactive GameObject child.")]
        public GameObject piece;
        [Header("A toggle to willingly make the original piece disappear.\nRarely needed as the script hides it natively.")]
        public bool disappear;
    }
    
    [Header("THIS MUSTN'T BE EMPTY! IT HAS TO CONTAIN\nTHE ORIGINAL GAMEOBJECT!")]
    public GameObject start;
    [Header("Fill this list in order that makes them give the illusion of cutting.")]
    public List<CutStage> steps;
    
    [Header("THIS AUTOMATICALLY FIRES ONCE REACHING THE END\nOF THE LIST!")]
    public UnityEvent uponFinish;
    
    private int _currentCut;
    private float _cooldown;

    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knife") && _cooldown <= 0 && _currentCut < steps.Count)
        {
            Advance();
        }
    }

    public void Advance()
    {
        _currentCut++;
        
        steps[_currentCut].piece.SetActive(true);

        if (steps[_currentCut].disappear || _currentCut >= steps.Count)
            start.SetActive(false);
        else
            start.transform.localScale = new Vector3(1f / _currentCut, 1f, 1f);
        
        if (_currentCut >= steps.Count)
            uponFinish.Invoke();

        _cooldown = 0.5f;
    }
}
