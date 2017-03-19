using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiQuizManager : Photon.PunBehaviour {

    PhotonView _photonView;

    public float myTime;
    public float otherTime;


    bool hasStarted = false;
    public bool hasButtonPushed;

    QuizUIManager quizUIManager;
    CardUIManager cardUIManager;
    QuizSceneManager quizSceneManager;
    QuizGetter quizGetter;
    enum ResultState
    {
        Other,
        Win,
        Lose
    }

    void Awake() {

        quizUIManager = GameObject.Find("GameManager").GetComponent<QuizUIManager>();
        _photonView = PhotonView.Get(this);
        quizSceneManager = GameObject.Find("GameManager").GetComponent<QuizSceneManager>();
        quizGetter = GameObject.Find("QuizManager").GetComponent<QuizGetter>();
        cardUIManager = GameObject.Find("GameManager").GetComponent<CardUIManager>();
    }
    void Start()
    {

        //quizUIManager = GameObject.Find("GameManager").GetComponent<QuizUIManager>();
        //_photonView = PhotonView.Get(this);
        //quizSceneManager = GameObject.Find("GameManager").GetComponent<QuizSceneManager>();
        //cardUIManager = GameObject.Find("GameManager").GetComponent<CardUIManager>();
        //quizGetter = GameObject.Find("QuizManager").GetComponent<QuizGetter>();
    }
    public IEnumerator WaitIntoRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            ExitGames.Client.Photon.Hashtable ht = new ExitGames.Client.Photon.Hashtable();
            ht.Add("RoomID", Random.Range(1, 9999));
            PhotonNetwork.room.SetCustomProperties(ht);
        }
        //while (quizGetter.quizRoomId == 0) yield return new WaitForEndOfFrame();
        while (PhotonNetwork.room.CustomProperties["RoomID"] == null)
        {
            Debug.Log("nulldayo");
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log((int)PhotonNetwork.room.CustomProperties["RoomID"]);
        quizGetter.quizRoomId = (int)PhotonNetwork.room.CustomProperties["RoomID"];
        //UserManager.isRoomMaster = PhotonNetwork.isMasterClient;
        yield return null;
    }

    public IEnumerator StartFromMaster(int quizTurn)
    {
        hasButtonPushed = false;
        Debug.Log("inRoom:" + PhotonNetwork.inRoom);
        
        Debug.Log("isMasterClient:" + PhotonNetwork.isMasterClient);

        //最初のみスタートボタンを出して一斉にスタート
        if (PhotonNetwork.isMasterClient)
        {
            if (quizTurn == 0)
            {
                quizUIManager.ShowStartButton();
                while (!hasButtonPushed) yield return new WaitForEndOfFrame();
                quizUIManager.TestButton.SetActive(false);
            }
            photonView.RPC("StartFlag", PhotonTargets.AllViaServer);
        }

        while (!hasStarted)
        {
            yield return new WaitForEndOfFrame();
        }
        hasStarted = false;
    }

    [PunRPC]
    void StartFlag()
    {
        hasStarted = true;
    }
    

    public void SendAllAnswerTime(float time,bool answer,CardSelectState card) {
        myTime = time;
        Debug.Log(myTime);
        _photonView.RPC("SendTimeRPC", PhotonTargets.Others,time,answer,card/*,isFasterThan()*/);

    }

    [PunRPC]
    void SendTimeRPC(float time,bool answer,CardSelectState card/*,UnityAction callback*/)
    {
        otherTime = time;
        quizSceneManager.rivalCard = card;
        quizSceneManager.isRivalCorrect = answer;
        //いい感じに分岐
        if(myTime == 0)
        {
            //相手から呼ばれたときに自分がまだ答えていない(mytimeが0)なら
            _photonView.RPC("ReturnResult", PhotonTargets.Others,MyResultState.OnlyAnswer,cardUIManager.cardSelectState);
            quizSceneManager.SetResultState(MyResultState.RivalAnswer);
            //自分のコルーチンは止まってゲージが止まる
            quizUIManager.otherAnswered = true;
            //負けUIアクションなどの更新
            //quizUIManager.LoseUIAction();
        }else if(myTime != 0)
        {
            //相手から呼ばれたときに自分が答えていたら
            //送られてきたタイムと比較して分岐

            //相手のタイマーバーを更新
            
            if (myTime < time)
            {
                //自分の方が遅ければ、相手に早かったって信号を送って自分には遅かった時の処理
                _photonView.RPC("ReturnResult", PhotonTargets.Others, MyResultState.IsFast);
                SendMeResult(MyResultState.IsSlow);
            }
            else if(myTime <= time)
            {//自分の方が早ければ、相手に早かったって信号を送って自分には早かった時の処理
                _photonView.RPC("ReturnResult", PhotonTargets.Others, MyResultState.IsSlow);
                SendMeResult(MyResultState.IsFast);

            }
        }
        //callback();
    }

    [PunRPC]
    private void ReturnResult(MyResultState state,CardSelectState card)
    {
        quizSceneManager.rivalCard = card;
        switch (state)
        {
            case MyResultState.OnlyAnswer:
                quizSceneManager.SetResultState(MyResultState.OnlyAnswer);
                break;
            case MyResultState.IsFast:
                quizSceneManager.SetResultState(MyResultState.IsFast);

                break;
            case MyResultState.IsSlow:
                quizSceneManager.SetResultState(MyResultState.IsSlow);

                break;
        }
        
    }

    private void SendMeResult(MyResultState state)
    {
        quizSceneManager.SetResultState(state);
    }

    /*
    public bool isFasterThan()
    {
        if (myTime > otherTime)
        {
            return true;
        }
        else return false;
    }*/

    public void InitTime()
    {
        myTime = 0;
        otherTime = 0;
    }
    
   
    /*
    [PunRPC]
    void SendQuizRoomId(int roomId)
    {
        quizGetter.quizRoomId = roomId;
    }
    */

    //何故か配列が渡せないので強引に　クソすぎ
    public void RPCSendMyCardStates()
    {
        float[] atk = new float[3];
        cardUIManager.myCardAttack.CopyTo(atk, 0);
        _photonView.RPC("SendMyCardStates", PhotonTargets.OthersBuffered,atk[0],atk[1],atk[2],cardUIManager.myCardGuard[0],cardUIManager.myCardGuard[1],cardUIManager.myCardGuard[2]);
        Debug.Log(atk[1]);
    }

    [PunRPC]
    void SendMyCardStates(float atk0, float atk1, float atk2, float grd0, float grd1, float grd2)
    {
        float[] atk = new float[3] { atk0, atk1, atk2 };
        float[] grd = new float[3] { grd0, grd1, grd2 };
        cardUIManager.CopySendedCard(atk, grd);
        foreach (float f in atk)
        {
            Debug.Log(f);
        }
    }

    public void AllLeaveRoom()
    {
        _photonView.RPC("BackToMain", PhotonTargets.All);
    }

    [PunRPC]
    public void BackToMain()
    {
        //PhotonNetwork.LoadLevel("Main");
        PhotonNetwork.LeaveRoom();
    }

}
