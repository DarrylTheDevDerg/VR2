using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionControl : MonoBehaviour
{
    [Serializable]
    public class Step
    {
        [TextArea] public string instruction;
        public AudioClip audio;
    }
    
    // THE STEPS MUST BE IN ORDER, OTHERWISE IT WON'T FOLLOW THE LINEAR RHYTHM
    [SerializeField] public List<Step> steps = new List<Step>();
    public TextMeshProUGUI instructionText, stepDisplay;

    private int _current;

    void Start()
    {
        instructionText.text = steps[0].instruction;
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
    }

    public void FetchAudioClip()
    {
        AudioManager.instance.PlaySFX(steps[_current].audio);
    }

    public void Progression()
    {
        _current++;
        
        instructionText.text = steps[_current].instruction;
        FetchAudioClip();
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
    }
}
