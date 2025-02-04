using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    AudioSource audioSource, seAudio;
    GameObject credit;

    bool isCredit = false;

    // Start is called before the first frame update
    void Start()
    {
        var gm = GameObject.Find("GameManager");
        if(gm == null)
        {
            gm = GameObject.Find("Scripts");
        }

        var coms = gm.GetComponents<AudioSource>();
        audioSource = coms[0];
        seAudio = coms[1];

        credit = transform.Find("CreditText").gameObject;

        var s = transform.Find("Master/Slider").GetComponent<Slider>();
        s.value = Data.Instance.MasterVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.MasterVolume = v;
            audioSource.volume = Data.Instance.MasterVolume * Data.Instance.MusicVolume;
            seAudio.volume = Data.Instance.MasterVolume * Data.Instance.SeVolume;
        });

        s = transform.Find("BGM/Slider").GetComponent<Slider>();
        s.value = Data.Instance.MusicVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.MusicVolume = v;
            audioSource.volume = Data.Instance.MasterVolume * Data.Instance.MusicVolume;
        });

        s = transform.Find("SE/Slider").GetComponent<Slider>();
        s.value = Data.Instance.SeVolume;
        s.onValueChanged.AddListener((v) =>
        {
            Data.Instance.SeVolume = v;
            seAudio.volume = Data.Instance.MasterVolume * Data.Instance.SeVolume;
        });

        transform.Find("Credit").GetComponent<Button>().onClick.AddListener(() =>
        {
            isCredit = true;
            credit.SetActive(true);
        });

        transform.Find("Feedback").GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.OpenURL("https://forms.office.com/r/Gg9e1i5s1c");
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
    }
}
