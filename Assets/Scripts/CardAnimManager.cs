using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardAnimManager : MonoBehaviour {

    QuizUIManager quizUIManager;
    CardUIManager cardUIManager;

    CardSelectState myCard;
    CardSelectState rivalCard;

    float attack;
    float guard;
	// Use this for initialization
	void Start () {
        quizUIManager = gameObject.GetComponent<QuizUIManager>();
        cardUIManager = gameObject.GetComponent<CardUIManager>();
	}

    public void SetCard(CardSelectState mine,CardSelectState rival,float atk,float grd)
    {
        myCard = mine;
        rivalCard = rival;
        attack = atk;
        guard = grd;
    }

    //相手に攻撃を与える
    public IEnumerator FastCorrectCoroutine()
    {
        GetCardState(true);
        quizUIManager.DamageHPGage(attack - guard, false);
        yield return null;
    }

    public IEnumerator FastWrongCoroutine()
    {
        GetCardState(true);
        quizUIManager.DamageHPGage(attack, true);
        yield return null;
    }
    public IEnumerator SlowCorrectCoroutine()
    {
        GetCardState(false);
        quizUIManager.DamageHPGage(attack - guard, true);
        yield return null;
    }
    public IEnumerator SlowWrongCoroutine()
    {
        GetCardState(false);
        quizUIManager.DamageHPGage(attack, false);
        yield return null;
    }

    void GetCardState(bool isMyAtk)
    {
        if (isMyAtk)
        {
            attack = cardUIManager.StateToFloat(myCard, true, true);
            guard = cardUIManager.StateToFloat(rivalCard, false, false);
        }else
        {
            attack = cardUIManager.StateToFloat(rivalCard, false, true);
            guard = cardUIManager.StateToFloat(myCard, true, false);
        }
    }

    }
