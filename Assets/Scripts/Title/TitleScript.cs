using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Data
{
    public readonly static Data Instance = new Data();

    public string TITLE_SCENE_NAME = "Title";

    public int BOARD_X = 8;
    public int BOARD_Y = 8;

    public bool IsOnline = false;
    public string RoomName = "room1";

    public bool IsWhite = false;

    public EAIKind AIKind = EAIKind.SIMPLE;

    public bool IsPadMode = false;

    public Process PadProcess;

    public bool isTutorial = false;
}

public class TitleScript : MonoBehaviour
{
    [SerializeField]
    TMP_InputField x;

    [SerializeField]
    TMP_InputField y;

    [SerializeField]
    Button offlineStart;

    [SerializeField]
    TMP_InputField roomName;

    [SerializeField]
    Button join;

    [SerializeField]
    TMP_Dropdown AiKindDropDown;

    [SerializeField]
    Button padMode;

    // Start is called before the first frame update
    void Start()
    {
        AiKindDropDown.value = (int)Data.Instance.AIKind;

        join.onClick.AddListener(() =>
        {
            Data.Instance.IsOnline = true;
            if(string.IsNullOrEmpty(roomName.text))
            {
                roomName.text = "room1";
            }

            Data.Instance.RoomName = roomName.text;

            Data.Instance.BOARD_X = int.Parse(x.text);
            Data.Instance.BOARD_Y = int.Parse(y.text);

            SceneManager.LoadScene("OnlineGame");
        });

        offlineStart.onClick.AddListener(() =>
        {
            Data.Instance.BOARD_X = int.Parse(x.text);
            Data.Instance.BOARD_Y = int.Parse(y.text);
            Data.Instance.IsOnline = false;

            Data.Instance.AIKind = (EAIKind)AiKindDropDown.value;

            SceneManager.LoadScene("Game");
        });

        padMode.onClick.AddListener(StartPadMode);

        Application.quitting += () => { if (Data.Instance.IsPadMode) Data.Instance.PadProcess.Kill(); };
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void StartPadMode()
    {
        if (Data.Instance.IsPadMode)
            return;

        Data.Instance.PadProcess = new Process();

        string para;

        using (StreamReader sr = new StreamReader(Path.Combine(Application.streamingAssetsPath, "para.txt")))
        {
            para = sr.ReadLine();
        }
        


        // プロセスを起動するときに使用する値のセットを指定
        Data.Instance.PadProcess.StartInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(Application.streamingAssetsPath, "MouseControllerWithGamePad.exe"),
            UseShellExecute = false,
            CreateNoWindow = true,
            ArgumentList = {para},
        };

        Data.Instance.PadProcess.Start();
        Data.Instance.IsPadMode = true;
    }

    private void OnApplicationQuit()
    {
        
    }
}
