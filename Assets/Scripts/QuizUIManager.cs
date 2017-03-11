using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizUIManager : MonoBehaviour {


    GameObject quizTextPanel;
    GameObject Button1;
    GameObject Button2;
    GameObject Button3;
    GameObject Button4;

    GameObject Timer;
    Slider timerSlider;
    Slider otherTimerSlider;

    MultiQuizManager multiQuizManager;

    public bool hasAnswered = false;
    public bool otherAnswered = false;

    void Start()
    {
        multiQuizManager = GameObject.Find("MultiQuizManager").GetComponent<MultiQuizManager>();
    }
    public void GetUIPanel(float maxTime)
    {
        quizTextPanel = GameObject.Find("Canvas/Panel/QuizText/Text");
        Button1 = GameObject.Find("Canvas/Panel/Choices/Button1/Text");
        Button2 = GameObject.Find("Canvas/Panel/Choices/Button2/Text");
        Button3 = GameObject.Find("Canvas/Panel/Choices/Button3/Text");
        Button4 = GameObject.Find("Canvas/Panel/Choices/Button4/Text");
        Timer = GameObject.Find("Canvas/Panel/Timer");
        timerSlider = Timer.GetComponent<Slider>();
        timerSlider.maxValue = maxTime;
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
                LoseUIAction();
                yield break;
            }

            yield return null;
        }
        //無限ループから抜けた理由(答えたかタイムアップ)で分岐
        //答えたならタイマーはそのままで
        if(hasAnswered == true)
        {
            multiQuizManager.SendAllAnswerTime(maxTime-timeCount);
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

}
