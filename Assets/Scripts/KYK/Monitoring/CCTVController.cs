using UnityEngine;
using UnityEngine.UI;
using System;

public class CCTVController : MonoBehaviour
{
    [System.Serializable]
    public class CCTVSettings
    {
        public float turnSpeed = 20f;
        public Vector2 rotationLimits = new Vector2(-80f, 80f); // X�� ȸ�� ����
        public bool useGameObject = true; // true: SetActive ���, false: enabled ���
    }

    [System.Serializable]
    public class CCTVComponents
    {
        public Button toggleButton;
        public RawImage displayImage;
        [Tooltip("CCTV ī�޶� ���� ������ ǥ���� UI ��� (���û���)")]
        public RectTransform directionIndicator;
    }

    [Header("CCTV Settings")]
    [SerializeField] private CCTVSettings settings = new CCTVSettings();

    [Header("UI Components")]
    [SerializeField] private CCTVComponents components;

    private bool isEnabled;
    public bool IsEnabled => isEnabled;

    // CCTV ���� ���� �̺�Ʈ
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
            Debug.LogError("�ʿ��� UI ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        // ��ư �̺�Ʈ ����
        components.toggleButton.onClick.AddListener(ToggleCCTV);

        // �ʱ� ���� ����
        SetCCTVState(false);
    }

    private void HandleCameraRotation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput == 0 && verticalInput == 0) return;

        // ���� ȸ���� ��������
        Vector3 currentRotation = transform.rotation.eulerAngles;

        // ���� ȸ��
        currentRotation.y += horizontalInput * settings.turnSpeed * Time.deltaTime;

        // ���� ȸ�� (���� ����)
        float newXRotation = currentRotation.x - (verticalInput * settings.turnSpeed * Time.deltaTime);
        if (newXRotation > 180) newXRotation -= 360;
        newXRotation = Mathf.Clamp(newXRotation, settings.rotationLimits.x, settings.rotationLimits.y);
        currentRotation.x = newXRotation;

        // ȸ�� ����
        transform.rotation = Quaternion.Euler(currentRotation);

        // ���� ǥ�ñ� ������Ʈ
        UpdateDirectionIndicator();
    }

    private void UpdateDirectionIndicator()
    {
        if (components.directionIndicator != null)
        {
            // CCTV�� ȸ���� UI�� �ݿ�
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

        // UI ���� ������Ʈ
        if (settings.useGameObject)
        {
            components.displayImage.gameObject.SetActive(state);
        }
        else
        {
            components.displayImage.enabled = state;
        }

        // �̺�Ʈ �߻�
        OnCCTVStateChanged?.Invoke(state);
    }

    // CCTV ���� ������Ʈ�� ���� public �޼���
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
        // �ν����Ϳ��� ���� ����� �� ��ȿ�� �˻�
        settings.turnSpeed = Mathf.Max(0, settings.turnSpeed);
        settings.rotationLimits.x = Mathf.Clamp(settings.rotationLimits.x, -90f, 90f);
        settings.rotationLimits.y = Mathf.Clamp(settings.rotationLimits.y, -90f, 90f);
    }
}