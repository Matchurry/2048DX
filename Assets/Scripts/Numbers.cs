using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Numbers : MonoBehaviour
{
    public static readonly UnityEvent<int[]> OnDoubleNum = new UnityEvent<int[]>();
    private SpriteRenderer _sr;
    private float[] Unitmov = new float[] { -2.775f, -0.925f, 0.925f, 2.775f };
    public Sprite[] nums_st;
    public int num = -1;
    public int[] pos = new int[] { -9, 0 };

    private const float MoveAniDuration = 0.5f;
    private float startTime;
    
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        while (num == -1 && pos[0] != -9) {;}
        
        startTime = Time.time;
        gameObject.transform.position = new Vector2(Unitmov[pos[0]], Unitmov[3-pos[1]]);
        //Debug.Log((int)Math.Log(num, 2) - 1);
        _sr.sprite = nums_st[(int)Math.Log(num, 2) - 1];
        Camera.OnMoveUp.AddListener(Move);
        OnDoubleNum.AddListener(DoubleNum);
    }

    void Update()
    {
        float progress = (Time.time - startTime) / MoveAniDuration;
        transform.position =
            Vector2.Lerp(transform.position, new Vector2(Unitmov[pos[0]], Unitmov[3 - pos[1]]), progress);
        if (progress >= 1f)
        {
            transform.position = new Vector2(Unitmov[pos[0]], Unitmov[3 - pos[1]]);
        }
    }
    
    void Move(int[] args)
    {
        //先判断自身是不是前两个数据对应的东西 再移动
        //如果有融合 告诉目标位置的方块升级
        if (pos[0] == args[0] && pos[1] == args[1])
        {
            //这里更改目标位置
            Camera.map[args[0], args[1]] = 0; //自身位置改为0
            pos = new[] { args[2], args[3] };
            startTime = Time.time; //启动动画
            if (Camera.map[args[2], args[3]] != 0)
            {
                //通知目标位置加倍
                Camera.map[args[2], args[3]] = num * 2;
                //自身在0.5f后消失
                StartCoroutine(DelayedDestroy(args[2],args[3]));
            }
            else
            {
                Camera.map[args[2], args[3]] = num;
            }
        }
    }

    void DoubleNum(int[] args)
    {
        if (pos[0] == args[0] && pos[1] == args[1])
        {
            num *= 2;
            _sr.sprite = nums_st[(int)Math.Log(num, 2) - 1];
        }
    }
    /// <summary>
    /// 等待0.5f是为了等待动画结束 需要传入目标位置（加倍数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    IEnumerator DelayedDestroy(int x, int y)
    {
        yield return new WaitForSeconds(0.1f); 
        OnDoubleNum.Invoke(new[] { x, y});
        Destroy(gameObject);
    }
}
