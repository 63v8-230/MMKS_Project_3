using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossStone : SkillStoneBase
{

    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Cross Skill Start!");
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < stoneManager.directions.Length; i += 2) 
        {
            var dir = stoneManager.directions[i];
            var flipPosition = position + dir;
            while(true)
            {
                if (stoneManager.CheckOutOfBoard((int)(flipPosition.x), (int)(flipPosition.y)))
                    break;

                stoneManager.FlipStone((int)(flipPosition.x), (int)(flipPosition.y),
                    baseTeam);

                SetHighLight(stoneManager, flipPosition, new Color(0, 0.439f, 0.753f));

                flipPosition += dir;
            }
        }

        yield return new WaitForSeconds(1);
        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Cross");
    }

    public override EStone GetStone()
    {
        return EStone.CROSS;
    }
}
