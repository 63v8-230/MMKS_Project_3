using System.Collections;
using System.Collections.Generic;
//using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Task = Cysharp.Threading.Tasks.UniTask;

public class TutorialPlayerEnemy : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;

    [SerializeField]
    private GameObject stone;

    public Deck MyDeck { set; get; }

    public ETeam Team { set => _team = value; get => _team; }

    public OnlineScript OnlineScriptRef { get; set; }

    private GameManager gameManager;

    [SerializeField]
    TutorialActions tutorial;

    async public UniTask<TurnInfo> DoTurn()
    {
        await Task.Delay(500);

        TurnInfo t = new TurnInfo();

        int x, y;
        EStone eStone;
        while(!tutorial.EnemyOnSelectCell(out x, out y, out eStone))
        {
            Debug.Log(tutorial.currentProcess);
            await Task.Delay(100);

            if (!Application.isPlaying)
                return t;
        }

        t.X = x;
        t.Y = y;
        t.PutStone = gameManager.StoneManagerRef.SelectStone(eStone);
        if(eStone > EStone.DEFAULT)
        {
            (t.PutStone as SkillStoneBase).IsOwnerOnline = true;
        }
        t.PutStone.SetTeam(Team);

        return t;

    }

    async public UniTask<TurnInfo> DoComboBonus(int bonus)
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

    public void Init(GameManager gManager)
    {
        gameManager = gManager;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
