using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RivalElement : MonoBehaviour
{
    public int RivalState = 0;

    [SerializeField]
    private GameObject start;

    // Start is called before the first frame update
    void Start()
    {
        if((RivalState-1)>Data.Instance.cUnlockRival)
        {
            Destroy(this.gameObject);
            return;
        }

        transform.Find("Chara").GetComponent<Image>().sprite =
            Resources.Load<Sprite>(Data.Instance.cRivalIconPath[RivalState-1] + "icon");

        transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "ライバル " + RivalState;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            Data.Instance.BOARD_X = 8;
            Data.Instance.BOARD_Y = 8;
            Data.Instance.IsOnline = false;

            Data.Instance.cChallengeState = RivalState - 1;

            var st = Instantiate(start);
            st.transform.SetParent(GameObject.Find("Canvas").transform, false);

            st.transform.Find("RivalElement/Chara").GetComponent<Image>().sprite = Resources.Load<Sprite>(Data.Instance.cRivalIconPath[RivalState - 1] + "icon");
            st.transform.Find("Desc/Text").GetComponent<TextMeshProUGUI>().text = Data.Instance.cRivalDesc[RivalState - 1];
            st.transform.Find("Start").GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject.Find("Scripts").GetComponents<AudioSource>()[1].PlayOneShot(Resources.Load<AudioClip>("Sound/Title/Title_dicision"), Data.Instance.CalcSeVolume());
                StartCoroutine(Data.Instance.DelayChangeScene("Game"));
            });

            st.transform.Find("RivalName").GetComponent<TextMeshProUGUI>().text = 
                "-- "+Data.Instance.cRivalNames[RivalState - 1]+" --";

            st.transform.Find("exit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(st);
            });

            {
                //石選択UIを取得
                var ct = st.transform.Find("StoneSelect/Scroll View/Viewport/Content").transform;
                var srcObj = Resources.Load("Prefab/UI/StoneContent") as GameObject;

                //通常石を追加
                var ui_Default = Instantiate(srcObj);
                ui_Default.transform.SetParent(ct, false);
                var ui_Default_Button = ui_Default.transform.Find("Vert/Button");
                Destroy(ui_Default_Button.Find("Stone_Logo").gameObject);
                ui_Default.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "";
                ui_Default_Button.transform.Find("Stone_Select").gameObject.SetActive(true);
                ui_Default.transform.Find("Desc").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                ui_Default.transform.Find("Vert/Button/Stone_BG").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Game/UI/StoneSelect_Button_BG_W");

                //デッキにある石を追加
                for (int i = 0; i < Data.Instance.cRivalDecks[RivalState-1].Stones.Count; i++)
                {
                    var ui = Instantiate(srcObj);
                    ui.transform.SetParent(ct, false);
                    var ui_Button = ui.transform.Find("Vert/Button");
                    int _index = i;
                    var lg = ui_Button.Find("Stone_Logo");
                    lg.GetComponent<Image>().sprite = Data.Instance.GetSprite(Data.Instance.cRivalDecks[RivalState - 1].Stones[i].Stone);

                    //もし矢印石なら方向ごとでロゴを回転させる
                    if (Data.Instance.cRivalDecks[RivalState - 1].Stones[i].Stone >= EStone.ARROW && Data.Instance.cRivalDecks[RivalState - 1].Stones[i].Stone <= EStone.ARROW_L)
                    {
                        var r = lg.transform.rotation;
                        switch (Data.Instance.cRivalDecks[RivalState - 1].Stones[i].Stone)
                        {
                            case EStone.ARROW:
                            case EStone.ARROW_U:
                                r.eulerAngles = Vector3.forward * 0;
                                lg.transform.rotation = r;
                                break;

                            case EStone.ARROW_D:
                                r.eulerAngles = Vector3.forward * 180;
                                lg.transform.rotation = r;
                                break;

                            case EStone.ARROW_L:
                                r.eulerAngles = Vector3.forward * 90;
                                lg.transform.rotation = r;
                                break;

                            case EStone.ARROW_R:
                                r.eulerAngles = Vector3.forward * -90;
                                lg.transform.rotation = r;
                                break;
                        }
                    }
                    ui.transform.Find("Vert/Count").GetComponent<TextMeshProUGUI>().text = "x" + Data.Instance.cRivalDecks[RivalState - 1].Stones[i].Amount;
                    Destroy(ui.transform.Find("Desc").gameObject);
                    ui.transform.Find("Vert/Button/Stone_BG").GetComponent<Image>().sprite = Resources.Load<Sprite>("Pictures/Game/UI/StoneSelect_Button_BG_W");
                }
            }


        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
