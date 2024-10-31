using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
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
    public static int[,] oldmap = new int[4, 4];
    public static bool[,] combined = new bool[4, 4];
    public static int old_score;
    public static int round = 1;
    public static int Score = 0;
    public static int HighestScore = 35268;
    public static char[] controls = new char[]{'w','a','s','d'};
    public static bool is_AIControl = false;

    public static int wholeRounds = 0;
    public static int winRounds = 0;
    //用于存储自动游戏存储决策
    private char AiDeci = 'n'; 
    // 用于判断是否正在计算 MiniMaxDecision_Launch
    private bool isCalculating = false;

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
    }

    void Start()
    {
        OnSummonNewNum.AddListener(SummonNum);
        NewGameButton.newGame.AddListener(newGame);
        Revocate.revocate.AddListener(revocate);
        NewGameButton.endGame.AddListener(AutoNewGame);
        newGame();
    }
    void Update()
    {
        if (isCalculating) return;
        HighestScore = Math.Max(Score, HighestScore);
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

        if (canMove && is_AIControl && !isCalculating)
        {
            isCalculating = true; 
            StartCoroutine(CalculateMiniMaxDecision(map)); 
        }
        
        if (is_AIControl && AiDeci != 'n' && !isCalculating) 
        {
            if(can_X(AiDeci))
                Move(AiDeci);
            else 
                NewGameButton.endGame.Invoke();
            AiDeci = 'n';
            canMove = false;
            StartCoroutine(NextTurn());
        }

        if (can_A() || can_D() || can_W() || can_S()) return;
        var cnt = 0;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                if (map[i, j] == 0)
                    cnt++;
        if (cnt == 16) return;
        NewGameButton.endGame.Invoke();
    }
    
    private IEnumerator CalculateMiniMaxDecision(int[,] m)
    {
        AiDeci = controls[MiniMaxDecision_Launch(m)];
        isCalculating = false; 
        yield return null;
    }
    
    private void newGame()
    {
        StopAllCoroutines();
        canMove = false;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                map[i, j] = 0;
                combined[i, j] = false;
            }
        Score = 0;
        wholeRounds++;
        OnSummonNewNum.Invoke(2);
        StartCoroutine(WaitAndStart(0.5f));
    }

    private void AutoNewGame()
    {
        StopAllCoroutines();
        Wait(1f);
        NewGameButton.newGame.Invoke();
    }
    
    IEnumerator WaitAndStart(float t)
    {
        yield return new WaitForSeconds(t);
        canMove = true;
    }
    
    IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
    }
    
    void Move(char c)
    {
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                oldmap[i, j] = map[i, j]; //保存当前状态用以撤销
                combined[i, j] = false; //清空是否合并地图组
            }
        old_score = Score; //保存分数用以撤销
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
                        while (tar - 1 >= 0 && (map[i, tar - 1] == 0 || map[i, tar - 1] == map[i, j] && !combined[i,j]))  tar--;
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
                        while (tar - 1 >= 0 && (map[tar-1, j] == 0 || map[tar-1, j] == map[i, j] && !combined[i,j]))  tar--;
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
                        while (tar + 1 < 4 && (map[i, tar + 1] == 0 || map[i, tar + 1] == map[i, j] && !combined[i,j]))  tar++;
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
                        while (tar + 1 < 4 && (map[tar+1, j] == 0 || map[tar+1, j] == map[i, j] && !combined[i,j]))  tar++;
                        if(tar==i) continue;
                        OnMoveUp.Invoke(new[] { i, j ,tar, j});
                    }
                }
                break;
            default:
                break;
        }
    }

    public static bool can_X(char c, int[,] m)
    {
        switch (c)
        {
            case 'a':
                return can_A(m);
            case 'w':
                return can_W(m);
            case 's':
                return can_S(m);
            case 'd':
                return can_D(m);
            default:
                return false;
        }
    }
    public static bool can_X(char c)
    {
        return can_X(c,map);
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
    public static bool can_W(int[,] m)
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 1; j < 4; j++)
            {
                if (m[i, j] == 0) continue;
                var tar = j;
                while (tar - 1 >= 0 && (m[i, tar - 1] == 0 || m[i, tar - 1] == m[i, j]))  tar--;
                if(tar==j) continue;
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
    public static bool can_A(int[,] m)
    {
        for (var j = 0; j < 4; j++) //走行
        {
            for (var i = 1; i < 4; i++) //走列 第一列跳过
            {
                if (m[i, j] == 0) continue;
                var tar = i;
                while (tar - 1 >= 0 && (m[tar-1, j] == 0 || m[tar-1, j] == m[i, j]))  tar--;
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
    public static bool can_S(int[,] m)
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 3; j >= 0 ; j--)
            {
                if (m[i, j] == 0) continue;
                var tar = j;
                while (tar + 1 < 4 && (m[i, tar + 1] == 0 || m[i, tar + 1] == m[i, j]))  tar++;
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
    public static bool can_D(int[,] m)
    {
        for (var j = 0; j < 4; j++) //走行
        {
            for (var i = 3; i >= 0; i--) //走列 最后一列跳过
            {
                if (m[i, j] == 0) continue;
                var tar = i;
                while (tar + 1 < 4 && (m[tar+1, j] == 0 || m[tar+1, j] == m[i, j]))  tar++;
                if(tar==i) continue;
                return true;
            }
        }
        return false;
    }
    
    private void SummonNum(int times)
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
    /// 生成对应数量个新数字 仅用于模拟至map
    /// </summary>
    private void SummonNumForSim(int times, int[,] _m)
    {
        for (var i = 0; i < times; i++)
        {
            var cnt = 0;
            foreach (var item in _m)
                if (item == 0) cnt++;
            if(cnt==0) continue;
            
            var tarPos = new int[]{Random.Range(0,4),Random.Range(0,4)};
            do
            {
                tarPos = new int[]{Random.Range(0, 4),Random.Range(0,4)};
            } while (_m[tarPos[0], tarPos[1]] != 0);
            var type = Random.Range(0, 10);
            if (type == 9) _m[tarPos[0], tarPos[1]] = 4;
            _m[tarPos[0], tarPos[1]] = 2;
        }
    }

    /// <summary>
    /// 这个函数将返回执行操作后的得分
    /// </summary>
    /// <param name="c">操作字符</param>
    /// <param name="_m">传入map</param>
    /// <param name="mapped">是否影响传入的map</param>
    /// <returns></returns>
    int ScoreCalPredict(char c,int[,] _m, bool mapped)
    {
        var m = new int[4,4];
        if (!mapped)
        {
            for(var i=0; i<4; i++)
                for (var j = 0; j < 4; j++)
                    m[i, j] = _m[i, j];
        }
        else m = _m;

        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
                m[i,j] = _m[i, j];
        int cal = 0;
        switch (c)
        {
            case 'w':
                if (!can_W()) break;
                for (var i = 0; i < 4; i++)
                    for (var j = 1; j < 4; j++)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = j;
                        while (tar - 1 >= 0 && (m[i,tar - 1] == 0 || m[i,tar - 1] == m[i,j]))  tar--;
                        if(tar==j) continue;
                        if (m[i,tar] == 0) //目标格为0
                            m[i,tar] = m[i,j];
                        else //目标格有数字
                        {
                            m[i,tar] *= 2;
                            cal += m[i,tar];
                        }
                        m[i,j] = 0; //原格子删除
                    }
                break;
            case 'a':
                if (!can_A()) break;
                for (var j = 0; j < 4; j++)
                    for (var i = 1; i < 4; i++)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = i;
                        while (tar - 1 >= 0 && (m[tar-1,j] == 0 || m[tar-1,j] == m[i,j]))  tar--;
                        if(tar==i) continue;
                        if (m[tar,j] == 0)
                            m[tar,j] = m[i,j];
                        else
                        {
                            m[tar,j] *= 2;
                            cal += m[tar,j];
                        }
                        m[i,j] = 0;
                    }
                break;
            case 's':
                if (!can_S()) break;
                for (var i = 0; i < 4; i++)
                    for (var j = 3; j >= 0 ; j--)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = j;
                        while (tar + 1 < 4 && (m[i,tar + 1] == 0 || m[i,tar + 1] == m[i,j]))  tar++;
                        if(tar==j) continue;
                        if (m[i,tar] == 0)
                            m[i,tar] = m[i,j];
                        else
                        {
                            m[i,tar] *= 2;
                            cal += m[i,tar];
                        }
                        m[i,j] = 0;
                    }
                break;
            case 'd':
                if (!can_D()) break;
                for (var j = 0; j < 4; j++)
                    for (var i = 3; i >= 0; i--)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = i;
                        while (tar + 1 < 4 && (m[tar+1,j] == 0 || m[tar+1,j] == m[i,j]))  tar++;
                        if(tar==i) continue;
                        if (m[tar,j] == 0)
                            m[tar,j] = m[i,j];
                        else
                        {
                            m[tar,j] *= 2;
                            cal += m[tar,j];
                        }
                        m[i,j] = 0;
                    }
                break;
        }
        return cal;
    }
    
    /// <summary>
    /// 这个函数将返回执行操作后的合并次数
    /// </summary>
    /// <param name="c">操作字符</param>
    /// <param name="_m">传入map</param>
    /// <param name="mapped">是否影响传入的map</param>
    /// <returns></returns>
    int CombineCalPredict(char c,int[,] _m, bool mapped)
    {
        var m = new int[4,4];
        if (!mapped)
        {
            for(var i=0; i<4; i++)
                for (var j = 0; j < 4; j++)
                    m[i, j] = _m[i, j];
        }
        else m = _m;

        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
                m[i,j] = _m[i, j];
        int cal = 0;
        switch (c)
        {
            case 'w':
                if (!can_W()) break;
                for (var i = 0; i < 4; i++)
                    for (var j = 1; j < 4; j++)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = j;
                        while (tar - 1 >= 0 && (m[i,tar - 1] == 0 || m[i,tar - 1] == m[i,j]))  tar--;
                        if(tar==j) continue;
                        if (m[i,tar] == 0) //目标格为0
                            m[i,tar] = m[i,j];
                        else //目标格有数字
                        {
                            m[i,tar] *= 2;
                            cal++;
                        }
                        m[i,j] = 0; //原格子删除
                    }
                break;
            case 'a':
                if (!can_A()) break;
                for (var j = 0; j < 4; j++)
                    for (var i = 1; i < 4; i++)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = i;
                        while (tar - 1 >= 0 && (m[tar-1,j] == 0 || m[tar-1,j] == m[i,j]))  tar--;
                        if(tar==i) continue;
                        if (m[tar,j] == 0)
                            m[tar,j] = m[i,j];
                        else
                        {
                            m[tar,j] *= 2;
                            cal++;
                        }
                        m[i,j] = 0;
                    }
                break;
            case 's':
                if (!can_S()) break;
                for (var i = 0; i < 4; i++)
                    for (var j = 3; j >= 0 ; j--)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = j;
                        while (tar + 1 < 4 && (m[i,tar + 1] == 0 || m[i,tar + 1] == m[i,j]))  tar++;
                        if(tar==j) continue;
                        if (m[i,tar] == 0)
                            m[i,tar] = m[i,j];
                        else
                        {
                            m[i,tar] *= 2;
                            cal++;
                        }
                        m[i,j] = 0;
                    }
                break;
            case 'd':
                if (!can_D()) break;
                for (var j = 0; j < 4; j++)
                    for (var i = 3; i >= 0; i--)
                    {
                        if (m[i,j] == 0) continue;
                        var tar = i;
                        while (tar + 1 < 4 && (m[tar+1,j] == 0 || m[tar+1,j] == m[i,j]))  tar++;
                        if(tar==i) continue;
                        if (m[tar,j] == 0)
                            m[tar,j] = m[i,j];
                        else
                        {
                            m[tar,j] *= 2;
                            cal++;
                        }
                        m[i,j] = 0;
                    }
                break;
        }
        return cal;
    }
    
    IEnumerator NextTurn()
    {
        round++;
        yield return new WaitForSeconds(0.08f);
        SummonNum(1);
        yield return new WaitForSeconds(0.02f);
        canMove = true;
    }
    
    /// <summary>
    /// 基于贪心思想的AI决策
    /// </summary>
    /// <returns>返回0，1，2，3表示wasd：上左下右</returns>
    private int GreedyDecision()
    {
        var pres = new int[]
        {
            ScoreCalPredict('w', map,false),ScoreCalPredict('a',map,false),
            ScoreCalPredict('s',map,false),ScoreCalPredict('d',map,false)
        };
        var highest = 0;
        var control = 0;
        for (var i = 0; i < 4; i++)
        {
            if (!can_X(controls[i])) continue;
            if (pres[i] >= highest)
            {
                highest = pres[i];
                control = i;
            }
        }
        return control;
    }

    private int MiniMaxDecision_Launch(int[,] nowMap)
    {
        var _map = new int[4, 4];
        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
                _map[i,j] = nowMap[i, j];
        
        var pre = new int[4] {MiniMaxDecision(_map, 0, 'w', 0),
            MiniMaxDecision(_map, 0, 'a', 0),
            MiniMaxDecision(_map, 0, 's', 0),
            MiniMaxDecision(_map, 0, 'd', 0)};
        var max = 0;
        var maxIndex = 0;
        for (var i = 0; i < 4; i++)
        {
            if (!can_X(controls[i])) continue;
            if (pre[i] >= max)
            {
                max = pre[i];
                maxIndex = i;
            }
        }
        return maxIndex;
    }
    
    /// <summary>
    /// 通过多次递归寻找最优的下一步方向
    /// </summary>
    /// <param name="nowMap">当前的map情况</param>
    /// <param name="nowScore">当前获取的分数情况</param>
    /// <param name="dir">即将进行的模拟方向</param>
    /// <param name="depth">当前递归深度</param>
    /// <returns></returns>
    private int MiniMaxDecision(int[,] nowMap, int nowScore, char dir, int depth)
    {
        if (depth == 7) return nowScore; //如果已经达到了第x层递归则结束 建议5-6
        if (!can_X(dir, nowMap)) return nowScore; //如果这个方向无法移动则剪枝
        var m = new int[4,4];
        var zerosCount = 0;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                m[i, j] = nowMap[i, j];
                if (m[i, j] == 0) zerosCount++;
            }

        //模拟当前方向的情况 改变map
        //评价移动得分的体系为：预计在接下来的递归中 能够合成的方块数*空格比例因子+得分；
        //空格越少时，能够合成的方块越多，这个策略约好。
        var ratio = (int)Mathf.Pow(2, Mathf.Max(8,Mathf.Min(0,8 - zerosCount)));
        nowScore += CombineCalPredict(dir, m, true)*ratio + ScoreCalPredict(dir, m, false);
        //然后开启新的一轮
        SummonNumForSim(1, m);

        var bestValue = int.MinValue;
        foreach (var move in controls)
        {
            var value = MiniMaxDecision(m, nowScore, move, depth + 1);
            bestValue = Math.Max(bestValue, value);
        }
        return bestValue;
    }

    private void revocate()
    {
        //清空地图
        for (var i = 0; i < 4; i++) 
            for (var j = 0; j < 4; j++)
                map[i, j] = 0;
        //回归分数
        Score = old_score;
        //根据旧地图创建新的数
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                map[i, j] = oldmap[i, j];
                if(oldmap[i,j]==0) continue;
                GameObject n = Instantiate(num);
                Numbers sc = n.GetComponent<Numbers>();
                sc.num = oldmap[i, j];
                sc.pos = new[] { i, j };
                n.transform.localScale = new Vector3(1, 1, 1);
            }
    }
}
