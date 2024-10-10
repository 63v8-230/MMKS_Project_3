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

    public ETeam Team { set => _team = value; get => _team; }

    private GameManager gameManager;

    async public Task<TurnInfo> DoTurn()
    {
        await Task.Delay(500);
        var puttablePosition = gameManager.StoneManagerRef.GetPuttablePosition(Team);

        PuttableCellInfo p = new PuttableCellInfo();
        p.Count = 0;

        foreach (var item in puttablePosition)
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
        t.PutStone = GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity).GetComponent<Stone>();
        t.PutStone.SetTeam(Team);

        return t;

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
