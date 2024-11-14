using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG : MonoBehaviour
{
    private SpriteRenderer _sr;
    public Config config;
    private ThemeSo _theme;
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
        HandleThemeSwitch(0);
    }

    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        if(gameObject.name is "BG" or "Score" or "HighestScore")
            _sr.color = _theme.themeBgUnitColors[0];
        else
            _sr.color = _theme.themeBgUnitColors[1];
    }
}
