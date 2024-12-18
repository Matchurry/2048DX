using System;
using System.Collections;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.Events;

public class Numbers : MonoBehaviour
{
    public static readonly UnityEvent<int[]> OnDoubleNum = new UnityEvent<int[]>();
    private static readonly UnityEvent<int[]> OnPureMove = new UnityEvent<int[]>();
    public Config config;
    private ThemeSo _colorTheme;
    public GameObject numBg;
    private GameObject _selfBg;
    private SpriteRenderer _selfBgSr;
    private SpriteRenderer _sr;
    private readonly float[] _unitmov = new float[] { -1.8f/1.22f, -0.6f/1.22f, 0.6f/1.22f, 1.8f/1.22f };
    public Sprite[] numsSt;
    public int num = -1;
    public int[] pos = new int[] { -9, 0 };
    private Vector3 _tarScale = new Vector3(0.65f,0.65f,0.65f);
    private Vector3 _tarpos = new Vector2();
    private const float MoveAniDuration = 0.5f;
    private bool _beDraging = false;
    
    void Start()
    {
        _sr = gameObject.GetComponent<SpriteRenderer>();
        while (num == -1 && pos[0] != -9) {;}
        _colorTheme = config.themes[config.themeIndex];
        _sr.color = _colorTheme.themeNumFontColors[Mathf.Min(2, (int)Math.Log(num, 2) - 1)];
        _selfBg = Instantiate(numBg,gameObject.transform);
        _selfBgSr = _selfBg.GetComponent<SpriteRenderer>();
        _selfBgSr.color = _colorTheme.themeNumBgColors[(int)Math.Log(num, 2) - 1];
        gameObject.transform.position = new Vector2(_unitmov[pos[0]], _unitmov[3-pos[1]]);
        _sr.sprite = numsSt[(int)Math.Log(num, 2) - 1];
        Camera.OnMoveUp.AddListener(Move);
        OnDoubleNum.AddListener(DoubleNum);
        OnPureMove.AddListener(HandlePureMove);
        NewGameButton.newGame.AddListener(newGame);
        Revocate.revocate.AddListener(newGame);
        Destory.OnEndDestory.AddListener(HandleEndDestory);
        Drag.OnEndDrag.AddListener(HandleEndDestory);
        ThemeUnit.OnThemeSwitch.AddListener(HandleThemeSwitch);
    }

    void Update()
    {
        if (Destory.IsDestroying || (Drag.IsDraging && !Drag.Draging)) transform.rotation = new Quaternion(transform.rotation.x,transform.rotation.y,
                                                                      Mathf.Sin(Time.time*20)*0.05f,transform.rotation.w);
        if (!Drag.IsDraging)
        {
            _tarpos = new Vector2(_unitmov[pos[0]], _unitmov[3 - pos[1]]);
            transform.position =
                Vector2.Lerp(transform.position,_tarpos, 0.25f);
        }
        else
        {
            transform.position =
                Vector2.Lerp(transform.position,_tarpos, 0.35f);
        }
        
        transform.localScale =
            Vector3.Lerp(transform.localScale, _tarScale, 0.25f);

        if (_beDraging && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
            {
                var endpos = UnityEngine.Camera.main.ScreenToWorldPoint(touch.position);
                var tarunitpos = new int[] { 0, 3 };
                if (endpos.x < _unitmov[0] - 0.5 || endpos.x > _unitmov[3] + 0.5 || endpos.y < _unitmov[0] - 0.5 ||
                    endpos.y > _unitmov[3] + 0.5)
                {
                    //在此结束Drag
                    _beDraging = false;
                    Drag.IsDraging = false;
                    _sr.sortingOrder = 2;
                    _selfBgSr.sortingOrder = 1;
                    Drag.OnEndDrag.Invoke();
                    return;
                }
                for (var i = 0; i < 3; i++)
                {
                    if (endpos.x >= _unitmov[i] + 0.5f)
                        tarunitpos[0]++;
                    if (endpos.y >= _unitmov[i] + 0.5f)
                        tarunitpos[1]--;
                }
                //Debug.Log(tarunitpos[0]+" "+tarunitpos[1]);
                if (Camera.map[tarunitpos[0], tarunitpos[1]] == 0)
                {
                    Camera.map[pos[0], pos[1]] = 0;
                    Camera.map[tarunitpos[0], tarunitpos[1]] = num;
                    pos = tarunitpos;
                }
                else if (Camera.map[tarunitpos[0], tarunitpos[1]] == num)
                {
                    //这边执行对应方块直接加倍的操作
                    OnDoubleNum.Invoke(new[] {tarunitpos[0], tarunitpos[1]});
                    Camera.map[tarunitpos[0], tarunitpos[1]] *= 2;
                    Camera.Score += num * 2;
                    Camera.map[pos[0], pos[1]] = 0;
                    pos = tarunitpos;
                    //自身播个动画消失
                    Destroy(gameObject);
                }
                else
                {
                    //这边与目标位置交换
                    //先操作完map
                    var temp = Camera.map[tarunitpos[0], tarunitpos[1]];
                    Camera.map[pos[0], pos[1]] = temp;
                    Camera.map[tarunitpos[0], tarunitpos[1]] = num;
                    //通知目标格子过来
                    OnPureMove.Invoke(new int[]{tarunitpos[0], tarunitpos[1], pos[0], pos[1]});
                    pos = tarunitpos;
                }
                Drag.IsDraging = false;
                _beDraging = false;
                _sr.sortingOrder = 3;
                _selfBgSr.sortingOrder = 2;
                Drag.OnEndDrag.Invoke();
            }
        }
    }

    void HandlePureMove(int[] args)
    {
        if (pos[0] == args[0] && pos[1] == args[1])
        {
            pos = new[] { args[2], args[3] };
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
            transform.localScale = new Vector3(1f, 1f, 1f);
            num *= 2;
            if (num == 2048) Camera.winRounds++;
            _sr.sprite = numsSt[(int)Math.Log(num, 2) - 1];
            _sr.color = _colorTheme.themeNumFontColors[Mathf.Min(2, (int)Math.Log(num, 2) - 1)];
            _selfBg.GetComponent<SpriteRenderer>().color = _colorTheme.themeNumBgColors[(int)Math.Log(num, 2) - 1];
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
        Camera.combined[x, y] = true;
        yield return new WaitForSeconds(0.025f); 
        OnDoubleNum.Invoke(new[] { x, y});
        Camera.Score += num;
        Destroy(gameObject);
    }

    private void newGame()
    {
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        if (Destory.IsDestroying)
        {
            if(Shake.can_shake)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            _tarScale = new Vector3(0, 0, 0);
            Camera.map[pos[0], pos[1]] = 0;
            StartCoroutine(WaitAndDestory());
            Destory.OnEndDestory.Invoke();
            Destory.IsDestroying = false;
        }
    }

    private void OnMouseDrag()
    {
        if (Drag.IsDraging && !Drag.Draging)
        {
            _sr.sortingOrder = 4;
            _selfBgSr.sortingOrder = 3;
            Camera.map[pos[0], pos[1]] = 0;
            Drag.Draging = true;
            _beDraging = true;
            Drag.OnEndDrag.Invoke();
        }
        if (_beDraging)
        {
            var pos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            _tarpos = new Vector3(pos.x, pos.y, 0);
        }
    }

    private void HandleEndDestory()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    IEnumerator WaitAndDestory()
    {
        yield return new WaitForSeconds(0.2f);
        var cnt = 0;
        for(var i=0; i<4; i++)
            for(var j=0; j<4; j++)
                if (Camera.map[i, j] == 0)
                    cnt++;
        if(cnt==16) NewGameButton.newGame.Invoke();
        Destroy(gameObject);
    }
    
    void HandleThemeSwitch(int args)
    {
        _colorTheme = config.themes[config.themeIndex];
        _sr.color = _colorTheme.themeNumFontColors[Mathf.Min(2, (int)Math.Log(num, 2) - 1)];
        _selfBgSr.color = _colorTheme.themeNumBgColors[(int)Math.Log(num, 2) - 1];
    }
}
