using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	public void SceneQuiz()
    {
        PhotonManager.CreateOrJoinRoom(1234);
        SceneManager.LoadScene("Quiz");
	}

	public void SceneUserData(){
		SceneManager.LoadScene ("UserData");
	}

	public void SceneMain(){
		SceneManager.LoadScene ("Main");
	}

}

