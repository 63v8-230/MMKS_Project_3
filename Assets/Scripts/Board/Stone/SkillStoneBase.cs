using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillStoneBase : MonoBehaviour, IStone
{
    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get => gameObject; }

    [SerializeField]
    private Texture tex;

    protected ETeam baseTeam = ETeam.NONE;

    protected float hideAlpha = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        tex = GetTexture();
        
        var r = gameObject.transform.Find("Plane").GetComponent<Renderer>();
        r.material.mainTexture = tex;
        var c = r.material.color;
        c.a = hideAlpha;
        r.material.color = c;
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


    public void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1)
    {    

        Debug.Log("fTeam: "+team.ToString());
        if (baseTeam == ETeam.NONE)
        {
            baseTeam = Team;
            //Debug.Log(team.ToString());
        }
        //Debug.Log(team.ToString());

        Debug.Log($"baseTeam: {baseTeam}\nteam: {team}\nStoneManager: {stoneManager}");
        if (baseTeam != Team && team == baseTeam && stoneManager != null)
        {
            SkillAction action = new SkillAction();
            action.Action = OnSKill;
            action.Position = new Vector2(x, y);
            stoneManager.AddSkillMethod(action);
            Debug.Log("ŽÀsÏ‚Ý");
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

        Debug.Log($"TeamChanged: {baseTeam}->{team}");

        if (baseTeam != Team)
        {
            var r = gameObject.transform.Find("Plane").GetComponent<Renderer>();
            var c = r.material.color;
            c.a = 1;
            r.material.color = c;
        }
        else
        {
            var r = gameObject.transform.Find("Plane").GetComponent<Renderer>();
            var c = r.material.color;
            c.a = hideAlpha;
            r.material.color = c;
        }
    }

    public virtual IEnumerator OnFlip()
    {
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
                    baseTeam);
        }

        yield break;
    }

    protected virtual Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Circle");
    }
}
