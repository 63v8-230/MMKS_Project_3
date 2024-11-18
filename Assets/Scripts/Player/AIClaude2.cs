using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIClaude2 : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;
    public ETeam Team { set => _team = value; get => _team; }

    public Deck MyDeck { set; get; }
    public OnlineScript OnlineScriptRef { get; set; }

    private GameManager gameManager;
    private int currentTurn = 0;
    private readonly int BOARD_SIZE = 8;

    // �ꏊ�̉��l�]���p�̏d�ݕt���}�b�v
    private readonly int[,] POSITION_WEIGHTS = new int[8, 8]
    {
        {100, -20,  10,   5,   5,  10, -20, 100},
        {-20, -30,   1,   1,   1,   1, -30, -20},
        { 10,   1,   5,   2,   2,   5,   1,  10},
        {  5,   1,   2,   1,   1,   2,   1,   5},
        {  5,   1,   2,   1,   1,   2,   1,   5},
        { 10,   1,   5,   2,   2,   5,   1,  10},
        {-20, -30,   1,   1,   1,   1, -30, -20},
        {100, -20,  10,   5,   5,  10, -20, 100}
    };

    public void Init(GameManager gManager)
    {
        gameManager = gManager;

        // �f�b�L�̏�����
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            new OwnStone { Stone = EStone.SHIELD, Amount = 1 },
            new OwnStone { Stone = EStone.SUN, Amount = 1 },
            new OwnStone { Stone = EStone.CROSS, Amount = 1 },
            new OwnStone { Stone = EStone.X, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW, Amount = 1 },
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
            new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 }
        };
        MyDeck = d;
    }

    public async Task<TurnInfo> DoTurn()
    {
        currentTurn++;
        await Task.Delay(100); // �v�l���ԉ��o

        var puttablePositions = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        if (puttablePositions.Length == 0)
        {
            return new TurnInfo { X = -1 };
        }

        // �œK�Ȏ��T��
        TurnInfo bestMove = FindBestMove(puttablePositions);

        // �΂�z�u���ĕԂ�
        if (bestMove.PutStone != null)
        {
            bestMove.PutStone.SetTeam(Team);
            if (bestMove.PutStone is SkillStoneBase skillStone)
            {
                skillStone.IsOwnerOnline = true;
            }
        }

        return bestMove;
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

        // ��{�_���F�ʒu�̏d��
        score += POSITION_WEIGHTS[pos.X, pos.Y] * 10;

        // �Ԃ���΂̐��ɂ��]��
        score += pos.Count * 5;

        // �R�[�i�[�ߕӂ̕]��
        if (IsCorner(pos.X, pos.Y))
        {
            score += 1000; // �R�[�i�[�͍ŏd�v
        }
        else if (IsNextToCorner(pos.X, pos.Y))
        {
            score -= 500; // �R�[�i�[�ׂ̗͊댯
        }

        // �G�̓���΂�Ԃ���ꍇ�̕]��
        if (CanFlipEnemySpecialStone(pos.X, pos.Y))
        {
            score += 300;
        }

        return score;
    }

    private IStone SelectBestStone(int x, int y)
    {
        // �R�[�i�[�̏ꍇ
        if (IsCorner(x, y) && HasSpecialStone(EStone.SHIELD))
        {
            return CreateSpecialStone(EStone.SHIELD);
        }

        // ���Ոȍ~�ŔՖʒ����̏ꍇ
        if (currentTurn > 30 && IsCenter(x, y))
        {
            if (HasSpecialStone(EStone.SUN))
                return CreateSpecialStone(EStone.SUN);
            if (HasSpecialStone(EStone.CROSS))
                return CreateSpecialStone(EStone.CROSS);
        }

        // ���ՂőO���̏ꍇ
        if (currentTurn < 20 && IsForwardPosition(x, y))
        {
            if (HasSpecialStone(EStone.ARROW))
                return CreateSpecialStone(EStone.ARROW);
        }

        // �G�̓���΂̋߂��̏ꍇ
        if (IsNextToEnemySpecialStone(x, y))
        {
            if (HasSpecialStone(EStone.CIRCLE))
                return CreateSpecialStone(EStone.CIRCLE);
            if (HasSpecialStone(EStone.CRYSTAL))
                return CreateSpecialStone(EStone.CRYSTAL);
        }

        // �f�t�H���g�̐΂��g�p
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
        // �f�b�L����΂�����
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

    private bool IsCenter(int x, int y)
    {
        return x >= 2 && x <= 5 && y >= 2 && y <= 5;
    }

    private bool IsForwardPosition(int x, int y)
    {
        return Team == ETeam.BLACK ? y < 3 : y > 4;
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
