using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalStone : SkillStoneBase
{

    protected override int SetCost()
    {
        stoneKind = EStone.CRYSTAL;
        return 5;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Circle Skill Start!");
        yield return new WaitForSeconds(0.1f);

        Vector2[] dirs =
        {
            new Vector2(-2,-1),
            new Vector2(-2,1),
            new Vector2(-1,-2),
            new Vector2(-1,2),
            new Vector2(2,-1),
            new Vector2(2,1),
            new Vector2(1,-2),
            new Vector2(1,2),
        };

        SetHighLight(stoneManager, position, GetColor() * 2);
        foreach (var dir in dirs)
        {
            if (!stoneManager.CheckOutOfBoard((int)(position.x + dir.x), (int)(position.y + dir.y)))
            {
                stoneManager.FlipStone((int)(position.x + dir.x), (int)(position.y + dir.y),
                    baseTeam);

                SetHighLight(stoneManager, position + dir, GetColor());
            }
                
        }

        yield return new WaitForSeconds(1);

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Crystal");
    }

    public override EStone GetStone()
    {
        return EStone.CRYSTAL;
    }

    protected override Color GetColor()
    {
        return new Color(0.655f, 0.894f, 0.949f);
    }
}

