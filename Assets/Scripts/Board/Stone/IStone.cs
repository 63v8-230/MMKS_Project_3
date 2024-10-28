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
    MAX,
}

public interface IStone
{
    /// <summary>
    /// �΂̐F
    /// </summary>
    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get; }

    /// <summary>
    /// �΂̐F��ύX
    /// </summary>
    /// <param name="team"></param>
    public void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1);

    /// <summary>
    /// <para>�΂��Ђ�����Ԃ��ꂽ�Ƃ��ɌĂ΂��B</para>
    /// <para>���ꂪSetTeam()�̌�ɌĂ΂��̂ŁA�A�j���[�V�����Ȃǂ̓s���ő҂��Ăق����ꍇ��Coroutine�łȂ�Ƃ�����B</para>
    /// </summary>
    public IEnumerator OnFlip();

    /// <summary>
    /// ����\�͔������ɌĂ΂��B
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnSKill(StoneManager stoneManager, Vector2 position);
}
