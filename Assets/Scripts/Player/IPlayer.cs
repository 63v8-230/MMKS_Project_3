using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// �^�[���I����Ƀ}�l�[�W���[�ɓn�����
/// </summary>
public struct TurnInfo
{
    public int X;
    public int Y;
    public IStone PutStone;
}

public interface IPlayer
{
    /// <summary>
    /// �����̃`�[��
    /// </summary>
    public ETeam Team { get; }

    /// <summary>
    /// �����������Ƃ�
    /// </summary>
    public void Init(GameManager gManager);

    /// <summary>
    /// �ǂ��ɐ΂�u���������߂āA���̏���Ԃ��B
    /// </summary>
    /// <returns></returns>
    public Task<TurnInfo> DoTurn();
}
