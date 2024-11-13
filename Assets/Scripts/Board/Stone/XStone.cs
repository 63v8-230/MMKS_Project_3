using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XStone : SkillStoneBase
{
    protected override int SetCost()
    {
        stoneKind = EStone.X;
        return 10;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("X Skill Start!");
        yield return new WaitForSeconds(0.1f);

        SetHighLight(stoneManager, position, GetColor() * 2);
        for (int i = 1; i < stoneManager.directions.Length; i += 2)
        {
            var dir = stoneManager.directions[i];
            var flipPosition = position + dir;
            while (true)
            {
                if (stoneManager.CheckOutOfBoard((int)(flipPosition.x), (int)(flipPosition.y)))
                    break;

                stoneManager.FlipStone((int)(flipPosition.x), (int)(flipPosition.y),
                    baseTeam, true);

                SetHighLight(stoneManager, flipPosition, GetColor());

                flipPosition += dir;
            }
        }

        yield return new WaitForSeconds(1);
        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/X");
    }

    public override EStone GetStone()
    {
        return EStone.X;
    }

    protected override Color GetColor()
    {
        return new Color(1, 0.753f, 0);
    }
}
