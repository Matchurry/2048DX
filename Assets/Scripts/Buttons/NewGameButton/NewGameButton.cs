using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class NewGameButton : MonoBehaviour
{
    public static readonly UnityEvent newGame = new UnityEvent();
    public static readonly UnityEvent endGame = new UnityEvent();
    private Vector3 inipos;
    private Vector3 tarpos;
    private float tarR_Y = 0;
    private SpriteRenderer _sr;
    void Start()
    {
        inipos = transform.position;
        tarpos = inipos;
        _sr = gameObject.GetComponent<SpriteRenderer>();
        GreedyAiButton.greedyAiAct.AddListener(HandbleAiAct);
        GreedyAiButton.greedyAiDeA.AddListener(HandleAiDeAct);
        newGame.AddListener(newGameAni);
        Destory.OnEndDestory.AddListener(OnMouseUp);
        Drag.OnEndDrag.AddListener(OnMouseUp);
    }
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, Mathf.Lerp(transform.rotation.eulerAngles.y, tarR_Y, 0.05f), 0);
        transform.position = Vector3.Lerp(transform.position, tarpos, 0.1f);
        if(Destory.IsDestroying || Drag.IsDraging)
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
    }

    void HandbleAiAct()
    {
        tarpos = new Vector3(inipos.x - 1.2f, inipos.y, inipos.z);
    }

    void HandleAiDeAct()
    {
        tarpos = inipos;
    }

    private void OnMouseDown()
    {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
        if(!Destory.IsDestroying && !Drag.IsDraging)
            newGame.Invoke();
    }

    private void OnMouseUp()
    {
        _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1);
    }

    private void newGameAni()
    {
        transform.rotation = new Quaternion(0, 0, 0,0);
        tarR_Y = 359f;
        //StartCoroutine(newGameAniIE());
    }
}
