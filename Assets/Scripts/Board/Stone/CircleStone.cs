using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleStone : SkillStoneBase
{

    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Circle Skill Start!");
        yield return new WaitForSeconds(0.1f);

        foreach (var dir in stoneManager.directions)
        {
            if (!stoneManager.CheckOutOfBoard((int)(position.x + dir.x), (int)(position.y + dir.y)))
                stoneManager.FlipStone((int)(position.x + dir.x), (int)(position.y + dir.y),
                    baseTeam);

            SetHighLight(stoneManager, position + dir,Å@GetColor());
        }

        yield return new WaitForSeconds(1);

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Circle");
    }

    public override EStone GetStone()
    {
        return EStone.CIRCLE;
    }

    protected override Color GetColor()
    {
        return new Color(0.569f, 0.812f, 0.314f);
    }
}
