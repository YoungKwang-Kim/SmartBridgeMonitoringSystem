using UnityEngine;
using System.IO.Ports;
using TMPro;
using System;
using System.Threading.Tasks;

/// <summary>
/// �Ƶ��̳� ���� �����͸� �����ϰ� UI�� ǥ���ϴ� Ŭ����
/// </summary>
public class SensorDataReceiver : MonoBehaviour
{
    /// <summary>
    /// �ø��� ��Ʈ ��� ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class SerialPortSettings
    {
        public string portName = "COM3";         // �ø��� ��Ʈ �̸�
        public int baudRate = 9600;              // ��� �ӵ�
        public int readTimeout = 1000;           // �б� Ÿ�Ӿƿ�(ms)
        public float updateInterval = 0.1f;      // ������ ������Ʈ ����(��)
    }

    /// <summary>
    /// UI �ؽ�Ʈ ǥ�� ��ҵ��� �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class SensorDisplays
    {
        [Header("Distance Sensors")]
        public TextMeshProUGUI distance1Text;    // ù ��° �Ÿ� ���� �ؽ�Ʈ
        public TextMeshProUGUI distance2Text;    // �� ��° �Ÿ� ���� �ؽ�Ʈ

        [Header("Temperature Sensor")]
        public TextMeshProUGUI temperatureText;  // �µ� ���� �ؽ�Ʈ

        [Header("Pressure Sensors")]
        public TextMeshProUGUI pressure1Text;    // ù ��° �з� ���� �ؽ�Ʈ
        public TextMeshProUGUI pressure2Text;    // �� ��° �з� ���� �ؽ�Ʈ
    }

    [SerializeField] private SerialPortSettings settings = new SerialPortSettings();
    [SerializeField] private SensorDisplays displays;

    private SerialPort serialPort;               // �ø��� ��Ʈ ��ü
    private bool isRunning = true;               // ������ ���� ���� ����
    private SensorData lastReceivedData;         // ���������� ���ŵ� ���� ������

    /// <summary>
    /// ���� �����͸� �����ϴ� Ŭ����
    /// </summary>
    private class SensorData
    {
        public float Distance1 { get; set; }     // ù ��° �Ÿ� ���� ��
        public float Distance2 { get; set; }     // �� ��° �Ÿ� ���� ��
        public float Temperature { get; set; }   // �µ� ���� ��
        public int Pressure1 { get; set; }       // ù ��° �з� ���� ��
        public int Pressure2 { get; set; }       // �� ��° �з� ���� ��
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
    /// �ø��� ��Ʈ �ʱ�ȭ �� ����
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
    /// �񵿱� ������ ���� ����
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
                await Task.Delay(1000); // ���� �߻� �� 1�� ���
            }
        }
    }

    /// <summary>
    /// �ø��� ��Ʈ���� ������ �񵿱� �б�
    /// </summary>
    private async Task ReadSerialDataAsync()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        string data = await Task.Run(() => serialPort.ReadLine());
        ParseAndDisplayData(data);
    }

    /// <summary>
    /// ���ŵ� ������ �Ľ� �� ó��
    /// </summary>
    /// <param name="data">���ŵ� ������ ���ڿ�</param>
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
    /// UI �ؽ�Ʈ ������Ʈ
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
    /// �ø��� ��Ʈ ���� ���� �� ����
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
    /// �ø��� ��Ʈ ���� ����
    /// </summary>
    /// <param name="newPortName">���ο� ��Ʈ �̸�</param>
    /// <param name="newBaudRate">���ο� ��� �ӵ�</param>
    public void ChangeSerialPort(string newPortName, int newBaudRate = 9600)
    {
        CleanupSerialPort();
        settings.portName = newPortName;
        settings.baudRate = newBaudRate;
        InitializeSerialPort();
        StartDataReception();
    }

    /// <summary>
    /// ���� ���� ������ ��ȯ
    /// </summary>
    /// <returns>���� ���� ������</returns>
    public SensorData GetCurrentSensorData()
    {
        return lastReceivedData;
    }

    /// <summary>
    /// ������ ������Ʈ ���� ����
    /// </summary>
    /// <param name="seconds">������Ʈ ����(��)</param>
    public void SetUpdateInterval(float seconds)
    {
        settings.updateInterval = Mathf.Max(0.1f, seconds);
    }
    #endregion
}