using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class DamagechartCapture : MonoBehaviour
{
    public Button captureButton; // UI ��ư�� �����ϱ� ���� ����
    string savePath = @"C:\Users\user\Desktop\Damaged Chart Image\"; // ��ũ���� ���� ���
    void Start()
    {
        // ��ư�� Click �̺�Ʈ�� ���� ������ �߰�
        captureButton.onClick.AddListener(CaptureScreenshot);
    }
    void CaptureScreenshot()
    {
        // ���丮�� ������ ����
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        // ��ũ���� �Կ�
        string screenshotName = "DamagedChartCapture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(savePath, screenshotName);
        ScreenCapture.CaptureScreenshot(fullPath);
        // �α� ���
        Debug.Log("Screenshot captured and saved at: " + fullPath);
    }
}