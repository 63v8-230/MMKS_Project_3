using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OfflinePlayer : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;
    public ETeam Team { set => _team = value; get => _team; }

    [SerializeField]
    private GameObject stone;

    private bool isInTurn = false;

    private TurnInfo turnInfo;

    private GameManager gameManager;

    private PuttableCellInfo[] puttablePosition;

    async public Task<TurnInfo> DoTurn()
    {

        turnInfo = new TurnInfo();
        turnInfo.X = -1;
        isInTurn = true;
        puttablePosition = gameManager.StoneManagerRef.GetPuttablePosition(Team);

        List<GameObject> puttableCell = new List<GameObject>();

        if (puttablePosition.Length > 0)
        {
            foreach (var item in puttablePosition)
            {
                puttableCell.Add(
                    GameObject.Instantiate(gameManager.SelectedCellPrefab,
                    gameManager.StoneManagerRef.CellPosition2Vector3((int)item.X, (int)item.Y),
                    Quaternion.identity));

                puttableCell[puttableCell.Count-1].transform.Find("tx").GetComponent<TextMeshPro>().text = item.Count.ToString();
            }
        }
        

        while (isInTurn)
        {
            await Task.Delay(100);

            if(puttablePosition.Length <= 0)
                isInTurn = false;
        }

        if (puttablePosition.Length > 0)
        {
            foreach (var item in puttableCell)
            {
                GameObject.Destroy(item);
            }
        }

        if(Data.Instance.IsOnline)
        {
            var hashTable1 = new ExitGames.Client.Photon.Hashtable();
            hashTable1[$"{Team.ToString()}_IsPutted"] = 1;
            hashTable1[$"{Team.ToString()}_TurnInfo_X"] = turnInfo.X;
            hashTable1[$"{Team.ToString()}_TurnInfo_Y"] = turnInfo.Y;

            hashTable1[$"{(Team == ETeam.BLACK ? ETeam.WHITE : ETeam.BLACK).ToString()}_IsPutted"] = 0;
            hashTable1[$"{(Team == ETeam.BLACK ? ETeam.WHITE : ETeam.BLACK).ToString()}_TurnInfo_X"] = 0;
            hashTable1[$"{(Team == ETeam.BLACK ? ETeam.WHITE : ETeam.BLACK).ToString()}_TurnInfo_Y"] = 0;

            PhotonNetwork.CurrentRoom.SetCustomProperties(hashTable1);
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
        if (isInTurn)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var sPos = Input.mousePosition;
                sPos.z = Camera.main.transform.position.y * 1.5f;
                Ray ray = new Ray(Camera.main.transform.position,
                    Camera.main.ScreenToWorldPoint(sPos) - Camera.main.transform.position);
                Debug.DrawLine(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(sPos), Color.red, 10);
                RaycastHit rHit;
                var hit = Physics.Raycast(ray,
                    out rHit,
                    Camera.main.transform.position.y * 1.5f,
                    LayerMask.GetMask(new string[] { "BoardCell" }));
                if(hit)
                {
                    var index = rHit.transform.gameObject.name.Split(' ');

                    if (index.Length != 2)
                        return;

                    int x;
                    if (!int.TryParse(index[0], out x))
                        return;

                    int y;
                    if (!int.TryParse(index[1], out y))
                        return;

                    if (gameManager.StoneManagerRef.Stones[x, y] != null)
                        return;

                    bool isCanPlace = false;
                    foreach (var item in puttablePosition)
                    {
                        if ((int)item.X == x && (int)item.Y == y)
                        {
                            isCanPlace = true;
                            break;
                        } 
                    }

                    if (!isCanPlace)
                        return;

                    turnInfo = new TurnInfo();
                    turnInfo.X = x;
                    turnInfo.Y = y;
                    turnInfo.PutStone = GameObject.Instantiate(stone,new Vector3(0,-10,0),Quaternion.identity).GetComponent<Stone>();
                    turnInfo.PutStone.SetTeam(Team);

                    isInTurn = false;

                }

            }
        }
    }
}
