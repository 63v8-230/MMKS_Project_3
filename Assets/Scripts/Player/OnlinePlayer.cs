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

    private TurnInfo turnInfo;

    private GameManager gameManager;

    async public Task<TurnInfo> DoTurn()
    {

        turnInfo = new TurnInfo();

        Debug.Log("Online Start");

        while (true)
        {
            await Task.Delay(100);

            var b = (PhotonNetwork.CurrentRoom.CustomProperties[$"{Team.ToString()}_IsPutted"] is int vb) ? vb : -1;



            if (b > 0) 
            {
                Debug.Log("Online Put");

                var x = PhotonNetwork.CurrentRoom.CustomProperties[$"{Team.ToString()}_TurnInfo_X"];
                var y = PhotonNetwork.CurrentRoom.CustomProperties[$"{Team.ToString()}_TurnInfo_Y"];

                if (x == null || y == null)
                    continue;

                turnInfo.X = (int)x;
                turnInfo.Y = (int)y;
                Debug.Log(b);

                switch ((EStone)b)
                {
                    case EStone.DEFAULT:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<Stone>();
                        break;

                    case EStone.SUN:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<SunStone>();
                        break;

                    case EStone.CROSS:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<CrossStone>();
                        break;

                    case EStone.X:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<XStone>();
                        break;

                    case EStone.CIRCLE:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<CircleStone>();
                        break;
                    case EStone.ARROW:
                        turnInfo.PutStone = GameObject.Instantiate
                            (stone, new Vector3(0, -10, 0), Quaternion.identity)
                            .AddComponent<ArrowStone>();
                        break;

                    default:
                        Debug.LogError("EStone None");
                        break;
                }

                turnInfo.PutStone.SetTeam(Team);

                break;
            }
        }


        return turnInfo;
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

