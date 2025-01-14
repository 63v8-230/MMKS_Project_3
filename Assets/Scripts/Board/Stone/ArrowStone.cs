using Photon.Pun;
using System.Collections;
using UnityEngine;

public class ArrowStone : SkillStoneBase
{
    protected override int SetCost()
    {
        return 5;
    }

    public EDirection StoneDirection;

    public void SetStoneKind(EStone eStone)
    {
        stoneKind = eStone;

        switch (stoneKind)
        {
            case EStone.ARROW_U:
                StoneDirection = EDirection.DOWN;
                break;
            case EStone.ARROW_D:
                StoneDirection = EDirection.TOP;
                break;
            case EStone.ARROW_R:
                StoneDirection = EDirection.RIGHT;
                break;
            case EStone.ARROW_L:
                StoneDirection = EDirection.LEFT;
                break;
        }
    }

    public override IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {

        if(baseTeam == ETeam.WHITE)
        {
            switch (stoneKind)
            {
                case EStone.ARROW_U:
                    StoneDirection = EDirection.TOP;
                    break;
                case EStone.ARROW_D:
                    StoneDirection = EDirection.DOWN;
                    break;
                case EStone.ARROW_R:
                    StoneDirection = EDirection.LEFT;
                    break;
                case EStone.ARROW_L:
                    StoneDirection = EDirection.RIGHT;
                    break;
            }
        }

        Debug.Log("Arrow Skill Start!");

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
        var ob = gameObject.transform.Find("Plane");

        switch (stoneKind)
        {
            case EStone.ARROW_U:
                break;
            case EStone.ARROW_D:
                ob.localEulerAngles -= (Vector3.up * 180);
                break;
            case EStone.ARROW_R:
                ob.localEulerAngles += (Vector3.up * 90);
                break;
            case EStone.ARROW_L:
                ob.localEulerAngles -= (Vector3.up * 90);
                break;
        }

        if (Team == ETeam.WHITE)
        {
            
            ob.localEulerAngles -= (Vector3.up * 180);
        }

        if (Data.Instance.IsOnline && !PhotonNetwork.IsMasterClient)
        {
            ob.localEulerAngles -= (Vector3.up * 180);
        }

        return (Texture)Resources.Load("Pictures/Arrow");
        
    }

    public override EStone GetStone()
    {
        return stoneKind;
    }

    protected override Color GetColor()
    {
        return new Color(0.847f, 0.431f, 0.8f);
    }
}
