using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStone
{
    /// <summary>
    /// 石の色
    /// </summary>
    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get; }

    /// <summary>
    /// 石の色を変更
    /// </summary>
    /// <param name="team"></param>
    public void SetTeam(ETeam team);

    /// <summary>
    /// <para>石がひっくり返されたときに呼ばれる。</para>
    /// <para>これがSetTeam()の後に呼ばれるので、アニメーションなどの都合で待ってほしい場合はCoroutineでなんとかする。</para>
    /// </summary>
    public IEnumerator OnFlip();
}
