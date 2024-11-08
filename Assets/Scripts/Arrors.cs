using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class Arrors : MonoBehaviour
{
    private SpriteRenderer _sr;
    private const float MoveAniDuration = 0.5f;
    private float startTime = 0f;
    private float Opa_startTime = 0f;
    private Color tarColor = new Color(0.4433962f, 0.3990566f, 0.3990566f, 0.5f);
    private Vector3 inipos;
    private Vector3 tarpos;
    void Start()
    {
        SwipeDetector.OnTouchInput.AddListener(Onmove);
        _sr = gameObject.GetComponent<SpriteRenderer>();
        tarpos = gameObject.transform.position;
        inipos = gameObject.transform.position;
    }
    
    void Update()
    {
        float progress = (Time.time - startTime) / MoveAniDuration;
        float opaProgress = (Time.time - Opa_startTime) / 0.1f;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, tarpos, progress);
        _sr.color = Color.Lerp(_sr.color, tarColor, opaProgress);
        
        switch (gameObject.name)
        {
            case "ArrorUp":
                if (Camera.can_W())
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 1);
                }
                else
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 0.25f);
                }
                break;
            case "ArrorLeft":
                if (Camera.can_A())
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 1);
                }
                else
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 0.25f);
                }
                break;
            case "ArrorDown":
                if (Camera.can_S())
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 1);
                }
                else
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 0.25f);
                }
                break;
            case "ArrorRight":
                if (Camera.can_D())
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 1);
                }
                else
                {
                    Opa_startTime = Time.time;
                    tarColor = new Color(tarColor.r, tarColor.g, tarColor.b, 0.25f);
                }
                break;
        }
    }

    private void Onmove(char c)
    {
        StartCoroutine(HandleOnMove(c));
    }

    IEnumerator HandleOnMove(char c)
    {
        switch (gameObject.name)
        {
            case "ArrorUp":
                if (c=='w')
                {
                    startTime = Time.time;
                    tarpos = new Vector3(inipos.x, inipos.y+0.25f, inipos.z);
                    yield return new WaitForSeconds(0.15f);
                    startTime = Time.time;
                    tarpos = inipos;
                }
                break;
            case "ArrorLeft":
                if (c=='a')
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x-0.25f, tarpos.y, tarpos.z);
                    yield return new WaitForSeconds(0.15f);
                    startTime = Time.time;
                    tarpos = inipos;
                }
                break;
            case "ArrorDown":
                if (c=='s')
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x, tarpos.y-0.25f, tarpos.z);
                    yield return new WaitForSeconds(0.15f);
                    startTime = Time.time;
                    tarpos = inipos;
                }
                break;
            case "ArrorRight":
                if (c=='d')
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x+0.25f, tarpos.y, tarpos.z);
                    yield return new WaitForSeconds(0.15f);
                    startTime = Time.time;
                    tarpos = inipos;
                }
                break;
        }
    }
}
