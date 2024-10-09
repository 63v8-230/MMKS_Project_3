using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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


    // Start is called before the first frame update
    void Start()
    {
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
        PhotonNetwork.JoinOrCreateRoom(Data.Instance.RoomName, new RoomOptions(), TypedLobby.Default);
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

        if (PhotonNetwork.CurrentRoom.CustomProperties["Data_X"] != null)
            Data.Instance.BOARD_X = (int)PhotonNetwork.CurrentRoom.CustomProperties["Data_X"];

        if (PhotonNetwork.CurrentRoom.CustomProperties["Data_Y"] != null)
            Data.Instance.BOARD_Y = (int)PhotonNetwork.CurrentRoom.CustomProperties["Data_Y"];


        //プレイヤーが2人そろえば
        if (PhotonNetwork.PlayerList.Length == 2) 
        {

            Debug.Log("Ready");
            if(PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Host");
                var hashTable1 = new ExitGames.Client.Photon.Hashtable();

                hashTable1[$"{ETeam.BLACK.ToString()}_IsPutted"] = 0;
                hashTable1[$"{ETeam.BLACK.ToString()}_TurnInfo_X"] = 0;
                hashTable1[$"{ETeam.BLACK.ToString()}_TurnInfo_Y"] = 0;

                hashTable1[$"{ETeam.WHITE.ToString()}_IsPutted"] = 0;
                hashTable1[$"{ETeam.WHITE.ToString()}_TurnInfo_X"] = 0;
                hashTable1[$"{ETeam.WHITE.ToString()}_TurnInfo_Y"] = 0;

                PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable1);
            }


            if(PhotonNetwork.IsMasterClient)
            {
                gameManagerRef.p1 = GameObject.Instantiate(offlinePlayerPrefab,Vector3.zero, Quaternion.identity);
                gameManagerRef.p1.GetComponent<IPlayer>().Team = ETeam.BLACK;

                gameManagerRef.p2 = GameObject.Instantiate(onlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                gameManagerRef.p2.GetComponent<IPlayer>().Team = ETeam.WHITE;
            }
            else
            {
                gameManagerRef.p2 = GameObject.Instantiate(offlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                gameManagerRef.p2.GetComponent<IPlayer>().Team = ETeam.WHITE;

                gameManagerRef.p1 = GameObject.Instantiate(onlinePlayerPrefab, Vector3.zero, Quaternion.identity);
                gameManagerRef.p1.GetComponent<IPlayer>().Team = ETeam.BLACK;
            }

            GameObject.Destroy(messageObject);

            gameManagerRef.LunchGame();
            isStarted = true;
        }
    }
}
