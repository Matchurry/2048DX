using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private SpriteRenderer _sr;
    private const float MoveAniDuration = 0.5f;
    private Color tarColor = new Color(1f, 1f, 1f, 0f);
    void Start()
    {
        tarColor = new Color(1f, 1f, 1f, 0f);
        _sr = gameObject.GetComponent<SpriteRenderer>();
        NewGameButton.endGame.AddListener(exist);
        NewGameButton.newGame.AddListener(fade);
        Revocate.revocate.AddListener(fade);
    }
    void Update()
    {
        _sr.color = Color.Lerp(_sr.color, tarColor, 0.1f);
        if (Camera.can_A() || Camera.can_D() || Camera.can_W() || Camera.can_S()) tarColor = new Color(1, 1, 1, 0);
    }
    private void exist()
    {
        tarColor = new Color(1, 1, 1, 1);
    }
    private void fade()
    {
        tarColor = new Color(1, 1, 1, 0);
    }
}
