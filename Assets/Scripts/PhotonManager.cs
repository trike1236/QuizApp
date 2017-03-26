using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhotonManager : Photon.PunBehaviour{

    UserManager userManager;
    public RoomNumInput roomNumInput;
    void Awake()
    {
        //勝手にロビーに入るようにする
        //本番はオフラインモードがある可能性があるのでfalse;
        PhotonNetwork.autoJoinLobby = true;
        if(PhotonNetwork.connected)
        PhotonNetwork.ConnectUsingSettings("0.1");
    }

    void Start()
    {
        userManager = UserManager.Instance.gameObject.GetComponent<UserManager>();
        if (UserManager.userData == null)
        {
            Action test = SaveUserNameOnPhoton;
            userManager.SaveUserData(UnityEngine.Random.Range(1, 4),test);
        }
    }

    public void SaveUserNameOnPhoton()
    {
        Debug.Log("This user is : " + UserManager.userData.name);
        PhotonNetwork.playerName = UserManager.userData.name;
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
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "roomNum", roomNum } };
        bool tf = PhotonNetwork.JoinOrCreateRoom(roomNum.ToString(), roomOptions,null);
        Debug.Log("result:" + tf);
    }
	
    void OnPhotonCreateRoomFailed()
    {
        Debug.Log("failed");
    }

    void OnPhotonJoinRoomFailed()
    {
        Debug.Log("join failed");
        MessageWindow.ShowMessageWindow("ルームは既に埋まっています",roomNumInput.KeyPadDisabled);

    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
