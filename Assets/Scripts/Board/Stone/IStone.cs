using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EStone
{
    NONE,
    DEFAULT,
    SUN,
    CROSS,
    X,
    CIRCLE,
    ARROW,
    ARROW_U,
    ARROW_D,
    ARROW_R,
    ARROW_L,
    SHIELD,
    CRYSTAL,
    MAX,
}

public interface IStone
{
    /// <summary>
    /// 石の色
    /// </summary>
    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get; }

    public Animator AnimatorComponent { get; }

    public EStone StoneKind { get;}

    /// <summary>
    /// 石の色を変更
    /// </summary>
    /// <param name="team"></param>
    public void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1);

    /// <summary>
    /// <para>石がひっくり返されたときに呼ばれる。</para>
    /// <para>これがSetTeam()の後に呼ばれるので、アニメーションなどの都合で待ってほしい場合はCoroutineでなんとかする。</para>
    /// </summary>
    public IEnumerator OnFlip(bool isSkill = false);

    /// <summary>
    /// 特殊能力発動時に呼ばれる。
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnSKill(StoneManager stoneManager, Vector2 position);

    public EStone GetStone();
}
