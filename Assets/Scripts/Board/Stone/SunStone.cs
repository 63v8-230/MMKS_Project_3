using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunStone : SkillStoneBase
{
    protected override int SetCost()
    {
        return 20;
    }
    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Sun Skill Start!");
        yield return new WaitForSeconds(0.1f);

        SetHighLight(stoneManager, position, GetColor() * 2);
        for (int i = 0; i < stoneManager.directions.Length; i += 1)
        {
            var dir = stoneManager.directions[i];
            var flipPosition = position + dir;
            while (true)
            {
                if (stoneManager.CheckOutOfBoard((int)(flipPosition.x), (int)(flipPosition.y)))
                    break;

                stoneManager.FlipStone((int)(flipPosition.x), (int)(flipPosition.y),
                    baseTeam);

                SetHighLight(stoneManager, flipPosition, GetColor());

                flipPosition += dir;
            }
        }

        yield return new WaitForSeconds(1);
        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Sun");
    }

    public override EStone GetStone()
    {
        return EStone.SUN;
    }

    protected override Color GetColor()
    {
        return new Color(1, 0, 0);
    }
}
