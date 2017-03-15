﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuizUIManager : MonoBehaviour
{


    GameObject quizTextPanel;
    GameObject Button1;
    GameObject Button2;
    GameObject Button3;
    GameObject Button4;

    GameObject Timer;
    Slider timerSlider;
    Slider rivalTimerSlider;

    MultiQuizManager multiQuizManager;

    public GameObject TestButton;
    GameObject BackButton;

    static GameObject DebugWindow;

    public bool hasAnswered = false;
    public bool otherAnswered = false;

    public float timermaxValue;

    void Awake()
    {
        BackButton = GameObject.Find("Canvas/Back");
        BackButton.SetActive(false);
    }
    void Start()
    {
        multiQuizManager = GameObject.Find("MultiQuizManager").GetComponent<MultiQuizManager>();

        //debug
        TestButton = GameObject.Find("Canvas/Button");
        TestButton.SetActive(false);
        DebugWindow = GameObject.Find("Canvas/DebugWindow/DebugText");
    }
    public void GetUIPanel(float maxTime)
    {
        timermaxValue = maxTime;
        quizTextPanel = GameObject.Find("Canvas/Panel/QuizText/Text");
        Button1 = GameObject.Find("Canvas/Panel/Choices/Button1/Text");
        Button2 = GameObject.Find("Canvas/Panel/Choices/Button2/Text");
        Button3 = GameObject.Find("Canvas/Panel/Choices/Button3/Text");
        Button4 = GameObject.Find("Canvas/Panel/Choices/Button4/Text");
        Timer = GameObject.Find("Canvas/Panel/Timer");
        timerSlider = Timer.GetComponent<Slider>();
        timerSlider.maxValue = maxTime;
        rivalTimerSlider = GameObject.Find("Canvas/Panel/RivalTimer").GetComponent<Slider>();
        rivalTimerSlider.maxValue = maxTime;

        //Debug用

    }

    //各UIに問題と選択肢を登録する
    public void SetQuizOnPanel(List<QuizSceneManager.Quizes> tmpquizes, int i)
    {
        quizTextPanel.GetComponent<Text>().text = tmpquizes[i].text;
        string[] array = tmpquizes[i].answer_txt.Split(',');
        Button1.GetComponent<Text>().text = array[0];
        Button2.GetComponent<Text>().text = array[1];
        Button3.GetComponent<Text>().text = array[2];
        Button4.GetComponent<Text>().text = array[3];
        //Debug.Log(tmpquizes[i].text);
    }


    //ボタンが押されたらhasAnsweredがtrue
    //タイムアップでisTimeOverがtrueに
    public IEnumerator TimerAndButtonCoroutine(float maxTime)
    {
        float timeCount = maxTime;
        bool isTimeOver = false;
        hasAnswered = false;
        otherAnswered = false;
        while ((!isTimeOver) && (!hasAnswered))
        {
            //Debug.Log(timeCount);
            timeCount -= Time.deltaTime;
            timerSlider.value = timeCount;
            if (timeCount < Time.deltaTime)
            {
                isTimeOver = true;
            }
            if (otherAnswered)
            {

                //負けUIアクションなどの更新
                LoseUIAction();
                yield break;
            }

            yield return null;
        }
        //無限ループから抜けた理由(答えたかタイムアップ)で分岐
        //答えたならタイマーはそのままで
        if (hasAnswered == true)
        {
            multiQuizManager.SendAllAnswerTime(maxTime - timeCount);
            yield break;
        }
        timeCount = 0f;
        timerSlider.value = 0f;

    }
    //自分が答えるより早く相手が答えたときにUIをいじるメソッド
    public void LoseUIAction()
    {
        Debug.Log("make");
    }
    //相手のtimerのバーを更新するメソッド

    //ゲームマスターが最初にクイズゲームをスタートするときにボタンを出現させるメソッド
    public void ShowStartButton()
    {
        TestButton.SetActive(true);
    }
    //上のボタンが押されたときにフラグを立てる
    public void MasterSendStart()
    {
        multiQuizManager.hasButtonPushed = true;
    }



    public static void DebugLogWindow(string text)
    {
        DebugWindow.GetComponent<Text>().text = text;
    }

    public void ShowBackButton()
    {
        BackButton.SetActive(true);
    }

    public void ToMaxIncreaseRivalBar(float time)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(
        DOVirtual.Float(0f, timermaxValue, 0.3f, value =>
            {
                rivalTimerSlider.value = value;
            }));
        sequence.Append(
            DOVirtual.Float(timermaxValue, timermaxValue - time, 0.5f, value =>
                 {
                     rivalTimerSlider.value = value;
                 }));
    }
}
