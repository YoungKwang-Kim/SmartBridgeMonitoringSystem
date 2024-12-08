using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAPI : MonoBehaviour
{
    /// <summary>
    /// ���� UI ������Ҹ� �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class WeatherUIComponents
    {
        public Image weatherIcon;          // ���� ������ �̹���
        public Text temperatureDisplay;    // �µ� ǥ�� �ؽ�Ʈ
    }

    /// <summary>
    /// Weather API ���� ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class WeatherAPISettings
    {
        public string apiKey = "9de2d70f99200d51e41cdc48f150976e"; // API Ű
        public float latitude = 37.5833f;  // ����
        public float longitude = 127f;     // �浵
        public float updateInterval = 300f; // ������Ʈ ���� (��)
    }

    [Header("UI Components")]
    [SerializeField] private WeatherUIComponents uiComponents;

    [Header("API Settings")]
    [SerializeField] private WeatherAPISettings apiSettings;

    // API ���
    private const string API_BASE_URL = "https://api.openweathermap.org/data/2.5/weather";
    private const float KELVIN_TO_CELSIUS = 273.15f;

    // �̺�Ʈ ��������Ʈ
    public event Action<float> OnTemperatureUpdated;         // �µ� ������Ʈ �� �߻��ϴ� �̺�Ʈ
    public event Action<string> OnWeatherConditionChanged;   // ���� ���� ���� �� �߻��ϴ� �̺�Ʈ

    /// <summary>
    /// ���� ������ ������Ʈ ����
    /// </summary>
    private void Start()
    {
        StartCoroutine(UpdateWeatherRoutine());
    }

    /// <summary>
    /// �ֱ������� ���� �����͸� ������Ʈ�ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator UpdateWeatherRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(FetchWeatherData());
            yield return new WaitForSeconds(apiSettings.updateInterval);
        }
    }

    /// <summary>
    /// API���� ���� �����͸� �������� �ڷ�ƾ
    /// </summary>
    private IEnumerator FetchWeatherData()
    {
        string url = BuildApiUrl();

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (IsRequestFailed(webRequest))
            {
                Debug.LogError($"Weather API Error: {webRequest.error}");
                yield break;
            }

            try
            {
                ProcessWeatherData(webRequest.downloadHandler.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Weather data processing error: {e.Message}");
            }
        }
    }

    /// <summary>
    /// API ��û URL ����
    /// </summary>
    private string BuildApiUrl()
    {
        return $"{API_BASE_URL}?lat={apiSettings.latitude}&lon={apiSettings.longitude}&appid={apiSettings.apiKey}";
    }

    /// <summary>
    /// API ��û ���� ���� Ȯ��
    /// </summary>
    private bool IsRequestFailed(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError;
    }

    /// <summary>
    /// JSON ������ ���� ������ ó��
    /// </summary>
    private void ProcessWeatherData(string jsonResponse)
    {
        WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);
        if (weatherData == null) return;

        UpdateTemperature(weatherData.main.temp);
        UpdateWeatherIcon(weatherData.weather[0].id);
    }

    /// <summary>
    /// �µ� ���� ������Ʈ �� UI ����
    /// </summary>
    private void UpdateTemperature(float kelvinTemp)
    {
        float celsius = kelvinTemp - KELVIN_TO_CELSIUS;
        if (uiComponents.temperatureDisplay != null)
        {
            uiComponents.temperatureDisplay.text = $"{celsius:F0}��C";
        }
        OnTemperatureUpdated?.Invoke(celsius);
    }

    /// <summary>
    /// ���� ������ ������Ʈ �� UI ����
    /// </summary>
    private void UpdateWeatherIcon(int weatherId)
    {
        string iconName = GetWeatherIconName(weatherId);
        Sprite weatherSprite = LoadWeatherSprite(iconName);

        if (weatherSprite != null && uiComponents.weatherIcon != null)
        {
            uiComponents.weatherIcon.sprite = weatherSprite;
        }

        OnWeatherConditionChanged?.Invoke(iconName);
    }

    /// <summary>
    /// ���� ID�� ���� ������ �̸� ��ȯ
    /// </summary>
    private string GetWeatherIconName(int weatherId)
    {
        return weatherId switch
        {
            >= 200 and <= 299 => "Thunder_J", // õ�չ���
            >= 300 and <= 399 => "Drizzle_J", // �̽���
            >= 500 and <= 599 => "Rain_J",    // ��
            >= 600 and <= 699 => "Snow_J",    // ��
            >= 700 and <= 799 => "Fog_J",     // �Ȱ�
            >= 801 and <= 899 => "Cloud_J",   // ����
            _ => "Sun_J"                      // ����
        };
    }

    /// <summary>
    /// ���� ������ ��������Ʈ �ε�
    /// </summary>
    private Sprite LoadWeatherSprite(string iconName)
    {
        Sprite sprite = Resources.Load<Sprite>($"Icons/{iconName}");
        if (sprite == null)
        {
            Debug.LogWarning($"Weather icon not found: {iconName}");
        }
        return sprite;
    }

    #region Data Classes
    [System.Serializable]
    private class WeatherData
    {
        public Main main;
        public Weather[] weather;
    }

    [System.Serializable]
    private class Main
    {
        public float temp;
    }

    [System.Serializable]
    private class Weather
    {
        public int id;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// API ���� ������Ʈ (����, �浵, API Ű)
    /// </summary>
    public void UpdateAPISettings(float lat, float lon, string newApiKey = null)
    {
        apiSettings.latitude = lat;
        apiSettings.longitude = lon;
        if (!string.IsNullOrEmpty(newApiKey))
        {
            apiSettings.apiKey = newApiKey;
        }
        StartCoroutine(FetchWeatherData());
    }

    /// <summary>
    /// ������Ʈ �ֱ� ���� (�ּ� 1��)
    /// </summary>
    public void SetUpdateInterval(float seconds)
    {
        apiSettings.updateInterval = Mathf.Max(60f, seconds);
    }
    #endregion
}