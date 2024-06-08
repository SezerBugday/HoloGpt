using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class HavaApi : MonoBehaviour
{
    
    ChatCPT chatCPT;
    IEnumerator Start()
    {
        
        chatCPT = GetComponent<ChatCPT>();
        string url = "https://api.weatherapi.com/v1/current.json?key=d90a3b5dd86344fca75185417241603&q=Hatay";
        
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonData = www.downloadHandler.text;
            WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonData);

            Debug.Log("Hava Durumu Bilgisi:");
            Debug.Log("Şehir: " + weatherData.location.name);
            Debug.Log("Sıcaklık (°C): " + weatherData.current.temp_c);
            Debug.Log("Hava Durumu: " + weatherData.current.condition.text);
            
            
          HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = weatherData.location.name+" hava durumu" +
            weatherData.current.temp_c + "derece ve " + weatherData.current.condition.text });
            
        }
        else
        {
            Debug.LogError("Hava durumu verileri alınamadı. Hata: " + www.error);
        }
    }
}

[System.Serializable]
public class WeatherData
{
    public Location location;
    public Current current;
}

[System.Serializable]
public class Location
{
    public string name;
}

[System.Serializable]
public class Current
{
    public float temp_c;
    public Condition condition;
}

[System.Serializable]
public class Condition
{
    public string text;
}