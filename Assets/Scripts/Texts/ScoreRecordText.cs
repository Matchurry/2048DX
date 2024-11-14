using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreRecordText : MonoBehaviour
{
    private TextMeshProUGUI _tm;
    private Color tarC;
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
        tarC = _tm.color;
        SettingsCornerUI.OnSetting.AddListener(SettingAni);
    }

    void Update()
    {
        _tm.color = Color.Lerp(_tm.color,tarC,0.15f);
    }
    
    private void SettingAni()
    {
        if(SettingsCornerUI.SettingsFramework)
            tarC = new Color(1, 1, 1, 0);
        else
            tarC = new Color(1, 1, 1, 1);
    }
}
