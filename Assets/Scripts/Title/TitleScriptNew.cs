using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TitleScriptNew : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> ui = new List<GameObject>();

    [SerializeField]
    private GameObject optionMenu;

    private GameObject canvas;

    private GameObject current;

    private AudioSource audioSource, seAudio;


    private bool isOption;

    // Start is called before the first frame update
    void Start()
    {
        Data.Instance.CheatCode = "baka";

        Data.Instance.isTutorial = false;

        Data.Instance.OptionMenu = optionMenu;

        var coms = GetComponents<AudioSource>();

        audioSource = coms[0];
        seAudio = coms[1];

        canvas = GameObject.Find("Canvas");

        OnAddTitle();

        if (!PlayerPrefs.HasKey("Master"))
            PlayerPrefs.SetFloat("Master", 1.0f);

        if (!PlayerPrefs.HasKey("Music"))
            PlayerPrefs.SetFloat("Music", 0.4f);

        if (!PlayerPrefs.HasKey("Se"))
            PlayerPrefs.SetFloat("Se", 0.8f);

        if (!PlayerPrefs.HasKey("unlock"))
            PlayerPrefs.SetInt("unlock", 0);

        PlayerPrefs.Save();


        Data.Instance.MasterVolume = PlayerPrefs.GetFloat("Master");
        Data.Instance.MusicVolume = PlayerPrefs.GetFloat("Music");
        Data.Instance.SeVolume = PlayerPrefs.GetFloat("Se");

        if(Data.Instance.CheatCode == "")
            Data.Instance.cUnlockRival = PlayerPrefs.GetInt("unlock");

        Data.Instance.CheatCode = "";


        audioSource.volume = Data.Instance.MasterVolume * Data.Instance.MusicVolume;
        seAudio.volume = Data.Instance.MasterVolume * Data.Instance.SeVolume;


        if (Data.Instance.IsPadMode)
            return;

        Data.Instance.PadProcess = new Process();

        string para1, para2;

        using (StreamReader sr = new StreamReader(Path.Combine(Application.streamingAssetsPath, "para.txt")))
        {
            para1 = sr.ReadLine();
            para2 = sr.ReadLine();
        }



        // プロセスを起動するときに使用する値のセットを指定
        Data.Instance.PadProcess.StartInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.streamingAssetsPath, "MouseControllerWithGamePad.exe"),
            UseShellExecute = false,
            CreateNoWindow = true,
            ArgumentList = { para1, para2 },
        };

        Data.Instance.PadProcess.Start();
        Data.Instance.IsPadMode = true;

#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += (s) =>
        {
            if(s == PlayModeStateChange.ExitingPlayMode)
            {
                if (Data.Instance.IsPadMode) Data.Instance.PadProcess.Kill();
            }
        };
#else
    Application.quitting += () => { if (Data.Instance.IsPadMode) Data.Instance.PadProcess.Kill(); };
#endif
    }

    void Update()
    {
    }

    private void OnAddTitle()
    {
        var title = Instantiate(ui[0]);
        title.transform.SetParent(canvas.transform, false);
        title.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnAddModeMenu);
        title.transform.Find("exit").GetComponent<Button>().onClick.AddListener(() => 
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        });
        current = title;

        var settingButton = title.transform.Find("Setting");

        settingButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            isOption = !isOption;

            if (isOption)
            {
                Data.Instance.InOption = true;
                optionMenu = Instantiate(Data.Instance.OptionMenu);
                optionMenu.transform.SetParent(title.transform, false);
                seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
                settingButton.transform.SetAsLastSibling();
            }
            else
            {
                PlayerPrefs.SetFloat("Master", Data.Instance.MasterVolume);
                PlayerPrefs.SetFloat("Music", Data.Instance.MusicVolume);
                PlayerPrefs.SetFloat("Se", Data.Instance.SeVolume);
                PlayerPrefs.Save();

                Data.Instance.InOption = false;
                Destroy(optionMenu);
                seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            }
        });
    }

    private void OnAddModeMenu()
    {
        seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"),Data.Instance.CalcSeVolume());
        Destroy(current);
        var cpu = Instantiate(ui[4]);
        cpu.transform.SetParent(canvas.transform, false);
        cpu.transform.Find("Battle").GetComponent<Button>().onClick.AddListener(OnAddBattleMenu);
        cpu.transform.Find("Tournament").GetComponent<Button>().onClick.AddListener(OnAddChallengeMenu);

        cpu.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnAddTitle);
    }

    private void OnAddChallengeMenu()
    {
        seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"), Data.Instance.CalcSeVolume());
        Destroy(current);
        var cpu = Instantiate(ui[5]);
        cpu.transform.SetParent(canvas.transform, false);

        cpu.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnAddModeMenu);
    }

    private void OnAddBattleMenu()
    {
        Data.Instance.cChallengeState = -1;
        seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"), Data.Instance.CalcSeVolume());
        Destroy(current);
        var cpu = Instantiate(ui[2]);
        cpu.transform.SetParent(canvas.transform, false);
        cpu.transform.Find("CPU").GetComponent<Button>().onClick.AddListener(OnAddCPUMenu);
        cpu.transform.Find("Online").GetComponent<Button>().onClick.AddListener(OnAddOnlineMenu);

        cpu.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnAddModeMenu);
    }

    private void OnAddOnlineMenu()
    {
        seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"), Data.Instance.CalcSeVolume());

        Destroy(current);
        var m = Instantiate(ui[3]);
        m.transform.SetParent(canvas.transform, false);

        m.transform.Find("RoomID").GetComponent<TMP_InputField>().text = $"Room{UnityEngine.Random.Range(0, 9999):0000}";

        m.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnAddBattleMenu);

        m.transform.Find("Start").GetComponent<Button>().onClick.AddListener(() =>
        {
            Data.Instance.IsOnline = true;
            Data.Instance.RoomName = m.transform.Find("RoomID").GetComponent<TMP_InputField>().text;

            if (Data.Instance.RoomName == "unlockall")
            {
                Data.Instance.cUnlockRival = 500000;
                return;
            }

            if(Data.Instance.RoomName == "baka")
            {
                Data.Instance.CheatCode = "baka";

                Data.Instance.BOARD_X = 8;
                Data.Instance.BOARD_Y = 8;
                Data.Instance.IsOnline = false;

                Data.Instance.cChallengeState = -1;

                Data.Instance.AIKind = EAIKind.CLAUDE2;

                StartCoroutine(Data.Instance.DelayChangeScene("Game"));
                return;
            }

            StartCoroutine(Data.Instance.DelayChangeScene("OnlineGame"));
        });
    }

    private void OnAddCPUMenu()
    {
        seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"), Data.Instance.CalcSeVolume());

        Destroy(current);
        var diffSelect = Instantiate(ui[1]);
        diffSelect.transform.SetParent(canvas.transform, false);

        diffSelect.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(OnAddBattleMenu);

        diffSelect.transform.Find("Tutorial").GetComponent<Button>().onClick.AddListener(() =>
        {
            seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            StartCoroutine(Data.Instance.DelayChangeScene("Tutorial"));
        });

        diffSelect.transform.Find("AI_Simple").GetComponent<Button>().onClick.AddListener(() =>
        {
            seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.S;

            StartCoroutine(Data.Instance.DelayChangeScene("Game"));
        });

        diffSelect.transform.Find("AI_S").GetComponent<Button>().onClick.AddListener(() =>
        {
            seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.M;

            StartCoroutine(Data.Instance.DelayChangeScene("Game"));
        });

        diffSelect.transform.Find("AI_M").GetComponent<Button>().onClick.AddListener(() =>
        {
            seAudio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"), Data.Instance.CalcSeVolume());
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.CLAUDE2;

            StartCoroutine(Data.Instance.DelayChangeScene("Game"));
        });

        //diffSelect.transform.Find("Claude1").GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    audio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"),Data.Instance.CalcSeVolume());
        //    Data.Instance.BOARD_X = 8;
        //    Data.Instance.BOARD_Y = 8;
        //    Data.Instance.IsOnline = false;

        //    Data.Instance.AIKind = EAIKind.CLAUDE1;

        //    StartCoroutine(DelayChangeScene("Game"));
        //});

        //diffSelect.transform.Find("Claude2").GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    audio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"),Data.Instance.CalcSeVolume());
        //    Data.Instance.BOARD_X = 8;
        //    Data.Instance.BOARD_Y = 8;
        //    Data.Instance.IsOnline = false;

        //    Data.Instance.AIKind = EAIKind.CLAUDE2;

        //    StartCoroutine(DelayChangeScene("Game"));
        //});
    }
}
