using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static System.Net.WebRequestMethods;

public class FireFrameUIController : MonoBehaviour
{
    private RawImage img;
    public VideoPlayer
        bw, b, g, r, rbw;

    int kind = 0;
    int count = 0;

    Sprite[] Sprites;
    Image comboImage;
    int currentIndex = 0;

    TextMeshProUGUI tx;

    public bool InComboBonus = false;

    int[] comboCount = new int[]
    {
        1,//青になるコンボ回数
        2,//緑になるコンボ回数
        3,//赤になるコンボ回数
        4,//虹になるコンボ回数
    };

    // Start is called before the first frame update
    void Start()
    {
        comboImage = transform.Find("Combo_Number_L").GetComponent<Image>();
        Sprites = Resources.LoadAll<Sprite>("Pictures/Game/UI/number_SS");
        var etr = transform.Find("ComboEnter").gameObject;
        transform.Find("skip").GetComponent<Button>().onClick.AddListener(() => { etr.SetActive(false); InComboBonus = false; });

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        currentIndex = 0;
        count = 0;
        kind = 0;

        img.texture = bw.targetTexture;
        comboImage.sprite = Sprites[currentIndex];

        tx.text = "";

        InComboBonus = false;
    }

    public void spInit()
    {
        tx = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        img = GetComponent<RawImage>();
        img.color = new Color(1, 1, 1);

        var coms = GetComponents<VideoPlayer>();
        {
            coms[0].url = "https://63v8-230.github.io/MMKS/mv/FireBW.mp4";
            coms[0].Prepare();
            bw = coms[0];

            coms[1].url = "https://63v8-230.github.io/MMKS/mv/FireBlue.mp4";
            coms[1].Prepare();
            b = coms[1];

            coms[2].url = "https://63v8-230.github.io/MMKS/mv/FireGreen.mp4";
            coms[2].Prepare();
            g = coms[2];

            coms[3].url = "https://63v8-230.github.io/MMKS/mv/FireRed.mp4";
            coms[3].Prepare();
            r = coms[3];

            coms[4].url = "https://63v8-230.github.io/MMKS/mv/FireRainbow.mp4";
            coms[4].Prepare();
            rbw = coms[4];
        }
    }


    public void SetColor()
    {
        if (kind >= 4)
            return;

        count++;

        if (count <= comboCount[kind])
            return;

        kind++;

        Debug.Log("Fire Kind : "+kind);

        switch(kind)
        {
            case 0:
                img.texture = bw.targetTexture;
                break;

            case 1:
                img.texture = b.targetTexture;
                break;

            case 2:
                img.texture = g.targetTexture;
                break;

            case 3:
                img.texture = r.targetTexture;
                break;

            case 4:
                img.texture = rbw.targetTexture;
                break;

            default:
                break;
        }
        
    }

    public void ComboCount()
    {
        currentIndex++;
        if(currentIndex > 0)
        {
            comboImage.sprite = Sprites[currentIndex];
        }
    }

    /// <summary>
    /// 0, 1, 2, 3, 4, でW, B, G, R, Rainbow
    /// </summary>
    /// <returns></returns>
    public int GetCurrentState()
    {
        switch (kind)
        {
            default:
            case 0:
                return -1;

            case 1:
                return 0;

            case 2:
                return 1;

            case 3:
                return 2;

            case 4:
                return 3;
        }
    }

    public void ShowComboEnter()
    {
        transform.Find("ComboEnter").gameObject.SetActive(true);
        InComboBonus = true;

        var e = DelayMethod(() => { transform.Find("ComboEnter").gameObject.SetActive(false); InComboBonus = false; }, 5);
        StartCoroutine(e);
    }

    public void SetText(string text)
    {
        tx.text = text;
    }

    private IEnumerator DelayMethod(Action act, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        act();
        yield break;
    }
}
