using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class TextFieldController : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI TextObject;

    List<Action> actions = new List<Action>();
    int index = 0;
    int prevIndex = 0;

    bool isPressAnyKey = false;

    List<Func<bool>> fWaits = new List<Func<bool>>();
    int fWaitID = -1;

    // Start is called before the first frame update
    void Start()
    {
        TextObject.text = "";

        fWaits.Add(() => { return (Input.GetKeyDown(KeyCode.Space)); });

        Init(Path.Combine(Application.streamingAssetsPath, "TextTest.txt"));

        actions[index]();
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
    }

    public void Init(string filePath)
    {
        index = 0;
        actions.Clear();
        using(System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
        {
            while(!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line[0] == '!')
                {//ƒRƒ}ƒ“ƒh‚Ìê‡
                    var act = GetCommand(line.Split(' '));
                    actions.Add(act);
                }
                else
                {//•’Ê‚Ì•¶Žš‚Ìê‡
                    
                    actions.Add(() => { TextObject.text = line; index++; });
                }
            }
        }

    }

    private IEnumerator DelayMethod(Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
        yield break;
    }

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
                int id = int.Parse(commands[1]);
                return () => { fWaitID = id; };

            default:
                return () => { index++; };
        }
    }
}
