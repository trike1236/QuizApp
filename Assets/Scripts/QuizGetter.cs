using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using LitJson;

public class QuizGetter : MonoBehaviour {
    string quizUrl;
    public int quizRoomId = 0;
    [Serializable]
    public class QuizData
    {
        public int type;
        //public int quiz_id;
        public string text;
        public string answer_txt;
        public int answer_num;
    }

    public List<Quizes> quizList = new List<Quizes>();
    public class Quizes
    {
        public int type;
        public int quiz_id;
        public string text;
        public string answer_txt;
        public int answer_num;
    }
    public class QuizArray
    {
        //public int count;
        public List<string> quizes;
    }

    [Serializable]
    public class QuizDatas
    {
        public string type;

    }
    void Awake()
    {
        quizUrl = "http://uinga2007.s25.xrea.com/quiz/quiz.php";
        //StartCoroutine(RequestQuizOne());
    }

    public void RandomRoomId()
    {
        quizRoomId = UnityEngine.Random.Range(1, 9999);
    }
    public IEnumerator RequestQuizOne()
    {
        
            WWWForm wwwForm = new WWWForm();
            wwwForm.AddField("quiz_id", 1);
            wwwForm.AddField("type", "yontaku");        //4択だけど直そう

            WWW result = new WWW(quizUrl, wwwForm);
            //Debug.Log("test");
            yield return result;

            if (!string.IsNullOrEmpty(result.error))
            {
                Debug.LogError("www Error:" + result.error);
            }
            else
            {
            Debug.Log(result.text);
                QuizData quizData = JsonUtility.FromJson<QuizData>(result.text);
                Debug.Log(quizData.type);
                Debug.Log(quizData.answer_txt);
            Debug.Log(quizData.text);

            //taskArray = todo.task.Split('|');
        }
    
}


    public IEnumerator RequestQuizes(UnityAction<List<Quizes>> callback,int quizCount)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("quiz_count", quizCount);
        wwwForm.AddField("type", "yontaku");        //4択だけど直そう
        wwwForm.AddField("room_id", quizRoomId);

        WWW result = new WWW(quizUrl, wwwForm);
        //Debug.Log("test");
        yield return result;

        if (!string.IsNullOrEmpty(result.error))
        {
            Debug.LogError("www Error:" + result.error);
            //受信しなおす必要がある
        }
        else
        {
            //Debug.Log(result.text);
            quizList = new List<Quizes>();
            //string[] quizArray = LitJson.JsonMapper.ToObject<string[]>(result.text);
            var quizArray = LitJson.JsonMapper.ToObject<string[]>(result.text);
            Debug.Log(quizArray[0]);
            foreach (string quiz in quizArray)
            {
                Quizes tmp = JsonUtility.FromJson<Quizes>(quiz);
                quizList.Add(tmp);
            }
            
            callback(quizList);
        }
    }

    //結果をpostで送信する　successが帰ってきたら成功
    public IEnumerator PostResult(string quiz_tf,string quiz_id,int id)
    {
        Debug.Log(quiz_tf);
        Debug.Log(quiz_id);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("quiz_id",quiz_id);
        wwwForm.AddField("quiz_tf", quiz_tf);
        wwwForm.AddField("id",id);
        wwwForm.AddField("type", "result");


        WWW result = new WWW(quizUrl, wwwForm);
        //Debug.Log("test");
        yield return result;

        if (!string.IsNullOrEmpty(result.error))
        {
            Debug.LogError("www Error:" + result.error);
            //受信しなおす必要がある
        }
        else
        {
            Debug.Log(result.text);
            
            
        }
    }

    public void testRequestQuizes()
    {
        //StartCoroutine(RequestQuizes(unko));
    }

    public void unko(List<Quizes> tako)
    {
        //Debug.Log(tako[0].text);
    }
}
