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
    private Vector3 tarpos;
    
    void Start()
    {
        inipos = transform.position;
        tarpos = inipos;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        Destory.OnEndDestory.AddListener(OnMouseUp);
        Drag.OnEndDrag.AddListener(OnMouseUp);
        Toggle.OnCheatChg.AddListener(HandleChearChg);
    }
    
    void Update()
    {
        if (Camera.is_AIControl)
        {
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
            transform.position = new Vector3(inipos.x, inipos.y + 0.15f * Mathf.Sin(Time.time * 2), inipos.z);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        }
        if(Destory.IsDestroying || Drag.IsDraging)
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
    }
    private void OnMouseDown()
    {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
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
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1);
    }
    
    void HandleChearChg()
    {
        if(Toggle.Cheat)
            tarpos = new Vector3(inipos.x, inipos.y, inipos.z);
        else
            tarpos = new Vector3(inipos.x, inipos.y - 1, inipos.z);
    }
}
