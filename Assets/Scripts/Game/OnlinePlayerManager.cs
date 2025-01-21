using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class OnlinePlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameManager gameManagerRef;

    [SerializeField]
    private GameObject onlinePlayerPrefab;

    [SerializeField]
    private GameObject offlinePlayerPrefab;

    [SerializeField]
    private GameObject messageObject;

    private bool isStarted = false;

    private bool isChecked = false;

    private bool isReadyToStart = false;


    // Start is called before the first frame update
    void Start()
    {
        Data.Instance.IsWhite = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// サーバーから切断
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"サーバーとの接続が切断されました: {cause.ToString()}");
    }

    /// <summary>
    /// マスターサーバーへ接続
    /// </summary>
    public override void OnConnectedToMaster()
    {
        var opt = new RoomOptions();
        opt.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(Data.Instance.RoomName, opt, TypedLobby.Default);
    }

    /// <summary>
    /// ルーム作成
    /// </summary>
    public override void OnCreatedRoom()
    {
        Debug.Log("ルームの作成に成功しました");

        var hashTable1 = new ExitGames.Client.Photon.Hashtable();
        hashTable1["Data_X"] = Data.Instance.BOARD_X;
        hashTable1["Data_Y"] = Data.Instance.BOARD_Y;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable1);
    }

    /// <summary>
    /// ルームへ参加
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームへ参加しました");
        PhotonNetwork.Instantiate("OnlineObject", Vector3.zero, Quaternion.identity);
    }


    void OnDestroy()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }


    // Update is called once per frame
    void Update()
    {

        if (isStarted || PhotonNetwork.CurrentRoom == null) 
            return;

        var isJust2 = DateTime.Now.Second % 2 == 0;

        Debug.Log($"Sec: {DateTime.Now.Second}");

        if (!isJust2)
            isChecked = false;

        if (isJust2 && !isChecked)
        {
            isChecked = true;

            if (isReadyToStart)
            {
                var l = GameObject.FindObjectsByType<PhotonView>(FindObjectsSortMode.None);

                Debug.Log($"p1:{l[0].gameObject.GetComponent<OnlineScript>().IsLoad}\np2:{l[1].gameObject.GetComponent<OnlineScript>().IsLoad}");

                if (l[0].gameObject.GetComponent<OnlineScript>().IsLoad &&
                    l[1].gameObject.GetComponent<OnlineScript>().IsLoad)
                {
                    gameManagerRef.LunchGame();
                    isStarted = true;
                    return;
                }
                return;
            }


            //プレイヤーが2人そろえば
            if (GameObject.FindObjectsByType<PhotonView>(FindObjectsSortMode.None).Length == 2)
            {
                Debug.Log("Ready");
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("Host");
                    var hashTable1 = new ExitGames.Client.Photon.Hashtable();

                    //hashTable1[$"{ETeam.BLACK.ToString()}_IsPutted"] = 0;
                    //hashTable1[$"{ETeam.BLACK.ToString()}_TurnInfo_X"] = 0;
                    //hashTable1[$"{ETeam.BLACK.ToString()}_TurnInfo_Y"] = 0;

                    //hashTable1[$"{ETeam.WHITE.ToString()}_IsPutted"] = 0;
                    //hashTable1[$"{ETeam.WHITE.ToString()}_TurnInfo_X"] = 0;
                    //hashTable1[$"{ETeam.WHITE.ToString()}_TurnInfo_Y"] = 0;

                    PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable1);
                }

                IPlayer pRef1, pRef2;
                OnlineScript mine = null;

                if (PhotonNetwork.IsMasterClient)
                {
                    gameManagerRef.p1 = GameObject.Instantiate(offlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                    pRef1 = gameManagerRef.p1.GetComponent<IPlayer>();
                    pRef1.Team = ETeam.BLACK;

                    gameManagerRef.p2 = GameObject.Instantiate(onlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                    pRef2 = gameManagerRef.p2.GetComponent<IPlayer>();
                    pRef2.Team = ETeam.WHITE;
                }
                else
                {
                    gameManagerRef.p2 = GameObject.Instantiate(offlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                    pRef1 = gameManagerRef.p2.GetComponent<IPlayer>();
                    pRef1.Team = ETeam.WHITE;

                    gameManagerRef.p1 = GameObject.Instantiate(onlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                    pRef2 = gameManagerRef.p1.GetComponent<IPlayer>();
                    pRef2.Team = ETeam.BLACK;

                    var cvs = GameObject.Find("Canvas");

                    RectTransform lu = cvs.transform.Find("StoneCountW").GetComponent<RectTransform>();
                    RectTransform rd = cvs.transform.Find("StoneCountB").GetComponent<RectTransform>();
                    lu.anchoredPosition = new Vector2 (-64, 64);
                    lu.anchorMin = new Vector2 (1, 0);
                    lu.anchorMax = new Vector2 (1, 0);

                    rd.anchoredPosition = new Vector2(64, -64);
                    rd.anchorMin = new Vector2(0, 1);
                    rd.anchorMax = new Vector2(0, 1);
                }


                foreach (var item in GameObject.FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
                {
                    if (item.IsMine)
                    {
                        mine = pRef1.OnlineScriptRef = item.gameObject.GetComponent<OnlineScript>();
                    }
                    else
                    {
                        pRef2.OnlineScriptRef = item.gameObject.GetComponent<OnlineScript>();
                    }
                }

                GameObject.Destroy(messageObject);
                mine.IsLoad = true;

                isReadyToStart = true;
            }

        }
    }
}
