using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class ScreenShot : MonoBehaviour
{
    [System.Serializable]
    public class ScreenShotSettings
    {
        public string baseDirectoryPath = "Screenshots/";
        public int imageQuality = 24;
        public TextureFormat textureFormat = TextureFormat.RGB24;
    }

    [SerializeField] private ScreenShotSettings settings = new ScreenShotSettings();

    /// <summary>
    /// 스크린샷을 캡처하고 저장합니다.
    /// </summary>
    /// <param name="path">저장 경로</param>
    /// <param name="camera">캡처할 카메라</param>
    /// <param name="imageList">이미지 정보 리스트</param>
    /// <param name="originalRenderTexture">원본 렌더 텍스처</param>
    /// <returns>캡처된 텍스처</returns>
    public Texture2D CaptureScreenshot(string path, Camera camera, List<DroneImageSaveInfo> imageList, RenderTexture originalRenderTexture)
    {
        if (camera == null)
        {
            Debug.LogError("Camera is null!");
            return null;
        }

        Texture2D screenshot = null;
        RenderTexture temporaryRT = null;

        try
        {
            screenshot = CreateScreenshot(camera, out temporaryRT);
            SaveScreenshot(path, screenshot, imageList.Count.ToString());
            return screenshot;
        }
        catch (Exception e)
        {
            Debug.LogError($"Screenshot failed: {e.Message}");
            return null;
        }
        finally
        {
            // 리소스 정리
            CleanupResources(camera, originalRenderTexture, temporaryRT);
        }
    }

    /// <summary>
    /// 스크린샷 텍스처를 생성합니다.
    /// </summary>
    private Texture2D CreateScreenshot(Camera camera, out RenderTexture temporaryRT)
    {
        int width = Screen.width;
        int height = Screen.height;

        // 임시 렌더 텍스처 생성
        temporaryRT = new RenderTexture(width, height, settings.imageQuality);
        camera.targetTexture = temporaryRT;

        // 스크린샷 텍스처 생성
        Texture2D screenshot = new Texture2D(width, height, settings.textureFormat, false);

        // 카메라 렌더링
        camera.Render();
        RenderTexture.active = temporaryRT;

        // 픽셀 데이터 읽기
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        return screenshot;
    }

    /// <summary>
    /// 스크린샷을 파일로 저장합니다.
    /// </summary>
    private void SaveScreenshot(string path, Texture2D screenshot, string fileNumber)
    {
        try
        {
            string fullPath = CreateScreenshotDirectory(path);
            string fileName = GenerateFileName(fullPath, fileNumber);

            byte[] imageData = screenshot.EncodeToPNG();
            File.WriteAllBytes(fileName, imageData);

            Debug.Log($"Screenshot saved: {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save screenshot: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// 스크린샷 저장 디렉토리를 생성합니다.
    /// </summary>
    private string CreateScreenshotDirectory(string basePath)
    {
        string fullPath = Path.Combine(Application.dataPath, basePath);

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        return fullPath;
    }

    /// <summary>
    /// 파일 이름을 생성합니다.
    /// </summary>
    private string GenerateFileName(string path, string fileNumber)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(path, $"{timestamp}_{fileNumber}.png");
    }

    /// <summary>
    /// 사용한 리소스를 정리합니다.
    /// </summary>
    private void CleanupResources(Camera camera, RenderTexture originalRT, RenderTexture temporaryRT)
    {
        if (camera != null)
        {
            camera.targetTexture = originalRT;
        }

        if (temporaryRT != null)
        {
            temporaryRT.Release();
            Destroy(temporaryRT);
        }

        RenderTexture.active = null;
    }
}