using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomNumInput : Photon.MonoBehaviour {

    Text roomNum;
    Button enterButton;
    Text enterText;

    Color textColor;

    SceneLoader sceneLoader;

    int roomPlayerCount;

    Transform keyPanel;

    GameObject waitStatePanel;
    Button startButton;
    Text roomStateText;
    public Color keyPanelTextColor;

    public string waitRivalText = "相手を待っています";

	// Use this for initialization
	void Awake()
    {
        roomNum = transform.FindChild("RoomNum/Text").gameObject.GetComponent<Text>();
        roomNum.text = "";
        enterButton = transform.FindChild("Panel/Enter").gameObject.GetComponent<Button>();
        enterText = transform.FindChild("Panel/Enter/Text").gameObject.GetComponent<Text>();
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        keyPanel = transform.FindChild("Panel");
        waitStatePanel = transform.FindChild("WaitStatePanel").gameObject;
        startButton = waitStatePanel.transform.FindChild("Button").GetComponent<Button>();
        roomStateText = waitStatePanel.transform.FindChild("Text").GetComponent<Text>();


        textColor = enterText.color;
        ChangeEnterButtonState(false);
        //gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
    }

    public void SetKeyPadActive(bool changeActive)
    {
        gameObject.SetActive(changeActive);
    }

    public void InputKey(int key)
    {
        if(roomNum.text.Length <= 4)
        roomNum.text += key.ToString();
        if (roomNum.text.Length == 5) ChangeEnterButtonState(true);
    }

    public void RemoveLast()
    {

        if (roomNum.text.Length == 5)ChangeEnterButtonState(false);
        if (roomNum.text.Length > 0)
            roomNum.text = roomNum.text.Substring(0, roomNum.text.Length - 1);
    }

    public void ClickEnter()
    {
        sceneLoader.SceneQuiz(int.Parse(roomNum.text));
        StartCoroutine(WaitRivalComming());
    }
    
    IEnumerator WaitRivalComming()
    {
        //キーを入力不可にする
        //ここ重い予感がする
        ColorBlock colors = enterButton.colors;
        colors.fadeDuration = 0.5f;
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f,0.4f);
        Color tColor = new Color(1f, 1f, 1f, 0.2f);
        foreach(Transform key in keyPanel)
        {
            key.gameObject.GetComponent<Button>().colors = colors;
            key.FindChild("Text").GetComponent<Text>().color = tColor;
            key.gameObject.GetComponent<Button>().interactable = false;
        }

        //UIパネルを作って状態を表示させる
        roomStateText.text = waitRivalText;
        yield return null;

        //バックボタンを押したらどうするか
        //ルームから退出しkeypadを小さくする
    }
    void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        roomPlayerCount = PhotonNetwork.room.PlayerCount;
        Debug.Log("player join : " + player.NickName);
        if(roomPlayerCount > 1)
        {
            roomStateText.text = "player : " + player.NickName;
            if (PhotonNetwork.isMasterClient) startButton.gameObject.SetActive(true);
            else roomStateText.text += "\n相手のスタート待ちです";
        }
    }

    void OnJoinedRoom()
    {
        //ルームに入ってマスターじゃないときに相手のプレイヤー名を表示するために呼び出す
        //普通によくないからちゃんと別のメソッドを定義するべき
        if(!PhotonNetwork.isMasterClient)OnPhotonPlayerConnected(PhotonNetwork.otherPlayers[0]);
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        roomPlayerCount = PhotonNetwork.room.PlayerCount;
        roomStateText.text = waitRivalText;

        //2人しか部屋に入れないからこのコールバックが呼ばれた時点でスタートボタン消しちゃう
        startButton.gameObject.SetActive(false);
    }

    public void KeyPadDisabled()
    {
        foreach(Transform key in keyPanel)
        {
            key.gameObject.GetComponent<Button>().interactable = true;
            key.FindChild("Text").GetComponent<Text>().color = keyPanelTextColor;
        }
        roomStateText.text = "";
        startButton.gameObject.SetActive(false);
        roomNum.text = "";
        gameObject.SetActive(false);
        if(PhotonNetwork.inRoom)PhotonNetwork.LeaveRoom();
    }



void ChangeEnterButtonState(bool isEnabled)
    {
        if (isEnabled)
        {
            //エンターキーを有効に
            enterButton.interactable = true;
            enterText.color = textColor;
        }
        else
        {
            enterButton.interactable = false;
            enterText.color = new Color(0.2f, 0.2f, 0.2f);
        }
    }
    
}
