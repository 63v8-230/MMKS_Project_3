using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineScript : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView PhotonViewRef;

    public bool IsLoad = false;

    public Action<int,int,int> OnValueSet;
    public Action<int, int> OnCombo;

    // Start is called before the first frame update
    void Start()
    {
        PhotonViewRef = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void SetTurn(int stoneKind, int x, int y)
    {
        if(OnValueSet != null)
            OnValueSet.Invoke(stoneKind, x, y);
    }

    [PunRPC]
    public void SetCombo(int x,int y)
    {
        if (OnCombo!= null)
            OnCombo.Invoke(x, y);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Transformの値をストリームに書き込んで送信する
            stream.SendNext(IsLoad);
        }
        else
        {
            // 受信したストリームを読み込んでTransformの値を更新する
            IsLoad = (bool)stream.ReceiveNext();
        }
    }
}
