using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OfflinePlayer : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;
    public ETeam Team { set => _team = value; get => _team; }

    public Deck MyDeck { set; get; }

    [SerializeField]

    private GameObject stone;

    public OnlineScript OnlineScriptRef { set; get; }

    private bool isInTurn = false;

    private TurnInfo turnInfo;

    private GameManager gameManager;

    private PuttableCellInfo[] puttablePosition;

    private int currentStoneIndex = -1;
    private GameObject prevSelectedStoneButton = null;

    async public Task<TurnInfo> DoTurn()
    {

        turnInfo = new TurnInfo();
        turnInfo.X = -1;
        isInTurn = true;
        var puttablePositionTask = gameManager.StoneManagerRef.GetPuttablePosition(Team);
        while (!puttablePositionTask.IsCompleted)
            await Task.Delay(10);

        puttablePosition = puttablePositionTask.Result;

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

    public void SetStone(int index, GameObject button)
    {
        Debug.Log("!!!!->"+index);
        currentStoneIndex = index;

        if(prevSelectedStoneButton!=null)
            prevSelectedStoneButton.transform.Find("Vert/Button/Stone_Select").gameObject.SetActive(false);

        button.transform.Find("Vert/Button/Stone_Select").gameObject.SetActive(true);

        prevSelectedStoneButton = button;
    }

    // Start is called before the first frame update
    void Start()
    {
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            new OwnStone { Stone = EStone.X, Amount = 1 },
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
            new OwnStone { Stone = EStone.SUN, Amount = 1 },
            new OwnStone { Stone = EStone.CRYSTAL, Amount = 1 },
            new OwnStone { Stone = EStone.CROSS, Amount = 1 },
            new OwnStone { Stone = EStone.SHIELD, Amount = 1 },
            new OwnStone { Stone = EStone.ARROW, Amount = 1 }
        };
        MyDeck = d;

        var ct = GameObject.Find("Canvas/StoneSelect/Scroll View/Viewport/Content").transform;

        var srcObj = Resources.Load("Prefab/UI/StoneContent") as GameObject;

        var ui_Default = Instantiate(srcObj);
        ui_Default.transform.SetParent(ct, false);
        var ui_Default_Button = ui_Default.transform.Find("Vert/Button");
        ui_Default_Button.GetComponent<Button>().onClick.AddListener(() => { SetStone(-1,ui_Default); });
        Destroy(ui_Default_Button.Find("Stone_Logo").gameObject);
        ui_Default.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "";
        ui_Default_Button.transform.Find("Stone_Select").gameObject.SetActive(true);
        ui_Default.transform.Find("Desc").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        prevSelectedStoneButton = ui_Default;

        var sManager = GameObject.Find("GameManager").GetComponent<StoneManager>();

        for (int i = 0; i < MyDeck.Stones.Count; i++) 
        {
            var ui = Instantiate(srcObj);
            ui.transform.SetParent(ct, false);
            var ui_Button = ui.transform.Find("Vert/Button");
            int _index = i;
            ui_Button.GetComponent<Button>().onClick.AddListener(() => { SetStone(_index,ui); });
            ui_Button.Find("Stone_Logo").GetComponent<Image>().sprite = sManager.GetSprite(MyDeck.Stones[i].Stone);
            ui.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "x"+ MyDeck.Stones[i].Amount;
            ui.transform.Find("Desc").GetComponent<Image>().sprite = sManager.GetDescription(MyDeck.Stones[i].Stone);
        }
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

                    Debug.Log(currentStoneIndex);
                    int kind = currentStoneIndex;
                    if (kind == -1)
                        kind = (int)EStone.DEFAULT;
                    else 
                    {
                        var st = MyDeck.Stones[kind];
                        st.Amount--;
                        MyDeck.Stones[kind] = st;
                        prevSelectedStoneButton.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "x" + st.Amount;
                        if (st.Amount <= 0) 
                        {
                            Destroy(prevSelectedStoneButton);
                            currentStoneIndex = -1;
                        }

                        kind = (int)MyDeck.Stones[kind].Stone;
                    }


                    turnInfo.PutStone = gameManager.StoneManagerRef.SelectStone((EStone)kind);
                    turnInfo.PutStone.SetTeam(Team);

                    isInTurn = false;

                }

            }
        }
    }
}
