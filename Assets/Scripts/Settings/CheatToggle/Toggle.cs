using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Toggle : MonoBehaviour
{
    public static readonly UnityEvent OnCheatChg = new UnityEvent();
    
    private SpriteRenderer _sr;
    private ThemeSo _theme;
    public Config config;
    
    private float _tarPosX = -1f/4f;
    
    public static bool Cheat = true;
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        HandleThemeSwitch(0);
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
        OnCheatChg.AddListener(HandleCheatChg);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.parent.position + new Vector3(_tarPosX,0,0), 0.25f);
    }

    void HandleCheatChg()
    {
        if (Cheat)
            _tarPosX = 1f/4f;
        else
            _tarPosX = -1f/4;
        Cheat = !Cheat;
    }

    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeBgUnitColors[1];
    }
}
