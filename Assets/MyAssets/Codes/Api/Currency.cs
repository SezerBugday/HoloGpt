using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Currency : MonoBehaviour
{
    ChatCPT chatGPT;
    public string jsondata;
    private string filePath;
    private DateTime lastUpdated; // Son güncelleme tarihini tutacak değişken

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "currencyData.json");
        Debug.Log("JSON file path: " + filePath);

        chatGPT = GetComponent<ChatCPT>();

        GetCurrency();
    }

    public void GetCurrency()
    {
        Debug.Log("Currency Data Fetching...");
        StartCoroutine(FetchCurrencyData());
    }

    private IEnumerator FetchCurrencyData()
    {
        string url = "https://raw.githubusercontent.com/SezerBugday/KotlinAdvancedTrials/master/CurrencyApp/currencyData.json";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            jsondata = www.downloadHandler.text;

            CurrencyJson[] currencyData = JsonUtility.FromJson<CurrencyArrayWrapper>("{\"currencies\":" + jsondata + "}").currencies;

            string allCurrencyInfo = "";
            foreach (var currency in currencyData)
            {
                string currencyInfo = $"Bir: {currency.Base} {currency.Value} kadar {currency.Name} eder.\n";
                Debug.Log(currencyInfo);
                allCurrencyInfo += currencyInfo;
            }

            // Son güncelleme tarihini ayarla
            lastUpdated = DateTime.Now;
            SaveCurrentDate();

            HistoryTalk.Instance.conversationHistory.Add(new Message { role = "user", content = allCurrencyInfo });
        }
        else
        {
            Debug.LogError("Currency data could not be fetched. Error: " + www.error);
        }
    }

    void SaveCurrentDate()
    {
        CurrencyArrayWrapper wrapper = new CurrencyArrayWrapper();
        wrapper.currencies = JsonUtility.FromJson<CurrencyArrayWrapper>("{\"currencies\":" + jsondata + "}").currencies;
        wrapper.lastUpdated = lastUpdated.ToString("yyyy-MM-dd HH:mm:ss");

        // JSON verisini dosyaya yaz
        string json = JsonUtility.ToJson(wrapper, true);
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Currency data saved successfully with last updated date.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to JSON file: " + ex.Message);
        }
    }

    void LoadDate()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                CurrencyArrayWrapper wrapper = JsonUtility.FromJson<CurrencyArrayWrapper>(json);

                Debug.Log("Loaded currency data: ");
                foreach (var currency in wrapper.currencies)
                {
                    Debug.Log($"Name: {currency.Name}, Base: {currency.Base}, Value: {currency.Value}");
                }
                Debug.Log($"Last updated: {wrapper.lastUpdated}");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read from JSON file: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No currency data found.");
        }
    }
}

[System.Serializable]
public class CurrencyJson
{
    public string Name;
    public string Base;
    public float Value;
}

[System.Serializable]
public class CurrencyArrayWrapper
{
    public CurrencyJson[] currencies;
    public string lastUpdated;
}
