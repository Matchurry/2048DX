using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class Camera : MonoBehaviour
{
    [FormerlySerializedAs("can_move")] public bool canMove = false;
    public static readonly UnityEvent OnSummonNewNum = new UnityEvent();
    public static readonly UnityEvent<int[]> OnMoveUp = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveLeft = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveDown = new UnityEvent<int[]>();
    public static readonly UnityEvent<int[]> OnMoveRight = new UnityEvent<int[]>();
    public GameObject num;
    public static int[,] map = new int[4, 4];
    void Start()
    {
        for (var i = 0; i < 4; i++)
            for (var j = 0; j < 4; j++)
                map[i, j] = 0;
        canMove = true;
        OnSummonNewNum.AddListener(SummonNum);
        OnSummonNewNum.Invoke();
        canMove = true;
    }
    void Update()
    {
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W) && can_W())
            {
                Move('w');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            if (Input.GetKeyDown(KeyCode.A) && can_A())
            {
                Move('a');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            if (Input.GetKeyDown(KeyCode.S) && can_S())
            {
                Move('s');
                canMove = false;
                StartCoroutine(NextTurn());
            }
            if (Input.GetKeyDown(KeyCode.D) && can_D())
            {
                Move('d');
                canMove = false;
                StartCoroutine(NextTurn());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var VARIABLE in map)
            {
                Debug.Log(VARIABLE);
            }
        }
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
    
    bool can_W()
    {
        return true;
    }

    bool can_A()
    {
        return true;
    }

    bool can_S()
    {
        return true;
    }

    bool can_D()
    {
        return true;
    }
    
    void SummonNum()
    {
        for (var i = 0; i < 2; i++)
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
            if (type == 10) _sc.num = 4;
            else _sc.num = 2;
            map[tarPos[0], tarPos[1]] = _sc.num;
        }
    }
    IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(0.1f);
        SummonNum();
        canMove = true;
    }
}
