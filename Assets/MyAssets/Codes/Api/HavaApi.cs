using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class HavaApi : MonoBehaviour
{
    public Toggle CheckHavaapi;
    public ChatCPT chatCPT;
    public string tespit_edilen_sehir;
    private string all_command;
    string filePath;
    private string jsonData;

    public void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "WeatherData.json");
        Debug.Log("JSON file path: " + filePath);
    }

    private WeatherDataCollection LoadWeatherDataCollection()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<WeatherDataCollection>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read from JSON file: " + ex.Message);
            }
        }
        return new WeatherDataCollection();
    }

    private WeatherData LoadWeatherDataForCity(string cityName)
    {
        WeatherDataCollection collection = LoadWeatherDataCollection();
        foreach (var weatherData in collection.weatherDataList)
        {
            if (weatherData.location.name.ToLower() == cityName.ToLower())
            {
                // Check if last update time is within 1 hour
                DateTime lastUpdateTime;
                if (DateTime.TryParse(weatherData.lastUpdated, out lastUpdateTime))
                {
                    TimeSpan timeSinceLastUpdate = DateTime.Now - lastUpdateTime;
                    if (timeSinceLastUpdate.TotalHours < 1)
                    {
                        return weatherData;
                    }
                }

                // If more than 1 hour has passed, return null to fetch new data
                return null;
            }
        }
        return null;
    }

    private void SaveCurrentWeather(WeatherData weatherData)
    {
        WeatherDataCollection collection = LoadWeatherDataCollection();
        int existingIndex = collection.weatherDataList.FindIndex(w => w.location.name.ToLower() == weatherData.location.name.ToLower());

        if (existingIndex >= 0)
        {
            collection.weatherDataList[existingIndex] = weatherData;
        }
        else
        {
            collection.weatherDataList.Add(weatherData);
        }

        string json = JsonUtility.ToJson(collection, true);
        try
        {
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to JSON file: " + ex.Message);
        }
    }

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
        string[] cities = new string[] { "Hatay", "Ankara", "İstanbul", "İzmir", "Adana","Berlin" };

        foreach (var city in cities)
        {
            if (command.ToLower().Contains(city.ToLower()))
            {
                Debug.Log("Tespit edilen şehir: " + city);
                tespit_edilen_sehir = city;
                WeatherData localData = LoadWeatherDataForCity(tespit_edilen_sehir);

                if (localData != null)
                {
                    DisplayWeatherData(localData);
                }
                else
                {
                    StartCoroutine(FetchWeatherData());
                }

                return;  // Şehir bulunduğunda döngüden çık
            }
        }
    }

    private IEnumerator FetchWeatherData()
    {
        string url = $"https://api.weatherapi.com/v1/current.json?key=d90a3b5dd86344fca75185417241603&q={tespit_edilen_sehir}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            chatCPT.chatText.text = "Hava Durumu Bilgisi gerçek zamanlı olarak çekilmiştir...";

            jsonData = www.downloadHandler.text;
            WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonData);
            weatherData.lastUpdated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SaveCurrentWeather(weatherData);
            DisplayWeatherData(weatherData);
        }
        else
        {
            Debug.LogError("Hava durumu verileri alınamadı. Hata: " + www.error);
        }
    }

    private void DisplayWeatherData(WeatherData weatherData)
    {
        Debug.Log("Hava Durumu Bilgisi:");
        Debug.Log("Şehir: " + weatherData.location.name);
        Debug.Log("Sıcaklık (°C): " + weatherData.current.temp_c);
        Debug.Log("Hava Durumu: " + weatherData.current.condition.text);
        Debug.Log("Güncelleme Saati: " + weatherData.lastUpdated);

        string weatherInfo = $"{weatherData.location.name} hava durumu: {weatherData.current.temp_c} derece ve {weatherData.current.condition.text}. Güncelleme saati: {weatherData.lastUpdated}.";
        HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = weatherInfo });

        chatCPT.Basla(all_command + weatherInfo);  // Komutu tekrar işleme
    }
}

[System.Serializable]
public class WeatherDataCollection
{
    public List<WeatherData> weatherDataList = new List<WeatherData>();
}

[System.Serializable]
public class WeatherData
{
    public Location location;
    public Current current;
    public string lastUpdated;
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
