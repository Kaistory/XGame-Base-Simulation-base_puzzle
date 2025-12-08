#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SpriteRenameWindow – Tool đổi tên file sprite trong Project
/// - Chỉ chấp nhận ảnh trong thư mục Assets
/// - Cho phép ghi đè (Overwrite) nếu trùng tên
/// </summary>
public class SpriteRenameWindow : EditorWindow
{
    private List<Object> sourceTextures = new List<Object>();
    private int startLevel = 1;
    private int levelStep = 1;
    private bool overwriteIfExists = true;
    private Vector2 scrollPos;

    [MenuItem("Tools/Jigblock Puzzle/Rename Sprite Files")]
    public static void ShowWindow()
    {
        GetWindow<SpriteRenameWindow>("Sprite Rename Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("🖼 SPRITE FILE RENAMER", EditorStyles.boldLabel);
        GUILayout.Space(5);

        EditorGUILayout.HelpBox("Kéo ảnh cần đổi tên (trong Project, thư mục Assets/) vào khung dưới:",
            MessageType.Info);
        DrawDragAndDropZone();

        // --- SCROLL VIEW ---
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(250));

        if (sourceTextures.Count > 0)
        {
            for (int i = 0; i < sourceTextures.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                sourceTextures[i] =
                    EditorGUILayout.ObjectField($"Texture {i + 1}", sourceTextures[i], typeof(Texture2D), false);
                if (GUILayout.Button("❌", GUILayout.Width(25)))
                {
                    sourceTextures.RemoveAt(i);
                    GUI.FocusControl(null);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("👉 Chưa có ảnh nào được thêm.", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (sourceTextures.Count > 0)
        {
            if (GUILayout.Button("🧹 Clear All", GUILayout.Height(22)))
                sourceTextures.Clear();
        }

        GUILayout.Space(10);
        startLevel = EditorGUILayout.IntField("Start Level", startLevel);
        levelStep = EditorGUILayout.IntField("Level Step", levelStep);
        overwriteIfExists =
            EditorGUILayout.Toggle(
                new GUIContent("Overwrite If Exists", "Nếu đã có file trùng tên thì xóa file cũ và ghi đè"),
                overwriteIfExists);

        GUILayout.Space(10);
        if (GUILayout.Button("✏️ Rename All Files", GUILayout.Height(30)))
        {
            RenameAllFiles();
        }
    }

    private void DrawDragAndDropZone()
    {
        Rect dropArea = GUILayoutUtility.GetRect(0f, 60f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "⬇️  Drag Textures Here  ⬇️", new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        });

        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object dragged in DragAndDrop.objectReferences)
                    {
                        if (dragged is Texture2D tex)
                        {
                            string path = AssetDatabase.GetAssetPath(tex);
                            if (!string.IsNullOrEmpty(path) && path.StartsWith("Assets/") &&
                                !sourceTextures.Contains(tex))
                            {
                                sourceTextures.Add(tex);
                            }
                            else
                            {
                                Debug.LogWarning($"⚠️ Bỏ qua: {tex.name} không nằm trong thư mục Assets/");
                            }
                        }
                    }
                }

                Event.current.Use();
                break;
        }
    }

    private void RenameAllFiles()
    {
        if (sourceTextures == null || sourceTextures.Count == 0)
        {
            EditorUtility.DisplayDialog("Lỗi", "Vui lòng kéo ít nhất một ảnh vào!", "OK");
            return;
        }

        int currentLevel = startLevel;
        int renamedCount = 0;

        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (Object texObj in sourceTextures)
            {
                if (texObj == null) continue;

                string texturePath = AssetDatabase.GetAssetPath(texObj);
                if (string.IsNullOrEmpty(texturePath) || !texturePath.StartsWith("Assets/"))
                {
                    Debug.LogWarning($"⚠️ Texture không nằm trong thư mục Assets: {texObj.name}");
                    currentLevel += levelStep;
                    continue;
                }

                string ext = Path.GetExtension(texturePath);
                if (string.IsNullOrEmpty(ext)) ext = ".png";

                string currentFolder = Path.GetDirectoryName(texturePath);
                string newFileName = $"Level_{currentLevel}{ext}";
                string newPath = Path.Combine(currentFolder, newFileName).Replace("\\", "/");

                // Nếu đã đúng tên thì bỏ qua
                if (PathsEqual(texturePath, newPath))
                {
                    Debug.Log($"✅ {texObj.name} đã đúng tên ({newFileName})");
                    renamedCount++;
                    currentLevel += levelStep;
                    continue;
                }

                // Nếu file đích đã tồn tại
                if (AssetDatabase.LoadAssetAtPath<Object>(newPath) != null)
                {
                    if (overwriteIfExists)
                    {
                        bool deleted = AssetDatabase.DeleteAsset(newPath);
                        if (!deleted)
                        {
                            Debug.LogWarning($"⚠️ Không thể xóa file cũ để ghi đè: {newPath}");
                            currentLevel += levelStep;
                            continue;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ Tồn tại file trùng tên, bỏ qua: {newPath}");
                        currentLevel += levelStep;
                        continue;
                    }
                }

                // Đổi tên
                string moveError = AssetDatabase.MoveAsset(texturePath, newPath);
                if (string.IsNullOrEmpty(moveError))
                {
                    Debug.Log($"✅ Đã đổi tên: {newFileName}");
                    renamedCount++;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Đổi tên thất bại ({texObj.name}) → {newFileName}:\n{moveError}");
                }

                currentLevel += levelStep;
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        EditorUtility.DisplayDialog("✅ Hoàn tất", $"Đã đổi tên thành công {renamedCount}/{sourceTextures.Count} ảnh.",
            "OK");
        startLevel += levelStep * sourceTextures.Count;
        sourceTextures.Clear();
    }

    private static bool PathsEqual(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
        string na = Path.GetFullPath(a).Replace("\\", "/");
        string nb = Path.GetFullPath(b).Replace("\\", "/");
#if UNITY_EDITOR_WIN
        return string.Equals(na, nb, System.StringComparison.OrdinalIgnoreCase);
#else
        return string.Equals(na, nb, System.StringComparison.Ordinal);
#endif
    }
}
#endif