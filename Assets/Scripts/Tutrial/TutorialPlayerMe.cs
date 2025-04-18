using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPlayerMe : MonoBehaviour, IPlayer
{
    [SerializeField]
    private ETeam _team;
    public ETeam Team { set => _team = value; get => _team; }

    public Deck MyDeck { set; get; }

    [SerializeField]

    private GameObject stone;

    [SerializeField]
    TutorialActions tutorial;

    public OnlineScript OnlineScriptRef { set; get; }

    private bool isInTurn = false;

    private TurnInfo turnInfo;

    private GameManager gameManager;

    private int currentStoneIndex = -1;
    private GameObject prevSelectedStoneButton = null;

    private bool isInBonus = false;

    private int currentBonus;
    private List<GameObject> highLight;

    [HideInInspector]
    public List<Vector2> highlightList = new List<Vector2>()
    {
        new Vector2(2,2),
    };

    

    async public Task<TurnInfo> DoTurn()
    {

        turnInfo = new TurnInfo();
        turnInfo.X = -1;
        isInTurn = true;

        List<GameObject> puttableCell = new List<GameObject>();

        if (highlightList.Count > 0)
        {
            foreach (var item in highlightList)
            {
                puttableCell.Add(
                    GameObject.Instantiate(gameManager.SelectedCellPrefab,
                    gameManager.StoneManagerRef.CellPosition2Vector3((int)item.x, (int)item.y),
                    Quaternion.identity));
            }
        }


        while (isInTurn)
        {
            await Task.Delay(100);
        }

        if (puttableCell.Count > 0)
        {
            foreach (var item in puttableCell)
            {
                GameObject.Destroy(item);
            }
        }



        return turnInfo;
    }

    async public Task<TurnInfo> DoComboBonus(int bonus)
    {
        isInBonus = true;
        currentBonus = bonus;


        var highlight = GameObject.Instantiate(gameManager.SelectedCellPrefab,
                    gameManager.StoneManagerRef.CellPosition2Vector3((int)highlightList[0].x, (int)highlightList[0].y),
                    Quaternion.identity);


        if (!SelectCell(out turnInfo, true, true))
        {
            turnInfo.X = 0;
            turnInfo.Y = 0;
        }


        highLight = new List<GameObject>();//範囲プレビュー
        foreach (var item in gameManager.StoneManagerRef.ComboBonus[bonus])
        {
            highLight.Add(
                    GameObject.Instantiate(gameManager.SelectedCellPrefab,
                    gameManager.StoneManagerRef.CellPosition2Vector3((int)item.x, (int)item.y),
                    Quaternion.identity));

            Destroy(highLight[highLight.Count - 1].transform.Find("tx").gameObject);
        }

        while (isInBonus) { await Task.Delay(10); }

        foreach (var item in highLight)
        {
            Destroy(item);
        }

        highLight.Clear();
        Destroy(highlight);

        return turnInfo;
    }

    private void UpdateComboHighlight(int x, int y)
    {
        for (int i = 0; i < highLight.Count; i++)
        {
            highLight[i].transform.position =
                gameManager.StoneManagerRef.CellPosition2Vector3
                ((int)gameManager.StoneManagerRef.ComboBonus[currentBonus][i].x + x,
                (int)gameManager.StoneManagerRef.ComboBonus[currentBonus][i].y + y);
        }
    }

    public void Init(GameManager gManager)
    {
        gameManager = gManager;

        if (Data.Instance.IsOnline && Team == ETeam.WHITE)
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
        EStone s;
        if (index == -1)
            s = EStone.DEFAULT;
        else
            s = EStone.CIRCLE;

        if (!tutorial.OnSelectStone(s))
            return;

        Debug.Log("!!!!->" + index);
        currentStoneIndex = index;

        if (prevSelectedStoneButton != null)
            prevSelectedStoneButton.transform.Find("Vert/Button/Stone_Select").gameObject.SetActive(false);

        button.transform.Find("Vert/Button/Stone_Select").gameObject.SetActive(true);

        prevSelectedStoneButton = button;
    }

    private void InitUI()
    {
        //石選択UIを取得
        var ct = GameObject.Find("Canvas/StoneSelect/Scroll View/Viewport/Content").transform;
        var srcObj = Resources.Load("Prefab/UI/StoneContent") as GameObject;

        //通常石を追加
        var ui_Default = Instantiate(srcObj);
        ui_Default.transform.SetParent(ct, false);
        var ui_Default_Button = ui_Default.transform.Find("Vert/Button");
        ui_Default_Button.GetComponent<Button>().onClick.AddListener(() => { SetStone(-1, ui_Default); });
        Destroy(ui_Default_Button.Find("Stone_Logo").gameObject);
        ui_Default.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "";
        ui_Default_Button.transform.Find("Stone_Select").gameObject.SetActive(true);
        ui_Default.transform.Find("Desc").GetComponent<Image>().color = new Color(0, 0, 0, 0);
        prevSelectedStoneButton = ui_Default;

        var sManager = GameObject.Find("GameManager").GetComponent<StoneManager>();

        //デッキにある石を追加
        for (int i = 0; i < MyDeck.Stones.Count; i++)
        {
            var ui = Instantiate(srcObj);
            ui.transform.SetParent(ct, false);
            var ui_Button = ui.transform.Find("Vert/Button");
            int _index = i;
            ui_Button.GetComponent<Button>().onClick.AddListener(() => { SetStone(_index, ui); });
            var lg = ui_Button.Find("Stone_Logo");
            lg.GetComponent<Image>().sprite = sManager.GetSprite(MyDeck.Stones[i].Stone);
            ui.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "x" + MyDeck.Stones[i].Amount;
            ui.transform.Find("Desc").GetComponent<Image>().sprite = sManager.GetDescription(MyDeck.Stones[i].Stone);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Deck d = new Deck();
        d.Stones = new List<OwnStone>
        {
            new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
        };
        MyDeck = d;

        InitUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInTurn)
        {
            InTurn();
        }

        if (isInBonus)
        {
            if (SelectCell(out turnInfo, true, true))
            {
                var canPass = tutorial.OnComboBonus(turnInfo.X, turnInfo.Y);
                if (Input.GetMouseButtonDown(0))
                {
                    if (!canPass)
                        return;

                    isInBonus = false;
                    return;
                }
                UpdateComboHighlight(turnInfo.X, turnInfo.Y);
            }
        }
    }

    private void InTurn()
    {
        if (SelectCell(out turnInfo))
        {
            if (!tutorial.OnSelectCell(turnInfo.X, turnInfo.Y))
                return;

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

    /// <summary>
    /// クリックでマスを選ぶ。これはUpdateで呼ぶ必要がある
    /// </summary>
    /// <param name="info">XとYだけ入って出てくる</param>
    /// <param name="disableCellCheck">Trueで取りあえず選んだセルを無条件に返して来る</param>
    /// <param name="withoutClick">クリックとか関係なし</param>
    /// <returns>もし取れたらTrue</returns>
    private bool SelectCell(out TurnInfo info, bool disableCellCheck = false, bool withoutClick = false)
    {
        info = new TurnInfo();

        if (Input.GetMouseButtonDown(0) || withoutClick)
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
            if (hit)
            {
                var index = rHit.transform.gameObject.name.Split(' ');

                if (index.Length != 2)
                    return false;

                int x;
                if (!int.TryParse(index[0], out x))
                    return false;

                int y;
                if (!int.TryParse(index[1], out y))
                    return false;

                if (disableCellCheck)
                {
                    info.X = x;
                    info.Y = y;
                    return true;
                }

                if (gameManager.StoneManagerRef.Stones[x, y] != null)
                    return false;

                info.X = x;
                info.Y = y;

                return true;
            }

        }

        return false;
    }
}
