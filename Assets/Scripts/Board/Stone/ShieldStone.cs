using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldStone : SkillStoneBase
{
    protected override int SetCost()
    {
        return 10;
    }
    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1)
    {

        Debug.Log("fTeam: " + team.ToString());
        if (baseTeam == ETeam.NONE)
        {
            baseTeam = team;
            //Debug.Log(team.ToString());
        }
        else
        {
            return;
        }
        

        Debug.Log($"baseTeam: {baseTeam}\nteam: {team}\nStoneManager: {stoneManager}");
        if (baseTeam == Team && team != baseTeam && stoneManager != null)
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
        SetStoneLogo(baseTeam == team);
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Shield Skill Start!");

        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Shield");
    }

    public override EStone GetStone()
    {
        return EStone.X;
    }

    protected override Color GetColor()
    {
        return new Color(0.439f, 0.188f, 0.627f);
    }
}
