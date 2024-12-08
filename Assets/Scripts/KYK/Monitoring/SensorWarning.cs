using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications.Android;
using System.Collections;


public class SensorWarning : MonoBehaviour
{
    #region UI Components
    [Header("UI Components")]
    public Text DronCheckText;                  // 드론 점검 알림 텍스트
    private bool isBlinking;                    // 텍스트 깜빡임 상태

    [Header("다리 신호등(순서는 1.왼쪽기둥, 2.왼쪽도로, 3.오른쪽기둥, 4.오른쪽도로)")]
    public List<Image> RedLightImage;          // 위험 상태 표시등 목록
    #endregion

    #region Sensor Components
    [Header("Sensors")]
    public TextMeshProUGUI leftDistanceSensor;  // 왼쪽 거리 센서
    public TextMeshProUGUI leftPressureSensor;  // 왼쪽 압력 센서
    public TextMeshProUGUI rightDistanceSensor; // 오른쪽 거리 센서
    public TextMeshProUGUI rightPressureSensor; // 오른쪽 압력 센서

    [Header("Danger Thresholds")]
    public float dangerDistanceValue;           // 거리 센서 위험 임계값
    public float dangerPressureValue;           // 압력 센서 위험 임계값
    #endregion

    /// <summary>
    /// 매 프레임마다 센서값을 확인하고 경고 상태 업데이트
    /// </summary>
    void Update()
    {
        // 각 센서 위치별 상태 확인
        WarningDistanceLight(leftDistanceSensor, dangerDistanceValue, RedLightImage[0]);    // 왼쪽 기둥 거리
        WarningLight(leftPressureSensor, dangerPressureValue, RedLightImage[1]);            // 왼쪽 도로 압력
        WarningDistanceLight(rightDistanceSensor, dangerDistanceValue, RedLightImage[2]);   // 오른쪽 기둥 거리
        WarningLight(rightPressureSensor, dangerPressureValue, RedLightImage[3]);           // 오른쪽 도로 압력
    }

    /// <summary>
    /// 거리 센서 값에 따른 경고등 제어
    /// </summary>
    /// <param name="sensorValue">센서값 텍스트</param>
    /// <param name="dangerValue">위험 임계값</param>
    /// <param name="warningLight">제어할 경고등</param>
    private void WarningDistanceLight(TextMeshProUGUI sensorValue, float dangerValue, Image warningLight)
    {
        float sensorFloatValue = float.Parse(sensorValue.text);
        bool isDangerous = sensorFloatValue < dangerValue;  // 거리가 임계값보다 작으면 위험

        warningLight.gameObject.SetActive(isDangerous);

        if (isDangerous && !isBlinking)
        {
            StartCoroutine(BlinkDronCheckText());
        }
    }

    /// <summary>
    /// 압력 센서 값에 따른 경고등 제어
    /// </summary>
    /// <param name="sensorValue">센서값 텍스트</param>
    /// <param name="dangerValue">위험 임계값</param>
    /// <param name="warningLight">제어할 경고등</param>
    private void WarningLight(TextMeshProUGUI sensorValue, float dangerValue, Image warningLight)
    {
        float sensorFloatValue = float.Parse(sensorValue.text);
        bool isDangerous = sensorFloatValue > dangerValue;  // 압력이 임계값보다 크면 위험

        warningLight.gameObject.SetActive(isDangerous);

        if (isDangerous && !isBlinking)
        {
            StartCoroutine(BlinkDronCheckText());
        }
    }

    /// <summary>
    /// 드론 점검 텍스트를 깜빡이는 코루틴
    /// </summary>
    private IEnumerator BlinkDronCheckText()
    {
        isBlinking = true;

        // 4회 깜빡임
        for (int i = 0; i < 4; i++)
        {
            DronCheckText.enabled = !DronCheckText.enabled;
            yield return new WaitForSeconds(1f);
        }

        // 깜빡임 종료 후 상태 초기화
        DronCheckText.enabled = true;
        isBlinking = false;
    }
}