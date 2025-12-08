#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

namespace GameTool.Editor
{
    public class SceneOpenerEditor : EditorWindow
    {
        private EditorBuildSettingsScene[] scenes;
        private Vector2 scrollPosition;
        private string searchKeyword = "";
        private double lastUpdateTime;
        private double refreshInterval = 1.0f; // kiểm tra mỗi 1 giây

        [MenuItem("MyTools/Scene Opener 🚀", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<SceneOpenerEditor>("Scene Opener");
        }

        private void OnEnable()
        {
            scenes = EditorBuildSettings.scenes;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (EditorApplication.timeSinceStartup - lastUpdateTime > refreshInterval)
            {
                lastUpdateTime = EditorApplication.timeSinceStartup;
                var currentScenes = EditorBuildSettings.scenes;
                if (!AreScenesEqual(scenes, currentScenes))
                {
                    scenes = currentScenes;
                    Repaint();
                }
            }
        }

        private bool AreScenesEqual(EditorBuildSettingsScene[] a, EditorBuildSettingsScene[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].path != b[i].path || a[i].enabled != b[i].enabled)
                    return false;
            }

            return true;
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.Label("🎬 Scenes in Build Settings", EditorStyles.boldLabel);

            // Ô tìm kiếm
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("🔍 Search: ", GUILayout.Width(60));
            searchKeyword = EditorGUILayout.TextField(searchKeyword);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var scene in scenes)
            {
                if (!scene.enabled) continue; // bỏ qua scene đang bị disable

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                if (!string.IsNullOrEmpty(searchKeyword) && !sceneName.ToLower().Contains(searchKeyword.ToLower()))
                    continue;

                GUIContent buttonContent = new GUIContent("🎮 " + sceneName, scene.path);

                if (GUILayout.Button(buttonContent))
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        // Shift = mở additive
                        var mode = Event.current.shift ? OpenSceneMode.Additive : OpenSceneMode.Single;
                        EditorSceneManager.OpenScene(scene.path, mode);
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(5);
            EditorGUILayout.HelpBox("Giữ Shift khi click để mở scene dạng Additive 💡", MessageType.Info);
        }
    }
}

#endif