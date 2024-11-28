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
    protected bool isEnemy = true;

    public Deck MyDeck { set; get; }

    public ETeam Team { set => _team = value; get => _team; }

    public OnlineScript OnlineScriptRef { get; set; }

    protected GameManager gameManager;

    async public virtual Task<TurnInfo> DoTurn()
    {
        await Task.Delay(500);
        return new TurnInfo();
    }

    async public virtual Task<TurnInfo> DoComboBonus()
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

    protected virtual bool IsEnemySkillStoneAround(int x, int y)
    {
        for (int i = 0; i < gameManager.StoneManagerRef.directions.Length; i++)
        {
            if (gameManager.StoneManagerRef.CheckOutOfBoard
                (x + (int)gameManager.StoneManagerRef.directions[i].x, y + (int)gameManager.StoneManagerRef.directions[i].y))
                continue;

            var s = gameManager.StoneManagerRef.Stones
                [x + (int)gameManager.StoneManagerRef.directions[i].x, y + (int)gameManager.StoneManagerRef.directions[i].y];

            if (s == null)
                continue;

            ETeam r = ETeam.WHITE;
            if(Team==r)
                r= ETeam.BLACK;

            if(s.Team == r && (int)s.StoneKind > 0)
            {
                return true;
            }
        }
        return false;
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
