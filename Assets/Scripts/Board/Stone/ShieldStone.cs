using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
            Team = team;
        }
        else if(team != baseTeam)
        {
            SetHighLight(stoneManager, new Vector2(x,y), GetColor() * 2);
            return;
        }
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
        return EStone.SHIELD;
    }

    protected override Color GetColor()
    {
        return new Color(0.439f, 0.188f, 0.627f);
    }
}
