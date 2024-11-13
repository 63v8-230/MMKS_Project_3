using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShieldStone : SkillStoneBase
{
    protected override int SetCost()
    {
        stoneKind = EStone.SHIELD;
        if(isOwnerOnline)
            SetStoneLogo(false);
        return 10;
    }

    public override void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1)
    {

        Debug.Log("fTeam: " + team.ToString());
        if (baseTeam == ETeam.NONE)
        {
            baseTeam = team;
            Team = team;
        }
        else if(team != baseTeam)
        {
            SetHighLight(stoneManager, new Vector2(x,y), GetColor() * 2);

            if(isOwnerOnline)
                StartCoroutine(OnSKill(null,Vector2.zero));

            return;
        }

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

        SetStoneLogo(!isOwnerOnline);
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Shield Skill Start!");

        SetStoneLogo(true);

        yield return new WaitForSeconds(1f);

        SetStoneLogo(false);

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Shield");
    }

    public override EStone GetStone()
    {
        return EStone.SHIELD;
    }

    protected override Color GetColor()
    {
        return new Color(0.439f, 0.188f, 0.627f);
    }
}
