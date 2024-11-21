using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayerM : AIPlayerBase
{

    private int currentTurn = 0;

    async public override Task<TurnInfo> DoTurn()
    {
        currentTurn++;
        await Task.Delay(500);
        var puttablePosition = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        while (!puttablePosition.IsCompleted)
            await Task.Delay(10);

        Debug.Log("Puttable: " + puttablePosition.Result.Length);
        TurnInfo t = new TurnInfo();
        t.X = -1;

        if (currentTurn <= 5) 
        {
            Task<TurnInfo>[] checkTasks =
                {
                    Check(IsCorner,puttablePosition.Result),
                    Check(IsEdge,puttablePosition.Result),
                };

            var ts = Task.WhenAll(checkTasks);

            while (!ts.IsCompleted)
            {
                await Task.Delay(1);
            }

            for (int i = 0; i < checkTasks.Length; i++)
            {
                if (checkTasks[i].Result.X != -1)
                {
                    var kind = EStone.DEFAULT;
                    Debug.Log(kind.ToString());

                    t = checkTasks[i].Result;
                    t.PutStone = gameManager.StoneManagerRef.SelectStone(GetStone(i));
                    if ((int)kind > 1)
                        (t.PutStone as SkillStoneBase).IsOwnerOnline = true;

                    break;
                }
            }
        }
        else if (currentTurn < 12)
        {
            Task<TurnInfo>[] checkTasks =
                {
                    Check(IsCorner,puttablePosition.Result),
                    Check(IsArrow,puttablePosition.Result),
                    Check(IsEdge,puttablePosition.Result),
                    Check(IsShield,puttablePosition.Result),
                    Check(IsCircle,puttablePosition.Result),
                    Check(IsCrystal,puttablePosition.Result),
                    Check(IsX,puttablePosition.Result),
                    Check(IsCross,puttablePosition.Result),
                    Check(IsSun,puttablePosition.Result),

                };

            var ts = Task.WhenAll(checkTasks);

            while (!ts.IsCompleted)
            {
                await Task.Delay(1);
            }

            for (int i = 0; i < checkTasks.Length; i++)
            {
                if (checkTasks[i].Result.X != -1)
                {
                    var kind = GetStone(i);
                    Debug.Log(kind.ToString());
                    bool isEmpty = false;
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

                    t = checkTasks[i].Result;
                    t.PutStone = gameManager.StoneManagerRef.SelectStone(kind);
                    if ((int)kind > 1)
                        (t.PutStone as SkillStoneBase).IsOwnerOnline = true;
                    Debug.Log($"===A:{t.PutStone.StoneKind}");

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

                    var pp = puttablePosition.Result[UnityEngine.Random.Range(0, puttablePosition.Result.Length)];
                    t.X = pp.X;
                    t.Y = pp.Y;
                    t.PutStone = gameManager.StoneManagerRef.SelectStone(d.Stone);
                    if ((int)d.Stone > 1)
                        (t.PutStone as SkillStoneBase).IsOwnerOnline = true;
                    Debug.Log($"===B:{t.PutStone.StoneKind}");


                    break;
                }

            }
        }

        Debug.Log("=X: " + t.X);

        if (t.X == -1 && puttablePosition.Result.Length > 0)
        {

            var pp = puttablePosition.Result[UnityEngine.Random.Range(0, puttablePosition.Result.Length)];
            t.X = pp.X;
            t.Y = pp.Y;
            t.PutStone = gameManager.StoneManagerRef.SelectStone(EStone.DEFAULT);
        }
        else if (puttablePosition.Result.Length == 0)
        {
            t.X = -1;
            return t;
        }

        t.PutStone.SetTeam(Team);

        Debug.Log($"Final:{t.PutStone.StoneKind}");

        return t;

    }

    // Start is called before the first frame update
    void Start()
    {
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            new OwnStone { Stone = EStone.X, Amount = 1 },
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
            new OwnStone { Stone = EStone.SUN, Amount = 1 },
            new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 },
            new OwnStone { Stone = EStone.CROSS, Amount = 1 },
            new OwnStone { Stone = EStone.SHIELD, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW, Amount = 1 }
        };

        MyDeck = d;
    }

    // Update is called once per frame
    void Update()
    {

    }


    protected override async Task<TurnInfo> Check(Func<int, int, Vector2, bool> f, PuttableCellInfo[] p)
    {
        TurnInfo turn = new TurnInfo();
        await Task.Delay(1);
        foreach (var item in p)
        {
            if (f(item.X, item.Y, gameManager.StoneManagerRef.GetBoardSize()))
            {
                if(item.Count >= 3 && IsEnemySkillStoneAround(item.X,item.Y))
                {
                    turn.X = item.X;
                    turn.Y = item.Y;
                    return turn;
                }
            }
        }
        turn.X = -1;
        return turn;
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
                return EStone.CROSS;
            case 5:
                return EStone.CRYSTAL;
            case 6:
                return EStone.SUN;
            case 7:
                return EStone.CIRCLE;
            case 8:
                return EStone.X;
            default:
                return EStone.DEFAULT;
        }
    }

    private bool IsArrow(int x, int y, Vector2 size)
    {
        if (Team != ETeam.WHITE)
        {
            return y < 3;
        }
        else
        {
            return y >= size.y - 4;
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
