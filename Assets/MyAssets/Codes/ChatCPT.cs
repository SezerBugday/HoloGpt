using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;

public class ChatCPT : MonoBehaviour
{
    private string openAIApiKey = "sk-proj-spkvHNqAEE3oX3Ta1Qj9T3BlbkFJtYee6Q3l8sRiryTtKHRW";
    private string openAIUrl = "https://api.openai.com/v1/chat/completions";
    public string yazi;
    
    //[SerializeField] private Dropdown modelDropdown;
   // [SerializeField] private Dropdown voiceDropdown;
    //  [SerializeField] private InputField input;
    [SerializeField] private TTSManager ttsManager;
    //  [SerializeField] private Image entityImage;
    private Coroutine talkingEffect;
    string answer_cpt;

    private HistoryTalk talkHistory;
    // Konuşma geçmişi
    

    public void Basla(string userInput)   // prompt gir sonucu bekle
    {   
        Debug.Log("Basla: " + userInput);
        
        
        if(HistoryTalk.Instance != null)
        {
            // Kullanıcı mesajını konuşma geçmişine ekleyin
            HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = userInput });
        }
        

        // Komutu gönderin
        StartCoroutine(SendPostRequest(userInput));
    }

    private IEnumerator SendPostRequest(string userInput)
    {
        // JSON veri yapısını oluşturuyoruz
        RequestData requestData = new RequestData
        {
            model = "gpt-3.5-turbo",
            messages = HistoryTalk.Instance.conversationHistory.ToArray()
        };

        string jsonData = JsonUtility.ToJson(requestData);
        
        Debug.Log("jsonData: " + jsonData);
        
        using (UnityWebRequest request = new UnityWebRequest(openAIUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + openAIApiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);
                if (response != null && response.choices.Length > 0)
                {
                    string chatResponse = response.choices[0].message.content;
                    Debug.Log("ChatGPT cevabı: " + chatResponse);
                    yazi = chatResponse;
                   
                    // Asistan mesajını konuşma geçmişine ekleyin
                   
                    if(HistoryTalk.Instance != null)
                    {
                        HistoryTalk.Instance.conversationHistory.Add(new Message { role = "assistant", content = chatResponse });
                    }
                   
                   
                    PlayGptVoice(yazi);
                }
            }
        }
    }
    
    /********************************************************************/
    public void PlayGptVoice(string cevapyazi)
    {
        if (ttsManager)
        {
            ttsManager.SynthesizeAndPlay(cevapyazi, TTSModel.TTS_1, TTSVoice.Alloy, 1);
        }
    }
    
    
}

[System.Serializable]
public class RequestData
{
    public string model;
    public Message[] messages;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}

[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }
}
