using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStone : SkillStoneBase
{

    public override IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        Debug.Log("Arrow Skill Start!");

        //TODO Ç∆ÇËÇ†Ç¶Ç∏è„ï˚å¸ÅBå¸Ç´Ç«Ç§Ç∑ÇÈñ‚ëË
        var dir = stoneManager.directions[0];
        position += dir;
        while (true)
        {
            if (stoneManager.CheckOutOfBoard((int)(position.x), (int)(position.y)))
                break;

            stoneManager.FlipStone((int)(position.x), (int)(position.y),
                Team != ETeam.BLACK ? ETeam.BLACK : ETeam.WHITE);

            SetHighLight(stoneManager, position, new Color(0.847f, 0.431f, 0.8f));

            position += dir;
        }

        yield break;
    }

    protected override Texture GetTexture()
    {
        return (Texture)Resources.Load("Pictures/Arrow");
    }
}
