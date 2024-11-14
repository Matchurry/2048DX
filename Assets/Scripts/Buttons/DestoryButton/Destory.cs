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
        Toggle.OnCheatChg.AddListener(HandleChearChg);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Drag.IsDraging) _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
        else _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1);
    }
    
    void HandleChearChg()
    {
        if(Toggle.Cheat)
            tarpos = new Vector3(inipos.x, inipos.y, inipos.z);
        else
            tarpos = new Vector3(inipos.x, inipos.y - 1, inipos.z);
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
