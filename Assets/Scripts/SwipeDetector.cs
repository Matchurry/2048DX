using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 _startTouch;
    private Vector2 _endTouch;
    private readonly float _swipeThreshold = 50f; //滑动阈值，根据需要调整
    public static readonly UnityEvent<char> OnTouchInput = new UnityEvent<char>();
    private bool _validTouch = true;

    void Update()
    {
        if (Input.touchCount > 0 && !Destory.IsDestroying && !Drag.IsDraging)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startTouch = touch.position;
                _validTouch = true;
                if (_startTouch.y is >= 1870f or <= 275f) _validTouch = false;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if(!_validTouch) return;
                _endTouch = touch.position;
                float deltaX = _endTouch.x - _startTouch.x;
                float deltaY = _endTouch.y - _startTouch.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY) && Mathf.Abs(deltaX) > _swipeThreshold)
                {
                    if (deltaX > 0)
                    {
                        OnTouchInput.Invoke('d');
                    }
                    else
                    {
                        OnTouchInput.Invoke('a');
                    }
                }
                else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) && Mathf.Abs(deltaY) > _swipeThreshold)
                {
                    if (deltaY > 0)
                    {
                        OnTouchInput.Invoke('w');
                    }
                    else
                    {
                        OnTouchInput.Invoke('s');
                    }
                }
                _startTouch = Vector2.zero;
                _endTouch = Vector2.zero;
            }
        }
    }

}
