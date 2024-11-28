using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FireFrameUIController : MonoBehaviour
{
    private RawImage img;
    private VideoPlayer ply;

    float colorDelta = 0;

    int kind = 0;
    int count = 0;

    Sprite[] Sprites;
    Image comboImage;
    int currentIndex = 0;

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
        return kind;
    }


}
