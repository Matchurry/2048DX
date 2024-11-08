using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GreedyAiButton : MonoBehaviour
{
    private SpriteRenderer _sr;
    public static readonly UnityEvent greedyAiAct = new UnityEvent();
    public static readonly UnityEvent greedyAiDeA = new UnityEvent();
    private Vector3 inipos;
    
    void Start()
    {
        inipos = transform.position;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        Destory.OnEndDestory.AddListener(OnMouseUp);
        Drag.OnEndDrag.AddListener(OnMouseUp);
    }
    
    void Update()
    {
        if (Camera.is_AIControl)
        {
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
            transform.position = new Vector3(inipos.x, inipos.y + 0.15f * Mathf.Sin(Time.time * 2), inipos.z);
        }
        else transform.position = inipos;
        if(Destory.IsDestroying || Drag.IsDraging)
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
    }
    private void OnMouseDown()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        if (!Camera.is_AIControl && !Destory.IsDestroying && !Drag.IsDraging)
        {
            Camera.is_AIControl = true;
            greedyAiAct.Invoke();
        }
        else
        {
            Camera.is_AIControl = false;
            greedyAiDeA.Invoke();
        }
    }

    private void OnMouseUp()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
    }
}
