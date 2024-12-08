using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ��� ī�޶�� �տ��� �����ϰ� �ڵ����� �̹����� �����ϴ� Ŭ����
/// </summary>
public class AutoSaveImage : MonoBehaviour, IRecyclableScrollRectDataSource
{
    /// <summary>
    /// ī�޶� ���� ������ �����ϴ� Ŭ����
    /// </summary>
    [System.Serializable]
    public class CameraSettings
    {
        public Camera droneCamera;             // ��� ī�޶�
        public RenderTexture renderTexture;    // ���� �ؽ�ó
        public float detectionRange = 3f;      // �տ� ���� �Ÿ�
        public float captureDelay = 1.3f;      // ĸó �� ��� �ð�
        public LayerMask detectionLayer;       // ������ ���̾�
    }

    [Header("UI Components")]
    [SerializeField] private RecyclableScrollRect autoSaveScrollRect;  // �ڵ� ���� ��ũ�Ѻ�

    [Header("Camera Settings")]
    [SerializeField] private CameraSettings cameraSettings;           // ī�޶� ����

    [Header("Save Settings")]
    [SerializeField] private string saveFolderName = "ScreenShotImages";  // ���� ������

    // �̹��� ���� ���
    private string SavePath => System.IO.Path.Combine(Application.dataPath, saveFolderName);
    private ScreenShot screenCapture;                                // ��ũ���� ĸó ��ü
    private SaveImageAndScrollView manualScrollView;                 // ���� ���� ��ũ�Ѻ�
    private bool isRayEnabled = true;                               // ����ĳ��Ʈ Ȱ��ȭ ����
    private List<DroneImageSaveInfo> autoSaveList = new List<DroneImageSaveInfo>();  // �ڵ� ����� �̹��� ���

    /// <summary>
    /// ������Ʈ �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    private void Start()
    {
        SetupReferences();
    }

    /// <summary>
    /// �տ� ���� �� ĸó ����
    /// </summary>
    private void Update()
    {
        if (isRayEnabled)
        {
            DetectAndCaptureCracks();
        }
    }

    /// <summary>
    /// ������Ʈ �ʱ�ȭ
    /// </summary>
    private void InitializeComponents()
    {
        screenCapture = new ScreenShot();
        autoSaveScrollRect.DataSource = this;
    }

    /// <summary>
    /// �ʿ��� ���� ����
    /// </summary>
    private void SetupReferences()
    {
        manualScrollView = FindObjectOfType<SaveImageAndScrollView>();
        if (manualScrollView == null)
        {
            Debug.LogWarning("SaveImageAndScrollView not found in scene!");
        }
    }

    /// <summary>
    /// �տ� ���� �� ĸó ó��
    /// </summary>
    private void DetectAndCaptureCracks()
    {
        if (!ValidateCameraSettings()) return;

        Vector3 cameraPosition = cameraSettings.droneCamera.transform.position;
        Vector3 cameraForward = cameraSettings.droneCamera.transform.forward;

        // ����׿� ���� �ð�ȭ
        Debug.DrawRay(cameraPosition, cameraForward * cameraSettings.detectionRange, Color.red);

        if (Physics.Raycast(cameraPosition, cameraForward, out RaycastHit hit,
            cameraSettings.detectionRange, cameraSettings.detectionLayer))
        {
            if (hit.collider.CompareTag("Crack"))
            {
                CaptureAndSaveImage(hit);
                StartCoroutine(ResetRayDetection(cameraSettings.captureDelay));
            }
        }
    }

    /// <summary>
    /// ī�޶� ���� ��ȿ�� �˻�
    /// </summary>
    private bool ValidateCameraSettings()
    {
        if (cameraSettings.droneCamera == null)
        {
            Debug.LogError("Drone camera not assigned!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// �̹��� ĸó �� ���� ó��
    /// </summary>
    private void CaptureAndSaveImage(RaycastHit hit)
    {
        var imageInfo = CreateImageInfo(hit);
        var damageInfo = CreateDamageInfo(imageInfo);
        var newGameObject = new GameObject($"DamageScreen_{imageInfo.ID}");

        UpdateLists(imageInfo, damageInfo, newGameObject);
        RefreshUI();

        isRayEnabled = false;
    }

    /// <summary>
    /// �̹��� ���� ����
    /// </summary>
    private DroneImageSaveInfo CreateImageInfo(RaycastHit hit)
    {
        return new DroneImageSaveInfo
        {
            Number = autoSaveList.Count.ToString(),
            droneImage = screenCapture.CaptureScreenshot(
                SavePath,
                cameraSettings.droneCamera,
                autoSaveList,
                cameraSettings.renderTexture
            ),
            Distance = Vector3.Distance(cameraSettings.droneCamera.transform.position, hit.point),
            ID = manualScrollView._damageChartList.Count
        };
    }

    /// <summary>
    /// �ջ� ���� ����
    /// </summary>
    private DamageChartInfo CreateDamageInfo(DroneImageSaveInfo imageInfo)
    {
        return new DamageChartInfo
        {
            damageImage = imageInfo.droneImage,
            ID = imageInfo.ID
        };
    }

    /// <summary>
    /// ����Ʈ ������Ʈ
    /// </summary>
    private void UpdateLists(DroneImageSaveInfo imageInfo, DamageChartInfo damageInfo, GameObject newObj)
    {
        autoSaveList.Add(imageInfo);
        manualScrollView._damageChartList.Add(damageInfo);
        manualScrollView._damageChartScreenLists.Add(newObj);
    }

    /// <summary>
    /// UI ���ΰ�ħ
    /// </summary>
    private void RefreshUI()
    {
        autoSaveScrollRect.ReloadData();
    }

    /// <summary>
    /// ���� ���� ��Ȱ��ȭ
    /// </summary>
    private IEnumerator ResetRayDetection(float delay)
    {
        yield return new WaitForSeconds(delay);
        isRayEnabled = true;
    }

    #region IRecyclableScrollRectDataSource Implementation
    /// <summary>
    /// ����� ������ �� ��ȯ
    /// </summary>
    public int GetItemCount()
    {
        return autoSaveList.Count;
    }

    /// <summary>
    /// �� ������ ����
    /// </summary>
    public void SetCell(ICell cell, int index)
    {
        if (cell is SaveCell saveCell)
        {
            saveCell.ConfigureCell(autoSaveList[index], index);
        }
    }
    #endregion
}