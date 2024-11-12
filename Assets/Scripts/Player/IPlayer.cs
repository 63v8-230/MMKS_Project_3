using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum EAIKind
{
    SIMPLE, // 1�ԑ�������ꏊ�ɒʏ�΂݂̂�u��
    S, // ����΂��g���B��킢
    M, // ����΂��g���B�ӂ�
}

public struct Deck
{
    public List<OwnStone> Stones;
}

public struct OwnStone
{
    public EStone Stone;
    public int Amount;
}

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
    public ETeam Team { set; get; }

    public Deck MyDeck { set; get; }

    public OnlineScript OnlineScriptRef { set; get; }

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
