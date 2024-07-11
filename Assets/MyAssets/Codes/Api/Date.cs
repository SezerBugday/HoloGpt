using UnityEngine;
using System;
using System.IO;

public class DateManager : MonoBehaviour
{
    private string filePath;

    void Start()
    {
        // Dosya yolunu belirle
        
        
        // C:\Users\sezer\AppData\LocalLow\Wheat\HoloGpt
        filePath = Path.Combine(Application.persistentDataPath, "dateData.json");
        Debug.Log("JSON file path: " + filePath);
        
        // Tarih bilgisini al ve dosyaya yaz
        SaveCurrentDate();
        // Mevcut tarih bilgisini yükle ve kontrol et
        LoadDate();
    }

    void SaveCurrentDate()
    {
        // Mevcut tarihi al
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // DateData objesi oluştur ve tarihi ata
        DateData dateData = new DateData { date = currentDate };

        // DateData objesini JSON string'e dönüştür
        string json = JsonUtility.ToJson(dateData, true);

        // JSON string'i dosyaya yaz
        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Current date saved: " + currentDate);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to JSON file: " + ex.Message);
        }
    }

    void LoadDate()
    {
        // Dosya mevcut mu kontrol et
        if (File.Exists(filePath))
        {
            try
            {
                // Dosyadan JSON string'i oku
                string json = File.ReadAllText(filePath);

                // JSON string'i DateData objesine dönüştür
                DateData dateData = JsonUtility.FromJson<DateData>(json);

                // Tarih bilgisini logla
                Debug.Log("Loaded date: " + dateData.date);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read from JSON file: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No date data found.");
        }
    }
}

[System.Serializable]
public class DateData
{
    public string date;
}