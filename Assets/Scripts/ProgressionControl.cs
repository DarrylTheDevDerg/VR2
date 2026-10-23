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
    public TextMeshProUGUI instructionText;

    private int _current;
    private bool _done;

    public void FetchAudioClip()
    {
        AudioManager.instance.PlaySFX(steps[_current].audio);
    }

    public void Progression()
    {
        _current++;
        
        instructionText.text = steps[_current].instruction;
        FetchAudioClip();
    }
}
