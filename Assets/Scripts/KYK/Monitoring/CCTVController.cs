using UnityEngine;
using UnityEngine.UI;
using System;

public class CCTVController : MonoBehaviour
{
    [System.Serializable]
    public class CCTVSettings
    {
        public float turnSpeed = 20f;
        public Vector2 rotationLimits = new Vector2(-80f, 80f); // X축 회전 제한
        public bool useGameObject = true; // true: SetActive 사용, false: enabled 사용
    }

    [System.Serializable]
    public class CCTVComponents
    {
        public Button toggleButton;
        public RawImage displayImage;
        [Tooltip("CCTV 카메라가 보는 방향을 표시할 UI 요소 (선택사항)")]
        public RectTransform directionIndicator;
    }

    [Header("CCTV Settings")]
    [SerializeField] private CCTVSettings settings = new CCTVSettings();

    [Header("UI Components")]
    [SerializeField] private CCTVComponents components;

    private bool isEnabled;
    public bool IsEnabled => isEnabled;

    // CCTV 상태 변경 이벤트
    public event Action<bool> OnCCTVStateChanged;

    private void Start()
    {
        InitializeCCTV();
    }

    private void Update()
    {
        if (isEnabled)
        {
            HandleCameraRotation();
        }
    }

    private void InitializeCCTV()
    {
        if (components.toggleButton == null || components.displayImage == null)
        {
            Debug.LogError("필요한 UI 컴포넌트가 할당되지 않았습니다!");
            return;
        }

        // 버튼 이벤트 설정
        components.toggleButton.onClick.AddListener(ToggleCCTV);

        // 초기 상태 설정
        SetCCTVState(false);
    }

    private void HandleCameraRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0 && verticalInput == 0) return;

        // 현재 회전값 가져오기
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // 수평 회전
        currentRotation.y += horizontalInput * settings.turnSpeed * Time.deltaTime;

        // 수직 회전 (제한 적용)
        float newXRotation = currentRotation.x - (verticalInput * settings.turnSpeed * Time.deltaTime);
        if (newXRotation > 180) newXRotation -= 360;
        newXRotation = Mathf.Clamp(newXRotation, settings.rotationLimits.x, settings.rotationLimits.y);
        currentRotation.x = newXRotation;

        // 회전 적용
        transform.rotation = Quaternion.Euler(currentRotation);

        // 방향 표시기 업데이트
        UpdateDirectionIndicator();
    }

    private void UpdateDirectionIndicator()
    {
        if (components.directionIndicator != null)
        {
            // CCTV의 회전을 UI에 반영
            components.directionIndicator.rotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.y);
        }
    }

    public void ToggleCCTV()
    {
        SetCCTVState(!isEnabled);
    }

    public void SetCCTVState(bool state)
    {
        isEnabled = state;

        // UI 상태 업데이트
        if (settings.useGameObject)
        {
            components.displayImage.gameObject.SetActive(state);
        }
        else
        {
            components.displayImage.enabled = state;
        }

        // 이벤트 발생
        OnCCTVStateChanged?.Invoke(state);
    }

    // CCTV 설정 업데이트를 위한 public 메서드
    public void UpdateTurnSpeed(float newSpeed)
    {
        settings.turnSpeed = Mathf.Max(0, newSpeed);
    }

    public void UpdateRotationLimits(float minAngle, float maxAngle)
    {
        settings.rotationLimits = new Vector2(minAngle, maxAngle);
    }

    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때 유효성 검사
        settings.turnSpeed = Mathf.Max(0, settings.turnSpeed);
        settings.rotationLimits.x = Mathf.Clamp(settings.rotationLimits.x, -90f, 90f);
        settings.rotationLimits.y = Mathf.Clamp(settings.rotationLimits.y, -90f, 90f);
    }
}