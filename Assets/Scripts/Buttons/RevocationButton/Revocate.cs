using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Revocate : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Vector3 inipos;
    private Vector3 tarpos;
    private float tarR_Y = 0;
    public static readonly UnityEvent revocate = new UnityEvent();
    void Start()
    {
        inipos = transform.position;
        tarpos = inipos;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        GreedyAiButton.greedyAiAct.AddListener(HandbleAiAct);
        GreedyAiButton.greedyAiDeA.AddListener(HandleAiDeAct);
        revocate.AddListener(RevocateAni);
        Destory.OnEndDestory.AddListener(OnMouseUp);
        Drag.OnEndDrag.AddListener(OnMouseUp);
    }
    
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Mathf.Lerp(transform.rotation.eulerAngles.y, tarR_Y, 0.05f), 0);
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Destory.IsDestroying || Drag.IsDraging)
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
    }
    void HandbleAiAct()
    {
        tarpos = new Vector3(inipos.x - 2, inipos.y, inipos.z);
    }

    void HandleAiDeAct()
    {
        tarpos = inipos;
    }

    private void OnMouseDown()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        if(Camera.round!=1 && !Destory.IsDestroying && !Drag.IsDraging)
            revocate.Invoke();
    }

    private void OnMouseUp()
    {
        _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 1);
    }

    private void RevocateAni()
    {
        transform.rotation = new Quaternion(0, 0, 0,0);
        tarR_Y = 359f;
    }
}
