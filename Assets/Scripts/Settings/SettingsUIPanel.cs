using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsUIPanel : MonoBehaviour
{
    private float _tarPosY = 6.5f;
    private ThemeSo _theme;
    public Config config;
    private SpriteRenderer _sr;
    void Start()
    {
        _theme = config.themes[config.themeIndex];
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = _theme.themeBgUnitColors[1];
        SettingsCornerUI.OnSetting.AddListener(OnAni);
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }
    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,
            new Vector3(transform.position.x, _tarPosY, transform.position.z), 0.1f);
    }

    void OnAni()
    {
        if (!SettingsCornerUI.SettingsFramework)
        {
            _tarPosY = 6.5f;
        }
        else
        {
            _tarPosY = 3.5f;
        }
    }
    
    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeBgUnitColors[1];
    }
}
