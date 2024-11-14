using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThemePointer : MonoBehaviour
{

    void Start()
    {
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }

    void HandleThemeSwitch(int arg)
    {
        switch (arg)
        {
            case 0:
                transform.position = transform.parent.position + new Vector3(2.82f/4, 0, 0);
                break;
            case 1:
                transform.position = transform.parent.position + new Vector3(5.82f/4, 0, 0);
                break;
            case 2:
                transform.position = transform.parent.position + new Vector3(8.82f/4, 0, 0);
                break;
        }
    }
}
