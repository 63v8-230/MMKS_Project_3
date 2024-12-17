using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AI�ɍ�点�Ă݂����V�уv���O����1
/// </summary>
public class AIClaude1 : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;

    [SerializeField]
    private bool isEnemy = true;

    public Deck MyDeck { get; set; }
    public ETeam Team { get => _team; set => _team = value; }
    public OnlineScript OnlineScriptRef { get; set; }

    protected GameManager gameManager;
    private int currentTurn = 0;

    // �e����΂̕]���l�i�����قǏd�v�j
    private readonly Dictionary<EStone, float> stoneValues = new Dictionary<EStone, float>
    {
        { EStone.SHIELD, 1.0f },    // �ŏd�v�F�m���ȗ̈�m��
        { EStone.SUN, 0.9f },       // �ǖʓ]���ɋ���
        { EStone.CROSS, 0.8f },     // �c���̐���
        { EStone.X, 0.7f },         // �΂߂̐���
        { EStone.ARROW, 0.6f },     // �������̂���U��
        { EStone.CRYSTAL, 0.5f },   // ����Ȉʒu���
        { EStone.CIRCLE, 0.4f }     // �Ǐ��I�Ȑ���
    };

    public virtual void Init(GameManager gManager)
    {
        gameManager = gManager;
        InitializeDeck();
    }

    private void InitializeDeck()
    {
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            new OwnStone { Stone = EStone.SUN, Amount = 1 },
            new OwnStone { Stone = EStone.CROSS, Amount = 1 },
            new OwnStone { Stone = EStone.X, Amount = 1 },
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW, Amount = 1 },
            new OwnStone { Stone = EStone.SHIELD, Amount = 1 },
            new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 }
        };
        MyDeck = d;
    }

    public async Task<TurnInfo> DoTurn()
    {
        currentTurn++;
        await Task.Delay(500); // �v�l���Ԃ̉��o

        var puttablePositions = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        while (!puttablePositions.IsCompleted)
            await Task.Delay(10);

        if (puttablePositions.Result.Length == 0)
        {
            return new TurnInfo { X = -1 };
        }

        // �헪�̑I��
        return await SelectStrategy(puttablePositions.Result);
    }

    async public Task<TurnInfo> DoComboBonus(int bonus)
    {
        List<Task<int>> cells = new List<Task<int>>();
        var bSize = gameManager.StoneManagerRef.GetBoardSize();
        for (int ix = 0; ix < bSize.x; ix++)
        {
            cells.Add(SelectCellFromColumn(ix));
        }

        var t = Task.WhenAll(cells);

        while (!t.IsCompleted) { await Task.Delay(10); }

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
                else if (Random.value < 0.3f)
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

    private async Task<TurnInfo> SelectStrategy(PuttableCellInfo[] puttablePositions)
    {
        if (currentTurn <= 4)
        {
            return await EarlyGameStrategy(puttablePositions);
        }
        else if (currentTurn <= 12)
        {
            return await MidGameStrategy(puttablePositions);
        }
        else
        {
            return await LateGameStrategy(puttablePositions);
        }
    }

    private async Task<TurnInfo> EarlyGameStrategy(PuttableCellInfo[] puttablePositions)
    {
        // ���Ղ͊p�ƕӂ��d��
        var cornerMove = await FindBestMove(puttablePositions, IsCorner);
        if (cornerMove.X != -1)
        {
            return CreateTurnInfo(cornerMove, EStone.SHIELD);
        }

        var edgeMove = await FindBestMove(puttablePositions, IsEdge);
        if (edgeMove.X != -1)
        {
            return CreateTurnInfo(edgeMove, EStone.DEFAULT);
        }

        return CreateRandomMove(puttablePositions);
    }

    private async Task<TurnInfo> MidGameStrategy(PuttableCellInfo[] puttablePositions)
    {
        // ���Ղ͓���΂�ϋɓI�Ɏg�p
        foreach (var stoneType in stoneValues.OrderByDescending(x => x.Value))
        {
            if (!HasStoneAvailable(stoneType.Key)) continue;

            var bestMove = await FindBestMove(puttablePositions,
                (x, y, size) => EvaluatePosition(x, y, size, stoneType.Key));

            if (bestMove.X != -1)
            {
                return CreateTurnInfo(bestMove, stoneType.Key);
            }
        }

        return CreateRandomMove(puttablePositions);
    }

    private async Task<TurnInfo> LateGameStrategy(PuttableCellInfo[] puttablePositions)
    {
        // �I�Ղ͎c��̓���΂��g���؂�
        foreach (var ownStone in MyDeck.Stones)
        {
            if (ownStone.Amount <= 0) continue;

            var bestMove = await FindBestMove(puttablePositions,
                (x, y, size) => EvaluatePosition(x, y, size, ownStone.Stone));

            if (bestMove.X != -1)
            {
                return CreateTurnInfo(bestMove, ownStone.Stone);
            }
        }

        return CreateRandomMove(puttablePositions);
    }

    private bool HasStoneAvailable(EStone stoneType)
    {
        return MyDeck.Stones.Any(s => s.Stone == stoneType && s.Amount > 0);
    }

    private bool EvaluatePosition(int x, int y, Vector2 size, EStone stoneType)
    {
        switch (stoneType)
        {
            case EStone.SHIELD:
                return IsCorner(x, y, size);
            case EStone.SUN:
                return x >= 2 && x <= size.x - 3 && y >= 2 && y <= size.y - 3;
            case EStone.ARROW:
                return Team == ETeam.BLACK ? y < 3 : y > size.y - 4;
            default:
                return true;
        }
    }

    private async Task<TurnInfo> FindBestMove(
        PuttableCellInfo[] positions,
        System.Func<int, int, Vector2, bool> evaluator)
    {
        TurnInfo result = new TurnInfo { X = -1 };
        int maxFlips = 0;

        foreach (var pos in positions)
        {
            if (evaluator(pos.X, pos.Y, gameManager.StoneManagerRef.GetBoardSize()))
            {
                if (pos.Count > maxFlips)
                {
                    maxFlips = pos.Count;
                    result.X = pos.X;
                    result.Y = pos.Y;
                }
            }
        }

        await Task.Delay(1);
        return result;
    }

    private TurnInfo CreateTurnInfo(TurnInfo position, EStone stoneType)
    {
        var stone = gameManager.StoneManagerRef.SelectStone(stoneType);

        if (stoneType != EStone.DEFAULT)
        {
            (stone as SkillStoneBase).IsOwnerOnline =isEnemy;
            UpdateDeck(stoneType);
        }

        stone.SetTeam(Team);

        return new TurnInfo
        {
            X = position.X,
            Y = position.Y,
            PutStone = stone
        };
    }

    private void UpdateDeck(EStone usedStone)
    {
        for (int i = 0; i < MyDeck.Stones.Count; i++)
        {
            if (MyDeck.Stones[i].Stone == usedStone)
            {
                var stone = MyDeck.Stones[i];
                stone.Amount--;
                MyDeck.Stones[i] = stone;
                break;
            }
        }
    }

    private TurnInfo CreateRandomMove(PuttableCellInfo[] positions)
    {
        var randomPos = positions[Random.Range(0, positions.Length)];
        return CreateTurnInfo(
            new TurnInfo { X = randomPos.X, Y = randomPos.Y },
            EStone.DEFAULT
        );
    }

    protected bool IsCorner(int x, int y, Vector2 size)
    {
        return (x == 0 || x == size.x - 1) && (y == 0 || y == size.y - 1);
    }

    protected bool IsEdge(int x, int y, Vector2 size)
    {
        return x == 0 || x == size.x - 1 || y == 0 || y == size.y - 1;
    }
}