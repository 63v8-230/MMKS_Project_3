using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunStone : SkillStoneBase
{

    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Sun Skill Start!");

        for (int i = 0; i < stoneManager.directions.Length; i += 1)
        {
            var dir = stoneManager.directions[i];
            var flipPosition = position + dir;
            while (true)
            {
                if (stoneManager.CheckOutOfBoard((int)(flipPosition.x), (int)(flipPosition.y)))
                    break;

                stoneManager.FlipStone((int)(flipPosition.x), (int)(flipPosition.y),
                    Team != ETeam.BLACK ? ETeam.BLACK : ETeam.WHITE);

                SetHighLight(stoneManager, flipPosition, new Color(1, 0, 0));

                flipPosition += dir;
            }
        }

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Sun");
    }
}
