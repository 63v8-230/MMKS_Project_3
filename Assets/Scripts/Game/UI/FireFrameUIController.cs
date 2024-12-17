using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FireFrameUIController : MonoBehaviour
{
    private RawImage img;
    private VideoPlayer ply;

    int kind = 0;
    int count = 0;

    Sprite[] Sprites;
    Image comboImage;
    int currentIndex = 0;

    TextMeshProUGUI tx;

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
        Sprites = Resources.LoadAll<Sprite>("Pictures/Game/UI/number_SS");
        comboImage = transform.Find("Combo_Number_L").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Init()
    {
        tx = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        img = GetComponent<RawImage>();
        img.color = new Color(1, 1, 1);

        ply = GetComponent<VideoPlayer>();

        ply.clip = Resources.Load<VideoClip>("Movie/FireBW");
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
                ply.clip = Resources.Load<VideoClip>("Movie/FireBW");
                break;

            case 1:
                ply.clip = Resources.Load<VideoClip>("Movie/FireBlue");
                break;

            case 2:
                ply.clip = Resources.Load<VideoClip>("Movie/FireGreen");
                break;

            case 3:
                ply.clip = Resources.Load<VideoClip>("Movie/FireRed");
                break;

            case 4:
                ply.clip = Resources.Load<VideoClip>("Movie/FireRainbow");
                break;

            default:
                break;
        }
        
    }

    public void ComboCount()
    {
        currentIndex++;
        if(currentIndex > 1)
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

        var e = DelayMethod(() => { Destroy(transform.Find("ComboEnter").gameObject); }, 4.5f);
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
