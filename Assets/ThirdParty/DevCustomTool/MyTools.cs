#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class MyTools
{
    private static string NameFolderScreenshots = "Screenshots";
    private static string PathProject = Application.dataPath.Replace("/Assets", "");
    private static string ExtensionFileScreenshots = ".png";
    private static string NameFileScreenshots = "screenshots_{0}";

    [MenuItem("MyTools/Screen Shot Without Canvas &x")]
    private static void GetScrShot()
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        canvas?.gameObject.SetActive(false);

        Screenshot();
    }

    private static void Screenshot()
    {
        // File path
        string folderPath = Path.Combine(PathProject, NameFolderScreenshots);

        // Create folder if not exists
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        int i = 0;
        string fileName;
        do
        {
            string rawFileName = string.Format(NameFileScreenshots, i) + ExtensionFileScreenshots;
            fileName = Path.Combine(folderPath, rawFileName).Replace("\\", "/");
            i++;
        } while (File.Exists(fileName));

        Log(fileName);

        // Capture screenshot
        ScreenCapture.CaptureScreenshot(fileName);
    }


    [MenuItem("MyTools/Screen Shot With Canvas &z")]
    private static void GetScrShotCanvas()
    {
        Screenshot();
    }

    // [MenuItem("MyTools/Re-Initialize CodeEditor")]
    // private static void InitializeCodeEditor()
    // {
    //     CodeEditor.CurrentEditor.Initialize(CodeEditor.CurrentEditorInstallation);
    // }

    [MenuItem("MyTools/Open Internal Storage")]
    private static void OpenStorage()
    {
#if UNITY_EDITOR_WIN
        string path = Application.persistentDataPath.Replace('/', '\\'); // Đổi tất cả dấu '/' thành '\'
        System.Diagnostics.Process.Start("cmd.exe", $"/c start explorer \"{path}\"");
#endif
        // Đối với macOS
#if UNITY_EDITOR_OSX
        System.Diagnostics.Process.Start("open", Application.persistentDataPath);
#endif
    }


    [MenuItem("MyTools/Open Screenshot Storage")]
    private static void OpenScreenshotStorage()
    {
        // File path
        string folderPath = Path.Combine(PathProject, NameFolderScreenshots);

        // Create folder if not exists
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
#if UNITY_EDITOR_WIN
        string path = folderPath.Replace('/', '\\'); // Đổi tất cả dấu '/' thành '\'
        System.Diagnostics.Process.Start("cmd.exe", $"/c start explorer \"{path}\"");
#endif
        // Đối với macOS
#if UNITY_EDITOR_OSX
            LogError("OpenScreenshotStorage with macOS");
#endif
    }

    #region Log API

#if UNITY_EDITOR
    private static readonly bool ENABLE_LOGGING = true;
#else
    private static readonly bool ENABLE_LOGGING = false;
#endif

    private static string LogRegion = $"{nameof(MyTools)}";

    private static void Log(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.Log($"[{LogRegion}] Log: {message}");
        }
    }

    private static void LogError(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogError($"[{LogRegion}] LogError: {message}");
        }
    }

    private static void LogWarning(object message)
    {
        if (ENABLE_LOGGING)
        {
            Debug.LogWarning($"[{LogRegion}] LogWarning: {message}");
        }
    }

    #endregion
}
#endif