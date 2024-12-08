using UnityEngine;
using System.IO.Ports;
using TMPro;
using System;
using System.Threading.Tasks;

/// <summary>
/// 아두이노 센서 데이터를 수신하고 UI에 표시하는 클래스
/// </summary>
public class SensorDataReceiver : MonoBehaviour
{
    /// <summary>
    /// 시리얼 포트 통신 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class SerialPortSettings
    {
        public string portName = "COM3";         // 시리얼 포트 이름
        public int baudRate = 9600;              // 통신 속도
        public int readTimeout = 1000;           // 읽기 타임아웃(ms)
        public float updateInterval = 0.1f;      // 데이터 업데이트 간격(초)
    }

    /// <summary>
    /// UI 텍스트 표시 요소들을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class SensorDisplays
    {
        [Header("Distance Sensors")]
        public TextMeshProUGUI distance1Text;    // 첫 번째 거리 센서 텍스트
        public TextMeshProUGUI distance2Text;    // 두 번째 거리 센서 텍스트

        [Header("Temperature Sensor")]
        public TextMeshProUGUI temperatureText;  // 온도 센서 텍스트

        [Header("Pressure Sensors")]
        public TextMeshProUGUI pressure1Text;    // 첫 번째 압력 센서 텍스트
        public TextMeshProUGUI pressure2Text;    // 두 번째 압력 센서 텍스트
    }

    [SerializeField] private SerialPortSettings settings = new SerialPortSettings();
    [SerializeField] private SensorDisplays displays;

    private SerialPort serialPort;               // 시리얼 포트 객체
    private bool isRunning = true;               // 데이터 수신 실행 상태
    private SensorData lastReceivedData;         // 마지막으로 수신된 센서 데이터

    /// <summary>
    /// 센서 데이터를 저장하는 클래스
    /// </summary>
    private class SensorData
    {
        public float Distance1 { get; set; }     // 첫 번째 거리 센서 값
        public float Distance2 { get; set; }     // 두 번째 거리 센서 값
        public float Temperature { get; set; }   // 온도 센서 값
        public int Pressure1 { get; set; }       // 첫 번째 압력 센서 값
        public int Pressure2 { get; set; }       // 두 번째 압력 센서 값
    }

    private void Start()
    {
        InitializeSerialPort();
        StartDataReception();
    }

    private void OnDestroy()
    {
        CleanupSerialPort();
    }

    /// <summary>
    /// 시리얼 포트 초기화 및 연결
    /// </summary>
    private void InitializeSerialPort()
    {
        try
        {
            serialPort = new SerialPort(settings.portName, settings.baudRate)
            {
                ReadTimeout = settings.readTimeout,
                DtrEnable = true,
                RtsEnable = true
            };
            serialPort.Open();
            Debug.Log($"Successfully opened port: {settings.portName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to open serial port {settings.portName}: {e.Message}");
        }
    }

    /// <summary>
    /// 비동기 데이터 수신 시작
    /// </summary>
    private async void StartDataReception()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                await ReadSerialDataAsync();
                await Task.Delay((int)(settings.updateInterval * 1000));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Error reading serial data: {e.Message}");
                await Task.Delay(1000); // 에러 발생 시 1초 대기
            }
        }
    }

    /// <summary>
    /// 시리얼 포트에서 데이터 비동기 읽기
    /// </summary>
    private async Task ReadSerialDataAsync()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        string data = await Task.Run(() => serialPort.ReadLine());
        ParseAndDisplayData(data);
    }

    /// <summary>
    /// 수신된 데이터 파싱 및 처리
    /// </summary>
    /// <param name="data">수신된 데이터 문자열</param>
    private void ParseAndDisplayData(string data)
    {
        try
        {
            string[] sensorData = data.Split(',');
            if (sensorData.Length < 5) return;

            lastReceivedData = new SensorData
            {
                Distance1 = float.Parse(sensorData[0]),
                Distance2 = float.Parse(sensorData[1]),
                Temperature = float.Parse(sensorData[2]),
                Pressure1 = int.Parse(sensorData[3]),
                Pressure2 = int.Parse(sensorData[4])
            };

            UpdateUI();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to parse sensor data: {e.Message}");
        }
    }

    /// <summary>
    /// UI 텍스트 업데이트
    /// </summary>
    private void UpdateUI()
    {
        if (lastReceivedData == null) return;

        try
        {
            displays.distance1Text.text = lastReceivedData.Distance1.ToString("F2");
            displays.distance2Text.text = lastReceivedData.Distance2.ToString("F2");
            displays.temperatureText.text = lastReceivedData.Temperature.ToString("F1");
            displays.pressure1Text.text = lastReceivedData.Pressure1.ToString();
            displays.pressure2Text.text = lastReceivedData.Pressure2.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating UI: {e.Message}");
        }
    }

    /// <summary>
    /// 시리얼 포트 연결 해제 및 정리
    /// </summary>
    private void CleanupSerialPort()
    {
        isRunning = false;
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            serialPort.Dispose();
        }
    }

    #region Public Methods
    /// <summary>
    /// 시리얼 포트 설정 변경
    /// </summary>
    /// <param name="newPortName">새로운 포트 이름</param>
    /// <param name="newBaudRate">새로운 통신 속도</param>
    public void ChangeSerialPort(string newPortName, int newBaudRate = 9600)
    {
        CleanupSerialPort();
        settings.portName = newPortName;
        settings.baudRate = newBaudRate;
        InitializeSerialPort();
        StartDataReception();
    }

    /// <summary>
    /// 현재 센서 데이터 반환
    /// </summary>
    /// <returns>현재 센서 데이터</returns>
    public SensorData GetCurrentSensorData()
    {
        return lastReceivedData;
    }

    /// <summary>
    /// 데이터 업데이트 간격 설정
    /// </summary>
    /// <param name="seconds">업데이트 간격(초)</param>
    public void SetUpdateInterval(float seconds)
    {
        settings.updateInterval = Mathf.Max(0.1f, seconds);
    }
    #endregion
}