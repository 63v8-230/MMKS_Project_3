using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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
public class Util : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
