using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GreedyAiButton : MonoBehaviour
{
    private SpriteRenderer _sr;
    public static readonly UnityEvent greedyAiAct = new UnityEvent();
    public static readonly UnityEvent greedyAiDeA = new UnityEvent();
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OnMouseDown();
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        }
        else if(Input.GetKeyUp(KeyCode.I))
        {
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
        }
    }
    private void OnMouseEnter()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
    }

    private void OnMouseExit()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
    }
    
    private void OnMouseDown()
    {
        Camera.is_AIControl = !Camera.is_AIControl;
        if(Camera.is_AIControl) greedyAiAct.Invoke();
        else greedyAiDeA.Invoke();
    }
}
