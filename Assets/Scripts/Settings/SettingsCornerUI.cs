using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsCornerUI : MonoBehaviour
{
    public static readonly UnityEvent OnSetting = new UnityEvent();
    public static bool SettingsFramework = false;
    private float _tarRz = 0;
    private ThemeSo _theme;
    public Config config;
    private SpriteRenderer _sr;
    
    void Start()
    {
        _theme = config.themes[config.themeIndex];
        _sr = GetComponent<SpriteRenderer>();
        _sr.color = _theme.themeButtonColor;
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
        SwipeDetector.OnTouchInput.AddListener(HandleCancel);
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.eulerAngles.z, _tarRz, 0.02f));
    }

    private void OnMouseDown()
    {
        SettingsFramework = !SettingsFramework;
        OnSetting.Invoke();
        if (SettingsFramework)
            _tarRz = 360f;
        else
            _tarRz = 30f;
    }
    
    void HandleThemeSwitch(int args)
    {
        _theme = config.themes[config.themeIndex];
        _sr.color = _theme.themeButtonColor;
    }

    void HandleCancel(char arg)
    {
        if(SettingsFramework)
            OnMouseDown();
    }
}
