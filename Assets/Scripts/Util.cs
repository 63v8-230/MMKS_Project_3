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
        float baseVolume = MasterVolume * MusicVolume;
        if (baseVolume <= 0)
            baseVolume = 1;
        return (1/(baseVolume))*MasterVolume * SeVolume;
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

    //====================================================

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

    public EAIKind[] cRivals = new EAIKind[]
    {
        EAIKind.M,//1
        EAIKind.S,//2
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
