using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Star : MonoBehaviour
{
    public static readonly UnityEvent StarSave = new UnityEvent();
    private SpriteRenderer _sr;
    private Vector3 _tarSc = Vector3.one * 0.25f;
    public static bool Cheated = false;
    void Start()
    {
        Cheated = PlayerPrefs.GetInt("Cheated") == 1;
        if (Cheated) OnCheated();
        _sr = GetComponent<SpriteRenderer>();
        Numbers.OnDoubleNum.AddListener(Ani);
        Destory.OnStartDestory.AddListener(OnCheated);
        Drag.OnStartDrag.AddListener(OnCheated);
        Revocate.revocate.AddListener(OnCheated);
        GreedyAiButton.greedyAiAct.AddListener(OnCheated);
        NewGameButton.newGame.AddListener(HandleNewGame);
    }
    void Update()
    {
        transform.rotation = new Quaternion(transform.rotation.x,transform.rotation.y,
            Mathf.Sin(Time.time*2)*0.2f,transform.rotation.w);

        transform.localScale = Vector3.Lerp(transform.localScale, _tarSc , 0.05f);
    }
    void Ani(int[] args)
    {
        if(!Cheated) transform.localScale += Vector3.one * 0.05f;
    }
    void OnCheated()
    {
        Cheated = true;
        _tarSc = Vector3.zero;
        StarSave.Invoke();
    }
    void HandleNewGame()
    {
        Cheated = false;
        _tarSc = Vector3.one * 0.25f;
    }
}
