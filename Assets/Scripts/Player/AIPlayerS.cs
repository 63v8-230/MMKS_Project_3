using System.Collections.Generic;
//using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Task = Cysharp.Threading.Tasks.UniTask;

public class AIPlayerS : AIPlayerBase
{

    private int currentTurn = 0;

    async public override UniTask<TurnInfo> DoTurn()
    {
        currentTurn++;
        Debug.Log("Turn:" + currentTurn);
        await Task.Delay(500);
        var puttablePosition = gameManager.StoneManagerRef.GetPuttablePosition(Team).Preserve();
        while (!puttablePosition.GetAwaiter().IsCompleted)
            await Task.Delay(10);

        Debug.Log("Puttable: " + puttablePosition.GetAwaiter().GetResult().Length);

        if(puttablePosition.GetAwaiter().GetResult().Length <= 0)
        {
            return new TurnInfo() { X = -1 };
        }

        TurnInfo t = new TurnInfo();
        t.X = -1;

        if (currentTurn < 12)
        {
            UniTask<TurnInfo>[] checkTasks =
                {
                    Check(IsCorner,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsArrow,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsEdge,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsShield,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsCircle,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsCrystal,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsX,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsCross,puttablePosition.GetAwaiter().GetResult()).Preserve(),
                    Check(IsSun,puttablePosition.GetAwaiter().GetResult()).Preserve(),

                };

            var ts = Task.WhenAll(checkTasks).Preserve();

            while (!ts.GetAwaiter().IsCompleted)
            {
                await Task.Delay(1);
            }

            for (int i = 0; i < checkTasks.Length; i++)
            {
                if (checkTasks[i].GetAwaiter().GetResult().X != -1)
                {
                    var kind = GetStone(i);
                    Debug.Log(kind.ToString());
                    bool isEmpty = true;
                    for (int i2 = 0; i2 < MyDeck.Stones.Count; i2++)
                    {
                        if (MyDeck.Stones[i2].Stone == kind && kind != EStone.DEFAULT)
                        {
                            if (MyDeck.Stones[i2].Amount > 0)
                            {
                                var d = MyDeck.Stones[i2];
                                d.Amount--;
                                MyDeck.Stones[i2] = d;
                                Debug.Log("Used");
                                isEmpty = false;
                                break;
                            }
                            else
                            {
                                Debug.Log("Empty");
                                isEmpty = true;
                                break;
                            }
                        }

                    }
                    if (isEmpty)
                        continue;

                    t = checkTasks[i].GetAwaiter().GetResult();
                    t.PutStone = gameManager.StoneManagerRef.SelectStone(kind);
                    if ((int)kind > 1)
                        (t.PutStone as SkillStoneBase).IsOwnerOnline = isEnemy;

                    break;
                }
            }
        }
        else
        {
            for (int i2 = 0; i2 < MyDeck.Stones.Count; i2++)
            {
                if (MyDeck.Stones[i2].Amount > 0)
                {
                    var d = MyDeck.Stones[i2];
                    d.Amount--;
                    MyDeck.Stones[i2] = d;
                    Debug.Log("Used");


                    var pp = puttablePosition.GetAwaiter().GetResult()[UnityEngine.Random.Range(0, puttablePosition.GetAwaiter().GetResult().Length-1)];
                    t.X = pp.X;
                    t.Y = pp.Y;
                    t.PutStone = gameManager.StoneManagerRef.SelectStone(d.Stone);
                    if ((int)d.Stone > 1)
                        (t.PutStone as SkillStoneBase).IsOwnerOnline = true;


                    break;
                }

            }
        }

        Debug.Log("=X: " + t.X);

        if (t.X == -1 && puttablePosition.GetAwaiter().GetResult().Length > 0)
        {

            var pp = puttablePosition.GetAwaiter().GetResult()[UnityEngine.Random.Range(0, puttablePosition.GetAwaiter().GetResult().Length)];
            t.X = pp.X;
            t.Y = pp.Y;
            t.PutStone = gameManager.StoneManagerRef.SelectStone(EStone.DEFAULT);
        }
        else if (puttablePosition.GetAwaiter().GetResult().Length == 0)
        {
            t.X = -1;
            return t;
        }

        t.PutStone.SetTeam(Team);

        return t;

    }

    async public override UniTask<TurnInfo> DoComboBonus(int bonus)
    {
        List<UniTask<int>> cells = new List<UniTask<int>>();
        var bSize = gameManager.StoneManagerRef.GetBoardSize();
        for (int ix = 0; ix < bSize.x; ix++)
        {
            cells.Add(SelectCellFromColumn(ix));
        }

        var t = Task.WhenAll(cells);

        while (!t.GetAwaiter().IsCompleted) { await Task.Delay(10); }

        int xv = -1, yv = -1;

        for (int i = 0; i < bSize.x; i++)
        {
            if (cells[i].GetAwaiter().GetResult() != -1)
            {
                if (xv == -1)
                {
                    xv = i;
                    yv = cells[i].GetAwaiter().GetResult();
                }
                else if (Random.value < 0.3f)
                {
                    xv = i;
                    yv = cells[i].GetAwaiter().GetResult();
                }
            }
        }

        return new TurnInfo() { X = xv, Y = yv };
    }

    async private UniTask<int> SelectCellFromColumn(int x)
    {
        await Task.Yield();

        int v = -1;
        for (int i = 0; i < gameManager.StoneManagerRef.Stones.GetLength(1); i++)
        {
            if (gameManager.StoneManagerRef.Stones[x, i] != null)
            {
                if (gameManager.StoneManagerRef.Stones[x, i].Team != Team)
                {
                    if (v == -1)
                    {
                        v = i;
                    }
                    else if (Random.value < 0.3f)
                    {
                        v = i;
                    }
                }
            }
        }

        return v;
    }

    // Start is called before the first frame update
    void Start()
    {
        Deck d = new Deck();
        d.Stones = new List<OwnStone>();
        d.Stones.Add(new OwnStone { Stone = EStone.SUN, Amount = 1 });
        d.Stones.Add(new OwnStone { Stone = EStone.CROSS, Amount = 1 });
        d.Stones.Add(new OwnStone { Stone = EStone.X, Amount = 1 });
        //d.Stones.Add(new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 });
        d.Stones.Add(new OwnStone { Stone = EStone.CIRCLE, Amount = 1 });
        //d.Stones.Add(new OwnStone { Stone = EStone.SHIELD, Amount = 1 });
        d.Stones.Add(new OwnStone { Stone = EStone.ARROW, Amount = 1 });

        MyDeck = d;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private EStone GetStone(int index)
    {
        switch (index)
        {
            case 0:
                return EStone.DEFAULT;
            case 1:
                return EStone.ARROW;
            case 2:
                return EStone.DEFAULT;
            case 3:
                return EStone.SHIELD;
            case 4:
                return EStone.CIRCLE;
            case 5:
                return EStone.CRYSTAL;
            case 6:
                return EStone.X;
            case 7:
                return EStone.CROSS;
            case 8:
                return EStone.SUN;
            default:
                return EStone.DEFAULT;
        }
    }

    private bool IsArrow(int x, int y, Vector2 size)
    {
        if (Team != ETeam.WHITE)
        {
            return y < 2;
        }
        else
        {
            return y >= size.y - 3;
        }
    }

    private bool IsShield(int x, int y, Vector2 size)
    {
        return (x == 1 || x == 2 || x == size.x - 2 || x == size.x - 3) &&
            (y == 1 || y == 2 || y == size.y - 2 || y == size.y - 3);
    }

    private bool IsCircle(int x, int y, Vector2 size)
    {
        return ((x == 1 || x == 3 || x == size.x - 2 || x == size.x - 4) && (y == 2 || y == size.y - 3)) ||
            ((x == 2 || x == size.x - 3) && (y == 3));
    }

    private bool IsCrystal(int x, int y, Vector2 size)
    {
        return ((x == 2 || x == size.x - 3) && (y == size.y - 4)) ||
            ((x == 3 || x == size.x - 4) && (y == 3 || y == size.y - 2));
    }

    private bool IsX(int x, int y, Vector2 size)
    {
        return ((x == 2 || x == size.x - 3) || (y == 3 || y == size.y - 4)) ||
            ((x == 3 || x == size.y - 4) && (y == 2 || y == size.y - 3));
    }

    private bool IsCross(int x, int y, Vector2 size)
    {
        return (x == 1 || x == 3 || x == -4 || x == -2) && (y == 1 || y == size.y - 2);
    }

    private bool IsSun(int x, int y, Vector2 size)
    {
        return ((x == 2 || x == size.x - 3) && (y == 2 || y == size.y - 3)) ||
            ((x == 3) && (y == 3)) || ((x == size.x - 4) && (y == size.y - 4));
    }
}
