using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slider : MonoBehaviour
{
    private SpriteRenderer _sr;
    private ThemeSo _theme;
    public Config config;
    private Color _tarColor;
    
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        HandleThemeSwitch(0);
        Toggle.OnCheatChg.AddListener(HandleCheatChg);
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }

    private void Update()
    {
        _sr.color = Color.Lerp(_sr.color, _tarColor, 0.25f);
    }

    void HandleCheatChg()
    {
        if(Toggle.Cheat)
            _tarColor = Color.white;
        else
            _tarColor = _theme.themeButtonColor;
    }
    
    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        if(Toggle.Cheat)
            _tarColor = _theme.themeButtonColor;
    }

    private void OnMouseDown()
    {
        Toggle.OnCheatChg.Invoke();
    }
}
