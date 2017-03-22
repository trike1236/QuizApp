using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuizUIManager : MonoBehaviour
{


    GameObject quizTextPanel;

    //ボタンのゲームオブジェクトじゃなくて下のtextのゲームオブジェクトを取得してた
    GameObject Button1;
    GameObject Button2;
    GameObject Button3;
    GameObject Button4;
    //いいんだけど分かりにくすぎてバグのもとになる　書いたときの自分死ね

    GameObject Timer;
    Slider timerSlider;
    Slider rivalTimerSlider;

    MultiQuizManager multiQuizManager;
    QuizSceneManager quizSceneManager;
    CardUIManager cardUIManager;

    public GameObject TestButton;
    GameObject BackButton;

    static GameObject DebugWindow;

    public bool hasAnswered = false;
    public bool otherAnswered = false;

    public bool isAnswerCorrect = false;
    public bool isRivalAnswerCorrect = false;

    //public int rivalCard = 0;


    public float timermaxValue;


    public float HP = 100f;
    public float currentHP;
    public Image HPGage;
    Text myHPtext;
    Text damageMine;
    Vector3 myDamagePos;

    public float rivalHP = 100f;
    public float currentRivalHP;
    public Image rivalHPGage;
    Text rivalHPtext;
    Text damageRival;
    Vector3 rivalDamagePos;
    public Color damageColor;

    public AnimationCurve _custumEasing;

    void Awake()
    {
        BackButton = GameObject.Find("Canvas/Back");
        BackButton.SetActive(false);

        cardUIManager = gameObject.GetComponent<CardUIManager>();
        quizSceneManager = gameObject.GetComponent<QuizSceneManager>();
        
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

        HPGage = GameObject.Find("Canvas/MyHP").GetComponent<Image>();
        rivalHPGage = GameObject.Find("Canvas/RivalHP").GetComponent<Image>();

        myHPtext = HPGage.transform.FindChild("HPtext").GetComponent<Text>();
        rivalHPtext = rivalHPGage.transform.FindChild("HPtext").GetComponent<Text>();
        damageMine = myHPtext.transform.FindChild("Text").GetComponent<Text>();
        damageRival = rivalHPtext.transform.FindChild("Text").GetComponent<Text>();
        myDamagePos = damageMine.transform.localPosition;
        rivalDamagePos = damageRival.transform.localPosition;


        ShowHPGageAndText();

        currentHP = HP;
        currentRivalHP = rivalHP;

        cardUIManager.SetCardStates(true);
        cardUIManager.SetTextStates(true);
        

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
        
        //最初のみ実行　デフォルトでカード2が選択されているようにする
        if (cardUIManager.cardSelectState == CardSelectState.None) cardUIManager.ChangeCardSelectState(CardSelectState.Card2);
        cardUIManager.canSelectCard = true;


        //相手のカードにステをセット
        cardUIManager.SetTextStates(false);

        ToEmptyBar(false);
        //Debug.Log(tmpquizes[i].text);
    }

    //答えの番号を渡すと答えのstringが返ってくるメソッド　バカっぽい
    public string RtnAnswerText(int answerNum)
    {
        string textFromButton;
        switch (answerNum)
        { 
            case 0:
                textFromButton = Button1.GetComponent<Text>().text;
                break;
            case 1:
                textFromButton = Button2.GetComponent<Text>().text;
                break;
            case 2:
                textFromButton = Button3.GetComponent<Text>().text;
                break;
            case 3:
                textFromButton = Button4.GetComponent<Text>().text;
                break;
            default:
                textFromButton = "error";
                break;
        }
        return textFromButton;
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
            multiQuizManager.SendAllAnswerTime(maxTime - timeCount,quizSceneManager.usersAnswerNum,cardUIManager.cardSelectState);
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

    //ゲージを0にするメソッド
    public void ToEmptyBar(bool mybar)
    {
        float currentBar;
        if (mybar)
        {
            currentBar = timerSlider.value;
            DOVirtual.Float(currentBar, 0f, 0.5f, value =>
               {
                   timerSlider.value = value;
               });
        }else
        {
            currentBar = rivalTimerSlider.value;
            DOVirtual.Float(currentBar, 0f, 0.5f, value =>
            {
                rivalTimerSlider.value = value;
            });
        }
    }

    public void ShowHPGageAndText()
    {
        var sequence = DOTween.Sequence();
        sequence.Join(DOVirtual.Float(0f, HP, 2.0f, value =>
           {
               myHPtext.text = Mathf.Round(value).ToString() + "/100";
           }).SetEase(_custumEasing));
        sequence.Join(DOVirtual.Float(0f, rivalHP, 2.0f, value =>
        {
            rivalHPtext.text = Mathf.Round(value).ToString() + "/100";
        }).SetEase(_custumEasing));
        sequence.Join(DOVirtual.Float(0f, HP, 2.0f, value =>
        {
            HPGage.fillAmount = value/HP;
        }).SetEase(_custumEasing));
        sequence.Join(DOVirtual.Float(0f, rivalHP, 2.0f, value =>
        {
            rivalHPGage.fillAmount = value/rivalHP;
        }).SetEase(_custumEasing));
    }

    //ゲージをmaxにしてtime分減らす動きをするメソッド
    //相手のバーのみなのでバーの指定はしない
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

    public void DamageHPGage(float damage,bool isMine)
    {
        var sequence = DOTween.Sequence();
        if (isMine)
        {
            damageMine.text = "-" + Mathf.Round(damage).ToString();
            damageMine.color = damageColor;
            damageMine.transform.localPosition = myDamagePos;

            if (damage > currentHP) damage = currentHP;
            sequence.Join(DOVirtual.Float(currentHP / HP, (currentHP - damage) / HP, 1f, value =>
            {
                HPGage.fillAmount = value;
            }));
            sequence.Join(DOVirtual.Float(currentHP, currentHP - damage, 1f, value =>
            {
                myHPtext.text = Mathf.Round(value).ToString() + "/100";
            }));

            sequence.Join(damageMine.transform.DOLocalMoveY(30f, 2f));
            sequence.Append(damageMine.DOColor(new Color(255f, 255f, 255f, 0f), 1f).OnComplete(()=> damageMine.text = ""));
            currentHP -= damage;
        }else
        {
            damageRival.text = "-" + Mathf.Round(damage).ToString();
            damageRival.color = damageColor;
            damageRival.transform.localPosition = rivalDamagePos;

            if (damage > currentRivalHP) damage = currentRivalHP;
            sequence.Join(DOVirtual.Float(currentRivalHP / rivalHP, (currentRivalHP - damage) / rivalHP, 1f, value =>
            {
                rivalHPGage.fillAmount = value;
            }));
            sequence.Join(DOVirtual.Float(currentRivalHP, currentRivalHP - damage, 1f, value =>
            {
                rivalHPtext.text = Mathf.Round(value).ToString() + "/100";
            }));

            sequence.Join(damageRival.transform.DOLocalMoveY(-30f, 2f));
            sequence.Append(damageRival.DOColor(new Color(255f, 255f, 255f, 0f), 1f).OnComplete(() => damageRival.text = ""));
            currentRivalHP -= damage;

        }
        if (currentHP <= 0)            QuizSceneManager.isLiving = false;
        else if (currentRivalHP <= 0) QuizSceneManager.isRivalLiving = false;
    }
}
