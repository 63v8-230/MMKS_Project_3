using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void SetTeam(ETeam team);

    /// <summary>
    /// <para>�΂��Ђ�����Ԃ��ꂽ�Ƃ��ɌĂ΂��B</para>
    /// <para>���ꂪSetTeam()�̌�ɌĂ΂��̂ŁA�A�j���[�V�����Ȃǂ̓s���ő҂��Ăق����ꍇ��Coroutine�łȂ�Ƃ�����B</para>
    /// </summary>
    public IEnumerator OnFlip();
}
