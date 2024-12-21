using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScriptNew : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> ui = new List<GameObject>();

    private GameObject canvas;

    private GameObject current;

    [HideInInspector]
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Data.Instance.isTutorial = false;

        audioSource = GetComponent<AudioSource>();

        canvas = GameObject.Find("Canvas");

        var title = Instantiate(ui[0]);
        title.transform.SetParent(canvas.transform, false);
        title.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnAddCPUMenu);
        current = title;


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

        Application.quitting += () => { if (Data.Instance.IsPadMode) Data.Instance.PadProcess.Kill(); };
    }

    private IEnumerator DelayChangeScene(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
        yield break;
    }

    private void OnAddCPUMenu()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"));

        Destroy(current);
        var diffSelect = Instantiate(ui[1]);
        diffSelect.transform.SetParent(canvas.transform, false);

        diffSelect.transform.Find("Exit").GetComponent<Button>().onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));

            Destroy(current);
            var title = Instantiate(ui[0]);
            title.transform.SetParent(canvas.transform, false);
            title.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnAddCPUMenu);
            current = title;

            
        });

        diffSelect.transform.Find("Tutorial").GetComponent<Button>().onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
            StartCoroutine(DelayChangeScene("Tutorial"));
        });

        diffSelect.transform.Find("AI_Simple").GetComponent<Button>().onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.S;

            StartCoroutine(DelayChangeScene("Game"));

            
        });

        diffSelect.transform.Find("AI_S").GetComponent<Button>().onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.M;

            StartCoroutine(DelayChangeScene("Game"));
        });

        diffSelect.transform.Find("AI_M").GetComponent<Button>().onClick.AddListener(() =>
        {
            audioSource.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = EAIKind.CLAUDE2;

            StartCoroutine(DelayChangeScene("Game"));
        });

        //diffSelect.transform.Find("Claude1").GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    audio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
        //    Data.Instance.BOARD_X = 8;
        //    Data.Instance.BOARD_Y = 8;
        //    Data.Instance.IsOnline = false;

        //    Data.Instance.AIKind = EAIKind.CLAUDE1;

        //    StartCoroutine(DelayChangeScene("Game"));
        //});

        //diffSelect.transform.Find("Claude2").GetComponent<Button>().onClick.AddListener(() =>
        //{
        //    audio.PlayOneShot(Resources.Load<AudioClip>("Sound/Menu/decision"));
        //    Data.Instance.BOARD_X = 8;
        //    Data.Instance.BOARD_Y = 8;
        //    Data.Instance.IsOnline = false;

        //    Data.Instance.AIKind = EAIKind.CLAUDE2;

        //    StartCoroutine(DelayChangeScene("Game"));
        //});
    }
}
