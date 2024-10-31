using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MapText : MonoBehaviour
{
    private TextMeshProUGUI _tm;
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string s = $"Rounds:{Camera.wholeRounds} Wins:{Camera.winRounds}";
        /*for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                s += Camera.map[j, i].ToString()+" ";
            }
            s += "\n";
        }*/
        _tm.text = s;
    }
}
