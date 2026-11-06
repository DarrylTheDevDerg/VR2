using System;
using UnityEngine;
using System.Collections.Generic;
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
    
    private int _current, _tasksFinished;
    private bool _marked;
    private CookingSimulator _timer;

    void Start()
    {
        instructionText.text = steps[0].instruction;
        stepDisplay.text = $"Paso {_current + 1} de {steps.Count}";
        
        _timer = FindFirstObjectByType<CookingSimulator>();
        
        CreateMarkers();
        steps[0].onStart?.Invoke();
    }

    void Update()
    {
        if (FetchDoneTaskAmt() >= steps[_current].groceryList.Count && !_timer.Inquiry())
        {
            steps[_current].onComplete?.Invoke();
            Progression();
            
            if (!_marked)
                CreateMarkers();
            
            _tasksFinished = 0;
            steps[_current].onStart?.Invoke();
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

    public void IncreaseTaskCompletion(string taskName)
    {
        SetUI();

        foreach (Step step in steps)
        {
            foreach (Task t in step.groceryList)
            {
                if (t.name.Equals(taskName)) t.isDone = true;
            }
        }
    }

    private void SetUI()
    {
        foreach (Step step in steps)
        {
            foreach (Task t in step.groceryList)
            {
                string done = t.isDone ? "¡LISTO!" : "PENDIENTE";
                
                taskList.text += $"{t.name}: {done}";
            }
        }
    }

    private int FetchDoneTaskAmt()
    {
        foreach (Step s in steps)
        {
            foreach (Task t in s.groceryList)
            {
                if (t.isDone) _tasksFinished++;
            }
        }
        
        return _tasksFinished;
    }
}
