using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class WeatherAPI : MonoBehaviour
{
    /// <summary>
    /// 날씨 UI 구성요소를 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class WeatherUIComponents
    {
        public Image weatherIcon;          // 날씨 아이콘 이미지
        public Text temperatureDisplay;    // 온도 표시 텍스트
    }

    /// <summary>
    /// Weather API 관련 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class WeatherAPISettings
    {
        public string apiKey = "9de2d70f99200d51e41cdc48f150976e"; // API 키
        public float latitude = 37.5833f;  // 위도
        public float longitude = 127f;     // 경도
        public float updateInterval = 300f; // 업데이트 간격 (초)
    }

    [Header("UI Components")]
    [SerializeField] private WeatherUIComponents uiComponents;

    [Header("API Settings")]
    [SerializeField] private WeatherAPISettings apiSettings;

    // API 상수
    private const string API_BASE_URL = "https://api.openweathermap.org/data/2.5/weather";
    private const float KELVIN_TO_CELSIUS = 273.15f;

    // 이벤트 델리게이트
    public event Action<float> OnTemperatureUpdated;         // 온도 업데이트 시 발생하는 이벤트
    public event Action<string> OnWeatherConditionChanged;   // 날씨 상태 변경 시 발생하는 이벤트

    /// <summary>
    /// 날씨 데이터 업데이트 시작
    /// </summary>
    private void Start()
    {
        StartCoroutine(UpdateWeatherRoutine());
    }

    /// <summary>
    /// 주기적으로 날씨 데이터를 업데이트하는 코루틴
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
    /// API에서 날씨 데이터를 가져오는 코루틴
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
    /// API 요청 URL 생성
    /// </summary>
    private string BuildApiUrl()
    {
        return $"{API_BASE_URL}?lat={apiSettings.latitude}&lon={apiSettings.longitude}&appid={apiSettings.apiKey}";
    }

    /// <summary>
    /// API 요청 실패 여부 확인
    /// </summary>
    private bool IsRequestFailed(UnityWebRequest request)
    {
        return request.result == UnityWebRequest.Result.ConnectionError ||
               request.result == UnityWebRequest.Result.ProtocolError;
    }

    /// <summary>
    /// JSON 형태의 날씨 데이터 처리
    /// </summary>
    private void ProcessWeatherData(string jsonResponse)
    {
        WeatherData weatherData = JsonUtility.FromJson<WeatherData>(jsonResponse);
        if (weatherData == null) return;

        UpdateTemperature(weatherData.main.temp);
        UpdateWeatherIcon(weatherData.weather[0].id);
    }

    /// <summary>
    /// 온도 정보 업데이트 및 UI 갱신
    /// </summary>
    private void UpdateTemperature(float kelvinTemp)
    {
        float celsius = kelvinTemp - KELVIN_TO_CELSIUS;
        if (uiComponents.temperatureDisplay != null)
        {
            uiComponents.temperatureDisplay.text = $"{celsius:F0}°C";
        }
        OnTemperatureUpdated?.Invoke(celsius);
    }

    /// <summary>
    /// 날씨 아이콘 업데이트 및 UI 갱신
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
    /// 날씨 ID에 따른 아이콘 이름 반환
    /// </summary>
    private string GetWeatherIconName(int weatherId)
    {
        return weatherId switch
        {
            >= 200 and <= 299 => "Thunder_J", // 천둥번개
            >= 300 and <= 399 => "Drizzle_J", // 이슬비
            >= 500 and <= 599 => "Rain_J",    // 비
            >= 600 and <= 699 => "Snow_J",    // 눈
            >= 700 and <= 799 => "Fog_J",     // 안개
            >= 801 and <= 899 => "Cloud_J",   // 구름
            _ => "Sun_J"                      // 맑음
        };
    }

    /// <summary>
    /// 날씨 아이콘 스프라이트 로드
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
    /// API 설정 업데이트 (위도, 경도, API 키)
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
    /// 업데이트 주기 설정 (최소 1분)
    /// </summary>
    public void SetUpdateInterval(float seconds)
    {
        apiSettings.updateInterval = Mathf.Max(60f, seconds);
    }
    #endregion
}