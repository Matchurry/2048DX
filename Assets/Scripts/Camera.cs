using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Lofelt.NiceVibrations;

public class Camera : MonoBehaviour
{
    private class DecisionValues
    {
        public int GetScores; //该决策下获取的分数
        public int CombineTimes; //该决策下合并方块数*系数
        public int Chaos; //该决策下减少的局面混乱度*系数
        public char step; //用于记录是何种决策

        public DecisionValues(int a, int b, int c, char d)
        {
            GetScores = a;
            CombineTimes = b;
            Chaos = c;
            step = d;
        }
        public int SumValues()
        {
            return GetScores + CombineTimes + Chaos;
        }
    
}
    
    [FormerlySerializedAs("can_move")] public bool canMove = false;
    public static readonly UnityEvent<int> OnSummonNewNum = new UnityEvent<int>();
    public static readonly UnityEvent<int[]> OnMoveUp = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveLeft = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveDown = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveRight = new UnityEvent<int[]>();
    public GameObject num;
    public Transform bg;
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
    // 用于存储自动决策协程给出的最终决策
    private char AiDeci = 'n'; 
    // 用于判断自动决策协程是否正在计算
    private bool isCalculating = false;
    private bool isSaved = false;
    public static double[] ratiosSum = new double[3];

    private void Awake()
    {
        HighestScore = PlayerPrefs.GetInt("highscore");
        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
            {
                oldmap[i, j] = PlayerPrefs.GetInt((i * 4 + j).ToString());
                if (oldmap[i, j] != 0) isSaved = true;
            }
        old_score = PlayerPrefs.GetInt("oldscore");
    }

    void Start()
    {
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 1;
        OnSummonNewNum.AddListener(SummonNum);
        NewGameButton.newGame.AddListener(DelayedNewGame);
        Revocate.revocate.AddListener(revocate);
        SwipeDetector.OnTouchInput.AddListener(handletouch);
        if (isSaved) revocate();
        else newGame();
    }
    void Update()
    {
        if (isCalculating) return;
        HighestScore = Math.Max(Score, HighestScore);

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
        if (cnt != 0) return;
        NewGameButton.endGame.Invoke();
    }

    private void handletouch(char c)
    {
        if (canMove && !is_AIControl)
        {
            OnApplicationQuit();
            if (c=='w' && can_W())
            {
                if(Shake.can_shake)
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                Move('w');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (c=='a' && can_A())
            {
                if(Shake.can_shake)
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                Move('a');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (c=='s' && can_S())
            {
                if(Shake.can_shake)
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                Move('s');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            else if (c=='d' && can_D())
            {
                if(Shake.can_shake)
                    HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
                Move('d');
                canMove = false;
                StartCoroutine(NextTurn());
            }
        }
    }
    
    private IEnumerator CalculateMiniMaxDecision(int[,] m)
    {
        var aiDecision = MiniMaxDecision_Launch(m);
        AiDeci = aiDecision.step;
        ratiosSum[0] += aiDecision.GetScores;
        ratiosSum[1] += aiDecision.CombineTimes;
        ratiosSum[2] += aiDecision.Chaos;
        isCalculating = false; 
        yield return null;
    }

    private void DelayedNewGame()
    {
        StartCoroutine(DelayedNewGameIE());
    }

    IEnumerator DelayedNewGameIE()
    {
        yield return new WaitForSeconds(0.3f);
        newGame();
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
                        while (tar - 1 >= 0 && (map[i, tar - 1] == 0 || map[i, tar - 1] == map[i, j] && !combined[i, tar - 1]))  tar--;
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
                        while (tar - 1 >= 0 && (map[tar-1, j] == 0 || map[tar-1, j] == map[i, j] && !combined[tar-1, j]))  tar--;
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
                        while (tar + 1 < 4 && (map[i, tar + 1] == 0 || map[i, tar + 1] == map[i, j] && !combined[i, tar + 1]))  tar++;
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
                        while (tar + 1 < 4 && (map[tar+1, j] == 0 || map[tar+1, j] == map[i, j] && !combined[tar+1, j]))  tar++;
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
            newNum.transform.SetParent(bg);
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
    
    /// <summary>
    /// 这个函数返回执行操作前后的局面混乱值
    /// </summary>
    /// <param name="c">操作字符</param>
    /// <param name="_m">传入map</param>
    /// <param name="mapped">是否影响传入的map</param>
    /// <returns></returns>
    int[] StabilityCalPredict(char c, int[,] _m, bool mapped)
    {
        var m = new int[4,4];
        if (!mapped)
        {
            for(var i=0; i<4; i++)
                for (var j = 0; j < 4; j++)
                    m[i, j] = _m[i, j];
        }
        else m = _m;

        var cal = new int[2];
        cal[0] = StabilityCal(m);
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
                        if (m[i,tar] == 0)
                            m[i,tar] = m[i,j];
                        else
                            m[i,tar] *= 2;
                        m[i,j] = 0;
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
                            m[tar,j] *= 2;
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
                            m[i,tar] *= 2;
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
                            m[tar,j] *= 2;
                        m[i,j] = 0;
                    }
                break;
        }
        cal[1] = StabilityCal(m);
        return cal;
    }
    
    /// <summary>
    /// 返回接受的map的局面稳定评分 不改变传入的map
    /// </summary>
    /// <param name="_m"></param>
    /// <returns></returns>
    public static int StabilityCal(int[,] _m)
    {
        var m = new int[4, 4];
        var count = new bool[4, 4];
        var cal = 0;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                count[i, j] = false;
                if (_m[i, j] == 0) m[i, j] = 0;
                else m[i, j] = (int)Math.Log(_m[i, j], 2); //这里已经做了指数变换  
            }
        
        // 遍历每一个格子 如果能够找到从格子出发的某个方向保持指数递减或不变 则每多延伸出一格这样的规律稳定值+1
        // 已经获得稳定分数的格子不给分 得分的格子还要乘上自身的指数
        // 空位没有得分 指数在2及以下的没有得分
        var steps_i = new int[] { -1, 0, 1, 0 };
        var steps_j = new int[] { 0, 1, 0, -1 };
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                //遍历上下左右 如果是递减的 则自身和对应格子标记为可得分
                for (var k = 0; k < 4; k++)
                {
                    var move_i = i + steps_i[k];
                    var move_j = j + steps_j[k];
                    if (move_i >= 0 && move_i < 4 && move_j >= 0 && move_j < 4
                        && m[move_i, move_j] > 2 && m[i, j] >= m[move_i, move_j] && m[i, j] - m[move_i, move_j] <= 1)
                    {
                        count[i, j] = true;
                        count[move_i, move_j] = true;
                    }
                }
            }
        //保证一个场面分 在其基础上追加稳定分
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                if (count[i, j])
                    cal += m[i, j] * 2;
                else cal += Mathf.Max(m[i, j] - 2, 0);
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
    /// 基于贪心思想的求解决策
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

    private DecisionValues MiniMaxDecision_Launch(int[,] nowMap)
    {
        var _map = new int[4, 4];
        for(var i=0; i<4; i++)
            for (var j = 0; j < 4; j++)
                _map[i,j] = nowMap[i, j];
        
        var pre = new DecisionValues[4] {MiniMaxDecision(_map, new DecisionValues(0,0,0,'w'), 'w', 0),
            MiniMaxDecision(_map, new DecisionValues(0,0,0,'a'), 'a', 0),
            MiniMaxDecision(_map, new DecisionValues(0,0,0,'s'), 's', 0),
            MiniMaxDecision(_map, new DecisionValues(0,0,0,'d'), 'd', 0)};
        var max = 0;
        var maxIndex = 0;
        for (var i = 0; i < 4; i++)
        {
            if (!can_X(controls[i])) continue;
            if (pre[i].SumValues() >= max)
            {
                max = pre[i].SumValues();
                maxIndex = i;
            }
        }
        return pre[maxIndex];
    }
    
    /// <summary>
    /// 通过多次递归寻找最优的下一步方向
    /// </summary>
    /// <param name="nowMap">当前的map情况</param>
    /// <param name="nowScore">当前获取的分数情况</param>
    /// <param name="dir">即将进行的模拟方向</param>
    /// <param name="depth">当前递归深度</param>
    /// <returns></returns>
    private DecisionValues MiniMaxDecision(int[,] nowMap, DecisionValues nowScore, char dir, int depth)
    {
        if (depth == 5)
            return nowScore; //如果已经达到了第x层递归则结束 建议5-6
        if (!can_X(dir, nowMap)) return nowScore; //如果这个方向无法移动则剪枝
        var m = new int[4,4];
        var zerosCount = 0;
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
            {
                m[i, j] = nowMap[i, j];
                if (m[i, j] == 0) zerosCount++;
            }
        
        // 对于合并产生的分数 自然是空位越多的时候进行合并 以尽量保持一个局面的干净
        // 以下为最初的启发式构想
        var ratio_zeros = (int)Mathf.Pow(2,Mathf.Max(12,zerosCount));
        var ratio_antizeros = (int)Mathf.Pow(2, Mathf.Min(16 - zerosCount,12));
        
        nowScore.CombineTimes += CombineCalPredict(dir, m, false) * ratio_zeros;
        nowScore.GetScores += ScoreCalPredict(dir, m, true) * ratio_zeros; //最后一次计算的时候改变map
        nowScore.Chaos += StabilityCal(m)/15 * ratio_zeros;
        
        //然后开启新的一轮
        SummonNumForSim(1, m);

        var bestValue = nowScore;
        foreach (var move in controls)
        {
            var value = MiniMaxDecision(m, nowScore, move, depth + 1);
            if (value.SumValues() >= nowScore.SumValues()) bestValue = value;
        }
        return bestValue;
    }

    private void revocate()
    {
        StartCoroutine(revocateIE());
    }

    IEnumerator revocateIE()
    {
        yield return new WaitForSeconds(0.2f);
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
                n.transform.SetParent(bg);
                Numbers sc = n.GetComponent<Numbers>();
                sc.num = oldmap[i, j];
                sc.pos = new[] { i, j };
                n.transform.localScale = new Vector3(0, 0, 0);
            }
        canMove = true;
    }

    private void OnApplicationQuit()
    {
        for(var i=0; i<4; i++)
            for(var j=0; j<4; j++)
                PlayerPrefs.SetInt((i*4+j).ToString(),map[i,j]);
        PlayerPrefs.SetInt("oldscore",Score);
        PlayerPrefs.SetInt("highscore",HighestScore);
        PlayerPrefs.Save();
    }
    
}
