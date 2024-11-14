using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUIUnitColorChange : MonoBehaviour
{
    private SpriteRenderer _sr;
    private ThemeSo _theme;
    public Config config;
    void Start()
    {
        _theme = config.themes[config.themeIndex];
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = _theme.themeButtonColor;
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }

    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeButtonColor;
    }
}
