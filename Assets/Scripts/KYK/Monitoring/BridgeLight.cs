using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 교량의 상태를 신호등으로 표시하는 클래스
/// </summary>
public class BridgeLight : MonoBehaviour
{
    #region UI Components
    [Header("다리 신호등(순서는 1.왼쪽기둥, 2.왼쪽도로, 3.오른쪽기둥, 4.오른쪽도로)")]
    /// <summary>
    /// 정상 상태를 나타내는 녹색등 목록
    /// </summary>
    public List<Image> GreenLight;

    /// <summary>
    /// 주의 상태를 나타내는 황색등 목록
    /// </summary>
    public List<Image> YellowLight;

    /// <summary>
    /// 위험 상태를 나타내는 적색등 목록
    /// </summary>
    public List<Image> RedLight;
    #endregion

    #region Sensor Components
    [Header("아두이노 센서들")]
    [Tooltip("다리 왼쪽의 거리센서")]
    public TextMeshProUGUI leftDistanceSensor;

    [Tooltip("다리 왼쪽의 압력센서")]
    public TextMeshProUGUI leftPressureSensor;

    [Tooltip("다리 오른쪽의 거리센서")]
    public TextMeshProUGUI rightDistanceSensor;

    [Tooltip("다리 오른쪽의 압력센서")]
    public TextMeshProUGUI rightPressureSensor;
    #endregion

    #region Threshold Values
    [Header("위험수치들")]
    public float yellowDistanceValue;  // 거리 센서 주의 임계값
    public float yellowPressureValue;  // 압력 센서 주의 임계값
    public float redDistanceValue;     // 거리 센서 위험 임계값
    public float redPressureValue;     // 압력 센서 위험 임계값
    #endregion

    /// <summary>
    /// 초기화 - 황색등과 적색등을 비활성화
    /// </summary>
    void Start()
    {
        foreach (var light in YellowLight)
        {
            light.enabled = false;
        }
        foreach (var light in RedLight)
        {
            light.enabled = false;
        }
    }

    /// <summary>
    /// 매 프레임마다 신호등 상태 업데이트
    /// </summary>
    void Update()
    {
        RedLightState();
        YellowLightState();
    }

    /// <summary>
    /// 주의 단계(황색등) 상태를 처리하는 메서드
    /// 각 센서값이 주의 임계값을 넘으면 해당 위치의 황색등을 활성화
    /// </summary>
    private void YellowLightState()
    {
        // 센서 데이터 파싱
        float leftDistanceValue = float.Parse(leftDistanceSensor.text);
        float rightDistanceValue = float.Parse(rightDistanceSensor.text);
        float leftPressureValue = float.Parse(leftPressureSensor.text);
        float rightPressureValue = float.Parse(rightPressureSensor.text);

        // 왼쪽 기둥 상태 확인
        YellowLight[0].enabled = yellowPressureValue < leftPressureValue;

        // 왼쪽 도로 상태 확인
        YellowLight[1].enabled = yellowDistanceValue > leftDistanceValue;

        // 오른쪽 기둥 상태 확인
        YellowLight[2].enabled = yellowPressureValue < rightPressureValue;

        // 오른쪽 도로 상태 확인
        YellowLight[3].enabled = yellowDistanceValue > rightDistanceValue;
    }

    /// <summary>
    /// 위험 단계(적색등) 상태를 처리하는 메서드
    /// 각 센서값이 위험 임계값을 넘으면 해당 위치의 적색등을 활성화
    /// </summary>
    private void RedLightState()
    {
        // 센서 데이터 파싱
        float leftDistanceValue = float.Parse(leftDistanceSensor.text);
        float rightDistanceValue = float.Parse(rightDistanceSensor.text);
        float leftPressureValue = float.Parse(leftPressureSensor.text);
        float rightPressureValue = float.Parse(rightPressureSensor.text);

        // 왼쪽 기둥 상태 확인
        RedLight[0].enabled = redPressureValue < leftPressureValue;

        // 왼쪽 도로 상태 확인
        RedLight[1].enabled = redDistanceValue > leftDistanceValue;

        // 오른쪽 기둥 상태 확인
        RedLight[2].enabled = redPressureValue < rightPressureValue;

        // 오른쪽 도로 상태 확인
        RedLight[3].enabled = redDistanceValue > rightDistanceValue;
    }
}