using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonThemeCg : MonoBehaviour
{
    private SpriteRenderer _sr;
    public Config config;
    private ThemeSo _theme;
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        HandleThemeSwitch(0);
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }
    
    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeButtonColor;
    }
}
