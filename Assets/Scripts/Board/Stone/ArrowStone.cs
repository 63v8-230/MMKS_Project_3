using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowStone : SkillStoneBase
{
    protected override int SetCost()
    {
        stoneKind = EStone.ARROW;
        return 5;
    }

    public EDirection StoneDirection = EDirection.TOP;

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        if (baseTeam == ETeam.BLACK)
            StoneDirection = EDirection.DOWN;

        Debug.Log("Arrow Skill Start!");
        var col = new ExEnumerator(ShowCutIn(baseTeam == ETeam.WHITE));
        StartCoroutine(col);
        yield return new WaitWhile(() => { return !col.IsEnd; });

        SetHighLight(stoneManager, position, GetColor() * 2);
        var dir = stoneManager.directions[(int)StoneDirection];
        position += dir;
        while (true)
        {
            if (stoneManager.CheckOutOfBoard((int)(position.x), (int)(position.y)))
                break;

            stoneManager.FlipStone((int)(position.x), (int)(position.y),
                baseTeam,true);

            SetHighLight(stoneManager, position, GetColor());

            position += dir;
        }

        yield return new WaitForSeconds(1);
        yield break;
    }

    protected override Texture GetTexture()
    {
        if (Team == ETeam.WHITE)
        {
            var ob = gameObject.transform.Find("Plane");
            ob.localEulerAngles -= (Vector3.up * 180);
        }

        if (Data.Instance.IsOnline && !PhotonNetwork.IsMasterClient)
        {
            var ob = gameObject.transform.Find("Plane");
            ob.localEulerAngles -= (Vector3.up * 180);
        }

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
