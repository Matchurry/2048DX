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
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Destory.IsDestroying) _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        else _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
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
