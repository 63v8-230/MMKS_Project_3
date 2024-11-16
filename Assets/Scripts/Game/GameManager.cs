using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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


public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public GameObject p1;
    [SerializeField]
    public GameObject p2;

    [SerializeField]
    private TextMeshProUGUI tmPro;

    [SerializeField]
    private TextMeshProUGUI stoneCountBlack;

    [SerializeField]
    private TextMeshProUGUI stoneCountWhite;

    [SerializeField]
    public GameObject SelectedCellPrefab;

    [SerializeField]
    private List<GameObject> aiPrefabs = new List<GameObject>();

    [HideInInspector]
    public StoneManager StoneManagerRef;

    private IPlayer[] players;

    private bool isPlay = false;

    private int currentPlayerIndex = 0;

    private Task<TurnInfo> turnTask;

    private Task flipTask;

    private EGameState currentGameState;

    bool isSkipped = false;

    private float gameEndCheckCounter = 0;
    private const float GAME_END_CHECK_TIME = 5;

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

        tmPro.text = "Waiting for Player";

        if (Data.Instance.IsOnline)
            return;

        p2 = aiPrefabs[(int)Data.Instance.AIKind];

        LunchGame();
    }

    /// <summary>
    /// ゲームを始める
    /// </summary>
    public void LunchGame()
    {
        StoneManagerRef.Init(Data.Instance.BOARD_X, Data.Instance.BOARD_Y);

        GameStart(p1.GetComponent<IPlayer>(), p2.GetComponent<IPlayer>());

        int b, w;
        StoneManagerRef.GetStoneCounts(out b, out w);
        tmPro.text = "";
        stoneCountBlack.text = "0";
        stoneCountWhite.text = "0";
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
            if(Data.Instance.IsOnline)
            {
                gameEndCheckCounter += Time.deltaTime;
                if (gameEndCheckCounter > GAME_END_CHECK_TIME)
                {
                    gameEndCheckCounter = 0;

                    if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
                    {
                        OnEndOfGame("Rival Player is leave.");
                    }
                }
            }
            

            switch (currentGameState)
            {
                case EGameState.PUT:
                    {
                        if (turnTask == null)
                            break;

                        if(turnTask.IsCompleted)
                        {
                            flipTask = StoneManagerRef.PutStone(turnTask.Result);
                            if(turnTask.Result.X == -1)
                            {
                                if (isSkipped)
                                    OnEndOfGame();
                                else
                                    isSkipped = true;
                            }
                            else
                            {
                                isSkipped = false;
                            }
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
                            stoneCountBlack.text = b.ToString();
                            stoneCountWhite.text = w.ToString();

                            turnTask = players[currentPlayerIndex].DoTurn();
                            currentGameState = EGameState.PUT;
                            StoneManagerRef.AddTurn();
                        }
                    }
                    break;
            }
        }
    }

    private void OnEndOfGame(string additionalMessage = "")
    {
        isPlay = false;
        int b, w;
        StoneManagerRef.GetStoneCounts(out b, out w);

        ETeam[] t =
        {
            ETeam.NONE,//win
            ETeam.NONE,//lose
        };

        int[] sc = { 0, 0 };

        AudioClip[] ad = 
        { 
            Resources.Load<AudioClip>("Sound/Game/BGM_clear"), 
            Resources.Load<AudioClip>("Sound/Game/BGM_gameover"),
        };

        //string winTeam = "Draw...";
        if (b >= w)
        {
            //winTeam = "Black Win!";
            t[0] = ETeam.BLACK;
            t[1] = ETeam.WHITE;
            sc[0] = b;
            sc[1] = w;
        }
        else
        {
            //winTeam = "White Win!";
            t[0] = ETeam.WHITE;
            t[1] = ETeam.BLACK;
            sc[0] = w;
            sc[1] = b;
        }

        //tmPro.text = $"{winTeam}\nBlack: {b}\nWhite: {w}\n{additionalMessage}";

        string[] s =
        {
            "Win",
            "Lose",
        };


        Camera.main.transform.position = new Vector3(7, 18, -3);
        var canvas = GameObject.Find("Canvas");
        for (int i = 0; i < canvas.transform.childCount; i++) 
        {
            canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        var result = Instantiate(Resources.Load<GameObject>("Prefab/UI/Result")).transform;
        result.SetParent(canvas.transform, false);

        for (int i = 0; i < 2; i++) 
        {
            if (t[i] == ETeam.BLACK)
            {
                StoneManagerRef.Sound.Stop();
                StoneManagerRef.Sound.PlayOneShot(ad[i]);
            }

            if(b==w)
            {
                result.Find(s[i] + "/Text").GetComponent<TextMeshProUGUI>().text = "引き分け...";
            }

            result.Find(s[i]+"/Chara_Main").GetComponent<Image>().sprite = GetCharacterPicture(t[i]);
            result.Find(s[i] + "/Count_Back").GetComponent<Image>().sprite = GetStoneCountSprite(t[i]);
            result.Find(s[i] + "/Count_Text").GetComponent<TextMeshProUGUI>().text = sc[i].ToString();
        }

        result.Find("AdditionalMessage").GetComponent<TextMeshProUGUI>().text = additionalMessage;

        result.Find("ReturnTitle").GetComponent<Button>().onClick.AddListener(() => { SceneManager.LoadScene(Data.Instance.TITLE_SCENE_NAME); });
    }


    private Sprite GetCharacterPicture(ETeam team)
    {
        if(team==ETeam.BLACK)
        {
            return Resources.Load<Sprite>("Pictures/Game/chara_white_n1");
        }
        else
        {
            return Resources.Load<Sprite>("Pictures/Game/chara_black_n1");
        }
    }

    private Sprite GetStoneCountSprite(ETeam team)
    {
        if (team == ETeam.BLACK)
        {
            return Resources.Load<Sprite>("Pictures/Game/UI/UI_StoneCountB_High");
        }
        else
        {
            return Resources.Load<Sprite>("Pictures/Game/UI/UI_StoneCountW_High");
        }
    }
}
