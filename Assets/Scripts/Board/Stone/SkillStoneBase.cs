using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class SkillStoneBase : MonoBehaviour, IStone
{
    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get => gameObject; }


    private Animator animCom;
    public Animator AnimatorComponent { get => animCom; }

    protected int cost;
    public int Cost { get => cost; }


    protected EStone stoneKind;
    public EStone StoneKind { get => stoneKind; }

    [SerializeField]
    private Texture tex;

    protected ETeam baseTeam = ETeam.NONE;

    protected float hideAlpha = 0.0f;

    protected bool isOwnerOnline = false;

    public bool IsOwnerOnline { private get { return isOwnerOnline; } set { isOwnerOnline = value; } }

    //最後にスキルを実行したターン。同じターンで2回も実行しない
    private int prevSkillTurn = -1;

    private void Update()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        cost = SetCost();

        animCom = GetComponent<Animator>();

        tex = GetTexture();

        var ob = gameObject.transform.Find("Plane");
        var r = ob.GetComponent<Renderer>();
        r.material.mainTexture = tex;
        if(Data.Instance.IsOnline && Data.Instance.IsWhite)
        ob.localEulerAngles += (Vector3.up * 180);

        if(IsOwnerOnline)
            hideAlpha = 0;
        else
            hideAlpha = 0.3f;

        //SetStoneLogo(true);
    }

    protected virtual int SetCost()
    {
        return 1;
    }

    protected void SetHighLight(StoneManager stoneManager, Vector2 position, Color highlightColor)
    {
        var g = GameObject.Instantiate(
        stoneManager.HighlightCellObject,
        stoneManager.CellPosition2Vector3((int)(position.x), (int)(position.y)),
        Quaternion.identity);
        var m = g.transform.Find("Model").gameObject.GetComponent<Renderer>().material;
        m.color = highlightColor;
    }


    public virtual void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1)
    {    

        Debug.Log("fTeam: "+team.ToString());
        if (baseTeam == ETeam.NONE)
        {
            baseTeam = team;
            //Debug.Log(team.ToString());
        }
        //Debug.Log(team.ToString());

        Debug.Log($"baseTeam: {baseTeam}\nteam: {team}\nStoneManager: {stoneManager}");
        if (baseTeam == Team && team != baseTeam && stoneManager != null && prevSkillTurn != stoneManager.CurrentTurn)
        {
            SkillAction action = new SkillAction();
            action.Action = this.OnSKill;
            action.Position = new Vector2(x, y);
            action.Name = GetStone().ToString() + "-" + Random.Range(0, 100);
            action.Team = baseTeam;
            action.Sound = GetSkillSound();
            stoneManager.AddSkillMethod(action);
            prevSkillTurn = stoneManager.CurrentTurn;
        }

        Team = team;

        Quaternion rot;
        switch (team)
        {
            default:
            case ETeam.BLACK:
                rot = gameObject.transform.Find("Model").localRotation;
                rot.eulerAngles = new Vector3(180, 0, 0);
                gameObject.transform.Find("Model").localRotation = rot;
                break;

            case ETeam.WHITE:
                rot = gameObject.transform.Find("Model").localRotation;
                rot.eulerAngles = new Vector3(0, 0, 0);
                gameObject.transform.Find("Model").localRotation = rot;
                break;
        }

        Debug.Log($"TeamChanged: {baseTeam}->{team} | {isOwnerOnline}");
        if(isOwnerOnline)
            SetStoneLogo(baseTeam != team);
        else
            SetStoneLogo(baseTeam == team);


    }

    protected void SetStoneLogo(bool isView)
    {
        Debug.Log($"==SetStoneLogo({isView})|a: {hideAlpha}");
        if (isView)
        {
            var r = gameObject.transform.Find("Plane").GetComponent<Renderer>();
            var c = r.material.color;
            if(isOwnerOnline)
                c.a = 0.3f;
            else
                c.a = 1;
            r.material.EnableKeyword("_EMISSION");
            r.material.color = c;
            if (!isOwnerOnline)
            {
                r.material.SetColor("_EmissionColor", GetColor() * 3f);
            }
                
        }
        else
        {
            var r = gameObject.transform.Find("Plane").GetComponent<Renderer>();
            var c = r.material.color;
            c.a = hideAlpha;
            r.material.EnableKeyword("_EMISSION");
            r.material.color = c;
            r.material.SetColor("_EmissionColor", Color.black);
        }
    }

    public virtual IEnumerator OnFlip(bool isSkill = false)
    {
        var c = GameObject.Instantiate(Resources.Load("Models/StoneLight"), gameObject.transform)
            .GetComponent<StoneLight>();

        if (isSkill)
            c.InitS();
        else if (Team == ETeam.BLACK)
            c.InitB();
        else
            c.InitW();

        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public virtual IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Skill Start!");

        foreach (var dir in stoneManager.directions)
        {
            if (!stoneManager.CheckOutOfBoard((int)(position.x + dir.x), (int)(position.y + dir.y)))
                stoneManager.FlipStone((int)(position.x + dir.x), (int)(position.y + dir.y),
                    baseTeam,true);
        }

        yield break;
    }

    protected virtual Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Circle");
    }

    public virtual EStone GetStone()
    {
        return EStone.NONE;
    }

    protected virtual Color GetColor()
    {
        return Color.white;
    }

    public virtual AudioClip GetSkillSound()
    {
        return Resources.Load<AudioClip>("Sound/Game/Special_straight");
    }
}
