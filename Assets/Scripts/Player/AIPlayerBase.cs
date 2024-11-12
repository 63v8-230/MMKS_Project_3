using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIPlayerBase : MonoBehaviour, IPlayer //AIのベースクラス
{
    [SerializeField]
    private ETeam _team;

    [SerializeField]
    protected GameObject stone;

    public Deck MyDeck { set; get; }

    public ETeam Team { set => _team = value; get => _team; }

    public OnlineScript OnlineScriptRef { get; set; }

    protected GameManager gameManager;

    async public virtual Task<TurnInfo> DoTurn()
    {
        await Task.Delay(500);
        return new TurnInfo();
    }

    public virtual void Init(GameManager gManager)
    {
        gameManager = gManager;
    }

    protected virtual async Task<TurnInfo> Check(Func<int, int, Vector2, bool> f, PuttableCellInfo[] p)
    {
        TurnInfo turn = new TurnInfo();
        await Task.Delay(1);
        foreach (var item in p)
        {
            if (f(item.X, item.Y, gameManager.StoneManagerRef.GetBoardSize()) && item.Count >= 3)
            {

                turn.X = item.X;
                turn.Y = item.Y;
                return turn;
            }
        }
        turn.X = -1;
        return turn;
    }

    protected bool IsCorner(int x, int y, Vector2 size)
    {
        return (x == 0 || x == (int)size.x) && (y == 0  || y == (int)size.y);
    }

    protected bool IsEdge(int x, int y, Vector2 size)
    {
        return x == 0 || x == (int)size.x || y == 0 || y == (int)size.y;
    }
}
