using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum EAIKind
{
    SIMPLE, // 1番多く取れる場所に通常石のみを置く
    S, // 特殊石も使う。よわい
    M, // 特殊石も使う。ふつう
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
/// ターン終了後にマネージャーに渡す情報
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
    /// 自分のチーム
    /// </summary>
    public ETeam Team { set; get; }

    public Deck MyDeck { set; get; }

    public OnlineScript OnlineScriptRef { set; get; }

    /// <summary>
    /// 初期化処理とか
    /// </summary>
    public void Init(GameManager gManager);

    /// <summary>
    /// どこに石を置くかを決めて、その情報を返す。
    /// </summary>
    /// <returns></returns>
    public Task<TurnInfo> DoTurn();
}
