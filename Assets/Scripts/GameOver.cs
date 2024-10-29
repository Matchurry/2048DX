using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private SpriteRenderer _sr;
    private const float MoveAniDuration = 0.5f;
    private float startTime = 0f;
    private Color tarColor = new Color(1f, 1f, 1f, 0f);
    void Start()
    {
        tarColor = new Color(1f, 1f, 1f, 0f);
        _sr = gameObject.GetComponent<SpriteRenderer>();
        NewGameButton.endGame.AddListener(exist);
        NewGameButton.newGame.AddListener(fade);
    }
    void Update()
    {
        float progress = (Time.time - startTime) / MoveAniDuration;
        _sr.color = Color.Lerp(_sr.color, tarColor, progress);
    }
    private void exist()
    {
        tarColor = new Color(1, 1, 1, 1);
        startTime = Time.time;
    }
    private void fade()
    {
        tarColor = new Color(1, 1, 1, 0);
        startTime = Time.time;
    }
}
