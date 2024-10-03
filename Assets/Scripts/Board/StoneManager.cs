using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;


public enum ETeam
{
    BLACK,
    WHITE,
}

public struct PuttableCellInfo
{
    public int X;
    public int Y;
    public int Count;
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

    /// <summary>
    /// 盤面上の石。何もないときはNullになる。
    /// </summary>
    public IStone[,] Stones;


    Vector2[] directions = new Vector2[]
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
        cam.transform.position = new Vector3(0, largeSize * 2, largeSize * -1);


        Stones[xSize / 2 - 1, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2 - 1),
            Quaternion.identity).GetComponent<IStone>();
        Stones[xSize / 2 - 1, ySize / 2 - 1].SetTeam(ETeam.BLACK);

        Stones[xSize / 2, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2 - 1),
            Quaternion.identity).GetComponent<IStone>();
        Stones[xSize / 2, ySize / 2 - 1].SetTeam(ETeam.WHITE);

        Stones[xSize / 2 - 1, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2),
            Quaternion.identity).GetComponent<IStone>();
        Stones[xSize / 2 - 1, ySize / 2].SetTeam(ETeam.WHITE);

        Stones[xSize / 2, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2),
            Quaternion.identity).GetComponent<IStone>();
        Stones[xSize / 2, ySize / 2].SetTeam(ETeam.BLACK);
    }

    async public Task PutStone(TurnInfo info)
    {
        if (info.X == -1)
            return;

        Stones[info.X,info.Y] = info.PutStone;

        info.PutStone.GameObjectRef.transform.position =
            new Vector3(
                (-Stones.GetLength(0) + 1) + (info.X * 2),
                0.15f,
                (-Stones.GetLength(1) + 1) + (info.Y * 2));
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
        await Task.WhenAll(flipTasks);
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
        var startPosition = new Vector3(-Stones.GetLength(0) + 1, 0, -Stones.GetLength(1) + 1);
        return startPosition + new Vector3(x * 2, 0, y * 2);

    }

    /// <summary>
    /// 範囲外チェック
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool CheckOutOfBoard(int x, int y)
    {
        if (x < 0 || y < 0) return true;
        if (x >= Stones.GetLength(0) || y >= Stones.GetLength(1)) return true;

        return false;
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

        

        while (true)
        {
            Stones[x, y].SetTeam(myTeam);
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
}
