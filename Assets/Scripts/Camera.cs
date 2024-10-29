using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Camera : MonoBehaviour
{
    [FormerlySerializedAs("can_move")] public bool canMove = false;
    public static readonly UnityEvent<int> OnSummonNewNum = new UnityEvent<int>();
    public static readonly UnityEvent<int[]> OnMoveUp = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveLeft = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveDown = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveRight = new UnityEvent<int[]>();
    public GameObject num;
    public static int[,] map = new int[4, 4];
    public static int Score = 0;
    public static bool is_AIControl = false;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }

    void Start()
    {
        OnSummonNewNum.AddListener(SummonNum);
        NewGameButton.newGame.AddListener(newGame);
        newGame();
    }
    void Update()
    {
        if (canMove && !is_AIControl)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && can_W())
            {
                Move('w');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && can_A())
            {
                Move('a');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && can_S())
            {
                Move('s');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && can_D())
            {
                Move('d');
                canMove = false;
                StartCoroutine(NextTurn());
            }
        }
        
        if (canMove && is_AIControl)
        {
            var cons = new char[] { 'w', 'a', 's', 'd' };
            Move(cons[AIDesicion()]);
            canMove = false;
            StartCoroutine(NextTurn());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NewGameButton.endGame.Invoke();
        }

        if (can_A() || can_D() || can_W() || can_S()) return;
        NewGameButton.endGame.Invoke();
    }

    private void newGame()
    {
        canMove = false;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                map[i, j] = 0;
        Score = 0;
        OnSummonNewNum.Invoke(2);
        StopAllCoroutines();
        StartCoroutine(WaitAndStart(0.5f));
    }

    IEnumerator WaitAndStart(float t)
    {
        yield return new WaitForSeconds(t);
        canMove = true;
    }
    
    void Move(char c)
    {
        switch (c)
        {
            case 'w':
                //向上移动 包含map更改
                //使用事件指定方块移动
                for (var i = 0; i < 4; i++) //走列
                {
                    for (var j = 1; j < 4; j++) //走行 第一行跳过
                    {
                        if (map[i, j] == 0) continue;
                        //往上走 空就继续往上 遇到方块停止 检查目标位置
                        var tar = j;
                        while (tar - 1 >= 0 && (map[i, tar - 1] == 0 || map[i, tar - 1] == map[i, j]))  tar--;
                        if(tar==j) continue;
                        //此时tar为目标位置 可以发送移动指令
                        OnMoveUp.Invoke(new[] { i, j ,i, tar}); //前两个是目标数值位置 后两个是其目标位置
                    }
                }
                break;
            case 'a':
                for (var j = 0; j < 4; j++) //走行
                {
                    for (var i = 1; i < 4; i++) //走列 第一列跳过
                    {
                        if (map[i, j] == 0) continue;
                        var tar = i;
                        while (tar - 1 >= 0 && (map[tar-1, j] == 0 || map[tar-1, j] == map[i, j]))  tar--;
                        if(tar==i) continue;
                        OnMoveUp.Invoke(new[] { i, j ,tar, j});
                    }
                }
                break;
            case 's':
                for (var i = 0; i < 4; i++) //走列
                {
                    for (var j = 3; j >= 0 ; j--) //走行 最后一行跳过
                    {
                        if (map[i, j] == 0) continue;
                        var tar = j;
                        while (tar + 1 < 4 && (map[i, tar + 1] == 0 || map[i, tar + 1] == map[i, j]))  tar++;
                        if(tar==j) continue;
                        OnMoveUp.Invoke(new[] { i, j ,i, tar});
                    }
                }
                break;
            case 'd':
                for (var j = 0; j < 4; j++) //走行
                {
                    for (var i = 3; i >= 0; i--) //走列 最后一列跳过
                    {
                        if (map[i, j] == 0) continue;
                        var tar = i;
                        while (tar + 1 < 4 && (map[tar+1, j] == 0 || map[tar+1, j] == map[i, j]))  tar++;
                        if(tar==i) continue;
                        OnMoveUp.Invoke(new[] { i, j ,tar, j});
                    }
                }
                break;
            default:
                break;
        }
    }
    
    public static bool can_W()
    {
        for (var i = 0; i < 4; i++) //走列
        {
            for (var j = 1; j < 4; j++) //走行 第一行跳过
            {
                if (map[i, j] == 0) continue;
                var tar = j;
                while (tar - 1 >= 0 && (map[i, tar - 1] == 0 || map[i, tar - 1] == map[i, j]))  tar--;
                if(tar==j) continue;
                //此时tar为目标位置 意味着其实可以执行
                return true;
            }
        }
        return false;
    }

    public static bool can_A()
    {
        for (var j = 0; j < 4; j++) //走行
        {
            for (var i = 1; i < 4; i++) //走列 第一列跳过
            {
                if (map[i, j] == 0) continue;
                var tar = i;
                while (tar - 1 >= 0 && (map[tar-1, j] == 0 || map[tar-1, j] == map[i, j]))  tar--;
                if(tar==i) continue;
                return true;
            }
        }
        return false;
    }

    public static bool can_S()
    {
        for (var i = 0; i < 4; i++) //走列
        {
            for (var j = 3; j >= 0 ; j--) //走行 最后一行跳过
            {
                if (map[i, j] == 0) continue;
                var tar = j;
                while (tar + 1 < 4 && (map[i, tar + 1] == 0 || map[i, tar + 1] == map[i, j]))  tar++;
                if(tar==j) continue;
                return true;
            }
        }
        return false;
    }

    public static bool can_D()
    {
        for (var j = 0; j < 4; j++) //走行
        {
            for (var i = 3; i >= 0; i--) //走列 最后一列跳过
            {
                if (map[i, j] == 0) continue;
                var tar = i;
                while (tar + 1 < 4 && (map[tar+1, j] == 0 || map[tar+1, j] == map[i, j]))  tar++;
                if(tar==i) continue;
                return true;
            }
        }
        return false;
    }
    
    void SummonNum(int times)
    {
        for (var i = 0; i < times; i++)
        {
            var cnt = 0;
            foreach (var item in map)
                if (item == 0) cnt++;
            if(cnt==0) continue;
            
            GameObject newNum = Instantiate(num);
            var _sc = newNum.GetComponent<Numbers>();
            var tarPos = new int[]{Random.Range(0,4),Random.Range(0,4)};
            do
            {
                tarPos = new int[]{Random.Range(0, 4),Random.Range(0,4)};
            } while (map[tarPos[0], tarPos[1]] != 0);
            _sc.pos = tarPos;
            var type = Random.Range(0, 10);
            if (type == 9) _sc.num = 4;
            else _sc.num = 2;
            map[tarPos[0], tarPos[1]] = _sc.num;
        }
    }

    /// <summary>
    /// 这个函数将返回执行操作后的得分 不影响原map
    /// </summary>
    /// <para name="c">操作字符</para>
    /// <param name="m">传入map</param>
    /// <returns></returns>
    int ScoreCalPredict(char c,int[,] _m)
    {
        var m = new int[4][];
        for (int index = 0; index < 4; index++)
        {
            m[index] = new int[4];
        }

        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
                m[i][j] = _m[i, j];
        int cal = 0;
        switch (c)
        {
            case 'w':
                for (var i = 0; i < 4; i++)
                    for (var j = 1; j < 4; j++)
                    {
                        if (m[i][j] == 0) continue;
                        var tar = j;
                        while (tar - 1 >= 0 && (m[i][tar - 1] == 0 || m[i][tar - 1] == m[i][j]))  tar--;
                        if(tar==j) continue;
                        if (m[i][tar] == 0) //目标格为0
                            m[i][tar] = m[i][j];
                        else //目标格有数字
                        {
                            m[i][tar] *= 2;
                            cal += m[i][tar];
                        }
                        m[i][j] = 0; //原格子删除
                    }
                break;
            case 'a':
                for (var j = 0; j < 4; j++)
                    for (var i = 1; i < 4; i++)
                    {
                        if (m[i][j] == 0) continue;
                        var tar = i;
                        while (tar - 1 >= 0 && (m[tar-1][j] == 0 || m[tar-1][j] == m[i][j]))  tar--;
                        if(tar==i) continue;
                        if (m[tar][j] == 0)
                            m[tar][j] = m[i][j];
                        else
                        {
                            m[tar][j] *= 2;
                            cal += m[tar][j];
                        }
                        m[i][j] = 0;
                    }
                break;
            case 's':
                for (var i = 0; i < 4; i++)
                    for (var j = 3; j >= 0 ; j--)
                    {
                        if (m[i][j] == 0) continue;
                        var tar = j;
                        while (tar + 1 < 4 && (m[i][tar + 1] == 0 || m[i][tar + 1] == m[i][j]))  tar++;
                        if(tar==j) continue;
                        if (m[i][tar] == 0)
                            m[i][tar] = m[i][j];
                        else
                        {
                            m[i][tar] *= 2;
                            cal += m[i][tar];
                        }
                        m[i][j] = 0;
                    }
                break;
            case 'd':
                for (var j = 0; j < 4; j++)
                    for (var i = 3; i >= 0; i--)
                    {
                        if (m[i][j] == 0) continue;
                        var tar = i;
                        while (tar + 1 < 4 && (m[tar+1][j] == 0 || m[tar+1][j] == m[i][j]))  tar++;
                        if(tar==i) continue;
                        if (m[tar][j] == 0)
                            m[tar][j] = m[i][j];
                        else
                        {
                            m[tar][j] *= 2;
                            cal += m[tar][j];
                        }
                        m[i][j] = 0;
                    }
                break;
        }
        return cal;
    }
    
    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(0.08f);
        SummonNum(1);
        yield return new WaitForSeconds(0.02f);
        canMove = true;
    }
    
    /// <summary>
    /// 基于贪心思想的AI决策
    /// </summary>
    /// <returns>返回0，1，2，3表示wasd：上左下右</returns>
    int AIDesicion()
    {
        var pres = new int[]
        {
            ScoreCalPredict('w', map),ScoreCalPredict('a',map),
            ScoreCalPredict('s',map),ScoreCalPredict('d',map)
        };
        var highest = 0;
        var control = 0;
        for (var i = 0; i < 4; i++)
        {
            if (pres[i] > highest)
            {
                highest = pres[i];
                control = i;
            }
        }
        return control;
    }
}
