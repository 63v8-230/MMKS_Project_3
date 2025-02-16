using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;




public class Data
{
    public readonly static Data Instance = new Data();

    public GameObject OptionMenu;

    public string TITLE_SCENE_NAME = "Title";

    public int BOARD_X = 8;
    public int BOARD_Y = 8;

    public bool IsOnline = false;
    public string RoomName = "room1";

    public bool IsWhite = false;

    public EAIKind AIKind = EAIKind.SIMPLE;

    public bool IsPadMode = false;

    public Process PadProcess;

    public bool isTutorial = false;

    public bool InOption = false;

    public float MasterVolume = 1.0f;

    public float MusicVolume = 1.0f;

    public float SeVolume = 1.0f;

    public float CalcSeVolume()
    {
        return MasterVolume * SeVolume;
    }

    public IEnumerator DelayChangeScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    public IEnumerator DelayAction(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
        yield break;
    }

    public string CheatCode = "";

    //====================================================

    public int cUnlockRival = 0;

    public int cChallengeState = -1;

    /// <summary>
    /// ライバルの写真のパス。<br/>
    /// アイコンの場合は後ろに「icon」を付ける。<br/>
    /// カットインは後ろに「cut」
    /// </summary>
    public string[] cRivalIconPath = new string[]
    {
        "Pictures/Game/rivals/1",
        "Pictures/Game/rivals/2",
        "Pictures/Game/rivals/3",
        "Pictures/Game/rivals/4",
        "Pictures/Game/rivals/5",
        "Pictures/Game/rivals/6",
        "Pictures/Game/rivals/7",
        "Pictures/Game/rivals/8",
        "Pictures/Game/rivals/9",
    };

    public string[] cRivalNames = new string[]
    {
        "灼熱の幻影",
        "叡智の守護者",
        "孤高の追跡者",
        "氷霧の戦略家",
        "聖剣の導師",
        "烈火の闘志",
        "神算の迷宮師",
        "白銀の姫",
        "闇夜の王子",
    };

    public string[] cRivalDesc = new string[]
    {
        "あなたと同じコマを使ってきます。\n落ち着いて頑張ってください！",
        "丸コマを多く使ってきます。\nかたまりを作らないことがコツかもしれません！",
        "Xコマを多く使ってきます。\n角を狙ってくるので注意しましょう！",
        "十字コマを多く使ってきます。\n十字や矢印で対抗しましょう！",
        "太陽コマを多く使ってきます！\n一瞬で制圧されるので危険です！",
        "十字コマと矢印コマが多いです。\n数の多さを逆に利用してコンボを繋げましょう！",
        "丸コマと矢印コマが多いです。\n外側からの猛攻に注意しましょう！",
        "十字コマとXコマが多いです。\n効果の軌道を意識して戦いましょう！",
        "全てのコマを２つずつ使ってきます。\nこれが最終戦です。全力で挑みましょう！",
    };

    public EAIKind[] cRivals = new EAIKind[]
    {
        EAIKind.S,//1
        EAIKind.M,//2
        EAIKind.CLAUDE2,//3
        EAIKind.CLAUDE2,//4
        EAIKind.CLAUDE2,//5
        EAIKind.S,//6
        EAIKind.M,//7
        EAIKind.M,//8
        EAIKind.CLAUDE2,//9
    };

    public Deck[] cRivalDecks = new Deck[]
    {
        new Deck()//1
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 1 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//2
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 1 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 3 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//3
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 1 },
                new OwnStone { Stone = EStone.X, Amount = 2 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//4
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 2 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//5
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 2 },
                new OwnStone { Stone = EStone.CROSS, Amount = 1 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//6
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 2 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 2 },
            }
        },

        new Deck()//7
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 1 },
                new OwnStone { Stone = EStone.CROSS, Amount = 1 },
                new OwnStone { Stone = EStone.X, Amount = 1 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 2 },
            }
        },

        new Deck()//8
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 2 },
                new OwnStone { Stone = EStone.CROSS, Amount = 2 },
                new OwnStone { Stone = EStone.X, Amount = 2 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 1 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 1 },
            }
        },

        new Deck()//9
        {
            Stones =new List<OwnStone>
            {
                new OwnStone { Stone = EStone.SUN, Amount = 2 },
                new OwnStone { Stone = EStone.CROSS, Amount = 2 },
                new OwnStone { Stone = EStone.X, Amount = 2 },
                new OwnStone { Stone = EStone.CIRCLE, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_U, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_R, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_D, Amount = 2 },
                new OwnStone { Stone = EStone.ARROW_L, Amount = 2 },
            }
        },
    };

    public Sprite GetSprite(EStone stone)
    {
        switch (stone)
        {
            case EStone.SUN:
                return Resources.Load<Sprite>("Pictures/Sun");

            case EStone.CROSS:
                return Resources.Load<Sprite>("Pictures/Cross");

            case EStone.X:
                return Resources.Load<Sprite>("Pictures/X");

            case EStone.CIRCLE:
                return Resources.Load<Sprite>("Pictures/Circle");

            case EStone.ARROW_U:
            case EStone.ARROW_D:
            case EStone.ARROW_R:
            case EStone.ARROW_L:
            case EStone.ARROW:
                return Resources.Load<Sprite>("Pictures/Arrow");

            case EStone.SHIELD:
                return Resources.Load<Sprite>("Pictures/Shield");

            case EStone.CRYSTAL:
                return Resources.Load<Sprite>("Pictures/Crystal");

            default:
                return null;
        }
    }

}
public class Util : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
