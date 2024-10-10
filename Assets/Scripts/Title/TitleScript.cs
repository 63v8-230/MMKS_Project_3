using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Data
{
    public readonly static Data Instance = new Data();

    public int BOARD_X = 8;
    public int BOARD_Y = 8;

    public bool IsOnline = false;
    public string RoomName = "room1";
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

    // Start is called before the first frame update
    void Start()
    {
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

            SceneManager.LoadScene("Game");
        });
    }

    // Update is called once per frame
    void Update()
    {
    }
}
