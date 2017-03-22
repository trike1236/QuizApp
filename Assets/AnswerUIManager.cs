using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AnswerUIManager : MonoBehaviour {

    QuizUIManager quizUIManager;

    //自分と相手が答えた時の吹き出しをUIで作って取得
    GameObject MyAnswerPanel;
    GameObject rivalAnswerPanel;

    public float hukidashiDurSpeed = 0.4f;

    private void Awake()
    {
        quizUIManager = gameObject.GetComponent<QuizUIManager>();
        //MyAnswerPanel = GameObject.Find("Canvas");
        //rivalAnswerPanel = GameObject.Find("Canvas//////////");
    }

    public void ShowUserAnswer(int answerNum,bool isMyAnswered,bool isCorrect)
    {
        string ansText = quizUIManager.RtnAnswerText(answerNum);
        GameObject thisAnimObj;
        if (isMyAnswered)
        {
            //自分のフキダシを表示させて正解不正解を表示
            MyAnswerPanel.transform.FindChild("Text").GetComponent<Text>().text = ansText;
            MyAnswerPanel.transform.DOScale(1f,hukidashiDurSpeed);
            thisAnimObj = MyAnswerPanel;
        }else
        {
            rivalAnswerPanel.transform.FindChild("Text").GetComponent<Text>().text = ansText;
            rivalAnswerPanel.transform.DOScale(1f, hukidashiDurSpeed);
            thisAnimObj = rivalAnswerPanel;
        }
        ShowAnswerWrongOrCorrect(isCorrect, thisAnimObj);
    }

    void ShowAnswerWrongOrCorrect(bool isCorrect,GameObject AnimObj)
    {
        var _sequence = DOTween.Sequence();
        if (isCorrect)
        {
            //_sequence.Append(AnimObj.)
        }else
        {


        }
    }
}
