using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 드론 카메라로 균열을 감지하고 자동으로 이미지를 저장하는 클래스
/// </summary>
public class AutoSaveImage : MonoBehaviour, IRecyclableScrollRectDataSource
{
    /// <summary>
    /// 카메라 관련 설정을 관리하는 클래스
    /// </summary>
    [System.Serializable]
    public class CameraSettings
    {
        public Camera droneCamera;             // 드론 카메라
        public RenderTexture renderTexture;    // 렌더 텍스처
        public float detectionRange = 3f;      // 균열 감지 거리
        public float captureDelay = 1.3f;      // 캡처 후 대기 시간
        public LayerMask detectionLayer;       // 감지할 레이어
    }

    [Header("UI Components")]
    [SerializeField] private RecyclableScrollRect autoSaveScrollRect;  // 자동 저장 스크롤뷰

    [Header("Camera Settings")]
    [SerializeField] private CameraSettings cameraSettings;           // 카메라 설정

    [Header("Save Settings")]
    [SerializeField] private string saveFolderName = "ScreenShotImages";  // 저장 폴더명

    // 이미지 저장 경로
    private string SavePath => System.IO.Path.Combine(Application.dataPath, saveFolderName);
    private ScreenShot screenCapture;                                // 스크린샷 캡처 객체
    private SaveImageAndScrollView manualScrollView;                 // 수동 저장 스크롤뷰
    private bool isRayEnabled = true;                               // 레이캐스트 활성화 상태
    private List<DroneImageSaveInfo> autoSaveList = new List<DroneImageSaveInfo>();  // 자동 저장된 이미지 목록

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// 참조 설정
    /// </summary>
    private void Start()
    {
        SetupReferences();
    }

    /// <summary>
    /// 균열 감지 및 캡처 실행
    /// </summary>
    private void Update()
    {
        if (isRayEnabled)
        {
            DetectAndCaptureCracks();
        }
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void InitializeComponents()
    {
        screenCapture = new ScreenShot();
        autoSaveScrollRect.DataSource = this;
    }

    /// <summary>
    /// 필요한 참조 설정
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
    /// 균열 감지 및 캡처 처리
    /// </summary>
    private void DetectAndCaptureCracks()
    {
        if (!ValidateCameraSettings()) return;

        Vector3 cameraPosition = cameraSettings.droneCamera.transform.position;
        Vector3 cameraForward = cameraSettings.droneCamera.transform.forward;

        // 디버그용 레이 시각화
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
    /// 카메라 설정 유효성 검사
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
    /// 이미지 캡처 및 저장 처리
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
    /// 이미지 정보 생성
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
    /// 손상 정보 생성
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
    /// 리스트 업데이트
    /// </summary>
    private void UpdateLists(DroneImageSaveInfo imageInfo, DamageChartInfo damageInfo, GameObject newObj)
    {
        autoSaveList.Add(imageInfo);
        manualScrollView._damageChartList.Add(damageInfo);
        manualScrollView._damageChartScreenLists.Add(newObj);
    }

    /// <summary>
    /// UI 새로고침
    /// </summary>
    private void RefreshUI()
    {
        autoSaveScrollRect.ReloadData();
    }

    /// <summary>
    /// 레이 감지 재활성화
    /// </summary>
    private IEnumerator ResetRayDetection(float delay)
    {
        yield return new WaitForSeconds(delay);
        isRayEnabled = true;
    }

    #region IRecyclableScrollRectDataSource Implementation
    /// <summary>
    /// 저장된 아이템 수 반환
    /// </summary>
    public int GetItemCount()
    {
        return autoSaveList.Count;
    }

    /// <summary>
    /// 셀 데이터 설정
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