using System;
using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;

public class ProgressionControl : MonoBehaviour
{
    [Serializable]
    public class Step
    {
        [TextArea] public string instruction;
        public AudioClip audio;
        [CanBeNull] public GameObject[] objectsToPoint;
        public int requiredStepsToAdvance = 1;
    }
    
    // THE STEPS MUST BE IN ORDER, OTHERWISE IT WON'T FOLLOW THE LINEAR RHYTHM
    [SerializeField] public List<Step> steps = new List<Step>();
    public TextMeshProUGUI instructionText, stepDisplay;
    public VisualPointer pointer;
    
    private int _current, _tasksFinished;
    private bool _marked;

    void Start()
    {
        instructionText.text = steps[0].instruction;
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
        
        CreateMarkers();
    }

    void Update()
    {
        if (_tasksFinished >= steps[_current].requiredStepsToAdvance)
        {
            Progression();
            
            if (!_marked)
                CreateMarkers();
            
            _tasksFinished = 0;
        }
    }

    public void FetchAudioClip()
    {
        AudioManager.instance.PlaySFX(steps[_current].audio);
    }

    public void Progression()
    {
        _marked = false;
        _current++;
        
        instructionText.text = steps[_current].instruction;
        FetchAudioClip();
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
    }

    private void CreateMarkers()
    {
        foreach (Step step in steps)
        {
            if (step.objectsToPoint != null)
                foreach (GameObject go in step.objectsToPoint)
                {
                    pointer.PointerVisualization(go);
                }
        }
        
        _marked = true;
    }

    public int IncreaseTaskCompletion(int amt)
    {
        return _tasksFinished += amt;
    }
}
