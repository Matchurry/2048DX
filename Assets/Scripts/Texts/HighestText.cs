using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class HighestText : MonoBehaviour
{
    private TextMeshProUGUI _tm;
    private float tarSize = 150;
    private const float MoveAniDuration = 1f;
    private float startTime;
    private Color tarC;
    
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
        tarC = _tm.color;
        startTime = Time.time;
        Numbers.OnDoubleNum.AddListener(Ani);
        NewGameButton.newGame.AddListener(StartAni);
        SettingsCornerUI.OnSetting.AddListener(SettingAni);
    }
    
    void Update()
    {
        _tm.color = Color.Lerp(_tm.color,tarC,0.15f);
        float progress = (Time.time - startTime) / MoveAniDuration;
        _tm.fontSize = Mathf.Lerp(_tm.fontSize, tarSize, progress);
        if(gameObject.name=="ScoreText")
            _tm.text = Camera.Score.ToString();
        else _tm.text = Camera.HighestScore.ToString();
        if (Camera.HighestScore >= 1000)
            tarSize = 100;
        else if (Camera.HighestScore >= 10000)
            tarSize = 85;
        else if (Camera.HighestScore >= 100000)
            tarSize = 45;
        else tarSize = 150;
    }

    private void Ani(int[] args)
    {
        if (gameObject.name != "ScoreText" && Camera.Score != Camera.HighestScore) return;
        _tm.fontSize += 30;
        startTime = Time.time;
    }

    private void StartAni()
    {
        StartCoroutine(StartAniIE());
    }

    private void SettingAni()
    {
        if(SettingsCornerUI.SettingsFramework)
            tarC = new Color(1, 1, 1, 0);
        else
            tarC = new Color(1, 1, 1, 1);
    }
    
    IEnumerator StartAniIE()
    {
        tarC = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(0.5f);
        tarC = new Color(1, 1, 1, 1);
    }
}
