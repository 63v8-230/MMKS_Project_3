using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public enum ETeam
{
    NONE,
    BLACK,
    WHITE,
    WALL,
}

public enum EDirection
{
    TOP,
    TOP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DOWN,
    DOWN_LEFT,
    LEFT,
    TOP_LEFT,
}

public struct PuttableCellInfo
{
    public int X;
    public int Y;
    public int Count;
}

public struct SkillAction
{
    public System.Func<StoneManager, Vector2, IEnumerator> Action;
    public Vector2 Position;
    public String Name;
}


public class ExEnumerator : IEnumerator
{
    private IEnumerator instance;
    public bool IsEnd { get; private set; }
    public ExEnumerator(IEnumerator enumerator)
    {
        this.instance = enumerator;
    }
    public object Current => instance.Current;

    public bool MoveNext()
    {
        bool hasNext = instance.MoveNext();
        IsEnd = !hasNext;
        return hasNext;
    }

    public void Reset()
    {
        instance.Reset();
        IsEnd = false;
    }
}


/// <summary>
/// 石を管理するクラス
/// </summary>
public class StoneManager : MonoBehaviour
{

    [Header("オセロ盤の真ん中")]
    /// <summary>
    /// オセロ盤の真ん中
    /// </summary>
    public Transform BoardCenter;

    [Header("オセロ盤セル")]
    /// <summary>
    /// オセロ盤の1マス
    /// </summary>
    public GameObject BoardCell;

    [SerializeField]
    private GameObject basicStone;

    [SerializeField]
    public TMP_Dropdown StoneOption;

    [SerializeField]
    public GameObject HighlightCellObject;

    [SerializeField]
    public GameObject LightStoneObject;

    /// <summary>
    /// 盤面上の石。何もないときはNullになる。
    /// </summary>
    public IStone[,] Stones;

    /// <summary>
    /// <para>特殊能力</para>
    /// <para>ここに特殊能力を実行するメソッドを入れる</para>
    /// </summary>
    private List<SkillAction> skillMethod = new List<SkillAction>();

    [NonSerialized]
    public Vector2[] directions = new Vector2[]
        {
            new Vector2(0, -1),  //T
            new Vector2(1, -1),  //RT
            new Vector2(1, 0),   //R
            new Vector2(1, 1),   //RD
            new Vector2(0, 1),   //D
            new Vector2(-1, 1),  //LD
            new Vector2(-1, 0),  //L
            new Vector2(-1, -1), //LT
        };

    public int CurrentTurn = 0;

    public void AddTurn()
    {
        CurrentTurn++;
    }

    public void Start()
    {
    }

    public void AddSkillMethod(SkillAction action)
    {
        Debug.Log($"====SkillAdd====\n{action.Name}\n{action.Position}");
        skillMethod.Add(action);
    }


    /// <summary>
    /// オセロ盤を作成して、色々初期化処理
    /// </summary>
    /// <param name="xSize">横方向のサイズ</param>
    /// <param name="ySize">縦方向のサイズ</param>
    public void Init(int xSize, int ySize)
    {
        Stones = new IStone[xSize, ySize];

        var startPosition = new Vector3(-xSize + 1, 0, -ySize + 1);

        for(int ix = 0; ix < xSize; ix++)
        {
            for (int iy = 0; iy < ySize; iy++)
            {
                var cell = GameObject.Instantiate
                    (BoardCell,
                    startPosition + new Vector3(ix * 2, 0, iy * 2),
                    Quaternion.identity);

                cell.name = $"{ix} {iy}";

                cell.transform.parent = BoardCenter;
            }
        }

        var cam = Camera.main.gameObject;
        int largeSize = xSize;
        if(ySize > xSize) largeSize = ySize;
        cam.transform.position = new Vector3(0, largeSize * 2.3f, largeSize * -0.7f);


        Stones[xSize / 2 - 1, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2 - 1),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2 - 1, ySize / 2 - 1].SetTeam(ETeam.BLACK);

        Stones[xSize / 2, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2 - 1),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2, ySize / 2 - 1].SetTeam(ETeam.WHITE);

        Stones[xSize / 2 - 1, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2 - 1, ySize / 2].SetTeam(ETeam.WHITE);

        Stones[xSize / 2, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2, ySize / 2].SetTeam(ETeam.BLACK);
    }

    private IEnumerator SetTransform(GameObject gObject, Vector3 value)
    {
        gObject.transform.position = value;

        yield break;
    }

    async public Task PutStone(TurnInfo info)
    {
        if (info.X == -1)
            return;

        Stones[info.X,info.Y] = info.PutStone;

        var e = SetTransform(info.PutStone.GameObjectRef,
            new Vector3(
                (-Stones.GetLength(0) + 1) + (info.X * 2),
                0.15f,
                (-Stones.GetLength(1) + 1) + (info.Y * 2)));
        StartCoroutine(e);

        info.PutStone.GameObjectRef.name = $"Stone {info.X}-{info.Y}";


        var checkTasks = directions.Select(dir => CheckLineAsync(info.X, info.Y, dir, info.PutStone.Team)).ToArray();
        var results = await Task.WhenAll(checkTasks);

        var flipTasks = new List<Task>();
        for (int i = 0; i < directions.Length; i++)
        {
            if (results[i])
            {
                flipTasks.Add(FlipStones(info.X, info.Y, directions[i], info.PutStone.Team));
            }
        }

        //flipTasksのタスクが全て完了するまで待つ
        var t = Task.WhenAll(flipTasks);

        while (!t.IsCompleted)
        {
            await Task.Delay(1);
        }

        Debug.Log("Skill Count: "+skillMethod.Count);

        while(skillMethod.Count!=0)
        {
            var skillCoroutine = new ExEnumerator(skillMethod[0].Action.Invoke(this, skillMethod[0].Position));
            StartCoroutine(skillCoroutine);

            while (!skillCoroutine.IsEnd)
            {
                await Task.Delay(1);
            }
            skillMethod.RemoveAt(0);
        }
        skillMethod.Clear();
    }

    public void FlipStone(int x, int y, ETeam team, bool isSkill = false)
    {
        var s = Stones[x, y];
        if(s != null)
        {
            if(s.Team != team)
            {
                s.SetTeam(team, this, x, y);
                var e = s.OnFlip(isSkill);
                StartCoroutine(e);
            }
                
        } 
    }

    /// <summary>
    /// 現在の石を数える。戻り値は合計。引数は色別。
    /// </summary>
    /// <param name="black"></param>
    /// <param name="white"></param>
    /// <returns></returns>
    public int GetStoneCounts(out int black, out int white)
    {
        black = 0;
        white = 0;
        for (int ix = 0; ix < Stones.GetLength(0); ix++)
        {
            for (int iy = 0; iy < Stones.GetLength(1); iy++)
            {
                if (Stones[ix, iy] == null)
                    continue;

                switch (Stones[ix, iy].Team)
                {
                    case ETeam.BLACK:
                        black++;
                        break;
                    case ETeam.WHITE:
                        white++;
                        break;
                }
            }
        }

        return black + white;
    }

    /// <summary>
    /// 盤の位置を座標に変換
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CellPosition2Vector3(int x, int y)
    {
        var startPosition = new Vector3(-Stones.GetLength(0) + 1, 0.15f , -Stones.GetLength(1) + 1);
        return startPosition + new Vector3(x * 2, 0, y * 2);

    }

    /// <summary>
    /// 範囲外チェック
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>範囲外ならTrue</returns>
    public bool CheckOutOfBoard(int x, int y)
    {
        if (x < 0 || y < 0) return true;
        if (x >= Stones.GetLength(0) || y >= Stones.GetLength(1)) return true;

        return false;
    }

    public Vector2 GetBoardSize()
    {
        return new Vector2(Stones.GetLength(0), Stones.GetLength(1));
    }

    /// <summary>
    /// 配置可能なマスを返す
    /// </summary>
    /// <returns></returns>
    public PuttableCellInfo[] GetPuttablePosition(ETeam myTeam)
    {
        List<PuttableCellInfo> returnList = new List<PuttableCellInfo>();

        for (int ix = 0; ix < Stones.GetLength(0); ix++)
        {
            for (int iy = 0; iy < Stones.GetLength(1); iy++)
            {
                if (Stones[ix, iy] != null)
                    continue;

                int pCount = 0;
                foreach (var dir in directions)
                {
                    if (CheckOutOfBoard(ix + (int)dir.x, iy + (int)dir.y))
                        continue;

                    var stone = Stones[ix + (int)dir.x, iy + (int)dir.y];
                    if (stone == null)
                        continue;

                    if (stone.Team == myTeam)
                        continue;

                    int count;
                    if (!CheckLine(ix, iy, dir, myTeam, out count))
                        continue;

                    pCount += count;
                }

                if (pCount > 0) 
                {
                    PuttableCellInfo p = new PuttableCellInfo();
                    p.X = ix;
                    p.Y = iy;
                    p.Count = pCount;
                    returnList.Add(p);
                }
            }
        }

                return returnList.ToArray();

    }

    /// <summary>
    /// 指定した方向で石がひっくり返るかどうかをチェックする。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    async private Task<bool> CheckLineAsync(int x, int y, Vector2 direction, ETeam myTeam)
    {
        await Task.Delay(1);

        bool canFlip = false;

        x += (int)direction.x;
        y += (int)direction.y;

        if (CheckOutOfBoard(x, y)) return false;

        if (Stones[x, y] == null ||
            Stones[x, y].Team == myTeam)
        {
            return false;
        }


        while (true)
        {
            x += (int)direction.x;
            y += (int)direction.y;


            if (CheckOutOfBoard(x, y)) break;

            if (Stones[x, y] == null) break;

            if (Stones[x, y].Team == myTeam)
            {
                canFlip = true;
                break;
            }
        }

        return canFlip;
    }

    private bool CheckLine(int x, int y, Vector2 direction, ETeam myTeam, out int puttableCount)
    {
        bool canFlip = false;

        x += (int)direction.x;
        y += (int)direction.y;

        puttableCount = 0;

        if (CheckOutOfBoard(x, y)) return false;

        if (Stones[x, y] == null ||
            Stones[x, y].Team == myTeam)
        {
            return false;
        }

        puttableCount++;

        while (true)
        {
            x += (int)direction.x;
            y += (int)direction.y;
            puttableCount++;


            if (CheckOutOfBoard(x, y)) break;

            if (Stones[x, y] == null) break;

            if (Stones[x, y].Team == myTeam)
            {
                canFlip = true;
                puttableCount--;
                break;
            }
        }

        if (!canFlip)
            puttableCount = 0;

        return canFlip;
    }

    private async Task FlipStones(int x, int y, Vector2 direction, ETeam myTeam)
    {
        x += (int)direction.x;
        y += (int)direction.y;

        //if (myTeam == ETeam.BLACK)
        //    myTeam = ETeam.WHITE;
        //else
        //    myTeam = ETeam.BLACK;

        while (true)
        {
            Stones[x, y].SetTeam(myTeam, this, x, y);
            var flipCoroutine = new ExEnumerator(Stones[x, y].OnFlip());
            StartCoroutine(flipCoroutine);

            while (!flipCoroutine.IsEnd)
            {
                await Task.Delay(1);
            }

            x += (int)direction.x;
            y += (int)direction.y;

            if (Stones[x, y].Team == myTeam)
            {
                return;
            }
        }
    }

    public IStone SelectStone(EStone selectedStone)
    {
        IStone s;

        Debug.Log(selectedStone.ToString());

        switch (selectedStone)
        {
            case EStone.DEFAULT:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<Stone>();
                break;

            case EStone.SUN:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<SunStone>();
                break;

            case EStone.CROSS:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrossStone>();
                break;

            case EStone.X:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<XStone>();
                break;

            case EStone.CIRCLE:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                break;
            case EStone.ARROW:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ArrowStone>();
                break;

            case EStone.SHIELD:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ShieldStone>();
                break;

            case EStone.CRYSTAL:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrystalStone>();
                break;

            default:
                s =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                Debug.LogError("EStone None");
                break;
        }

        Debug.Log(s);

        return s;
    }

    public Sprite GetSprite(EStone stone)
    {
        switch (stone)
        {
            case EStone.SUN:
                return (Sprite)Resources.Load<Sprite>("Pictures/Sun");

            case EStone.CROSS:
                return (Sprite)Resources.Load<Sprite>("Pictures/Cross");

            case EStone.X:
                return (Sprite)Resources.Load<Sprite>("Pictures/X");

            case EStone.CIRCLE:
                return (Sprite)Resources.Load<Sprite>("Pictures/Circle");

            case EStone.ARROW:
                return (Sprite)Resources.Load<Sprite>("Pictures/Arrow");

            case EStone.SHIELD:
                return (Sprite)Resources.Load<Sprite>("Pictures/Shield");

            case EStone.CRYSTAL:
                return (Sprite)Resources.Load<Sprite>("Pictures/Crystal");

            default:
                return null;
        }
    }
}
