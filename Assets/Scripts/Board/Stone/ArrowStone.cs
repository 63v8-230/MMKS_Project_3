using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStone : SkillStoneBase
{
    public EDirection StoneDirection = EDirection.TOP;

    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Arrow Skill Start!");
        yield return new WaitForSeconds(0.1f);

        //TODO Ç∆ÇËÇ†Ç¶Ç∏è„ï˚å¸ÅBå¸Ç´Ç«Ç§Ç∑ÇÈñ‚ëË
        var dir = stoneManager.directions[(int)StoneDirection];
        position += dir;
        while (true)
        {
            if (stoneManager.CheckOutOfBoard((int)(position.x), (int)(position.y)))
                break;

            stoneManager.FlipStone((int)(position.x), (int)(position.y),
                baseTeam);

            SetHighLight(stoneManager, position, GetColor());

            position += dir;
        }

        yield return new WaitForSeconds(1);
        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Arrow");
    }

    public override EStone GetStone()
    {
        return EStone.ARROW;
    }

    protected override Color GetColor()
    {
        return new Color(0.847f, 0.431f, 0.8f);
    }
}
