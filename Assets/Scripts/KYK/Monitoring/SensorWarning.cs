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
    public Text DronCheckText;                  // ��� ���� �˸� �ؽ�Ʈ
    private bool isBlinking;                    // �ؽ�Ʈ ������ ����

    [Header("�ٸ� ��ȣ��(������ 1.���ʱ��, 2.���ʵ���, 3.�����ʱ��, 4.�����ʵ���)")]
    public List<Image> RedLightImage;          // ���� ���� ǥ�õ� ���
    #endregion

    #region Sensor Components
    [Header("Sensors")]
    public TextMeshProUGUI leftDistanceSensor;  // ���� �Ÿ� ����
    public TextMeshProUGUI leftPressureSensor;  // ���� �з� ����
    public TextMeshProUGUI rightDistanceSensor; // ������ �Ÿ� ����
    public TextMeshProUGUI rightPressureSensor; // ������ �з� ����

    [Header("Danger Thresholds")]
    public float dangerDistanceValue;           // �Ÿ� ���� ���� �Ӱ谪
    public float dangerPressureValue;           // �з� ���� ���� �Ӱ谪
    #endregion

    /// <summary>
    /// �� �����Ӹ��� �������� Ȯ���ϰ� ��� ���� ������Ʈ
    /// </summary>
    void Update()
    {
        // �� ���� ��ġ�� ���� Ȯ��
        WarningDistanceLight(leftDistanceSensor, dangerDistanceValue, RedLightImage[0]);    // ���� ��� �Ÿ�
        WarningLight(leftPressureSensor, dangerPressureValue, RedLightImage[1]);            // ���� ���� �з�
        WarningDistanceLight(rightDistanceSensor, dangerDistanceValue, RedLightImage[2]);   // ������ ��� �Ÿ�
        WarningLight(rightPressureSensor, dangerPressureValue, RedLightImage[3]);           // ������ ���� �з�
    }

    /// <summary>
    /// �Ÿ� ���� ���� ���� ���� ����
    /// </summary>
    /// <param name="sensorValue">������ �ؽ�Ʈ</param>
    /// <param name="dangerValue">���� �Ӱ谪</param>
    /// <param name="warningLight">������ ����</param>
    private void WarningDistanceLight(TextMeshProUGUI sensorValue, float dangerValue, Image warningLight)
    {
        float sensorFloatValue = float.Parse(sensorValue.text);
        bool isDangerous = sensorFloatValue < dangerValue;  // �Ÿ��� �Ӱ谪���� ������ ����

        warningLight.gameObject.SetActive(isDangerous);

        if (isDangerous && !isBlinking)
        {
            StartCoroutine(BlinkDronCheckText());
        }
    }

    /// <summary>
    /// �з� ���� ���� ���� ���� ����
    /// </summary>
    /// <param name="sensorValue">������ �ؽ�Ʈ</param>
    /// <param name="dangerValue">���� �Ӱ谪</param>
    /// <param name="warningLight">������ ����</param>
    private void WarningLight(TextMeshProUGUI sensorValue, float dangerValue, Image warningLight)
    {
        float sensorFloatValue = float.Parse(sensorValue.text);
        bool isDangerous = sensorFloatValue > dangerValue;  // �з��� �Ӱ谪���� ũ�� ����

        warningLight.gameObject.SetActive(isDangerous);

        if (isDangerous && !isBlinking)
        {
            StartCoroutine(BlinkDronCheckText());
        }
    }

    /// <summary>
    /// ��� ���� �ؽ�Ʈ�� �����̴� �ڷ�ƾ
    /// </summary>
    private IEnumerator BlinkDronCheckText()
    {
        isBlinking = true;

        // 4ȸ ������
        for (int i = 0; i < 4; i++)
        {
            DronCheckText.enabled = !DronCheckText.enabled;
            yield return new WaitForSeconds(1f);
        }

        // ������ ���� �� ���� �ʱ�ȭ
        DronCheckText.enabled = true;
        isBlinking = false;
    }
}