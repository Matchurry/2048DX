using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Destory : MonoBehaviour
{
    public static readonly UnityEvent OnStartDestory = new UnityEvent();
    public static readonly UnityEvent OnEndDestory = new UnityEvent();
    public static bool IsDestroying = false;
    private SpriteRenderer _sr;
    private Vector3 inipos;
    private Vector3 tarpos;
    
    void Start()
    {
        inipos = transform.position;
        tarpos = inipos;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        GreedyAiButton.greedyAiAct.AddListener(HandbleAiAct);
        GreedyAiButton.greedyAiDeA.AddListener(HandleAiDeAct);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Drag.IsDraging) _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        else _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
    }
    
    void HandbleAiAct()
    {
        tarpos = new Vector3(inipos.x + 2, inipos.y, inipos.z);
    }

    void HandleAiDeAct()
    {
        tarpos = inipos;
    }
    
    private void OnMouseDown()
    {
        if (!IsDestroying && !Drag.IsDraging)
        {
            IsDestroying = true;
            OnStartDestory.Invoke();
        }
        else
        {
            IsDestroying = false;
            OnEndDestory.Invoke();
        }
    }
}
