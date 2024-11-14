using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ThemeUnit : MonoBehaviour
{
    public static readonly UnityEvent<int> OnThemeSwitch = new UnityEvent<int>();
    public Config config;
    private int _selfIndex;
    void Start()
    {
        _selfIndex = gameObject.name switch
        {
            "Basic" => 0,
            "Blue" => 1,
            "Green" => 2,
            _ => _selfIndex
        };
    }
    
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        if(config.themeIndex == _selfIndex) return;
        config.themeIndex = _selfIndex;
        //Debug.Log(_selfIndex+" "+config.themeIndex);
        OnThemeSwitch.Invoke(_selfIndex);
    }
}
