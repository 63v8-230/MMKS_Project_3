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

    public OnlineScript OnlineScriptRef { set; get; }

    private bool isInTurn = false;

    private TurnInfo turnInfo;

    private GameManager gameManager;

    private PuttableCellInfo[] puttablePosition;

    private EStone selectedStone = 0;

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

                var txObj = puttableCell[puttableCell.Count - 1].transform.Find("tx");
                txObj.GetComponent<TextMeshPro>().text = item.Count.ToString();

                if (Data.Instance.IsOnline && Data.Instance.IsWhite)
                    txObj.localEulerAngles += Vector3.up * 180;
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
            OnlineScriptRef.gameObject.GetComponent<PhotonView>().RPC(
                nameof(OnlineScriptRef.SetTurn),
                RpcTarget.AllViaServer,
                (int)turnInfo.PutStone.GetStone(), turnInfo.X, turnInfo.Y);

            OnlineScriptRef.SetTurn((int)turnInfo.PutStone.GetStone(), turnInfo.X, turnInfo.Y);
        }

        return turnInfo;
    }

    public void Init(GameManager gManager)
    {
        gameManager = gManager;

        if(Data.Instance.IsOnline && Team==ETeam.WHITE)
        {
            Data.Instance.IsWhite = true;
            Camera.main.transform.localEulerAngles += (Vector3.up * 180);
            Camera.main.transform.localPosition =
                new Vector3(
                    Camera.main.transform.localPosition.x,
                    Camera.main.transform.localPosition.y,
                    Camera.main.transform.localPosition.z * -1);
        }
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
                Vector3 sPos;
                sPos = Input.mousePosition;

                //sPos = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

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

                    turnInfo.PutStone = SelectStone();
                    turnInfo.PutStone.GameObjectRef.transform.Find("Plane").localPosition = new Vector3(0, 0.086f, 0);
                    turnInfo.PutStone.SetTeam(Team);

                    isInTurn = false;

                }

            }
        }
    }


    private IStone SelectStone()
    {
        IStone s;

        selectedStone = (EStone)
            (gameManager.StoneManagerRef.StoneOption.value+1);

        Debug.Log(selectedStone.ToString());

        switch (selectedStone)
        {
            case EStone.DEFAULT:
                s = 
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<Stone>();
                break;

            case EStone.SUN:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<SunStone>();
                break;

            case EStone.CROSS:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrossStone>();
                break;

            case EStone.X:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<XStone>();
                break;

            case EStone.CIRCLE:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                break;
            case EStone.ARROW:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ArrowStone>();
                break;

            case EStone.SHIELD:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ShieldStone>();
                break;

            case EStone.CRYSTAL:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrystalStone>();
                break;

            default:
                s =
                    GameObject.Instantiate(stone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                Debug.LogError("EStone None");
                break;
        }

        Debug.Log(s);

        return s;
    }
}
