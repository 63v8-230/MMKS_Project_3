using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オセロの石。ベースクラス
/// </summary>
public class Stone : MonoBehaviour , IStone
{

    public ETeam Team { get; set; }

    public GameObject GameObjectRef { get => gameObject; }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTeam(ETeam team)
    {
        Team = team;
        Quaternion rot;
        switch (team)
        {
            default:
            case ETeam.BLACK:
                rot = gameObject.transform.rotation;
                rot.eulerAngles = new Vector3(180, 0, 0);
                gameObject.transform.rotation = rot;
                break;

            case ETeam.WHITE:
                rot = gameObject.transform.rotation;
                rot.eulerAngles = new Vector3(0, 0, 0);
                gameObject.transform.rotation = rot;
                break;
        }
    }

    public IEnumerator OnFlip()
    {
        yield return new WaitForSeconds(0.1f);

        yield break;
    }
}
