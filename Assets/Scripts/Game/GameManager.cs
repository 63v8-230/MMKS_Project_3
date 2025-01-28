using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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

    [SerializeField]
    private bool isTutorial = false;

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

    private bool isOption = false;
    private GameObject optionMenu;

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

        await Task.Yield();
    }

    public void GameStart()
    {
        GameStart(p1.GetComponent<IPlayer>(), p2.GetComponent<IPlayer>());
    }

    // Start is called before the first frame update
    void Start()
    {

        StoneManagerRef = gameObject.GetComponent<StoneManager>();

        StoneManagerRef.GameManagerRef = this;

        tmPro.text = $"RoomID: {Data.Instance.RoomName}\r\nプレイヤーを待っています...";

        if (Data.Instance.IsOnline)
            return;

        if(!isTutorial)
            p2 = aiPrefabs[(int)Data.Instance.AIKind];

        if (Data.Instance.cChallengeState >= 0)
        { 
            p2 = aiPrefabs[(int)Data.Instance.cRivals[Data.Instance.cChallengeState]];
            p2.GetComponent<IPlayer>().MyDeck = Data.Instance.cRivalDecks[Data.Instance.cChallengeState];
        }

        if(Data.Instance.CheatCode == "baka")
        {
            p2.GetComponent<IPlayer>().MyDeck = new Deck
            {
                Stones = new List<OwnStone>
                {
                    new OwnStone { Stone = EStone.SUN, Amount = 9999999 },
                }
            };
        }

        LunchGame();
    }

    /// <summary>
    /// ゲームを始める
    /// </summary>
    public void LunchGame()
    {
        if(Data.Instance.cChallengeState>=0)
        {
            var img = GameObject.Find("Canvas").transform.Find("Chara-W/Image").GetComponent<Image>();
            img.sprite = GetCharacterPicture(ETeam.WHITE);
        }
        

        StoneManagerRef.Init(Data.Instance.BOARD_X, Data.Instance.BOARD_Y);

        if(!Data.Instance.isTutorial)
            GameStart(p1.GetComponent<IPlayer>(), p2.GetComponent<IPlayer>());

        tmPro.text = "";
        stoneCountBlack.text = "2";
        stoneCountWhite.text = "2";

        if(Data.Instance.IsOnline)
        {
            tmPro.text = $"RoomID: {Data.Instance.RoomName}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            isOption = !isOption;

            if(isOption)
            {
                Data.Instance.InOption = true;
                optionMenu = Instantiate(Data.Instance.OptionMenu);
                StoneManagerRef.Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"),Data.Instance.CalcSeVolume());
            }
            else
            {
                PlayerPrefs.SetFloat("Master", Data.Instance.MasterVolume);
                PlayerPrefs.SetFloat("Music", Data.Instance.MusicVolume);
                PlayerPrefs.SetFloat("Se", Data.Instance.SeVolume);
                PlayerPrefs.Save();

                Data.Instance.InOption = false;
                Destroy(optionMenu);
                StoneManagerRef.Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            }
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
                        OnEndOfGame("相手のプレイヤーが試合を放棄しました...");
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
                                {
                                    Debug.Log("GameEnd");
                                    var e = DelayMethod(() => { OnEndOfGame(); }, 1);
                                    StartCoroutine(e);
                                    
                                } 
                                else
                                {
                                    isSkipped = true;
                                }
                                    
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

                            UpdateStoneCount();

                            var w = Task.Run(() => StoneManagerRef.GetPuttablePosition(ETeam.WHITE));
                            var b = Task.Run(() => StoneManagerRef.GetPuttablePosition(ETeam.BLACK));

                            if (w.Result.Length == 0 && b.Result.Length == 0)
                            {
                                Debug.Log("GameEnd");
                                var e = DelayMethod(() => { OnEndOfGame(); }, 1);
                                StartCoroutine(e);
                                break;
                            }

                            turnTask = players[currentPlayerIndex].DoTurn();
                            currentGameState = EGameState.PUT;
                            StoneManagerRef.AddTurn();
                        }
                    }

                    break;
            }
        }
    }

    public  void UpdateStoneCount()
    {
        var result = Task.Run(StoneManagerRef.GetStoneCounts);
        stoneCountBlack.text = result.Result[0].ToString();
        stoneCountWhite.text = result.Result[1].ToString();
    }

    private void OnEndOfGame(string additionalMessage = "")
    {
        isPlay = false;
        var stoneCount = Task.Run(StoneManagerRef.GetStoneCounts);

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
        if (stoneCount.Result[0] >= stoneCount.Result[1])
        {
            //winTeam = "Black Win!";
            t[0] = ETeam.BLACK;
            t[1] = ETeam.WHITE;
            sc[0] = stoneCount.Result[0];
            sc[1] = stoneCount.Result[1];
        }
        else
        {
            //winTeam = "White Win!";
            t[0] = ETeam.WHITE;
            t[1] = ETeam.BLACK;
            sc[0] = stoneCount.Result[1];
            sc[1] = stoneCount.Result[0];
        }

        //tmPro.text = $"{winTeam}\nBlack: {b}\nWhite: {w}\n{additionalMessage}";

        string[] s =
        {
            "Win",
            "Lose",
        };

        IPlayer _p = p1.GetComponent<IPlayer>();

        if (!(_p is OfflinePlayer))
        {
            _p = p2.GetComponent<IPlayer>();
        }

        
        if(_p.Team == ETeam.BLACK)
        {
            Camera.main.transform.position = new Vector3(7, 18, -3);
        }
        else
        {
            Camera.main.transform.position = new Vector3(-7, 18, 3);
        }

        
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

            if (stoneCount.Result[0] == stoneCount.Result[1]) 
            {
                result.Find(s[i] + "/Text").GetComponent<TextMeshProUGUI>().text = "引き分け...";
                ad[0] = Resources.Load<AudioClip>("Sound/Game/maou_game_jingle07");
            }

            result.Find(s[i]+"/Chara_Main").GetComponent<Image>().sprite = GetCharacterPicture(t[i]);
            result.Find(s[i] + "/Count_Back").GetComponent<Image>().sprite = GetStoneCountSprite(t[i]);
            result.Find(s[i] + "/Count_Text").GetComponent<TextMeshProUGUI>().text = sc[i].ToString();
        }

        result.Find("AdditionalMessage").GetComponent<TextMeshProUGUI>().text = additionalMessage;

        if (Data.Instance.cChallengeState < 0) 
        {
            result.Find("ReturnTitle").GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManager.LoadScene(Data.Instance.TITLE_SCENE_NAME);
            });
        }
        else
        {
            result.Find("ReturnTitle").GetComponent<Button>().onClick.AddListener(() =>
            {
                var rc = Instantiate(Resources.Load<GameObject>("Prefab/UI/Result_Challenge"));
                rc.transform.SetParent(canvas.transform, false);

                if(Data.Instance.cChallengeState >= Data.Instance.cRivalIconPath.Length-1 || t[0] == ETeam.WHITE || stoneCount.Result[0] == stoneCount.Result[1])
                {
                    Destroy(rc.transform.Find("Buttons/next").gameObject);
                }
                else
                {
                    rc.transform.Find("Buttons/next").GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (Data.Instance.cChallengeState <= Data.Instance.cUnlockRival)
                            Data.Instance.cUnlockRival++;

                        Data.Instance.cChallengeState++;

                        PlayerPrefs.SetInt("unlock", Data.Instance.cUnlockRival);
                        PlayerPrefs.Save();

                        SceneManager.LoadScene("Game");
                    });
                }

                rc.transform.Find("Buttons/retry").GetComponent<Button>().onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("Game");
                });

                rc.transform.Find("Buttons/toSelect").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Data.Instance.cChallengeState++;
                    SceneManager.LoadScene("Title");
                });
            });
        }

        
    }

    public IEnumerator DelayMethod(Action act, float seconds)
    {
        Debug.Log("GameEnd-Start");
        yield return new WaitForSeconds(seconds);
        Debug.Log("GameEnd-ACT");
        act();
        yield break;
    }

    private Sprite GetCharacterPicture(ETeam team)
    {
        if(team==ETeam.BLACK)
        {
            return Resources.Load<Sprite>("Pictures/Game/chara_white_n1");
        }
        else
        {
            if(Data.Instance.cChallengeState<0)
            {
                return Resources.Load<Sprite>("Pictures/Game/chara_black_n1");
            }
            else
            {
                return Resources.Load<Sprite>(Data.Instance.cRivalIconPath[Data.Instance.cChallengeState]+"icon");
            }

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
