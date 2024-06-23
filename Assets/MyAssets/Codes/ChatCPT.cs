using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class ChatCPT : MonoBehaviour
{
    private string openAIApiKey = "sk-proj-spkvHNqAEE3oX3Ta1Qj9T3BlbkFJtYee6Q3l8sRiryTtKHRW";
    private string openAIUrl = "https://api.openai.com/v1/chat/completions";
    public string yazi;
    public TextMeshProUGUI chatText;
    public AudioSource audioSourceTSS;
    [SerializeField] private TTSManager ttsManager;

    public Animator modelAnim;
    string answer_cpt;
    public string tespit_edilen_sehir;
    private HistoryTalk talkHistory;

    public void Basla(string userInput) // Prompt gir sonucu bekle
    {
        Debug.Log("Basla: " + userInput);

        if (HistoryTalk.Instance != null)
        {
            // Kullanıcı mesajını konuşma geçmişine ekleyin
            HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = userInput });
            
            // Sistem mesajlarını yalnızca ilk mesaj olarak ekleyin
            if (HistoryTalk.Instance.conversationHistory.Count == 1)
            {
                HistoryTalk.Instance.conversationHistory.Insert(0, new Message { role = "system", content = "Lütfen tüm yanıtları Türkçe olarak ver." });
                HistoryTalk.Instance.conversationHistory.Insert(1, new Message { role = "system", content = "Senin adın HoloGPT, kendini tanıt." });
            }
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
            messages = HistoryTalk.Instance?.conversationHistory.ToArray() // HistoryTalk.Instance null olabilir
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
                    if (HistoryTalk.Instance != null)
                    {
                        HistoryTalk.Instance.conversationHistory.Add(new Message { role = "assistant", content = chatResponse });
                    }

                    if (chatText != null)
                    {
                        chatText.text = "HoloGPT : " + chatResponse;
                    }
                    PlayGptVoice(yazi);
                }
                else
                {
                    Debug.LogWarning("Boş yanıt alındı.");
                }
            }
        }
    }

    public void PlayGptVoice(string cevapyazi)
    {
        Debug.LogError("konusma basladi"+audioSourceTSS.isPlaying);
        if (ttsManager != null)
        {
            ttsManager.SynthesizeAndPlay(cevapyazi, TTSModel.TTS_1, TTSVoice.Alloy, 1);
            Debug.LogError("konusma bitti");
        }
        else
        {
            Debug.LogWarning("TTSManager atanmadı.");
        }
        
    }

    private void Update()
    {
        if(audioSourceTSS.isPlaying)
        {
          modelAnim.SetBool("isSpeaking",true);
        }
        else
        {
            modelAnim.SetBool("isSpeaking",false);
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
