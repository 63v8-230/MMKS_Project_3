using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

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
    public ETeam Team { get; }

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
