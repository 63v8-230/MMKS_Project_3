using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    AudioSource audioSource;
    GameObject credit;

    bool isCredit = false;

    // Start is called before the first frame update
    void Start()
    {
        
        audioSource = GameObject.Find("GameManager").GetComponent<AudioSource>();
        var cvs = GameObject.Find("Canvas");
        transform.SetParent(cvs.transform, false);

        credit = transform.Find("CreditText").gameObject;

        var s = transform.Find("Master/Slider").GetComponent<Slider>();
        s.value = Data.Instance.MasterVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.MasterVolume = v;
            SetVolume();
        });

        s = transform.Find("BGM/Slider").GetComponent<Slider>();
        s.value = Data.Instance.MusicVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.MusicVolume = v;
            SetVolume();
        });

        s = transform.Find("SE/Slider").GetComponent<Slider>();
        s.value = Data.Instance.SeVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.SeVolume = v;
        });

        transform.Find("Credit").GetComponent<Button>().onClick.AddListener(() =>
        {
            isCredit = true;
            credit.SetActive(true);
        });

    }

    // Update is called once per frame
    void Update()
    {
        if(isCredit && Input.GetMouseButtonDown(1))
        {
            isCredit = false;
            credit.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if(Data.Instance.MasterVolume * Data.Instance.MusicVolume <= 0)
        {
            audioSource.Stop();
        }
    }

    private void SetVolume()
    {
        var v = Data.Instance.MasterVolume * Data.Instance.MusicVolume;
        if(v<=0)
        {
            v = 1;
            audioSource.Pause();
        }
        else if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        audioSource.volume = v;
    }
}
