using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    private SpriteRenderer _sr;
    private Vector3 tarS;
    private Color tarC;
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        tarS = transform.localScale;
        tarC = new Color(0.9882354f, 0.9411765f, 0.8941177f, 0);
        NewGameButton.newGame.AddListener(StartAni);
        Revocate.revocate.AddListener(StartAniForRevocate);
    }
    
    void Update()
    {
        _sr.color = Color.Lerp(_sr.color, tarC, 0.025f);
        transform.localScale = Vector3.Lerp(transform.localScale, tarS, 0.05f);
    }

    private void StartAni()
    {
        if(gameObject.name=="CircleForNewGame")
            StartCoroutine(StartAniIE());
    }

    IEnumerator StartAniIE()
    {
        tarC = new Color(0.9882354f, 0.9411765f, 0.8941177f,1);
        _sr.color = new Color(0.9882354f, 0.9411765f, 0.8941177f, 1);
        transform.localScale = new Vector3(0, 0, 0);
        tarS = new Vector3(30, 30, 30);
        yield return new WaitForSeconds(0.2f);
        tarC = new Color(0.9882354f, 0.9411765f, 0.8941177f,0);
    }

    private void StartAniForRevocate()
    {
        if(gameObject.name=="CircleForRevocate")
            StartCoroutine(StartAniForRevocateIE());
    }

    IEnumerator StartAniForRevocateIE()
    {
        _sr.color = new Color(0.9882354f, 0.9411765f, 0.8941177f, 1);
        transform.localScale = new Vector3(0, 0, 0);
        tarS = new Vector3(30, 30, 30);
        yield return new WaitForSeconds(0.3f);
        tarC = new Color(0.9882354f, 0.9411765f, 0.8941177f,0);
    }
}
