using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighestText : MonoBehaviour
{
    private TextMeshProUGUI _tm;
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        _tm.text = Camera.HighestScore.ToString();
        _tm.text = Camera.HighestScore.ToString();
        if (Camera.HighestScore >= 1000)
            _tm.fontSize = 72;
        else if (Camera.HighestScore >= 10000)
            _tm.fontSize = 56;
        else if (Camera.HighestScore >= 100000)
            _tm.fontSize = 32;
        else _tm.fontSize = 100;
    }
}
