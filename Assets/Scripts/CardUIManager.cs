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
    Text text1;
    Text text2;
    Text text3;

    GameObject RivalCard1;
    GameObject RivalCard2;
    GameObject RivalCard3;
    Text rivalText1;
    Text rivalText2;
    Text rivalText3;


    float moveTime = 0.5f;

    public bool canSelectCard = false;
    

    public CardSelectState cardSelectState = CardSelectState.None;
    public CardSelectState RivalCardSelectState;

    
    public float[] myCardAttack;
    public float[] myCardGuard;
    public float[] rivalCardAttack;
    public float[] rivalCardGuard;
    /*
    public List<float> myCardAttack = new List<float>();
    public List<float> myCardGuard = new List<float>();
    public List<float> rivalCardAttack = new List<float>();
    public List<float> rivalCardGuard = new List<float>();
    */

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
        
        text1 = Card1.transform.FindChild("Text").gameObject.GetComponent<Text>();
        text2 = Card2.transform.FindChild("Text").gameObject.GetComponent<Text>();
        text3 = Card3.transform.FindChild("Text").gameObject.GetComponent<Text>();
        rivalText1 = RivalCard1.transform.FindChild("Text").gameObject.GetComponent<Text>();
        rivalText2 = RivalCard2.transform.FindChild("Text").gameObject.GetComponent<Text>();
        rivalText3 = RivalCard3.transform.FindChild("Text").gameObject.GetComponent<Text>();

        rivalText1.text = "tako";
        
        myCardAttack = new float[3];
        myCardGuard = new float[3];
        rivalCardAttack = new float[3];
        rivalCardGuard = new float[3];
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

            for (int i = 0; i < 3; ++i)
        {
            if (isMine)
            {
                myCardAttack[i] = ReturnRandomStates(true);
                myCardGuard[i] = ReturnRandomStates(false);
            }
            else
            {
                rivalCardAttack[i] = ReturnRandomStates(true);
                rivalCardGuard[i] = ReturnRandomStates(false);
            }
        }
        
    }

    public void SetTextStates()
    {
        text1.text = "ATK : " + myCardAttack[0] + "\nGRD : " + myCardGuard[0];
        text2.text = "ATK : " + myCardAttack[1] + "\nGRD : " + myCardGuard[1];
        text3.text = "ATK : " + myCardAttack[2] + "\nGRD : " + myCardGuard[2];
        rivalText1.text = "ATK : " + rivalCardAttack[0] + "\nGRD : " + rivalCardGuard[0];
        rivalText2.text = "ATK : " + rivalCardAttack[1] + "\nGRD : " + rivalCardGuard[1];
        rivalText3.text = "ATK : " + rivalCardAttack[2] + "\nGRD : " + rivalCardGuard[2];
    }

    float ReturnRandomStates(bool isAtk)
    {
        if (isAtk)
        {
            return Mathf.Round(Random.Range(grdmax, atkmax));
        }
        else return Mathf.Round(Random.Range(0, grdmax));
    }

    int i;
    public float StateToFloat(CardSelectState card,bool isMine,bool isAtk)
    {
        
        switch(card){
            case CardSelectState.Card1:
                i = 0;
                break;
            case CardSelectState.Card2:
                i = 1;
                break;
            case CardSelectState.Card3:
                i = 2;
                break;
        }
        if (isMine)
        {
            if (isAtk) return myCardAttack[i];
            else return myCardGuard[i];
        }else
        {
            if (isAtk) return rivalCardAttack[i];
            else return rivalCardGuard[i];
        }
    }
}
