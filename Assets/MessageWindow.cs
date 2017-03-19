using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class MessageWindow : MonoBehaviour {

    static Text message;
    static Button OKButton;
    static GameObject tako;
    void Awake()
    {
        message = transform.FindChild("Message").GetComponent<Text>();
        OKButton = transform.FindChild("Button").GetComponent<Button>();
        tako = gameObject;
    }
	

    public static void ShowMessageWindow(string msg,UnityAction action)
    {
        message.text = msg;
        tako.transform.DOScale(Vector3.one,0.5f).SetEase(Ease.InOutExpo);
        Debug.Log(msg);
        OKButton.onClick.AddListener(action);
    }


    public void HideWindow()
    {
        tako.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo);
    }
}
