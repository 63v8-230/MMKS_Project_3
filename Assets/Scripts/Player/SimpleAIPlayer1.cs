using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleAIPlayer1 : MonoBehaviour , IPlayer
{
    [SerializeField]
    private ETeam _team;

    [SerializeField]
    private GameObject stone;

    public Deck MyDeck { set; get; }

    public ETeam Team { set => _team = value; get => _team; }

    public OnlineScript OnlineScriptRef { get; set; }

    private GameManager gameManager;

    async public Task<TurnInfo> DoTurn()
    {
        await Task.Delay(500);
        var puttablePosition = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        while (!puttablePosition.IsCompleted)
            await Task.Delay(10);

        PuttableCellInfo p = new PuttableCellInfo();
        p.Count = 0;

        foreach (var item in puttablePosition.Result)
        {
            if (item.Count > p.Count)
            {
                p = item;
            }
            else if(item.Count == p.Count)
            {
                if (Random.value > 0.5f)
                    p = item;
            }
        }

        TurnInfo t = new TurnInfo();

        if (p.Count == 0)
        {
            
            t.X = -1;
            return t;
        }

        t.X = p.X;
        t.Y = p.Y;
        t.PutStone = GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity).AddComponent<Stone>();
        t.PutStone.SetTeam(Team);

        return t;

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

        while (!t.IsCompleted) { await Task.Delay(10); }

        int xv = -1, yv = -1;

        for(int i=0; i<bSize.x; i++)
        {
            if (cells[i].Result!=-1)
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
        for(int i = 0; i < gameManager.StoneManagerRef.Stones.GetLength(1); i++)
        {
            if (gameManager.StoneManagerRef.Stones[x,i]!=null)
            {
                if(gameManager.StoneManagerRef.Stones[x, i].Team != Team)
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
