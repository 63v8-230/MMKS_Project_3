using Photon.Pun.Demo.Procedural;
using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public enum ETeam
{
    NONE,
    BLACK,
    WHITE,
    WALL,
}

public enum EDirection
{
    TOP,
    TOP_RIGHT,
    RIGHT,
    DOWN_RIGHT,
    DOWN,
    DOWN_LEFT,
    LEFT,
    TOP_LEFT,
}

public struct PuttableCellInfo
{
    public int X;
    public int Y;
    public int Count;
}

public struct SkillAction
{
    public System.Func<StoneManager, Vector2, IEnumerator> Action;
    public Vector2 Position;
    public String Name;
    public ETeam Team;
    public AudioClip Sound;
}


public class ExEnumerator : IEnumerator
{
    private IEnumerator instance;
    public bool IsEnd { get; private set; }
    public ExEnumerator(IEnumerator enumerator)
    {
        this.instance = enumerator;
    }
    public object Current => instance.Current;

    public bool MoveNext()
    {
        bool hasNext = instance.MoveNext();
        IsEnd = !hasNext;
        return hasNext;
    }

    public void Reset()
    {
        instance.Reset();
        IsEnd = false;
    }
}


/// <summary>
/// 石を管理するクラス
/// </summary>
public class StoneManager : MonoBehaviour
{

    [Header("オセロ盤の真ん中")]
    /// <summary>
    /// オセロ盤の真ん中
    /// </summary>
    public Transform BoardCenter;

    [Header("オセロ盤セル")]
    /// <summary>
    /// オセロ盤の1マス
    /// </summary>
    public GameObject BoardCell;

    [SerializeField]
    private GameObject basicStone;

    [SerializeField]
    public TMP_Dropdown StoneOption;

    [SerializeField]
    public GameObject HighlightCellObject;

    [SerializeField]
    public GameObject LightStoneObject;

    public GameManager GameManagerRef;

    /// <summary>
    /// 盤面上の石。何もないときはNullになる。
    /// </summary>
    public IStone[,] Stones;

    /// <summary>
    /// <para>特殊能力</para>
    /// <para>ここに特殊能力を実行するメソッドを入れる</para>
    /// </summary>
    private List<SkillAction> skillMethod = new List<SkillAction>();

    public AudioSource Sound;

    private AudioSource pitchTunableAudio;

    [NonSerialized]
    public Vector2[] directions = new Vector2[]
        {
            new Vector2(0, -1),  //T
            new Vector2(1, -1),  //RT
            new Vector2(1, 0),   //R
            new Vector2(1, 1),   //RD
            new Vector2(0, 1),   //D
            new Vector2(-1, 1),  //LD
            new Vector2(-1, 0),  //L
            new Vector2(-1, -1), //LT
        };

    public int CurrentTurn = 0;

    public void AddTurn()
    {
        CurrentTurn++;
    }

    public void Start()
    {
        Sound = GetComponent<AudioSource>();
        pitchTunableAudio = gameObject.AddComponent<AudioSource>();
        pitchTunableAudio.volume = Sound.volume;
    }

    public void AddSkillMethod(SkillAction action)
    {
        Debug.Log($"====SkillAdd====\n{action.Name}\n{action.Position}");
        skillMethod.Add(action);
    }


    /// <summary>
    /// オセロ盤を作成して、色々初期化処理
    /// </summary>
    /// <param name="xSize">横方向のサイズ</param>
    /// <param name="ySize">縦方向のサイズ</param>
    public void Init(int xSize, int ySize)
    {
        Stones = new IStone[xSize, ySize];

        var startPosition = new Vector3(-xSize + 1, 0, -ySize + 1);

        for(int ix = 0; ix < xSize; ix++)
        {
            for (int iy = 0; iy < ySize; iy++)
            {
                var cell = GameObject.Instantiate
                    (BoardCell,
                    startPosition + new Vector3(ix * 2, 0, iy * 2),
                    Quaternion.identity);

                cell.name = $"{ix} {iy}";

                cell.transform.parent = BoardCenter;
            }
        }

        var cam = Camera.main.gameObject;
        int largeSize = xSize;
        if(ySize > xSize) largeSize = ySize;
        cam.transform.position = new Vector3(0, largeSize * 2.3f, largeSize * -0.7f);


        Stones[xSize / 2 - 1, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2 - 1),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2 - 1, ySize / 2 - 1].SetTeam(ETeam.BLACK);

        Stones[xSize / 2, ySize / 2 - 1] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2 - 1),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2, ySize / 2 - 1].SetTeam(ETeam.WHITE);

        Stones[xSize / 2 - 1, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2 - 1, ySize / 2),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2 - 1, ySize / 2].SetTeam(ETeam.WHITE);

        Stones[xSize / 2, ySize / 2] = GameObject.Instantiate(
            basicStone,
            CellPosition2Vector3(xSize / 2, ySize / 2),
            Quaternion.identity).AddComponent<Stone>();
        Stones[xSize / 2, ySize / 2].SetTeam(ETeam.BLACK);
    }

    private IEnumerator SetTransform(GameObject gObject, Vector3 value)
    {
        gObject.transform.position = value;

        yield break;
    }

    private void OnSkill(FireFrameUIController frameCom)
    {
        Debug.Log("OnSKill Frame");
        frameCom.SetColor();
        frameCom.ComboCount();
    }

    async public Task PutStone(TurnInfo info)
    {
        if (info.X == -1)
            return;

        Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/Put"));

        Stones[info.X,info.Y] = info.PutStone;

        var e = SetTransform(info.PutStone.GameObjectRef,
            new Vector3(
                (-Stones.GetLength(0) + 1) + (info.X * 2),
                0.15f,
                (-Stones.GetLength(1) + 1) + (info.Y * 2)));
        StartCoroutine(e);

        info.PutStone.GameObjectRef.name = $"Stone {info.X}-{info.Y}";


        var checkTasks = directions.Select(dir => CheckLineAsync(info.X, info.Y, dir, info.PutStone.Team)).ToArray();
        var results = await Task.WhenAll(checkTasks);

        await Task.Delay(300);

        var flipTasks = new List<Task>();
        for (int i = 0; i < directions.Length; i++)
        {
            if (results[i])
            {
                flipTasks.Add(FlipStones(info.X, info.Y, directions[i], info.PutStone.Team));
            }
        }

        //flipTasksのタスクが全て完了するまで待つ
        var t = Task.WhenAll(flipTasks);

        while (!t.IsCompleted)
        {
            await Task.Delay(1);
        }

        Debug.Log("Skill Count: "+skillMethod.Count);

        if (skillMethod.Count <= 0)
        {
            skillMethod.Clear();

            await Task.Delay(100);

            return;
        }
            

        IPlayer comboPlayer = null;
        if(skillMethod[0].Team == ETeam.BLACK)
        {
            var p1 = GameManagerRef.p1.GetComponent<IPlayer>();
            if (p1.Team==ETeam.WHITE)
                comboPlayer = p1;
            else
                comboPlayer = GameManagerRef.p2.GetComponent<IPlayer>();
        }
        else
        {
            var p1 = GameManagerRef.p1.GetComponent<IPlayer>();
            if (p1.Team == ETeam.BLACK)
                comboPlayer = p1;
            else
                comboPlayer = GameManagerRef.p2.GetComponent<IPlayer>();
        }

        var frame = Instantiate(Resources.Load<GameObject>("Movie/FireScreen"));
        frame.transform.SetParent(GameObject.Find("Canvas").transform, false);
        var frameCom = frame.GetComponent<FireFrameUIController>();
        frameCom.Init();
        //frameCom.SetColor();
        Debug.Log("Fire Create");

        int count = 0;

        while(skillMethod.Count!=0)
        {
            var l = new List<ExEnumerator>();
            var la = new List<AudioClip>();
            var cTeam = ETeam.NONE;
            int _index = -1;
            while(true)
            {
                _index++;
                if (_index >= skillMethod.Count)
                    break;

                if(cTeam==ETeam.NONE)
                    cTeam = skillMethod[_index].Team;

                if(cTeam== skillMethod[_index].Team)
                {
                    l.Add(new ExEnumerator(skillMethod[_index].Action.Invoke(this, skillMethod[_index].Position)));
                    la.Add(skillMethod[_index].Sound);
                }
                else
                {
                    break;
                }
            }

            OnSkill(frameCom);
            count++;
            var cti = new ExEnumerator(ShowCutIn(cTeam == ETeam.WHITE, count));
            StartCoroutine(cti);
            while (!cti.IsEnd) { await Task.Delay(1); }

            

            for (int i = 0; i < l.Count; i++) 
            {
                StartCoroutine(l[i]);
                Sound.PlayOneShot(la[i]);
            }

            bool isEnd = false;

            while (!isEnd)
            {
                await Task.Delay(1);
                foreach (var col in l)
                {
                    if (col.IsEnd)
                    {
                        isEnd = true;
                        break;
                    }
                }
            }



            while (skillMethod[0].Team == cTeam)
            {
                skillMethod.RemoveAt(0);
                if (skillMethod.Count <= 0)
                    break;
            }
        }

        int BonusCount = frameCom.GetCurrentState();

        bool isPlayerOffline = false;

        if (comboPlayer is OfflinePlayer && BonusCount > 0) 
        {
            frameCom.ShowComboEnter();
            Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/ComboBonus"), 0.8f);
            await Task.Delay(5000);
            isPlayerOffline = true;
        }

        

        //コンボボーナス
        for (int i=0; i<BonusCount; i++)
        {
            if(isPlayerOffline)
                frameCom.SetText($"コンボボーナスにより、あと{BonusCount - i}回\r\n任意の敵の色を自分の色に変更出来ます。");

            var comboTask = comboPlayer.DoComboBonus();
            while (!comboTask.IsCompleted) { await Task.Delay(10); }
            FlipStone(comboTask.Result.X, comboTask.Result.Y, comboPlayer.Team, true, true);

            var pt = GetPuttablePosition(comboPlayer.Team);
            while (!pt.IsCompleted) { await Task.Delay(10); }
            if (pt.Result.Length <= 0)
                break;
        }
        
        Debug.Log("Destroy Fire");
        Destroy(frame);
        skillMethod.Clear();

        await Task.Delay(100);
        
    }

    public void FlipStone(int x, int y, ETeam team, bool isSkill = false, bool isComboBonus = false)
    {
        var s = Stones[x, y];
        if(s != null)
        {
            if(s.Team != team)
            {
                Debug.Log(isSkill);
                if(isComboBonus)
                    s.SetTeam(team, null, x, y);
                else
                    s.SetTeam(team, this, x, y);
                if (!isSkill)
                    Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/Turn"));

                if(isComboBonus)
                    Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/ComboFlip"));

                var e = s.OnFlip(isSkill);
                StartCoroutine(e);
            }
                
        } 
    }

    /// <summary>
    /// 現在の石を数える。。
    /// </summary>
    /// <returns>[0]:黒 [1]:白</returns>
    public async Task<int[]> GetStoneCounts()
    {
        int black = 0;
        int white = 0;

        int boardSizeX = Stones.GetLength(0);

        List < Task<int[]>> t = new List<Task<int[]>>();

        for (int i = 0; true; i += 3) 
        {
            if (i+3 < boardSizeX)
            {
                t.Add(CountStone(i, i+3));
                Debug.Log($"async {i} - {i + 3}");
            }
            else
            {
                t.Add(CountStone(i, boardSizeX));
                Debug.Log($"async {i} - {boardSizeX}");
                break;
            }
            
        }

        await Task.WhenAll(t);



        foreach (var item in t)
        {
            black += item.Result[0];
            white += item.Result[1];
        }

        return new int[] { black, white };
    }

    /// <summary>
    /// 非同期で指定した行範囲の石の数を数える
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    /// <returns>[0]が黒、[1]が白</returns>
    private async Task<int[]> CountStone(int startX, int endX)
    {
        await Task.Yield();

        int[] count = new[] { 0, 0 };

        for (int ix = startX; ix < endX; ix++)
        {
            for (int iy = 0; iy < Stones.GetLength(1); iy++)
            {
                if (Stones[ix, iy] == null)
                    continue;

                switch (Stones[ix, iy].Team)
                {
                    case ETeam.BLACK:
                        count[0]++;
                        break;
                    case ETeam.WHITE:
                        count[1]++;
                        break;
                }
            }
        }

        return count;
    }

    /// <summary>
    /// 盤の位置を座標に変換
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CellPosition2Vector3(int x, int y)
    {
        var startPosition = new Vector3(-Stones.GetLength(0) + 1, 0.15f , -Stones.GetLength(1) + 1);
        return startPosition + new Vector3(x * 2, 0, y * 2);

    }

    /// <summary>
    /// 範囲外チェック
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>範囲外ならTrue</returns>
    public bool CheckOutOfBoard(int x, int y)
    {
        if (x < 0 || y < 0) return true;
        if (x >= Stones.GetLength(0) || y >= Stones.GetLength(1)) return true;

        return false;
    }

    public Vector2 GetBoardSize()
    {
        return new Vector2(Stones.GetLength(0), Stones.GetLength(1));
    }

    /// <summary>
    /// 配置可能なマスを返す
    /// </summary>
    /// <returns></returns>
    public async Task<PuttableCellInfo[]> GetPuttablePosition(ETeam myTeam)
    {
        List<Task<List<PuttableCellInfo>>> t = new List<Task<List<PuttableCellInfo>>>();

        int boardSizeX = Stones.GetLength(0);

        for (int i = 0; true; i += 3)
        {
            if (i + 3 < boardSizeX)
            {
                t.Add(SearchPuttableCells(myTeam,i, i + 3));
                Debug.Log($"async {i} - {i + 3}");
            }
            else
            {
                t.Add(SearchPuttableCells(myTeam, i, boardSizeX));
                Debug.Log($"async {i} - {boardSizeX}");
                break;
            }
        }

        var wt = Task.WhenAll(t);
        while (!wt.IsCompleted) { await Task.Delay(10); }

        List<PuttableCellInfo> p = new List<PuttableCellInfo>();

        foreach (var item in t)
        {
            p.AddRange(item.Result);
        }

        return p.ToArray();

    }

    private async Task<List<PuttableCellInfo>> SearchPuttableCells(ETeam myTeam, int startX, int endX)
    {
        List<PuttableCellInfo> returnList = new List<PuttableCellInfo>();

        for (int ix = startX; ix < endX; ix++)
        {
            for (int iy = 0; iy < Stones.GetLength(1); iy++)
            {
                if (Stones[ix, iy] != null)
                    continue;

                List<Task<int>> t = new List<Task<int>>();

                int pCount = 0;
                foreach (var dir in directions)
                {
                    if (CheckOutOfBoard(ix + (int)dir.x, iy + (int)dir.y))
                        continue;

                    var stone = Stones[ix + (int)dir.x, iy + (int)dir.y];
                    if (stone == null)
                        continue;

                    if (stone.Team == myTeam)
                        continue;

                    t.Add(CheckLine(ix, iy, dir, myTeam));

                    //int count = t.Result;
                    //if (count == -1)
                    //    continue;

                    //pCount += count;
                }

                await Task.WhenAll(t);

                foreach (var task in t)
                {
                    if (task.Result == -1)
                        continue;

                    pCount += task.Result;
                }

                if (pCount > 0)
                {
                    PuttableCellInfo p = new PuttableCellInfo();
                    p.X = ix;
                    p.Y = iy;
                    p.Count = pCount;
                    returnList.Add(p);
                }
            }
        }

        return returnList;
    }

    /// <summary>
    /// 指定した方向で石がひっくり返るかどうかをチェックする。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    async private Task<bool> CheckLineAsync(int x, int y, Vector2 direction, ETeam myTeam)
    {
        await Task.Delay(1);

        bool canFlip = false;

        x += (int)direction.x;
        y += (int)direction.y;

        if (CheckOutOfBoard(x, y)) return false;

        if (Stones[x, y] == null ||
            Stones[x, y].Team == myTeam)
        {
            return false;
        }


        while (true)
        {
            x += (int)direction.x;
            y += (int)direction.y;


            if (CheckOutOfBoard(x, y)) break;

            if (Stones[x, y] == null) break;

            if (Stones[x, y].Team == myTeam)
            {
                canFlip = true;
                break;
            }
        }

        return canFlip;
    }

    /// <summary>
    /// 指定した場所から指定した方向へ、ひっくり返せるかどうかを判定する。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="direction"></param>
    /// <param name="myTeam"></param>
    /// <returns>ひっくり返せなければ-1、ひっくり返せる場合はその数を返す</returns>
    private async Task<int> CheckLine(int x, int y, Vector2 direction, ETeam myTeam)
    {
        await Task.Yield();

        bool canFlip = false;

        x += (int)direction.x;
        y += (int)direction.y;

        int puttableCount = 0;

        if (CheckOutOfBoard(x, y)) return -1;

        if (Stones[x, y] == null ||
            Stones[x, y].Team == myTeam)
        {
            return -1;
        }

        puttableCount++;

        while (true)
        {
            x += (int)direction.x;
            y += (int)direction.y;
            puttableCount++;


            if (CheckOutOfBoard(x, y)) break;

            if (Stones[x, y] == null) break;

            if (Stones[x, y].Team == myTeam)
            {
                canFlip = true;
                puttableCount--;
                break;
            }
        }

        if (!canFlip)
            return -1;

        return puttableCount;
    }

    private async Task FlipStones(int x, int y, Vector2 direction, ETeam myTeam)
    {
        x += (int)direction.x;
        y += (int)direction.y;

        //if (myTeam == ETeam.BLACK)
        //    myTeam = ETeam.WHITE;
        //else
        //    myTeam = ETeam.BLACK;

        while (true)
        {
            Sound.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/Turn"));
            Stones[x, y].SetTeam(myTeam, this, x, y);
            var flipCoroutine = new ExEnumerator(Stones[x, y].OnFlip());
            StartCoroutine(flipCoroutine);

            while (!flipCoroutine.IsEnd)
            {
                await Task.Delay(1);
            }

            x += (int)direction.x;
            y += (int)direction.y;

            if (Stones[x, y].Team == myTeam)
            {
                return;
            }
        }
    }

    public IStone SelectStone(EStone selectedStone)
    {
        IStone stone;

        Debug.Log(selectedStone.ToString());

        switch (selectedStone)
        {
            case EStone.DEFAULT:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<Stone>();
                break;

            case EStone.SUN:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<SunStone>();
                break;

            case EStone.CROSS:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrossStone>();
                break;

            case EStone.X:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<XStone>();
                break;

            case EStone.CIRCLE:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                break;

            
            case EStone.ARROW:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ArrowStone>();
                break;

            case EStone.ARROW_U:
            case EStone.ARROW_D:
            case EStone.ARROW_R:
            case EStone.ARROW_L:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ArrowStone>();
                (stone as ArrowStone).SetStoneKind(selectedStone);
                break;

            case EStone.SHIELD:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<ShieldStone>();
                break;

            case EStone.CRYSTAL:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CrystalStone>();
                break;

            default:
                stone =
                    GameObject.Instantiate(basicStone, new Vector3(0, -10, 0), Quaternion.identity)
                    .AddComponent<CircleStone>();
                Debug.LogError("EStone None");
                break;
        }

        Debug.Log(stone);

        if(stone.GetStone() > EStone.DEFAULT)
            stone.GameObjectRef.transform.Find("Plane").localPosition = new Vector3(0, 0.086f, 0);

        return stone;
    }

    public Sprite GetSprite(EStone stone)
    {
        switch (stone)
        {
            case EStone.SUN:
                return Resources.Load<Sprite>("Pictures/Sun");

            case EStone.CROSS:
                return Resources.Load<Sprite>("Pictures/Cross");

            case EStone.X:
                return Resources.Load<Sprite>("Pictures/X");

            case EStone.CIRCLE:
                return Resources.Load<Sprite>("Pictures/Circle");

            case EStone.ARROW_U:
            case EStone.ARROW_D:
            case EStone.ARROW_R:
            case EStone.ARROW_L:
            case EStone.ARROW:
                return Resources.Load<Sprite>("Pictures/Arrow");

            case EStone.SHIELD:
                return Resources.Load<Sprite>("Pictures/Shield");

            case EStone.CRYSTAL:
                return Resources.Load<Sprite>("Pictures/Crystal");

            default:
                return null;
        }
    }

    public Sprite GetDescription(EStone stone)
    {
        switch (stone)
        {
            case EStone.SUN:
                return Resources.Load<Sprite>("Pictures/Description/Sun");

            case EStone.CROSS:
                return Resources.Load<Sprite>("Pictures/Description/Cross");

            case EStone.X:
                return Resources.Load<Sprite>("Pictures/Description/X");

            case EStone.CIRCLE:
                return Resources.Load<Sprite>("Pictures/Description/Circle");

            case EStone.ARROW_U:
            case EStone.ARROW_D:
            case EStone.ARROW_R:
            case EStone.ARROW_L:
            case EStone.ARROW:
                return Resources.Load<Sprite>("Pictures/Description/Arrow");

            case EStone.SHIELD:
                return Resources.Load<Sprite>("Pictures/Description/Shield");

            case EStone.CRYSTAL:
                return Resources.Load<Sprite>("Pictures/Description/Crystal");

            default:
                return null;

        }
    }

    protected virtual IEnumerator ShowCutIn(bool isEnemy, int count)
    {
        Debug.Log("CutIn");
        var t = GameObject.Find("Canvas").transform;
        GameObject c;
        pitchTunableAudio.pitch = 1 + (count * 0.15f);
        pitchTunableAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Game/Cutin"));
        if (isEnemy)
        {
            var o = Resources.Load<GameObject>("Pictures/Game/EnemySkillCut");
            c = Instantiate(o);
            c.transform.SetParent(t, false);

        }
        else
        {
            var o = Resources.Load<GameObject>("Pictures/Game/OwnSkillCut");
            c = Instantiate(o);
            c.transform.SetParent(t, false);
        }
        c.transform.SetSiblingIndex(c.transform.GetSiblingIndex()-1);

        yield return new WaitForSeconds(1.7f);
        Destroy(c);
        Debug.Log("CutIn-Out");
        yield break;
    }
}
