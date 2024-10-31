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
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnMouseDown();
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        }
        else if(Input.GetKeyUp(KeyCode.R))
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
        newGame.Invoke();
    }
}
