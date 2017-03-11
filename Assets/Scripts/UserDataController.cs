using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDataController : MonoBehaviour {
	public Text userIdText;
	public Text userNameText;
	public Text createTimeText;
	public Text	totalAnswerText;
	public Text correctAnswerText;

    int userId = 2;
	// Use this for initialization
	void Start () {
        GameObject userManager = GameObject.Find("UserManager");
        userManager.GetComponent<UserManager>().SaveUserData(userId);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void UserDataReceived(int userId,string userName,string createTime,int totalAnswer,int correctAnswer){
		sendUserId (userId);
        sendUserName(userName);
        sendCreateTime(createTime);
        sendTotalAnswer(totalAnswer);
        sendCorrectAnswer(correctAnswer);
	}

	//自分のIDのUIを更新するメソッド
	public void sendUserId(int userId){
		userIdText.text = "UserID:" + userId;  //textに代入
	}

	//自分の名前のUIを更新するメソッド
	public void sendUserName(string userName){
		userNameText.text = "Name:" + userName;  //textに代入
	}

	//自分のアカウント取得日時のUIを更新するメソッド
	public void sendCreateTime(string createTime){
		createTimeText.text = "Registration Date:" + createTime;  //textに代入
	}

	//回答した問題の総数のUIを更新するメソッド
	public void sendTotalAnswer(int totalAnswer){
		totalAnswerText.text = "Total Answer:" + totalAnswer;  //textに代入
	}

	//正解総数のUIを更新するメソッド
	public void sendCorrectAnswer(int correctAnswer){
		correctAnswerText.text = "Total correct Answer:" + correctAnswer;  //textに代入
	}
}
