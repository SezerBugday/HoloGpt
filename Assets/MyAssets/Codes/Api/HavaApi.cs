using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class HavaApi : MonoBehaviour
{
    public Toggle CheckHavaapi;
    public ChatCPT chatCPT;
    public string tespit_edilen_sehir;
    private string all_command;

    public void HavaDurumu(string command)
    {
        all_command = command;  // Komutu sakla
        if (command.ToLower().Contains("hava durumu"))
        {
            ExtractCityFromCommand(command);
        }
        else
        {
            // Diğer komutlar için buraya eklemeler yapabilirsiniz
            Debug.Log("Bilinmeyen komut: " + command);
            chatCPT.Basla(command);
        }
    }

    private void ExtractCityFromCommand(string command)
    {
        string[] cities = new string[] { "Hatay", "Ankara", "İstanbul", "İzmir", "Adana" };

        foreach (var city in cities)
        {
            if (command.ToLower().Contains(city.ToLower()))
            {
                Debug.Log("Tespit edilen şehir: " + city);
                tespit_edilen_sehir = city;
                StartCoroutine(FetchWeatherData());
                return;  // Şehir bulunduğunda döngüden çık
            }
        }
    }

    private IEnumerator FetchWeatherData()
    {
        chatCPT.chatText.text = "Hava Durumu Bilgisi gerçek zamanlı olarak çekilmiştir...";
        string url = $"https://api.weatherapi.com/v1/current.json?key=d90a3b5dd86344fca75185417241603&q={tespit_edilen_sehir}";

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

            string weatherInfo = $"{weatherData.location.name} hava durumu: {weatherData.current.temp_c} derece ve {weatherData.current.condition.text}.";
            HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = weatherInfo });

            chatCPT.Basla(all_command+weatherInfo);  // Komutu tekrar işleme
            
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
