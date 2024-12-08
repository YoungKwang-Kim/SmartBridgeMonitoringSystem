using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ������ ���¸� ��ȣ������ ǥ���ϴ� Ŭ����
/// </summary>
public class BridgeLight : MonoBehaviour
{
    #region UI Components
    [Header("�ٸ� ��ȣ��(������ 1.���ʱ��, 2.���ʵ���, 3.�����ʱ��, 4.�����ʵ���)")]
    /// <summary>
    /// ���� ���¸� ��Ÿ���� ����� ���
    /// </summary>
    public List<Image> GreenLight;

    /// <summary>
    /// ���� ���¸� ��Ÿ���� Ȳ���� ���
    /// </summary>
    public List<Image> YellowLight;

    /// <summary>
    /// ���� ���¸� ��Ÿ���� ������ ���
    /// </summary>
    public List<Image> RedLight;
    #endregion

    #region Sensor Components
    [Header("�Ƶ��̳� ������")]
    [Tooltip("�ٸ� ������ �Ÿ�����")]
    public TextMeshProUGUI leftDistanceSensor;

    [Tooltip("�ٸ� ������ �з¼���")]
    public TextMeshProUGUI leftPressureSensor;

    [Tooltip("�ٸ� �������� �Ÿ�����")]
    public TextMeshProUGUI rightDistanceSensor;

    [Tooltip("�ٸ� �������� �з¼���")]
    public TextMeshProUGUI rightPressureSensor;
    #endregion

    #region Threshold Values
    [Header("�����ġ��")]
    public float yellowDistanceValue;  // �Ÿ� ���� ���� �Ӱ谪
    public float yellowPressureValue;  // �з� ���� ���� �Ӱ谪
    public float redDistanceValue;     // �Ÿ� ���� ���� �Ӱ谪
    public float redPressureValue;     // �з� ���� ���� �Ӱ谪
    #endregion

    /// <summary>
    /// �ʱ�ȭ - Ȳ����� �������� ��Ȱ��ȭ
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
    /// �� �����Ӹ��� ��ȣ�� ���� ������Ʈ
    /// </summary>
    void Update()
    {
        RedLightState();
        YellowLightState();
    }

    /// <summary>
    /// ���� �ܰ�(Ȳ����) ���¸� ó���ϴ� �޼���
    /// �� �������� ���� �Ӱ谪�� ������ �ش� ��ġ�� Ȳ������ Ȱ��ȭ
    /// </summary>
    private void YellowLightState()
    {
        // ���� ������ �Ľ�
        float leftDistanceValue = float.Parse(leftDistanceSensor.text);
        float rightDistanceValue = float.Parse(rightDistanceSensor.text);
        float leftPressureValue = float.Parse(leftPressureSensor.text);
        float rightPressureValue = float.Parse(rightPressureSensor.text);

        // ���� ��� ���� Ȯ��
        YellowLight[0].enabled = yellowPressureValue < leftPressureValue;

        // ���� ���� ���� Ȯ��
        YellowLight[1].enabled = yellowDistanceValue > leftDistanceValue;

        // ������ ��� ���� Ȯ��
        YellowLight[2].enabled = yellowPressureValue < rightPressureValue;

        // ������ ���� ���� Ȯ��
        YellowLight[3].enabled = yellowDistanceValue > rightDistanceValue;
    }

    /// <summary>
    /// ���� �ܰ�(������) ���¸� ó���ϴ� �޼���
    /// �� �������� ���� �Ӱ谪�� ������ �ش� ��ġ�� �������� Ȱ��ȭ
    /// </summary>
    private void RedLightState()
    {
        // ���� ������ �Ľ�
        float leftDistanceValue = float.Parse(leftDistanceSensor.text);
        float rightDistanceValue = float.Parse(rightDistanceSensor.text);
        float leftPressureValue = float.Parse(leftPressureSensor.text);
        float rightPressureValue = float.Parse(rightPressureSensor.text);

        // ���� ��� ���� Ȯ��
        RedLight[0].enabled = redPressureValue < leftPressureValue;

        // ���� ���� ���� Ȯ��
        RedLight[1].enabled = redDistanceValue > leftDistanceValue;

        // ������ ��� ���� Ȯ��
        RedLight[2].enabled = redPressureValue < rightPressureValue;

        // ������ ���� ���� Ȯ��
        RedLight[3].enabled = redDistanceValue > rightDistanceValue;
    }
}