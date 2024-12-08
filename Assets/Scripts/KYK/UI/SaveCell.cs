using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// 드론 이미지 스크롤뷰의 각 셀을 관리하는 클래스
/// </summary>
public class SaveCell : MonoBehaviour, ICell
{
    #region UI Components
    [Header("UI Components")]
    public TextMeshProUGUI numberLabel;    // 셀 번호 텍스트
    public RawImage droneImage;            // 드론 캡처 이미지
    #endregion

    #region Private Variables
    private DroneImageSaveInfo _droneImageSaveInfo;  // 셀의 이미지 정보
    private int _cellIndex;                          // 셀 인덱스

    // 손상 분석 화면 관련
    public GameObject damageChartScreen;             // 손상 분석 화면 프리팹
    private GameObject canvas;                       // UI 캔버스
    private RawImage damageImage;                    // 손상 이미지
    private int damageImageIndex;                    // 손상 이미지 인덱스
    private SaveImageAndScrollView menualScrollView; // 수동 저장 스크롤뷰 참조
    #endregion

    /// <summary>
    /// 드론과 균열 사이의 거리 타입을 정의하는 열거형
    /// </summary>
    public enum DistanceType
    {
        Near,   // 근거리
        Far     // 원거리
    }

    public DistanceType distanceType { get; set; }

    /// <summary>
    /// 컴포넌트 초기화 및 버튼 이벤트 설정
    /// </summary>
    private void Awake()
    {
        menualScrollView = GameObject.FindObjectOfType<SaveImageAndScrollView>();
        canvas = GameObject.Find("Canvas");

        // 셀 클릭 이벤트 설정
        GetComponent<Button>().onClick.AddListener(ButtonListener);
    }

    /// <summary>
    /// 셀의 데이터와 UI를 구성하는 메서드
    /// </summary>
    /// <param name="droneImageSaveInfo">드론 이미지 정보</param>
    /// <param name="cellIndex">셀 인덱스</param>
    public void ConfigureCell(DroneImageSaveInfo droneImageSaveInfo, int cellIndex)
    {
        // 셀 번호는 1부터 시작
        int labelIndex = int.Parse(droneImageSaveInfo.Number) + 1;

        _cellIndex = cellIndex;
        _droneImageSaveInfo = droneImageSaveInfo;

        numberLabel.text = labelIndex.ToString();
        droneImage.texture = droneImageSaveInfo.droneImage;
        damageImageIndex = droneImageSaveInfo.ID;

        // 거리에 따른 타입 설정
        distanceType = droneImageSaveInfo.Distance > 2 ? DistanceType.Far : DistanceType.Near;
    }

    /// <summary>
    /// 셀 클릭 시 손상 분석 화면을 생성하거나 표시하는 메서드
    /// </summary>
    public void ButtonListener()
    {
        Debug.Log(distanceType.ToString());

        // 기존 손상 분석 화면 검색
        string screenName = "damageChartScreen" + damageImageIndex;
        GameObject existingDamageChartScreen = menualScrollView._damageChartScreenLists
            .Find(obj => obj.name == screenName);

        if (existingDamageChartScreen != null)
        {
            // 기존 화면이 있으면 활성화
            existingDamageChartScreen.SetActive(true);
        }
        else
        {
            // 새로운 손상 분석 화면 생성
            GameObject prefabInstance = Instantiate(damageChartScreen, canvas.transform);
            prefabInstance.name = screenName;
            Debug.Log(prefabInstance.name);

            // 손상 이미지 설정
            damageImage = prefabInstance.GetComponentInChildren<RawImage>();
            damageImage.texture = droneImage.texture;

            // 생성된 화면을 리스트에 추가
            menualScrollView._damageChartScreenLists.Add(prefabInstance);
        }
    }
}