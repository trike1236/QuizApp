﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum GameState
{
    Start,
    Prepare,
    Wait,
    Answer,
    Result
}

public enum MyResultState
{
    Wait,
    OnlyAnswer,
    RivalAnswer,
    IsFast,
    IsSlow
}

public class QuizSceneManager : MonoBehaviour {
    //public static QuizSceneManager Instance;

    private GameState currentGameState;

    private MyResultState myResultState;
    //クイズの個数　どこかで入力させる
    int quizCount = 10;

    GameObject QuizManager;
    QuizUIManager quizUIManager;
    CardUIManager cardUIManager;
    CardAnimManager cardAnimManager;
    QuizGetter quizGetter;
    MultiQuizManager multiQuizManager;
    GameObject userManager;
    AnswerUIManager answerUIManager;

    
    public int usersAnswerNum;

    float maxTime;
    
    int quizTurn;

    public int rivalAnswerNum;
    public bool isRivalCorrect;
    public CardSelectState rivalCard;

    static public bool isLiving = true;
    static public bool isRivalLiving = true;

    List<Quizes> quizes = new List<Quizes>();
    public class Quizes
    {
        public int type;
        public int quiz_id;
        public string text;
        public string answer_txt;
        public int answer_num;
    }

    List<bool> quizResults = new List<bool>();

    void Awake()
    {
        //Instance = this;
        QuizManager = GameObject.Find("QuizManager");
        quizUIManager = gameObject.GetComponent<QuizUIManager>();
        cardUIManager = gameObject.GetComponent<CardUIManager>();
        cardAnimManager = gameObject.GetComponent<CardAnimManager>();
        quizGetter = QuizManager.GetComponent<QuizGetter>();
        multiQuizManager = GameObject.Find("MultiQuizManager").GetComponent<MultiQuizManager>();
        answerUIManager = gameObject.GetComponent<AnswerUIManager>();

        userManager = UserManager.Instance.gameObject;
    }

    void Start()
    {
        SetCurrentState(GameState.Start);
    }

    public void SetCurrentState(GameState state)
    {
        currentGameState = state;
        OnGameStateChanged(currentGameState);
    }

    public void SetResultState(MyResultState state)
    {
        myResultState = state;
    }

    void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Start:
                StartCoroutine(StartAction());
                break;
            case GameState.Prepare:
                StartCoroutine(PrepareAction());
                break;
            case GameState.Wait:
                StartCoroutine(WaitAction());
                break;
            case GameState.Answer:
                StartCoroutine(AnswerAction());
                break;
            case GameState.Result:
                StartCoroutine(ResultAction());
                break;

        }
    }
    
	//startは呼び出し1回のみ　終わったらprepare
	IEnumerator StartAction() {
        
        //クイズを受信するコルーチンを呼び出す　終わったらリストをコピーする
        yield return StartCoroutine(multiQuizManager.WaitIntoRoom());
        yield return StartCoroutine(quizGetter.RequestQuizes(SaveReceivedQuizes,quizCount));
        quizTurn = 0;      //ここ汚い
        //とりあえず時間のマックスは10秒に設定
        maxTime = 10f;
        isLiving = true;
        isRivalLiving = true;
        quizUIManager.GetUIPanel(maxTime);

        //自分のステを相手に送信
        multiQuizManager.RPCSendMyCardStates();
        SetCurrentState(GameState.Prepare);
	}
	
    //prepare：だいたいここの状態を通る
    //クイズをUIに
    IEnumerator PrepareAction()
    {
        Debug.Log("Turn : " + (quizTurn + 1));
        multiQuizManager.InitTime();
        myResultState = MyResultState.Wait;

        //マスターがゲームスタートの合図を送る
        //初回のみボタンクリックでスタート、2回目以降はタイミングのみ合わせる
        yield return StartCoroutine(multiQuizManager.StartFromMaster(quizTurn));

        yield return new WaitForSeconds(1);quizUIManager.SetQuizOnPanel(quizes, quizTurn);
        quizTurn = quizTurn + 1;
        SetCurrentState(GameState.Wait);
    }

    //wait：ユーザーのボタン入力を待つ
    IEnumerator WaitAction()
    {
        //ユーザーの入力した答えの番号を0(=未入力)にして入力待ちの状態にする
        usersAnswerNum = 0;
        maxTime = 10f;
        yield return StartCoroutine(quizUIManager.TimerAndButtonCoroutine(maxTime));
        //早い遅い判定の受付待ち
        yield return StartCoroutine(WaitSetMyState());

        SetCurrentState(GameState.Answer);
    }


    //分岐　長すぎ
    IEnumerator AnswerAction()
    {
        //相手のカードを出す
        cardUIManager.RivalCardShow(rivalCard);
        Debug.Log(cardUIManager.myCardAttack[1]);
        cardAnimManager.SetCard(cardUIManager.cardSelectState, rivalCard, 50, 20);
        switch (myResultState)
        {
            case MyResultState.IsFast:

                quizUIManager.ToMaxIncreaseRivalBar(multiQuizManager.otherTime);
                if (IsAnswerCorrect(usersAnswerNum))
                {
                    QuizUIManager.DebugLogWindow("right!");
                    //答えたときのフキダシとそれが正解かのアニメーションをする
                    answerUIManager.ShowUserAnswer(usersAnswerNum, true, true);
                    quizResults.Add(true);
                    yield return StartCoroutine(cardAnimManager.FastCorrectCoroutine());
                    break;
                }
                else
                {
                    QuizUIManager.DebugLogWindow("Wrong!");
                    answerUIManager.ShowUserAnswer(usersAnswerNum, true, false);
                    quizResults.Add(false);
                    yield return StartCoroutine(cardAnimManager.FastWrongCoroutine());
                    break;
                }

            case MyResultState.OnlyAnswer:
                {
                    quizUIManager.ToEmptyBar(false);

                    if (IsAnswerCorrect(usersAnswerNum))
                    {
                        QuizUIManager.DebugLogWindow("right!");
                        answerUIManager.ShowUserAnswer(usersAnswerNum, true, true);
                        quizResults.Add(true);
                        yield return StartCoroutine(cardAnimManager.FastCorrectCoroutine());
                        break;
                    }
                    else
                    {
                        QuizUIManager.DebugLogWindow("Wrong!");
                        answerUIManager.ShowUserAnswer(usersAnswerNum, true, false);
                        quizResults.Add(false);
                        yield return StartCoroutine(cardAnimManager.FastWrongCoroutine());
                        break;
                    }
                }
            case MyResultState.IsSlow:

                quizUIManager.ToMaxIncreaseRivalBar(multiQuizManager.otherTime);
                if (isRivalCorrect)
                {

                    QuizUIManager.DebugLogWindow("Too Late");
                    answerUIManager.ShowUserAnswer(rivalAnswerNum, false, true);
                    quizResults.Add(false);
                    yield return StartCoroutine(cardAnimManager.SlowCorrectCoroutine());
                    break;
                }else
                {
                    QuizUIManager.DebugLogWindow("Too Late");
                    answerUIManager.ShowUserAnswer(rivalAnswerNum, false, false);
                    quizResults.Add(false);
                    yield return StartCoroutine(cardAnimManager.SlowWrongCoroutine());
                    break;
                }
            case MyResultState.RivalAnswer:

                quizUIManager.ToMaxIncreaseRivalBar(multiQuizManager.otherTime);
                quizUIManager.ToEmptyBar(true);

                if (isRivalCorrect)
                {
                    QuizUIManager.DebugLogWindow("Too Late");
                    answerUIManager.ShowUserAnswer(rivalAnswerNum, false, true);
                    quizResults.Add(false);
                    yield return StartCoroutine(cardAnimManager.SlowCorrectCoroutine());
                    break;
                }
                else
                {
                    QuizUIManager.DebugLogWindow("Too Late");
                    answerUIManager.ShowUserAnswer(rivalAnswerNum, false, false);
                    quizResults.Add(false);
                    yield return StartCoroutine(cardAnimManager.SlowWrongCoroutine());
                    break;
                }
        }
        

        yield return new WaitForSeconds(3);
        cardUIManager.RivalCardHide();

        //クイズが最後だったら
        if ((quizTurn == quizCount) || !isLiving || !isRivalLiving)
        {
            SetCurrentState(GameState.Result);
        }
        else
        {
            SetCurrentState(GameState.Prepare);
        }

    }

    IEnumerator ResultAction()
    {
        string quizTF = JoinQuizTFData(quizResults);
        string quizId = JoinQuizId(quizes);
        int userId = UserManager.userData.id;
        yield return StartCoroutine(quizGetter.PostResult(quizTF,quizId,userId));

        quizUIManager.ShowBackButton();
    }
    

    //コルーチンの結果をコールバックで受け取ってquizesに保存する
    public void SaveReceivedQuizes(List<QuizGetter.Quizes> receivedQuizes)
    {
        //値のコピー　たぶんもっと簡単に書ける
        foreach(QuizGetter.Quizes quiz in receivedQuizes)
        {
            Quizes tmp = new Quizes();
            tmp.type = quiz.type;
            tmp.quiz_id = quiz.quiz_id;
            tmp.text = quiz.text;
            tmp.answer_txt = quiz.answer_txt;
            tmp.answer_num = quiz.answer_num;
            quizes.Add(tmp);
        }
        Debug.Log(quizes[1].text);
        Debug.Log(quizes[1].answer_txt);
        Debug.Log(quizes[1].answer_num);
    }


    //ボタンの入力を受け付けるメソッド
    //ボタン入力を待つメソッド(コルーチン)はUIManagerへ　ちゃんと設計しろ
    public void ButtonClicked(int answerNum)
    {
        if (currentGameState == GameState.Wait)
        {
            usersAnswerNum = answerNum;
            quizUIManager.hasAnswered = true;
            quizUIManager.isAnswerCorrect = IsAnswerCorrect(answerNum);
        }
    }

    IEnumerator WaitSetMyState()
    {
        while(myResultState == MyResultState.Wait)
        {
            yield return null;
        }
    }
    
    //結果をpostで送るときにクイズのidを結合して送るメソッド
    //ちゃんとlinqつかってほしい
    string JoinQuizId(List<Quizes> list)
    {
        string postQuizId = "";
        int i = 0;
        foreach(Quizes quiz in list)
        {
            Debug.Log(quiz.quiz_id);
            if (postQuizId == "") postQuizId = quiz.quiz_id.ToString();
            else postQuizId += "," + quiz.quiz_id.ToString();
            i++;
            if (i > quizTurn-1) return postQuizId;
        }
        return postQuizId;
    }
    //Linqつかえ
    string JoinQuizTFData(List<bool> list)
    {
        string postQuizTF = "";
        foreach(bool result in list)
        {
            if (result == true)postQuizTF += ",1";
            else postQuizTF += ",0";
        }

        return postQuizTF.Substring(1);
    }

    public bool IsAnswerCorrect(int i)
    {
        if (i == quizes[quizTurn-1].answer_num) return true;
        else return false;
    }

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

}
