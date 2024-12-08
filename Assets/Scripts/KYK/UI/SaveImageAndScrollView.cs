using PolyAndCode.UI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� �̹��� ���� ������ ��� ����ü
/// </summary>
public struct DroneImageSaveInfo
{
    public string Number;       // �̹��� ��ȣ
    public Texture2D droneImage; // ��� ī�޶� �̹���
    public int ID;             // ���� �ĺ���
    public float Distance;      // �տ����� �Ÿ�
}

/// <summary>
/// �ջ� ��Ʈ ������ ��� ����ü
/// </summary>
public struct DamageChartInfo
{
    public Texture2D damageImage; // �ջ� �̹���
    public int ID;               // ���� �ĺ���
}

/// <summary>
/// �̹��� ���� �� ��ũ�Ѻ� ���� Ŭ����
/// </summary>
public class SaveImageAndScrollView : MonoBehaviour, IRecyclableScrollRectDataSource
{
    #region Components and Settings
    [SerializeField]
    private RecyclableScrollRect _MenualSave;        // ���� ��ũ���� ��ũ�Ѻ�

    [SerializeField]
    private Camera droneCamera;                      // ��� ī�޶�

    [SerializeField]
    private RenderTexture droneCameRenderTexture;    // ��� ī�޶� ���� �ؽ�ó

    private ScreenShot capture = new ScreenShot();   // ��ũ���� ĸó ��ü
    private string path = Application.dataPath + "/ScreenShotImages/";  // ���� ���
    #endregion

    #region Lists
    // �ջ� ��Ʈ ������ ����
    public List<DamageChartInfo> _damageChartList { get; set; }
    public List<GameObject> _damageChartScreenLists { get; set; }
    private List<DroneImageSaveInfo> _MenualSaveList = new List<DroneImageSaveInfo>();
    #endregion

    /// <summary>
    /// �ʱ�ȭ �޼���
    /// </summary>
    private void Awake()
    {
        _MenualSave.DataSource = this;
        _damageChartList = new List<DamageChartInfo>();
        _damageChartScreenLists = new List<GameObject>();
    }

    /// <summary>
    /// ���� ��ũ���� ��ư Ŭ�� �ڵ鷯
    /// ���� ȭ���� ĸó�ϰ� ��ũ�Ѻ信 �߰�
    /// </summary>
    public void OnClickMenualScreenshotButton()
    {
        // �̹��� ���� ��ü ����
        DroneImageSaveInfo obj = new DroneImageSaveInfo();
        DamageChartInfo damageImage = new DamageChartInfo();
        GameObject newObj = new GameObject();

        // �տ������� �Ÿ� ����
        float hitDistance = 0;
        if (Physics.Raycast(droneCamera.transform.position, droneCamera.transform.forward, out RaycastHit hit, 3f))
        {
            hitDistance = Vector3.Distance(droneCamera.transform.position, hit.point);
        }

        // �̹��� ���� ����
        obj.Number = _MenualSaveList.Count.ToString();
        obj.droneImage = capture.CaptureScreenshot(path, droneCamera, _MenualSaveList, droneCameRenderTexture);
        obj.Distance = hitDistance;
        damageImage.damageImage = obj.droneImage;
        damageImage.ID = _damageChartList.Count;
        obj.ID = damageImage.ID;

        // ����Ʈ�� �߰�
        _damageChartList.Add(damageImage);
        _MenualSaveList.Add(obj);
        _damageChartScreenLists.Add(newObj);

        // UI ����
        _MenualSave.ReloadData();
    }

    #region IRecyclableScrollRectDataSource Implementation
    /// <summary>
    /// ��ũ�Ѻ� ������ �� ��ȯ
    /// </summary>
    public int GetItemCount()
    {
        return _MenualSaveList.Count;
    }

    /// <summary>
    /// ��ũ�Ѻ� �� ����
    /// </summary>
    /// <param name="cell">���� ������ ��</param>
    /// <param name="index">�� �ε���</param>
    public void SetCell(ICell cell, int index)
    {
        var item = cell as SaveCell;
        item.ConfigureCell(_MenualSaveList[index], index);
    }
    #endregion
}