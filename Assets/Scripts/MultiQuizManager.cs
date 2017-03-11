using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiQuizManager : Photon.PunBehaviour {

    PhotonView _photonView;

    float myTime;
    float otherTime;

    QuizUIManager quizUIManager;

    void Start() {

        quizUIManager = GameObject.Find("GameManager").GetComponent<QuizUIManager>();
        _photonView = GetComponent<PhotonView>();

    }

    public void SendAllAnswerTime(float time) {
        myTime = time;
        _photonView.RPC("SendTimeRPC", PhotonTargets.Others,time,isFasterThan());

    }

    [PunRPC]
    private void SendTimeRPC(float time,UnityAction callback)
    {
        Debug.Log("test");
        otherTime = time;

        //相手から呼ばれたときに自分がまだ答えていない(mytimeが0)なら
        if(myTime == 0)
        {
            _photonView.RPC("ReturnResult", PhotonTargets.Others,"Other");
            //自分のコルーチンは止まってゲージが止まる
            quizUIManager.otherAnswered = true;
            //負けUIアクションなどの更新
            quizUIManager.LoseUIAction();
        }
        //callback();
    }

    [PunRPC]
    private void ReturnResult(string result)
    {
        //自分のほうが圧倒的に早かったなら　相手から送られてきたのでotherになってるけど早かったのは自分
        if(result == "Other")
        {
            QuizSceneManager.isFast = true;
        }
    }
    public bool isFasterThan()
    {
        if (myTime > otherTime)
        {
            return true;
        }
        else return false;
    }

    public void InitTime()
    {
        myTime = 0;
        otherTime = 0;
    }
}
