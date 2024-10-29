using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    private TextMeshProUGUI _tm;
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
    }
    
    void Update()
    {
        _tm.text = Camera.Score.ToString();
        if (Camera.Score >= 1000)
            _tm.fontSize = 72;
        else if (Camera.Score >= 10000)
            _tm.fontSize = 56;
        else _tm.fontSize = 100;
    }
}
