using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // ��ư Ÿ�� ����
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
        public GameObject[] objectsToToggleOff;  // ���� ������Ʈ��
        public GameObject[] objectsToToggleOn;   // ���� ������Ʈ��
        public Color backgroundColorChange = Color.white;
    }

    // Inspector���� ������ �� �ִ� ������
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private Button button;
    [SerializeField] private SwitchButtonSettings switchSettings;

    // ����ġ ���� ������
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

            // �ʱ� ���� ����
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
            // �ڵ� ��ġ ����
            switchSettings.handleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;

            // ���� ����
            if (backImage != null)
            {
                backImage.color = on ? switchSettings.backgroundColorChange : defaultBackgroundColor;
            }

            // ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
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