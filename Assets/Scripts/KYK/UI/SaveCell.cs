using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// ��� �̹��� ��ũ�Ѻ��� �� ���� �����ϴ� Ŭ����
/// </summary>
public class SaveCell : MonoBehaviour, ICell
{
    #region UI Components
    [Header("UI Components")]
    public TextMeshProUGUI numberLabel;    // �� ��ȣ �ؽ�Ʈ
    public RawImage droneImage;            // ��� ĸó �̹���
    #endregion

    #region Private Variables
    private DroneImageSaveInfo _droneImageSaveInfo;  // ���� �̹��� ����
    private int _cellIndex;                          // �� �ε���

    // �ջ� �м� ȭ�� ����
    public GameObject damageChartScreen;             // �ջ� �м� ȭ�� ������
    private GameObject canvas;                       // UI ĵ����
    private RawImage damageImage;                    // �ջ� �̹���
    private int damageImageIndex;                    // �ջ� �̹��� �ε���
    private SaveImageAndScrollView menualScrollView; // ���� ���� ��ũ�Ѻ� ����
    #endregion

    /// <summary>
    /// ��а� �տ� ������ �Ÿ� Ÿ���� �����ϴ� ������
    /// </summary>
    public enum DistanceType
    {
        Near,   // �ٰŸ�
        Far     // ���Ÿ�
    }

    public DistanceType distanceType { get; set; }

    /// <summary>
    /// ������Ʈ �ʱ�ȭ �� ��ư �̺�Ʈ ����
    /// </summary>
    private void Awake()
    {
        menualScrollView = GameObject.FindObjectOfType<SaveImageAndScrollView>();
        canvas = GameObject.Find("Canvas");

        // �� Ŭ�� �̺�Ʈ ����
        GetComponent<Button>().onClick.AddListener(ButtonListener);
    }

    /// <summary>
    /// ���� �����Ϳ� UI�� �����ϴ� �޼���
    /// </summary>
    /// <param name="droneImageSaveInfo">��� �̹��� ����</param>
    /// <param name="cellIndex">�� �ε���</param>
    public void ConfigureCell(DroneImageSaveInfo droneImageSaveInfo, int cellIndex)
    {
        // �� ��ȣ�� 1���� ����
        int labelIndex = int.Parse(droneImageSaveInfo.Number) + 1;

        _cellIndex = cellIndex;
        _droneImageSaveInfo = droneImageSaveInfo;

        numberLabel.text = labelIndex.ToString();
        droneImage.texture = droneImageSaveInfo.droneImage;
        damageImageIndex = droneImageSaveInfo.ID;

        // �Ÿ��� ���� Ÿ�� ����
        distanceType = droneImageSaveInfo.Distance > 2 ? DistanceType.Far : DistanceType.Near;
    }

    /// <summary>
    /// �� Ŭ�� �� �ջ� �м� ȭ���� �����ϰų� ǥ���ϴ� �޼���
    /// </summary>
    public void ButtonListener()
    {
        Debug.Log(distanceType.ToString());

        // ���� �ջ� �м� ȭ�� �˻�
        string screenName = "damageChartScreen" + damageImageIndex;
        GameObject existingDamageChartScreen = menualScrollView._damageChartScreenLists
            .Find(obj => obj.name == screenName);

        if (existingDamageChartScreen != null)
        {
            // ���� ȭ���� ������ Ȱ��ȭ
            existingDamageChartScreen.SetActive(true);
        }
        else
        {
            // ���ο� �ջ� �м� ȭ�� ����
            GameObject prefabInstance = Instantiate(damageChartScreen, canvas.transform);
            prefabInstance.name = screenName;
            Debug.Log(prefabInstance.name);

            // �ջ� �̹��� ����
            damageImage = prefabInstance.GetComponentInChildren<RawImage>();
            damageImage.texture = droneImage.texture;

            // ������ ȭ���� ����Ʈ�� �߰�
            menualScrollView._damageChartScreenLists.Add(prefabInstance);
        }
    }
}