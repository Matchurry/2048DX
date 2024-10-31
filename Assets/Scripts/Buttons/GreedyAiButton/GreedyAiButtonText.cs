using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GreedyAiButtonText : MonoBehaviour
{
    private const float MoveAniDuration = 0.5f;
    private float startTime = 0f;
    private float Opa_startTime = 0f;
    private Color tarColor = new Color(1, 1, 1, 0.5f);
    private Vector3 inipos;
    private Vector3 tarpos;
    private TextMeshProUGUI _tm;
    void Start()
    {
        _tm = gameObject.GetComponent<TextMeshProUGUI>();
        tarpos = gameObject.transform.position;
        inipos = gameObject.transform.position;
        GreedyAiButton.greedyAiAct.AddListener(Acti);
        GreedyAiButton.greedyAiDeA.AddListener(DeActi);
    }
    
    void Update()
    {
        float progress = (Time.time - startTime) / MoveAniDuration;
        float opaProgress = (Time.time - Opa_startTime) / 0.5f;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, tarpos, progress);
        _tm.color = Color.Lerp(_tm.color, tarColor, opaProgress);
    }

    private void Acti()
    {
        StartCoroutine(ActiIE("Activated"));
    }

    private void DeActi()
    {
        StartCoroutine(ActiIE("Greedy AI"));
    }

    IEnumerator ActiIE(string s)
    {
        tarpos = new Vector3(inipos.x + 50, inipos.y, inipos.z);
        tarColor = new Color(1, 1, 1, 0);
        startTime = Time.time;
        Opa_startTime = Time.time;
        yield return new WaitForSeconds(0.25f);
        _tm.text = s;
        transform.position = new Vector3(inipos.x - 50, inipos.y, inipos.z);
        tarpos = inipos;
        tarColor = new Color(1, 1, 1, 1);
        startTime = Time.time;
        Opa_startTime = Time.time;
    }
}
