using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum EGameState
{
    /// <summary>
    /// プレイヤーが石を配置する
    /// </summary>
    PUT,

    /// <summary>
    /// マネージャーが石をひっくり返したりする
    /// </summary>
    FLIP,
}


public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject p1;
    [SerializeField]
    public GameObject p2;

    [SerializeField]
    private TextMeshProUGUI tmPro;

    [SerializeField]
    public GameObject SelectedCellPrefab;


    [HideInInspector]
    public StoneManager StoneManagerRef;

    private IPlayer[] players;

    private bool isPlay = false;

    private int currentPlayerIndex = 0;

    private Task<TurnInfo> turnTask;

    private Task flipTask;

    private EGameState currentGameState;

    public async void GameStart(IPlayer player1, IPlayer player2)
    {
        players = new IPlayer[2];
        players[0] = player1;
        players[1] = player2;

        foreach (IPlayer p in players)
        {
            p.Init(this);
        }

        currentPlayerIndex = 0;
        currentGameState = EGameState.PUT;

        isPlay = true;

        turnTask = players[currentPlayerIndex].DoTurn();
        Debug.Log(players[currentPlayerIndex].Team + " is Start");

        await Task.Delay(1);
    }

    // Start is called before the first frame update
    void Start()
    {
        StoneManagerRef = gameObject.GetComponent<StoneManager>();

        StoneManagerRef.Init(Data.Instance.BOARD_X, Data.Instance.BOARD_Y);

        GameStart(p1.GetComponent<IPlayer>(), p2.GetComponent<IPlayer>());

        int b, w;
        StoneManagerRef.GetStoneCounts(out b, out w);
        tmPro.text =
            $"{players[currentPlayerIndex].Team.ToString()}\nB:{b} W:{w}";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }

        if (isPlay)
        {
            switch (currentGameState)
            {
                case EGameState.PUT:
                    {
                        if (turnTask == null)
                            break;

                        if(turnTask.IsCompleted)
                        {
                            Debug.Log(players[currentPlayerIndex].Team + $" is Put {turnTask.Result.X}-{turnTask.Result.Y}");
                            flipTask = StoneManagerRef.PutStone(turnTask.Result);
                            currentGameState = EGameState.FLIP;
                        }
                    }
                    break;

                case EGameState.FLIP:
                    {
                        if(flipTask.IsCompleted)
                        {

                            currentPlayerIndex++;
                            if (currentPlayerIndex >= players.Length)
                                currentPlayerIndex = 0;

                            int b, w;
                            StoneManagerRef.GetStoneCounts(out b, out w);
                            tmPro.text =
                                $"{players[currentPlayerIndex].Team.ToString()}\nB:{b} W:{w}";

                            turnTask = players[currentPlayerIndex].DoTurn();
                            Debug.Log(players[currentPlayerIndex].Team + " is Start");
                            currentGameState = EGameState.PUT;
                        }
                    }
                    break;
            }
        }
    }
}
