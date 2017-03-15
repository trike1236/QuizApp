using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public enum CardSelectState
{
    None = -1,
    Card1,
    Card2,
    Card3
}

public class CardUIManager : MonoBehaviour {


    GameObject Card1;
    GameObject Card2;
    GameObject Card3;

    float moveTime = 0.5f;

    public bool canSelectCard = false;
    

    public CardSelectState cardSelectState = CardSelectState.None;

    // Use this for initialization
    void Start () {
        Card1 = GameObject.Find("Canvas/CardSelect/Card1");
        Card2 = GameObject.Find("Canvas/CardSelect/Card2");
        Card3 = GameObject.Find("Canvas/CardSelect/Card3");
        cardSelectState = CardSelectState.Card2;
    }

    void ChangeCardSelectState(CardSelectState state)
    {
        if(cardSelectState != CardSelectState.None)
        {
            //カードを下に戻すアニメーション'
            MoveCardUp(StateToGameObject(cardSelectState), false);
        }
        cardSelectState = state;
        MoveCardUp(StateToGameObject(state),true);
    }
	
    //カードを移動させるメソッド、第2引数がtrueで上に
    void MoveCardUp(GameObject card,bool up)
    {
        if (up)
        {
            card.GetComponent<RectTransform>().DOLocalMoveY(50, moveTime);
        }else
        {
            card.GetComponent<RectTransform>().DOLocalMoveY(0, moveTime);
        }
    }

    GameObject StateToGameObject(CardSelectState state)
    {
        switch (state)
        {
            case CardSelectState.Card1:
                return Card1;
            case CardSelectState.Card2:
                return Card2;
            case CardSelectState.Card3:
                return Card3;
            default:
                return null;
        }
    }

    //クリック時に呼ぶメソッド　どのカードかをint１～３で渡す
    public void OnClickCardEvent(int i)
    {
        if (canSelectCard)
        {
            switch (i)
            {
                case 1:
                    ChangeCardSelectState(CardSelectState.Card1);
                    break;
                case 2:
                    ChangeCardSelectState(CardSelectState.Card2);
                    break;
                case 3:
                    ChangeCardSelectState(CardSelectState.Card3);
                    break;
                default:
                    break;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
