using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeDetector : MonoBehaviour
{
    private Vector2 startTouch;
    private Vector2 endTouch;
    private float swipeThreshold = 50f; //滑动阈值，根据需要调整
    public static readonly UnityEvent<char> OnTouchInput = new UnityEvent<char>();

    void Update()
    {
        if (Input.touchCount > 0 && !Destory.IsDestroying && !Drag.IsDraging)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouch = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouch = touch.position;
                float deltaX = endTouch.x - startTouch.x;
                float deltaY = endTouch.y - startTouch.y;

                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY) && Mathf.Abs(deltaX) > swipeThreshold)
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
                else if (Mathf.Abs(deltaY) > Mathf.Abs(deltaX) && Mathf.Abs(deltaY) > swipeThreshold)
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
                startTouch = Vector2.zero;
                endTouch = Vector2.zero;
            }
        }
    }

}
