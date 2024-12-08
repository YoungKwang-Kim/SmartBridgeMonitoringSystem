using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // 버튼 타입 정의
    public enum ButtonType
    {
        Back,
        Exit,
        Switch,
        SignSwitch
    }

    [System.Serializable]
    public class SwitchButtonSettings
    {
        public RectTransform handleRectTransform;
        public GameObject[] objectsToToggleOff;  // 꺼질 오브젝트들
        public GameObject[] objectsToToggleOn;   // 켜질 오브젝트들
        public Color backgroundColorChange = Color.white;
    }

    // Inspector에서 설정할 수 있는 변수들
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private Button button;
    [SerializeField] private SwitchButtonSettings switchSettings;

    // 스위치 관련 변수들
    private Toggle toggle;
    private Image backImage;
    private Color defaultBackgroundColor;
    private Vector2 handlePosition;

    private void Awake()
    {
        InitializeButton();
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleButtonClick);
        }
    }

    private void InitializeButton()
    {
        switch (buttonType)
        {
            case ButtonType.Switch:
            case ButtonType.SignSwitch:
                InitializeSwitchButton();
                break;
        }
    }

    private void InitializeSwitchButton()
    {
        if (switchSettings.handleRectTransform != null)
        {
            toggle = GetComponent<Toggle>();
            handlePosition = switchSettings.handleRectTransform.anchoredPosition;
            backImage = switchSettings.handleRectTransform.parent.GetComponent<Image>();
            defaultBackgroundColor = backImage.color;

            // 초기 상태 설정
            SetObjectsActive(false);

            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(OnSwitch);
                if (toggle.isOn)
                {
                    OnSwitch(true);
                }
            }
        }
    }

    private void HandleButtonClick()
    {
        switch (buttonType)
        {
            case ButtonType.Back:
                SceneManager.LoadScene("DashBoard Scene_Juheee");
                break;
            case ButtonType.Exit:
                Application.Quit();
                break;
        }
    }

    private void OnSwitch(bool on)
    {
        if (switchSettings.handleRectTransform != null)
        {
            // 핸들 위치 조정
            switchSettings.handleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;

            // 배경색 변경
            if (backImage != null)
            {
                backImage.color = on ? switchSettings.backgroundColorChange : defaultBackgroundColor;
            }

            // 오브젝트 활성화/비활성화
            SetObjectsActive(on);
        }
    }

    private void SetObjectsActive(bool on)
    {
        if (switchSettings.objectsToToggleOff != null)
        {
            foreach (var obj in switchSettings.objectsToToggleOff)
            {
                if (obj != null)
                    obj.SetActive(!on);
            }
        }

        if (switchSettings.objectsToToggleOn != null)
        {
            foreach (var obj in switchSettings.objectsToToggleOn)
            {
                if (obj != null)
                    obj.SetActive(on);
            }
        }
    }
}