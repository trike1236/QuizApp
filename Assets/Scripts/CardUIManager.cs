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

    GameObject RivalCard1;
    GameObject RivalCard2;
    GameObject RivalCard3;

    float moveTime = 0.5f;

    public bool canSelectCard = false;
    

    public CardSelectState cardSelectState = CardSelectState.None;
    public CardSelectState RivalCardSelectState;

    public List<float> myCardAttack = new List<float>();
    public List<float> myCardGuard = new List<float>();
    public List<float> rivalCardAttack = new List<float>();
    public List<float> rivalCardGuard = new List<float>();

    float atkmax = 40f;
    float grdmax = 20f;

    GameObject RivalCardSelected;

    // Use this for initialization
    void Start () {
        Card1 = GameObject.Find("Canvas/CardSelect/Card1");
        Card2 = GameObject.Find("Canvas/CardSelect/Card2");
        Card3 = GameObject.Find("Canvas/CardSelect/Card3");
        RivalCard1 = GameObject.Find("Canvas/RivalCardSelect/Card1");
        RivalCard2 = GameObject.Find("Canvas/RivalCardSelect/Card2");
        RivalCard3 = GameObject.Find("Canvas/RivalCardSelect/Card3");
    }

    public void ChangeCardSelectState(CardSelectState state)
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

    public void RivalCardShow(CardSelectState rivalCard)
    {
        switch (rivalCard)
        {
            case CardSelectState.Card1:
                RivalCard1.GetComponent<RectTransform>().DOLocalMoveY(-50f, moveTime);
                RivalCardSelected = RivalCard1;
                break;
            case CardSelectState.Card2:
                RivalCard2.GetComponent<RectTransform>().DOLocalMoveY(-50f, moveTime);
                RivalCardSelected = RivalCard2;
                break;
            case CardSelectState.Card3:
                RivalCard3.GetComponent<RectTransform>().DOLocalMoveY(-50f, moveTime);
                RivalCardSelected = RivalCard3;
                break;
        }
    }

    public void RivalCardHide()
    {
        RivalCardSelected.GetComponent<RectTransform>().DOLocalMoveY(0f, moveTime);
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


    //カードにランダムな値をセット
    public void SetCardStates(bool isMine)
    {
        for (int i = 0; i > 3; i++)
        {
            if (isMine)
            {
                myCardAttack.Add(ReturnRandomStates(true));
                myCardGuard.Add(ReturnRandomStates(false));
            }else
            {
                rivalCardAttack.Add(ReturnRandomStates(true));
                rivalCardGuard.Add(ReturnRandomStates(false));
            }
        }
        
    }

    float ReturnRandomStates(bool isAtk)
    {
        if (isAtk)
        {
            return Random.Range(grdmax, atkmax);
        }
        else return Random.Range(0, grdmax);
    }

    public float StateToFloat(CardSelectState card,bool isMine,bool isAtk)
    {
        if (isMine)
        {
            if (isAtk) return myCardAttack[(int)card];
            else return myCardGuard[(int)card];
        }else
        {
            if (isAtk) return rivalCardAttack[(int)card];
            else return rivalCardGuard[(int)card];
        }
    }
}
