using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// TextFieldControllerの選択肢用
/// </summary>
public struct TextSelectItem
{
    public string Text;
    public int Index;
}

public class TextFieldController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TextObject;

    Transform selectTarget;

    List<Action> actions = new List<Action>();
    int index = 0;
    int prevIndex = 0;

    bool isPressAnyKey = false;

    List<TextSelectItem> selects = new List<TextSelectItem>();

    List<Func<bool>> fWaits = new List<Func<bool>>();
    int fWaitID = -1;

    List<Func<int>> fJumps= new List<Func<int>>();
    int fJumpID = -1;

    // Start is called before the first frame update
    void Start()
    {


        //テスト用
        fWaits.Add(() => { return (Input.GetKeyDown(KeyCode.Space)); });
        fWaits.Add(() => { SceneManager.LoadScene("TextFieldTest"); return true; });

        var s = SFB.StandaloneFileBrowser.OpenFilePanel("テキストを選択", Application.streamingAssetsPath, "txt", false);


        if(s.Length>0)
        {
            Debug.Log(s[0]);
            Init(s[0]);
        }
        else
        {
            Application.Quit();
        }

        


        //Init(Path.Combine(Application.streamingAssetsPath, "TextTest.txt"));
    }

    // Update is called once per frame
    void Update()
    {
        if(index!=prevIndex)
        {
            if (index >= actions.Count)
                return;

            prevIndex = index;
            actions[index]();
        }

        if(isPressAnyKey)
        {
            if(Input.anyKeyDown)
            {
                index++;
                isPressAnyKey = false;
                return;
            }    
        }

        if(fWaitID>=0)
        {
            if (fWaits[fWaitID]())
            {
                index++;
                fWaitID = -1;
            }
        }

        if (fJumpID >= 0)
        {
            index = fJumps[fJumpID]();
            fJumpID = -1;
        }
    }

    public void Init(string filePath)
    {
        TextObject.text = "";
        selectTarget = GameObject.Find("Canvas").transform.Find("Select");

        index = 0;
        actions.Clear();
        using(System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
        {
            while(!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line == null || line.Length==0)
                {
                    actions.Add(() => { index++; });
                    continue;
                }
                if (line[0] == '!')
                {//コマンドの場合
                    var act = GetCommand(line.Split(' '));
                    actions.Add(act);
                }
                else
                {//普通の文字の場合
                    
                    actions.Add(() => { TextObject.text = line; index++; });
                }
            }
        }

        actions[index]();

    }

    private IEnumerator DelayMethod(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
        yield break;
    }

    /// <summary>
    /// コマンドを挿入する
    /// </summary>
    /// <param name="commands">line.Split(' ')をした戻り値</param>
    /// <returns></returns>
    private Action GetCommand(string[] commands)
    {
        switch(commands[0])
        {
            case "!wait":
                int t = int.Parse(commands[1]);
                return () => { var e = DelayMethod(() => { index++; }, t * 0.001f); StartCoroutine(e); };

            case "!key":
                return () => { isPressAnyKey = true; };

            case "!fWait":
                int idw = int.Parse(commands[1]);
                return () => { fWaitID = idw; };

            case "!jump":
                int idx = int.Parse(commands[1]) - 1;
                return () => { index = idx; };

            case "!fJump":
                int idj = int.Parse(commands[1]);
                return () => { fJumpID = idj; };

            case "!select":
                return () =>
                {
                    selects.Clear();
                    selects.Add(new TextSelectItem { Text = commands[1], Index = int.Parse(commands[2]) - 1 });
                    selects.Add(new TextSelectItem { Text = commands[3], Index = int.Parse(commands[4]) - 1 });
                    var obj = Resources.Load<GameObject>("UI/Prefab/TextButton");
                    foreach (var item in selects)
                    {
                        var s = GameObject.Instantiate(obj);
                        s.transform.SetParent(selectTarget, false);
                        var btn = s.transform.Find("B").GetComponent<UnityEngine.UI.Button>();
                        int idx = item.Index;
                        string str = item.Text;
                        s.transform.Find("B/TX").GetComponent<TextMeshProUGUI>().text = str;
                        btn.onClick.AddListener(() => 
                        { 
                            index = idx; 
                            for (int i = selectTarget.childCount - 1; i >= 0; i--) 
                            { 
                                Destroy(selectTarget.GetChild(i).gameObject);
                            }
                        });
                    }
                };


            case "!memo":
            default:
                return () => { index++; };
        }
    }
}
