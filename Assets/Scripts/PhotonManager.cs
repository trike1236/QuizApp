using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.PunBehaviour{


    void Awake()
    {
        //勝手にロビーに入るようにする
        //本番はオフラインモードがある可能性があるのでfalse;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined!");
    }

    public static void CreateOrJoinRoom(int roomNum)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 2;

        bool tf = PhotonNetwork.JoinOrCreateRoom(roomNum.ToString(), roomOptions,null);
        Debug.Log("result:" + tf);
    }
	
    void OnPhotonCreateRoomFailed()
    {
        Debug.Log("failed");
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
