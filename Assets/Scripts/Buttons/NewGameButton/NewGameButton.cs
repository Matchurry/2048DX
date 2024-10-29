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
    private SpriteRenderer _sr;
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        
    }
    private void OnMouseEnter()
    {
        _sr.color = new Color(1, 1, 1, 0.5f);
    }

    private void OnMouseExit()
    {
        _sr.color = new Color(1, 1, 1, 1);
    }

    private void OnMouseDown()
    {
        newGame.Invoke();
    }
}
