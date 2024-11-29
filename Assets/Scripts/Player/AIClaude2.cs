using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AIに作らせてみたお遊びプログラム2
/// 一部加筆修正・・・どころでは無くなってきた。結構改造しちゃっている
/// </summary>
public class AIClaude2 : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;

    [SerializeField]
    private bool isEnemy = true;

    public ETeam Team { set => _team = value; get => _team; }

    public Deck MyDeck { set; get; }
    public OnlineScript OnlineScriptRef { get; set; }

    private GameManager gameManager;
    private int currentTurn = 0;
    private readonly int BOARD_SIZE = 8;

    // 場所の価値評価用の重み付けマップ
    private readonly int[,] POSITION_WEIGHTS = new int[8, 8]
    {
        {100,  80,  50,  35,  35,  50,  80, 100},
        { 80,  90,  50,  35,  35,  50,  90,  80},
        { 50,  10,  80,   2,   2,  80,  10,  50},
        { 35,  35,   2,   1,   1,   2,  35,  35},
        { 35,  35,   2,   1,   1,   2,  35,  35},
        { 50,  10,  80,   2,   2,  80,  10,  50},
        { 80,  90,  50,  35,  35,  50,  90,  80},
        {100,  80,  50,  35,  35,  50,  80, 100}
    };

    public void Init(GameManager gManager)
    {
        gameManager = gManager;

        // デッキの初期化
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            //new OwnStone { Stone = EStone.SHIELD, Amount = 1 },
            new OwnStone { Stone = EStone.SUN, Amount = 1 },
            new OwnStone { Stone = EStone.CROSS, Amount = 1 },
            new OwnStone { Stone = EStone.X, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
            //new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 }
        };
        MyDeck = d;
    }

    public async Task<TurnInfo> DoTurn()
    {
        currentTurn++;
        await Task.Delay(100); // 思考時間演出

        var puttablePositions = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        while (!puttablePositions.IsCompleted)
            await Task.Delay(10);

        if (puttablePositions.Result.Length == 0)
        {
            return new TurnInfo { X = -1 };
        }

        // 最適な手を探す
        TurnInfo bestMove = FindBestMove(puttablePositions.Result);

        // 石を配置して返す
        if (bestMove.PutStone != null)
        {
            if (bestMove.PutStone is SkillStoneBase skillStone)
            {
                skillStone.IsOwnerOnline = isEnemy;
            }
            bestMove.PutStone.SetTeam(Team);
        }

        return bestMove;
    }

    async public Task<TurnInfo> DoComboBonus()
    {
        List<Task<int>> cells = new List<Task<int>>();
        var bSize = gameManager.StoneManagerRef.GetBoardSize();
        for (int ix = 0; ix < bSize.x; ix++)
        {
            cells.Add(SelectCellFromColumn(ix));
        }

        var t = Task.WhenAll(cells);

        while (!t.IsCompleted) {await Task.Delay(10); }

        int xv = -1, yv = -1;

        for (int i = 0; i < bSize.x; i++)
        {
            if (cells[i].Result != -1)
            {
                if (xv == -1)
                {
                    xv = i;
                    yv = cells[i].Result;
                }
                else if (
                    i == 0 ||//もしXの端なら
                    i == bSize.x - 1 )//もしXの端なら
                {
                    xv = i;
                    yv = cells[i].Result;

                    break;
                }
                else if (
                    cells[i].Result == 0 ||//もし端なら
                    cells[i].Result == bSize.y-1)//もし端なら
                {
                    xv = i;
                    yv = cells[i].Result;

                    break;
                }
                else if(Random.value < 0.3f)//角や端が無ければランダム
                {
                    xv = i;
                    yv = cells[i].Result;
                }
            }
        }

        return new TurnInfo() { X = xv, Y = yv };
    }

    async private Task<int> SelectCellFromColumn(int x)
    {
        await Task.Yield();

        if (gameManager.StoneManagerRef.Stones[x, 0] != null)
            if (gameManager.StoneManagerRef.Stones[x, 0].Team != Team)
                return 0;

        if (gameManager.StoneManagerRef.Stones[x, gameManager.StoneManagerRef.Stones.GetLength(1) - 1] != null)
            if (gameManager.StoneManagerRef.Stones[x, gameManager.StoneManagerRef.Stones.GetLength(1) - 1].Team != Team)
                return gameManager.StoneManagerRef.Stones.GetLength(1) - 1;

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

    private TurnInfo FindBestMove(PuttableCellInfo[] puttablePositions)
    {
        TurnInfo bestMove = new TurnInfo { X = -1 };
        int bestScore = int.MinValue;

        foreach (var pos in puttablePositions)
        {
            int score = EvaluateMove(pos);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove.X = pos.X;
                bestMove.Y = pos.Y;
                bestMove.PutStone = SelectBestStone(pos.X, pos.Y);
            }
        }

        return bestMove;
    }

    private int EvaluateMove(PuttableCellInfo pos)
    {
        int score = 0;

        // 基本点数：位置の重み
        score += POSITION_WEIGHTS[pos.X, pos.Y] * 10;

        // 返せる石の数による評価
        score += pos.Count * 5;

        // コーナー近辺の評価
        if (IsCorner(pos.X, pos.Y))
        {
            score += 1000; // コーナーは最重要
        }
        else if (IsNextToCorner(pos.X, pos.Y))
        {
            score += 200; // コーナーの隣もそこそこ重要
        }

        // 敵の特殊石を返せる場合の評価
        if (CanFlipEnemySpecialStone(pos.X, pos.Y))
        {
            score += 300;
        }

        return score;
    }

    private IStone SelectBestStone(int x, int y)
    {
        Debug.Log("================\nCurrent Turn - " + currentTurn);

        Debug.Log("Corner");
        // コーナーの場合
        if (IsCorner(x, y) && HasSpecialStone(EStone.SHIELD))
        {
            Debug.Log("ON");
            return CreateSpecialStone(EStone.SHIELD);
        }

        Debug.Log("Diagonal");
        // 対角線上の場合
        if (IsDiagonal(x, y))
        {
            Debug.Log("ON");
            if (HasSpecialStone(EStone.SUN))
            {
                Debug.Log("SUN");
                return CreateSpecialStone(EStone.SUN);
            }

            if (HasSpecialStone(EStone.X))
            {
                Debug.Log("X");
                return CreateSpecialStone(EStone.X);
            }
        }

        Debug.Log("Center");
        //中央より
        if(IsCenter(x, y))
        {
            if (HasSpecialStone(EStone.CROSS))
            {
                Debug.Log("CROSS");
                return CreateSpecialStone(EStone.CROSS);
            }
        }

        Debug.Log("Arrow-U");
        //矢印
        if (IsForwardPosition(x, y))
        {
            Debug.Log("U");
            if (HasSpecialStone(EStone.ARROW_D))
            {
                Debug.Log("ON");
                return CreateSpecialStone(EStone.ARROW_D);
            }

        }

        Debug.Log("Arrow-D");
        if (IsBackwardPosition(x, y))
        {
            Debug.Log("D");
            if (HasSpecialStone(EStone.ARROW_U))
            {
                Debug.Log("ON");
                return CreateSpecialStone(EStone.ARROW_U);
            }
        }

        Debug.Log("Arrow-L");
        if (IsLeftPosition(x, y))
        {
            Debug.Log("L");
            if (HasSpecialStone(EStone.ARROW_L))
            {
                Debug.Log("ON");
                return CreateSpecialStone(EStone.ARROW_L);
            }
        }

        Debug.Log("Arrow-R");
        if (IsRightPosition(x, y))
        {
            Debug.Log("R");
            if (HasSpecialStone(EStone.ARROW_R))
            {
                Debug.Log("ON");
                return CreateSpecialStone(EStone.ARROW_R);
            }

        }

        Debug.Log("Near");
        // 敵の特殊石の近くの場合
        if (IsNextToEnemySpecialStone(x, y))
        {
            Debug.Log("ON");
            if (HasSpecialStone(EStone.CIRCLE))
                return CreateSpecialStone(EStone.CIRCLE);
            if (HasSpecialStone(EStone.CRYSTAL))
                return CreateSpecialStone(EStone.CRYSTAL);
        }

        Debug.Log("End");
        //もし終盤なら特殊コマを全部使う
        if (currentTurn > 10)
        {
            foreach (var item in MyDeck.Stones)
            {
                if (item.Amount > 0)
                {
                    Debug.Log(item.Stone.ToString()+" is used");
                    return CreateSpecialStone(item.Stone);
                }
            }
        }

        Debug.Log("Default");
        // デフォルトの石を使用
        return gameManager.StoneManagerRef.SelectStone(EStone.DEFAULT);
    }

    private bool HasSpecialStone(EStone stoneType)
    {
        foreach (var stone in MyDeck.Stones)
        {
            if (stone.Stone == stoneType && stone.Amount > 0)
                return true;
        }
        return false;
    }

    private IStone CreateSpecialStone(EStone stoneType)
    {
        // デッキから石を消費
        for (int i = 0; i < MyDeck.Stones.Count; i++)
        {
            if (MyDeck.Stones[i].Stone == stoneType)
            {
                var stone = MyDeck.Stones[i];
                stone.Amount--;
                MyDeck.Stones[i] = stone;
                break;
            }
        }

        return gameManager.StoneManagerRef.SelectStone(stoneType);
    }

    private bool IsCorner(int x, int y)
    {
        return (x == 0 || x == BOARD_SIZE - 1) && (y == 0 || y == BOARD_SIZE - 1);
    }

    private bool IsNextToCorner(int x, int y)
    {
        return (x == 0 || x == 1 || x == BOARD_SIZE - 2 || x == BOARD_SIZE - 1) &&
               (y == 0 || y == 1 || y == BOARD_SIZE - 2 || y == BOARD_SIZE - 1) &&
               !IsCorner(x, y);
    }

    private bool IsDiagonal(int x, int y)
    {
        return ((x - y) == 0) && (x != 0 && y != 0 && x != BOARD_SIZE - 1 && y != BOARD_SIZE - 1);
    }

    private bool IsCenter(int x, int y)
    {
        return x > 2 && x < BOARD_SIZE-3 && y > 2 && y < BOARD_SIZE - 3;
    }

    private bool IsForwardPosition(int x, int y)
    {
        return Team == ETeam.BLACK ? y >BOARD_SIZE - 3 : y < 2;
    }

    private bool IsBackwardPosition(int x, int y)
    {
        return Team != ETeam.BLACK ? y > BOARD_SIZE - 3 : y < 2;
    }

    private bool IsLeftPosition(int x, int y)
    {
        return Team == ETeam.BLACK ? x > BOARD_SIZE - 3 : x < 2;
    }

    private bool IsRightPosition(int x, int y)
    {
        return Team != ETeam.BLACK ? x > BOARD_SIZE - 3 : x < 2;
    }

    private bool CanFlipEnemySpecialStone(int x, int y)
    {
        foreach (var dir in gameManager.StoneManagerRef.directions)
        {
            int checkX = x + (int)dir.x;
            int checkY = y + (int)dir.y;

            if (!gameManager.StoneManagerRef.CheckOutOfBoard(checkX, checkY))
            {
                var stone = gameManager.StoneManagerRef.Stones[checkX, checkY];
                if (stone != null && stone.Team != Team && (int)stone.StoneKind > 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsNextToEnemySpecialStone(int x, int y)
    {
        foreach (var dir in gameManager.StoneManagerRef.directions)
        {
            int checkX = x + (int)dir.x;
            int checkY = y + (int)dir.y;

            if (!gameManager.StoneManagerRef.CheckOutOfBoard(checkX, checkY))
            {
                var stone = gameManager.StoneManagerRef.Stones[checkX, checkY];
                if (stone != null && stone.Team != Team && stone.StoneKind != EStone.DEFAULT)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
