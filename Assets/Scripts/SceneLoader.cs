using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Photon.MonoBehaviour {

    void Awake()
    {
        PhotonNetwork.automaticallySyncScene = true;
    }
	public void SceneQuiz(int num)
    {
        PhotonManager.CreateOrJoinRoom(num);
        StartCoroutine(WaitIntoRoom());
	}
    IEnumerator WaitIntoRoom()
    {
        while (!PhotonNetwork.inRoom)
        {
            yield return new WaitForEndOfFrame();
        }
        //if(PhotonNetwork.isMasterClient)PhotonNetwork.LoadLevel("Quiz");
    }

    public void MasterLoadQuiz()
    {
        PhotonNetwork.LoadLevel("Quiz");
    }

	public void SceneUserData(){
		SceneManager.LoadScene ("UserData");
	}

	public void SceneMain(){
        SceneManager.LoadScene("Main");
        if (PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
	}
    
    public void OnLeftRoom()
    {
        SceneManager.LoadScene("Main");
    }

    public void DebugJoin()
    {
        PhotonManager.CreateOrJoinRoom(11111);
    }
    
}

