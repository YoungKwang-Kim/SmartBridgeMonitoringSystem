using PolyAndCode.UI;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드론 이미지 저장 정보를 담는 구조체
/// </summary>
public struct DroneImageSaveInfo
{
    public string Number;       // 이미지 번호
    public Texture2D droneImage; // 드론 카메라 이미지
    public int ID;             // 고유 식별자
    public float Distance;      // 균열과의 거리
}

/// <summary>
/// 손상 차트 정보를 담는 구조체
/// </summary>
public struct DamageChartInfo
{
    public Texture2D damageImage; // 손상 이미지
    public int ID;               // 고유 식별자
}

/// <summary>
/// 이미지 저장 및 스크롤뷰 관리 클래스
/// </summary>
public class SaveImageAndScrollView : MonoBehaviour, IRecyclableScrollRectDataSource
{
    #region Components and Settings
    [SerializeField]
    private RecyclableScrollRect _MenualSave;        // 수동 스크린샷 스크롤뷰

    [SerializeField]
    private Camera droneCamera;                      // 드론 카메라

    [SerializeField]
    private RenderTexture droneCameRenderTexture;    // 드론 카메라 렌더 텍스처

    private ScreenShot capture = new ScreenShot();   // 스크린샷 캡처 객체
    private string path = Application.dataPath + "/ScreenShotImages/";  // 저장 경로
    #endregion

    #region Lists
    // 손상 차트 데이터 관리
    public List<DamageChartInfo> _damageChartList { get; set; }
    public List<GameObject> _damageChartScreenLists { get; set; }
    private List<DroneImageSaveInfo> _MenualSaveList = new List<DroneImageSaveInfo>();
    #endregion

    /// <summary>
    /// 초기화 메서드
    /// </summary>
    private void Awake()
    {
        _MenualSave.DataSource = this;
        _damageChartList = new List<DamageChartInfo>();
        _damageChartScreenLists = new List<GameObject>();
    }

    /// <summary>
    /// 수동 스크린샷 버튼 클릭 핸들러
    /// 현재 화면을 캡처하고 스크롤뷰에 추가
    /// </summary>
    public void OnClickMenualScreenshotButton()
    {
        // 이미지 정보 객체 생성
        DroneImageSaveInfo obj = new DroneImageSaveInfo();
        DamageChartInfo damageImage = new DamageChartInfo();
        GameObject newObj = new GameObject();

        // 균열까지의 거리 측정
        float hitDistance = 0;
        if (Physics.Raycast(droneCamera.transform.position, droneCamera.transform.forward, out RaycastHit hit, 3f))
        {
            hitDistance = Vector3.Distance(droneCamera.transform.position, hit.point);
        }

        // 이미지 정보 설정
        obj.Number = _MenualSaveList.Count.ToString();
        obj.droneImage = capture.CaptureScreenshot(path, droneCamera, _MenualSaveList, droneCameRenderTexture);
        obj.Distance = hitDistance;
        damageImage.damageImage = obj.droneImage;
        damageImage.ID = _damageChartList.Count;
        obj.ID = damageImage.ID;

        // 리스트에 추가
        _damageChartList.Add(damageImage);
        _MenualSaveList.Add(obj);
        _damageChartScreenLists.Add(newObj);

        // UI 갱신
        _MenualSave.ReloadData();
    }

    #region IRecyclableScrollRectDataSource Implementation
    /// <summary>
    /// 스크롤뷰 아이템 수 반환
    /// </summary>
    public int GetItemCount()
    {
        return _MenualSaveList.Count;
    }

    /// <summary>
    /// 스크롤뷰 셀 구성
    /// </summary>
    /// <param name="cell">재사용 가능한 셀</param>
    /// <param name="index">셀 인덱스</param>
    public void SetCell(ICell cell, int index)
    {
        var item = cell as SaveCell;
        item.ConfigureCell(_MenualSaveList[index], index);
    }
    #endregion
}