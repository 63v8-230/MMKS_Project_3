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

    int kind = -1;
    

    //(1,2,3) = (x, y, z); (-/+) = up/down value
    int index = 1;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var c = img.color;
        float v = colorDelta*Time.deltaTime;
        if (index < 0)
            v *= -1;

        switch(Mathf.Abs(index))
        {
            case 1:
                c.r += v;
                if (c.r >= 1)
                    index = -2;
                break;

            case 2:
                c.g += v;
                if (c.g >= 1)
                    index = -3;
                break;

            case 3:
                c.b += v;
                if (c.b >= 1)
                    index = -1;
                break;
        }
    }

    public void Init()
    {
        img = GetComponent<RawImage>();
        img.color = new Color(1, 1, 1);

        ply = GetComponent<VideoPlayer>();
    }


    /// <summary>
    /// (0, 1, 2, 3)
    /// </summary>
    /// <param name="_index"></param>
    public void SetColor()
    {
        kind++;
        Debug.Log("Fire Kind : "+kind);

        switch(kind)
        {
            case 0:
                ply.clip = Resources.Load<VideoClip>("Movie/FireBlue");
                break;

            case 1:
                ply.clip = Resources.Load<VideoClip>("Movie/FireGreen");
                break;

            case 2:
                ply.clip = Resources.Load<VideoClip>("Movie/FireRed");
                break;

            case 3:
                colorDelta = 1;
                index = -1;
                break;
        }
        
    }
}
