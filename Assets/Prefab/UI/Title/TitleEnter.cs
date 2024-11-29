using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class TitleEnter : MonoBehaviour
{
    private const float FADE_TIME = 3;
    private float fade = 0;

    private float time = 0;
    private bool isPlay = false;
    private bool isInFade = false;

    private VideoPlayer player;
    private RawImage img;

    AudioSource bgmAudio;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.Find("Demo").GetComponent<VideoPlayer>();
        img = player.gameObject.GetComponent<RawImage>();
        bgmAudio = GameObject.Find("Scripts").GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if(isPlay)
        {
            if (isInFade)
            {
                fade += Time.deltaTime;
                img.color = new Color(1, 1, 1, 1 - (fade / FADE_TIME));

                if (fade >= FADE_TIME)
                {
                    isInFade = false;
                    isPlay = false;
                    player.Stop();
                    player.transform.SetAsFirstSibling();
                }
                return;
            }

            time += Time.deltaTime;
            if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) || Input.anyKeyDown || time >= player.length - FADE_TIME) 
            {
                fade = 0;
                isInFade = true;
                player.SetDirectAudioMute(0, true);
                bgmAudio.Play();
            }
        }
        else
        {
            if(isInFade)
            {
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.anyKeyDown)
                {
                    player.SetDirectAudioMute(0, true);
                    bgmAudio.Play();
                    isPlay = true;
                }
                    

                fade += Time.deltaTime;
                img.color = new Color( 1, 1, 1, fade / FADE_TIME);

                if (fade >= FADE_TIME)
                {
                    isInFade = false;
                    isPlay = true;
                }
                return;
            }

            time += Time.deltaTime;
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || Input.anyKeyDown)
                time = 0;

            if (time > 30)
            {
                player.gameObject.transform.SetAsLastSibling();
                time = 0;
                player.time = 0;
                fade = 0;
                player.Play();
                bgmAudio.Stop();
                player.SetDirectAudioMute(0, false);
                isInFade = true;
            }
                
        }
    }
}
