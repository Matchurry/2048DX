using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
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
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    startTime = Time.time;
                    tarpos = new Vector3(inipos.x, inipos.y+0.25f, inipos.z);
                }
                else if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    startTime = Time.time;
                    tarpos = inipos;
                }
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
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x-0.25f, tarpos.y, tarpos.z);
                }
                else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
                {
                    startTime = Time.time;
                    tarpos = inipos;
                }
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
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x, tarpos.y-0.25f, tarpos.z);
                }
                else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
                {
                    startTime = Time.time;
                    tarpos = inipos;
                }
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
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    startTime = Time.time;
                    tarpos = new Vector3(tarpos.x+0.25f, tarpos.y, tarpos.z);
                }
                else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) 
                {
                    startTime = Time.time;
                    tarpos = inipos;
                }
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
}
