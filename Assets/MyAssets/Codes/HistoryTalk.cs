using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryTalk : MonoBehaviour
{
    private static HistoryTalk _instance;
    public static HistoryTalk Instance 
    { 
        get
        {
            if (_instance == null)
            {
                GameObject historyTalkObject = new GameObject("HistoryTalk");
                _instance = historyTalkObject.AddComponent<HistoryTalk>();
                DontDestroyOnLoad(historyTalkObject);
            }
            return _instance;
        }
    }

    public List<Message> conversationHistory = new List<Message>();
}