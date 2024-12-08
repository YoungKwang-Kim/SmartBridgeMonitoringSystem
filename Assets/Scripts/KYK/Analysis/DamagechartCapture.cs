using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class DamagechartCapture : MonoBehaviour
{
    public Button captureButton; // UI 버튼을 참조하기 위한 변수
    string savePath = @"C:\Users\user\Desktop\Damaged Chart Image\"; // 스크린샷 저장 경로
    void Start()
    {
        // 버튼에 Click 이벤트에 대한 리스너 추가
        captureButton.onClick.AddListener(CaptureScreenshot);
    }
    void CaptureScreenshot()
    {
        // 디렉토리가 없으면 생성
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        // 스크린샷 촬영
        string screenshotName = "DamagedChartCapture_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string fullPath = Path.Combine(savePath, screenshotName);
        ScreenCapture.CaptureScreenshot(fullPath);
        // 로그 출력
        Debug.Log("Screenshot captured and saved at: " + fullPath);
    }
}