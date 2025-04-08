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
    public GameObject
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

        comboImage.sprite = Sprites[currentIndex];

        tx.text = "";

        InComboBonus = false;

        bw.SetActive(true);
        b.SetActive(false);
        g.SetActive(false);
        r.SetActive(false);
        rbw.SetActive(false);
    }

    public void spInit()
    {
        tx = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        
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

        bw.SetActive(false);
        b.SetActive(false);
        g.SetActive(false);
        r.SetActive(false);
        rbw.SetActive(false);

        switch (kind)
        {
            case 0:
                bw.SetActive(true);
                break;

            case 1:
                b.SetActive(true);
                break;

            case 2:
                g.SetActive(true);
                break;

            case 3:
                r.SetActive(true);
                break;

            case 4:
                rbw.SetActive(true);
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
        var obj = transform.Find("ComboEnter").gameObject;
        obj.SetActive(true);
        InComboBonus = true;
        obj.GetComponent<Animator>().SetTrigger("Start");

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
