using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        try
        {
            string s =
                $"Auto-Game Ratio:\n" +
                $"PointsEarning:{Camera.ratiosSum[0] / Camera.ratiosSum.Sum():P0}\n" +
                $"Combination:{Camera.ratiosSum[1] / Camera.ratiosSum.Sum():P0}\n" +
                $"Stability:{Camera.ratiosSum[2] / Camera.ratiosSum.Sum():P0}  {Camera.StabilityCal(Camera.map)}";
            
            //string s =
            //    $"{Camera.ratiosSum[0]},{Camera.ratiosSum[1]},{Camera.ratiosSum[2]}";

            _tm.text = s;
        }
        catch
        {
            ;}
        //string s = $"Rounds:{Camera.wholeRounds} Wins:{Camera.winRounds}";
        /*for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                s += Camera.map[j, i].ToString()+" ";
            }
            s += "\n";
        }*/

    }
}
