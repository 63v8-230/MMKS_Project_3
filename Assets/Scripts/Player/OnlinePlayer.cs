using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OnlinePlayer : MonoBehaviourPunCallbacks, IPlayer
{
    [SerializeField]
    private ETeam _team;
    public ETeam Team { set => _team = value; get => _team; }

    [SerializeField]
    private GameObject stone;

    public Deck MyDeck { set; get; }

    public OnlineScript OnlineScriptRef { set; get; }

    private TurnInfo turnInfo;

    private GameManager gameManager;

    private bool isTurned = false;

    async public Task<TurnInfo> DoTurn()
    {

        turnInfo = new TurnInfo();

        Debug.Log("Online Start");

        while (true)
        {
            await Task.Delay(100);
            
            if(isTurned)
            {
                isTurned = false;
                break;
            }
        }


        return turnInfo;
    }

    private void OnAction(int kind, int x, int y)
    {
        turnInfo = new TurnInfo();
        turnInfo.PutStone = gameManager.StoneManagerRef.SelectStone((EStone)kind);
        if(kind > 1)
            (turnInfo.PutStone as SkillStoneBase).IsOwnerOnline = true;
        turnInfo.PutStone.SetTeam(Team);
        turnInfo.X = x;
        turnInfo.Y = y;
        isTurned = true;
    }

    public void Init(GameManager gManager)
    {
        gameManager = gManager;

        OnlineScriptRef.OnValueSet += OnAction;
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

