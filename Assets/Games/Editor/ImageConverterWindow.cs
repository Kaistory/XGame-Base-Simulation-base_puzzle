using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ImageConverterWindow : EditorWindow
{
    public enum ConversionMode
    {
        Stretch,
        CropFromCenter
    }

    private string selectedFolderPath = "Chưa chọn thư mục";
    private Vector2Int targetSize = new Vector2Int(720, 1080);
    private ConversionMode mode = ConversionMode.CropFromCenter;

    private bool enableRenaming = false;
    private string userPrefix = "";
    private int startLevel = 1;

    [MenuItem("Tools/Image Converter")]
    public static void ShowWindow()
    {
        GetWindow<ImageConverterWindow>("Image Converter");
    }

    void OnGUI()
    {
        GUILayout.Label("Công cụ chuyển đổi và Resize ảnh", EditorStyles.boldLabel);
        GUILayout.Space(10);

        // --- Phần 1: Chọn thư mục ---
        GUILayout.Label("1. Chọn thư mục chứa ảnh JPEG:", EditorStyles.label);
        EditorGUILayout.LabelField("Thư mục:", selectedFolderPath, EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Browse..."))
        {
            string path = EditorUtility.OpenFolderPanel("Chọn thư mục chứa ảnh", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                if (!path.StartsWith(Application.dataPath))
                {
                    EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một thư mục bên trong thư mục 'Assets' của dự án.", "OK");
                    selectedFolderPath = "Chưa chọn thư mục";
                }
                else
                {
                    selectedFolderPath = NormalizeSlashes(path);
                }
            }
        }

        // --- Kéo thả thư mục ---
        DrawFolderDropZone();

        GUILayout.Space(10);

        // --- Phần 2: Tùy chọn Resize ---
        GUILayout.Label("2. Tùy chọn chuyển đổi:", EditorStyles.label);
        mode = (ConversionMode)EditorGUILayout.EnumPopup("Chế độ chuyển đổi", mode);
        targetSize = EditorGUILayout.Vector2IntField("Kích thước (Width/Height)", targetSize);

        if (mode == ConversionMode.Stretch)
            EditorGUILayout.HelpBox("Stretch: Sẽ kéo dãn hoặc nén ảnh để vừa khung (gây méo ảnh).", MessageType.Info);
        else
            EditorGUILayout.HelpBox("Crop From Center: Sẽ phóng to ảnh (giữ tỷ lệ) và cắt từ tâm để lấp đầy khung.", MessageType.Info);

        GUILayout.Space(10);

        // --- Phần 3: Tùy chọn đổi tên ---
        GUILayout.Label("3. Tùy chọn đổi tên file:", EditorStyles.label);
        enableRenaming = EditorGUILayout.Toggle("Kích hoạt đổi tên", enableRenaming);

        EditorGUI.BeginDisabledGroup(!enableRenaming);
        userPrefix = EditorGUILayout.TextField("Tiền tố (Prefix)", userPrefix);
        startLevel = EditorGUILayout.IntField("Số bắt đầu", startLevel);
        string exampleName = $"{userPrefix}{startLevel}.png";
        EditorGUILayout.HelpBox($"Tên file sẽ có dạng: [Tiền tố][Số].png\nVí dụ: {exampleName}", MessageType.Info);
        EditorGUI.EndDisabledGroup();

        GUILayout.Space(20);

        // --- Nút thực thi ---
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("BẮT ĐẦU CHUYỂN ĐỔI (CẨN THẬN!)"))
        {
            if (selectedFolderPath == "Chưa chọn thư mục" || !Directory.Exists(selectedFolderPath))
            {
                EditorUtility.DisplayDialog("Lỗi", "Bạn phải chọn một thư mục hợp lệ trước.", "OK");
                return;
            }
            if (targetSize.x <= 0 || targetSize.y <= 0)
            {
                EditorUtility.DisplayDialog("Lỗi", "Kích thước phải lớn hơn 0.", "OK");
                return;
            }

            if (EditorUtility.DisplayDialog(
                "Xác nhận",
                "Bạn có chắc chắn muốn chuyển đổi TẤT CẢ file .jpeg/.jpg trong thư mục này thành .png và XÓA file gốc không?\n\n" +
                "HÃY SAO LƯU TRƯỚC KHI TIẾP TỤC!",
                "Tôi đã hiểu, tiếp tục", "Hủy bỏ"))
            {
                ProcessFiles(selectedFolderPath, targetSize.x, targetSize.y);
            }
        }
        GUI.backgroundColor = Color.white;
    }

    // ====== KHU VỰC KÉO–THẢ THƯ MỤC ======
    private void DrawFolderDropZone()
    {
        GUILayout.Space(8);
        var dropRect = GUILayoutUtility.GetRect(0, 60, GUILayout.ExpandWidth(true));
        GUI.Box(dropRect, "KÉO THƯ MỤC VÀO ĐÂY (bên trong Assets/)", EditorStyles.helpBox);

        var evt = Event.current;
        if (!dropRect.Contains(evt.mousePosition))
            return;

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                // 1) Từ Finder/Explorer: DragAndDrop.paths có thể là absolute hoặc asset paths
                foreach (var p in DragAndDrop.paths)
                {
                    if (TryResolveFolderPath(p, out var abs))
                    {
                        selectedFolderPath = abs;
                        GUI.changed = true;
                        evt.Use();
                        return;
                    }
                }

                // 2) Từ Project window: dùng objectReferences
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    var assetPath = AssetDatabase.GetAssetPath(obj);
                    if (!string.IsNullOrEmpty(assetPath) && AssetDatabase.IsValidFolder(assetPath))
                    {
                        if (TryResolveFolderPath(assetPath, out var abs))
                        {
                            selectedFolderPath = abs;
                            GUI.changed = true;
                            evt.Use();
                            return;
                        }
                    }
                }

                // Không tìm thấy thư mục hợp lệ
                EditorUtility.DisplayDialog("Không hợp lệ",
                    "Hãy kéo một THƯ MỤC nằm bên trong 'Assets/'.", "OK");
                evt.Use();
            }
            else
            {
                evt.Use();
            }
        }
    }

    private bool TryResolveFolderPath(string draggedPath, out string absolutePath)
    {
        absolutePath = null;
        if (string.IsNullOrEmpty(draggedPath)) return false;

        // Normalize
        draggedPath = NormalizeSlashes(draggedPath);

        // Trường hợp là asset path kiểu "Assets/FolderA/Sub"
        if (draggedPath.StartsWith("Assets/") || draggedPath == "Assets")
        {
            // đảm bảo là thư mục asset
            if (AssetDatabase.IsValidFolder(draggedPath))
            {
                absolutePath = ToAbsolutePathFromAssetPath(draggedPath);
            }
            return absolutePath != null && Directory.Exists(absolutePath);
        }

        // Trường hợp là absolute path từ hệ điều hành
        if (Path.IsPathRooted(draggedPath))
        {
            // chỉ nhận nếu nằm trong Assets/
            var dataPath = NormalizeSlashes(Application.dataPath);
            if (!draggedPath.StartsWith(dataPath))
                return false;

            if (Directory.Exists(draggedPath))
            {
                absolutePath = draggedPath;
                return true;
            }
        }

        return false;
    }

    private string ToAssetPath(string absolutePath)
    {
        absolutePath = NormalizeSlashes(absolutePath);
        string dataPath = NormalizeSlashes(Application.dataPath);
        if (absolutePath.StartsWith(dataPath))
        {
            return "Assets" + absolutePath.Substring(dataPath.Length);
        }
        return null;
    }

    private static string ToAbsolutePathFromAssetPath(string assetPath)
    {
        // assetPath: "Assets/Folder/Sub"
        string dataPath = NormalizeSlashes(Application.dataPath);
        // cắt "Assets" -> nối với .../Assets
        var rel = assetPath.Substring("Assets".Length); // có thể là "" hoặc "/Folder/Sub"
        var abs = dataPath + rel;
        return NormalizeSlashes(abs);
    }

    private static string NormalizeSlashes(string p)
    {
        return string.IsNullOrEmpty(p) ? p : p.Replace("\\", "/");
    }

    private void ProcessFiles(string path, int newWidth, int newHeight)
    {
        var imagePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".jpg") || f.EndsWith(".jpeg"));

        List<string> files = imagePaths.ToList();
        int processedCount = 0;

        try
        {
            for (int i = 0; i < files.Count; i++)
            {
                string fullPath = files[i];

                byte[] fileData = File.ReadAllBytes(fullPath);
                Texture2D srcTex = new Texture2D(2, 2);
                srcTex.LoadImage(fileData);

                RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
                RenderTexture.active = rt;
                GL.Clear(true, true, Color.clear);

                if (mode == ConversionMode.Stretch)
                {
                    Graphics.Blit(srcTex, rt);
                }
                else // CropFromCenter
                {
                    GL.PushMatrix();
                    GL.LoadPixelMatrix(0, newWidth, newHeight, 0);

                    float srcWidth = srcTex.width;
                    float srcHeight = srcTex.height;
                    float dstWidth = newWidth;
                    float dstHeight = newHeight;

                    float srcAspect = srcWidth / srcHeight;
                    float dstAspect = dstWidth / dstHeight;

                    Rect destPixelRect;
                    if (srcAspect > dstAspect)
                    {
                        float scale = dstHeight / srcHeight;
                        float newDrawWidth = srcWidth * scale;
                        float xOffset = (dstWidth - newDrawWidth) / 2.0f;
                        destPixelRect = new Rect(xOffset, 0, newDrawWidth, dstHeight);
                    }
                    else
                    {
                        float scale = dstWidth / srcWidth;
                        float newDrawHeight = srcHeight * scale;
                        float yOffset = (dstHeight - newDrawHeight) / 2.0f;
                        destPixelRect = new Rect(0, yOffset, dstWidth, newDrawHeight);
                    }

                    Graphics.DrawTexture(destPixelRect, srcTex);
                    GL.PopMatrix();
                }

                Texture2D resultTex = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
                resultTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
                resultTex.Apply();

                byte[] pngData = resultTex.EncodeToPNG();

                string pngPath;
                if (enableRenaming)
                {
                    int currentLevel = startLevel + i;
                    string directory = Path.GetDirectoryName(fullPath);
                    string newName = $"{userPrefix}{currentLevel}.png";
                    pngPath = Path.Combine(directory, newName);
                }
                else
                {
                    pngPath = Path.ChangeExtension(fullPath, ".png");
                }

                // Ghi file .png
                File.WriteAllBytes(pngPath, pngData);

                // Xóa file gốc .jpg/.jpeg
                File.Delete(fullPath);

                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(rt);
                DestroyImmediate(srcTex);
                DestroyImmediate(resultTex);

                processedCount++;
            }
        }
        finally
        {
            AssetDatabase.Refresh();
        }

        Debug.Log($"[Image Converter] Đã hoàn tất! Chuyển đổi/resize {processedCount} file, reimport & set Sprite (2D and UI).");
    }
}
