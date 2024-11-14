using System;
using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static bool can_shake = true;
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

    private void OnMouseDown()
    {
        if (can_shake)
        {
            can_shake = false;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 0.5f);
        }
        else
        {
            can_shake = true;
            _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b, 1f);
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
    }
    
    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeButtonColor;
    }
}
