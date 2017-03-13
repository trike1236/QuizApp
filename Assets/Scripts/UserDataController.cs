using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UserDataController : MonoBehaviour {
	public Text userIdText;
	public Text userNameText;
	public Text createTimeText;
	public Text	totalAnswerText;
	public Text correctAnswerText;

    int userId = 3;

    UserManager userManager;
	// Use this for initialization
	void Start () {
        userManager = GameObject.Find("UserManager").GetComponent<UserManager>();
        Action delegateMethod = UserDataReceived;
        StartCoroutine(userManager.RequestUserData(delegateMethod, userId));
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void UserDataReceived(){
        sendUserId(UserManager.userData.id);
        sendUserName(UserManager.userData.name);
        sendCreateTime(UserManager.userData.add_time);
        sendTotalAnswer(UserManager.userData.count_all);
        sendCorrectAnswer(UserManager.userData.count_correct);
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
