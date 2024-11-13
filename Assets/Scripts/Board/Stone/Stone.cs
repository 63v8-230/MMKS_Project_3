using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// オセロの石。ベースクラス
/// </summary>
public class Stone : MonoBehaviour , IStone
{

    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get => gameObject; }

    private Animator animCom;
    public Animator AnimatorComponent { get => animCom; }

    public EStone StoneKind { get => EStone.DEFAULT; }


    // Start is called before the first frame update
    void Start()
    {
        animCom = transform.Find("Model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTeam(ETeam team, StoneManager stoneManager = null, int x = -1, int y = -1)
    {
        Team = team;
        Quaternion rot;

        if(animCom == null || true)
        {
            switch (team)
            {
                default:
                case ETeam.BLACK:
                    rot = gameObject.transform.Find("Model").localRotation;
                    rot.eulerAngles = new Vector3(180, 0, 0);
                    gameObject.transform.Find("Model").localRotation = rot;
                    break;

                case ETeam.WHITE:
                    rot = gameObject.transform.Find("Model").localRotation;
                    rot.eulerAngles = new Vector3(0, 0, 0);
                    gameObject.transform.Find("Model").localRotation = rot;
                    break;
            }
            return;
        }

        switch (team)
        {
            default:
            case ETeam.BLACK:
                //rot = gameObject.transform.Find("Model").localRotation;
                //rot.eulerAngles = new Vector3(180, 0, 0);
                //gameObject.transform.Find("Model").localRotation = rot;
                AnimatorComponent.SetTrigger("W2B");
                break;

            case ETeam.WHITE:
                //rot = gameObject.transform.Find("Model").localRotation;
                //rot.eulerAngles = new Vector3(0, 0, 0);
                //gameObject.transform.Find("Model").localRotation = rot;
                AnimatorComponent.SetTrigger("B2W");
                break;
        }
    }

    public IEnumerator OnFlip(bool isSkill = false)
    {
        var c = GameObject.Instantiate(Resources.Load("Models/StoneLight"), gameObject.transform)
            .GetComponent<StoneLight>();

        if (Team == ETeam.BLACK)
            c.InitB();
        else
            c.InitW();

        yield return new WaitForSeconds(0.1f);

        yield break;
    }

    public IEnumerator OnSKill(StoneManager stoneManager, Vector2 position)
    {
        yield break;
    }

    public EStone GetStone()
    {
        return EStone.DEFAULT;
    }
}
