using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Revocate : MonoBehaviour
{
    private SpriteRenderer _sr;
    public static readonly UnityEvent revocate = new UnityEvent();
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && Camera.round!=1)
        {
            OnMouseDown();
            _sr.color = new Color(0.5490196f, 0.5137255f, 0.5137255f, 0.5f);
        }
        else if(Input.GetKeyUp(KeyCode.P))
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
        if(Camera.round!=1)
            revocate.Invoke();
    }
}
