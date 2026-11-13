using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.Events;

public class ProgressionControl : MonoBehaviour
{
    [Serializable]
    public class Step
    {
        [CanBeNull] public UnityEvent onStart;
        [TextArea] public string instruction;
        public AudioClip audio;
        [CanBeNull] public GameObject[] objectsToPoint;

        public List<Task> groceryList;
        [CanBeNull] public UnityEvent onComplete;
    }

    [Serializable]
    public class Task
    {
        public string name;
        public bool isDone;
    }
    
    // THE STEPS MUST BE IN ORDER, OTHERWISE IT WON'T FOLLOW THE LINEAR RHYTHM
    [SerializeField] public List<Step> steps = new List<Step>();
    public TextMeshProUGUI instructionText, stepDisplay, taskList;
    public VisualPointer pointer;
    
    private int _current = 0, _tasksFinished;
    private bool _marked, _finished;
    private CookingSimulator _timer;
    private readonly StringBuilder _sb = new StringBuilder();
    private const string done = "[X]";
    private const string notdone = "[ ]";

    void Start()
    {
        if (steps.Count == 0)
        {
            enabled = false;
            return;
        }
        
        _timer = FindFirstObjectByType<CookingSimulator>();
        
        CreateMarkers();
        steps[0].onStart?.Invoke();
        SetUI();
        
        FetchAudioClip();
        
        if (!_marked)
            CreateMarkers();
    }

    void Update()
    {
        if (_finished) return;
        if (!CurrentStepDone()) return;
        if (_timer && _timer.Inquiry()) return;
        
        steps[_current].onComplete?.Invoke();
        Progression();
    }

    public void FetchAudioClip()
    {
        AudioManager.instance.PlaySFX(steps[_current].audio);
    }

    public void Progression()
    {
        if (_finished) return;

        if (_current >= steps.Count - 1)
        {
            Finish();
            return;
        }
        
        _current++;
        SetUI();
        FetchAudioClip();
        steps[_current].onStart?.Invoke();
    }

    private void CreateMarkers()
    {
        if (!pointer) return;
        if (steps[_current].objectsToPoint == null) return;
        
        foreach (GameObject g in steps[_current].objectsToPoint)
        {
            if (g) pointer.PointerVisualization(g);
        }
    }

    public void IncreaseTaskCompletion(string taskName)
    {
        if (_finished) return;

        bool found = false;
        
        foreach (Task t in steps[_current].groceryList)
        {
            if (t.name.Equals(taskName))
            {
                t.isDone = true;
                found = true;
                break;
            }
        }
        
        SetUI();
    }

    private void Finish()
    {
        _finished = true;
        
        instructionText.text = "Has finalizado la experiencia.";
        stepDisplay.text = "";
        taskList.text = "";
    }
    
    public void SetUI()
    {
        Step step = steps[_current];
        
        instructionText.text = step.instruction;
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
        
        _sb.Clear();
        
        foreach (Task t in step.groceryList)
        {
            _sb.AppendLine($"{t.name} {(t.isDone ? done : notdone)}");
        }
        
        taskList.text = _sb.ToString();
    }

    private bool CurrentStepDone()
    {
        foreach (Task t in steps[_current].groceryList)
        {
            if (!t.isDone) return false;
        }
        
        return true;
    }
}
