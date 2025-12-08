using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class FilterTexture : EditorWindow
{
    public List<Texture2D> m_Textures;
    public Dictionary<Texture2D, bool> m_Object = new Dictionary<Texture2D, bool>();
    public string[] m_Textures_Guild;
    private SerializedObject serializedObject;
    private Vector2 scrollPos;

    private void OnEnable()
    {
        m_Textures = new List<Texture2D>();
        titleContent = new GUIContent("Filter Texture");
        serializedObject = new SerializedObject(this);
    }

    private void OnGUI()
    {
        EditorGUIUtility.labelWidth = 100f;
        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(6f);
        GUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Space(6f);
        if (Selection.activeObject != null)
        {
            if (Selection.activeObject as Sprite)
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out var guid, out long file);
                EditorGUILayout.TextField("guild", !string.IsNullOrEmpty(guid) ? $"fileID: {file}, guid: {guid}" : "");
            }
            else
            {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Selection.activeObject, out var guid, out long file);
                EditorGUILayout.TextField("guild", !string.IsNullOrEmpty(guid) ? guid : "");
            }

        }
        if (Selection.activeObject != null)
        {
           var p = Selection.activeGameObject;
           if(p != null){
               EditorGUILayout.TextField("path", GetGameObjectPath(p) == null ? "" : GetGameObjectPath(p));
           }
            //
        }

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
        GUILayout.BeginVertical();
        GUILayout.Space(10f);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Textures"), true);
        serializedObject.ApplyModifiedProperties();
        m_Textures_Guild = new string[m_Textures.Count];
        GUILayout.EndVertical();
        GUILayout.Space(6f);
        GUILayout.EndScrollView();

        if (GUILayout.Button("ClaimSize&DisableMipmap"))
        {
            foreach (var vv in m_Textures)
            {
                if (vv != null)
                {
                    try
                    {
                        var ttip = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(vv));
      
                        if (vv.width == vv.height && vv.width == 1024 || vv.width == 2048 || ttip.maxTextureSize == 2048 || ttip.mipmapEnabled)
                        {
                            var tmp = File.ReadAllText(AssetDatabase.GetAssetPath(vv) + ".meta");


                            if (vv.width == vv.height && vv.width == 1024 || vv.width == 2048)
                            {
                                if (ttip.maxTextureSize == 1024)
                                {
                                    tmp = tmp.Replace("maxTextureSize: 1024", "maxTextureSize: 512");
                                }
                            }

                            if (ttip.mipmapEnabled)
                            {
                                tmp = tmp.Replace("enableMipMap: 1", "enableMipMap: 0");
                            }
                            File.WriteAllText(AssetDatabase.GetAssetPath(vv) + ".meta", tmp);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                
                }

            }
        }
        
        if (GUILayout.Button("Find All"))
        {
            var paths = AssetDatabase.GetAllAssetPaths();

            for (int j = 0; j < paths.Length; j++)
            {
                if (j >= paths.Length) break;
                var s = paths[j];
                var ob = AssetDatabase.LoadAssetAtPath<Texture2D>(s);

                if (ob != null)
                {
                    m_Textures.Add(ob);
                    m_Textures_Guild  = new string[m_Textures.Count];
                }
            }
            
            FindAll();
        }
        
        if (GUILayout.Button("Find"))
        {
            FindAll();
        }


        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void OnSelectionChange()
    {
        Repaint();
    }
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    public void FindAll()
    {
        var paths = AssetDatabase.GetAllAssetPaths();
        for (int j = 0; j < paths.Length; j++)
        {
            if (j >= paths.Length) break;
            var s = paths[j];
            Find(s);
        }
            
        var a = 0;
        foreach (var v in m_Object)
        {
            if (!v.Value)
            {
                var path = AssetDatabase.GetAssetPath(v.Key);
                if (string.IsNullOrEmpty(path) || path.Contains("GamePlugin") || path.Contains("Resources") || path.Contains("IconSplash") || path.Contains(".fbx") || path.Contains(".obj")) continue;
                File.Delete(path);
                a++;
            }
        }
        AssetDatabase.Refresh();
        Debug.Log($"Find all delete : {a} file");
    }
    
    public void Find(string s)
    {
        var t = s.Split('.')[s.Split('.').Length - 1];
        if (File.Exists(s) && s.StartsWith("Assets") && (t == "prefab" || t == "unity" || t == "asset" || t == "mat" || t == "anim"))
        {
            var tmp = File.ReadAllLines(s);
            foreach (var v in tmp)
            {
                if (v.Contains(" guid:"))
                {
                    for (int j = 0; j < m_Textures.Count; j++)
                    {
                        if (!m_Object.ContainsKey(m_Textures[j]))
                        {
                            m_Object.Add(m_Textures[j], false);
                        }

                        if (!m_Object[m_Textures[j]])
                        {
                            var guid = m_Textures_Guild[j];
                            if (string.IsNullOrEmpty(guid))
                            {
                                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m_Textures[j], out guid, out long file);
                                m_Textures_Guild[j] = guid;
                            }

                            if (v.Contains($" guid: {guid}"))
                            {
                                m_Object[m_Textures[j]] = true;
                            }
                        }
                    }
                }
            }
        }
    }

    [MenuItem("Tool/Filter Texture")]
    public static void ShowWindow()
    {
        GetWindow(typeof(FilterTexture));
    }
}