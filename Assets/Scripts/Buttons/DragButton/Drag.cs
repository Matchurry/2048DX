using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Drag : MonoBehaviour
{
    public static bool IsDraging = false;
    public static bool Draging = false;
    public static readonly UnityEvent OnStartDrag = new UnityEvent();
    public static readonly UnityEvent OnEndDrag = new UnityEvent();
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
        if(Destory.IsDestroying) _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
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
        tarpos = new Vector3(inipos.x + 1.2f, inipos.y, inipos.z);
    }

    void HandleAiDeAct()
    {
        tarpos = inipos;
    }
    
    private void OnMouseDown()
    {
        if (!IsDraging && !Destory.IsDestroying)
        {
            IsDraging = true;
            Draging = false;
            OnStartDrag.Invoke();
        }
        else
        {
            IsDraging = false;
            OnEndDrag.Invoke();
        }
    }
}
