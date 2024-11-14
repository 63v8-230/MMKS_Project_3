using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossStone : SkillStoneBase
{
    protected override int SetCost()
    {
        stoneKind = EStone.CROSS;
        return 10;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Cross Skill Start!");
        var col = new ExEnumerator(ShowCutIn(baseTeam == ETeam.WHITE));
        StartCoroutine(col);
        yield return new WaitWhile(() => { return !col.IsEnd; });

        SetHighLight(stoneManager, position, GetColor() * 2);
        for (int i = 0; i < stoneManager.directions.Length; i += 2) 
        {
            var dir = stoneManager.directions[i];
            var flipPosition = position + dir;
            while(true)
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
        return (Texture)Resources.Load("Pictures/Cross");
    }

    public override EStone GetStone()
    {
        return EStone.CROSS;
    }

    protected override Color GetColor()
    {
        return new Color(0, 0.439f, 0.753f);
    }
}
