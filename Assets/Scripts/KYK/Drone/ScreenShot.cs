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
    /// ��ũ������ ĸó�ϰ� �����մϴ�.
    /// </summary>
    /// <param name="path">���� ���</param>
    /// <param name="camera">ĸó�� ī�޶�</param>
    /// <param name="imageList">�̹��� ���� ����Ʈ</param>
    /// <param name="originalRenderTexture">���� ���� �ؽ�ó</param>
    /// <returns>ĸó�� �ؽ�ó</returns>
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
            // ���ҽ� ����
            CleanupResources(camera, originalRenderTexture, temporaryRT);
        }
    }

    /// <summary>
    /// ��ũ���� �ؽ�ó�� �����մϴ�.
    /// </summary>
    private Texture2D CreateScreenshot(Camera camera, out RenderTexture temporaryRT)
    {
        int width = Screen.width;
        int height = Screen.height;

        // �ӽ� ���� �ؽ�ó ����
        temporaryRT = new RenderTexture(width, height, settings.imageQuality);
        camera.targetTexture = temporaryRT;

        // ��ũ���� �ؽ�ó ����
        Texture2D screenshot = new Texture2D(width, height, settings.textureFormat, false);

        // ī�޶� ������
        camera.Render();
        RenderTexture.active = temporaryRT;

        // �ȼ� ������ �б�
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        return screenshot;
    }

    /// <summary>
    /// ��ũ������ ���Ϸ� �����մϴ�.
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
    /// ��ũ���� ���� ���丮�� �����մϴ�.
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
    /// ���� �̸��� �����մϴ�.
    /// </summary>
    private string GenerateFileName(string path, string fileNumber)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(path, $"{timestamp}_{fileNumber}.png");
    }

    /// <summary>
    /// ����� ���ҽ��� �����մϴ�.
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