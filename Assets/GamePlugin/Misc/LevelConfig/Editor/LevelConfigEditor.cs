using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(LevelConfig))]
public class LevelConfigEditor : Editor
{
    public int clientLevelLength = 100;
    public int clientIconLength = 4;
    public int groupLevelSize = 25;
    public int groupIconSize = 25;

    private string pathData = $"Assets/Games/GameData/";

    public override void OnInspectorGUI()
    {
        var levelConfig = (LevelConfig)target;
        GUILayout.Label("Addressable Setup", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();

        GUILayout.Label("Client Level Length", GUILayout.Width(125));
        clientLevelLength = EditorGUILayout.IntField(clientLevelLength, GUILayout.Width(45));

        GUILayout.Label("Group Level Size", GUILayout.Width(75));
        groupLevelSize = EditorGUILayout.IntField(groupLevelSize, GUILayout.Width(45));

        GUILayout.Label("Client Icon Length", GUILayout.Width(125));
        clientIconLength = EditorGUILayout.IntField(clientIconLength, GUILayout.Width(45));

        GUILayout.Label("Icon Group Size", GUILayout.Width(100));
        groupIconSize = EditorGUILayout.IntField(groupIconSize, GUILayout.Width(45));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("Helper", EditorStyles.boldLabel);

        if (GUILayout.Button("To Json"))
        {
            File.WriteAllText(Path.Combine(pathData, "cf.txt"), JsonConvert.SerializeObject(
                new Dictionary<int, LevelConfig.LevelConfigData>
                {
                    {
                        AppConfig.verapp,
                        new LevelConfig.LevelConfigData() { version = 0, levelInfos = levelConfig.levelInfos }
                    }
                }));

            Debug.Log("To Json Success");
        }

        if (GUILayout.Button("To Csv"))
        {
            var ss = "";
            // foreach (var i in levelConfig.levelInfos)
            // {
            //     if (i != null)
            //         ss +=
            //             $"{i.level},{i.levelID},{(int)i.gridType},{i.levelTime},{i.backgroundID},{(int)i.levelType},{i.levelRewardValue}\n";
            // }

            File.WriteAllText(Path.Combine(pathData, "levelConfig.csv"), ss);
            Debug.Log("To Csv Success!");
        }


        if (GUILayout.Button("Delete Level Not Use"))
        {
            var ss = "";
            var prefabPaths =
                Directory.GetFiles("Assets/Games/Prefabs/Levels", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < prefabPaths.Length; i++)
            {
                try
                {
                    var lv = int.Parse(prefabPaths[i].Split("_").Last().Replace(".png", ""));
                    if (levelConfig.levelInfos.IndexOf(x => x.levelID == lv) == -1)
                    {
                        File.Delete(prefabPaths[i]);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(prefabPaths[i] + " : " + e.Message);
                }
            }
        }

        if (GUILayout.Button("From Csv"))
        {
            var arr = new List<LevelConfig.LevelInfo>();
            var ss = File.ReadAllLines(Path.Combine(pathData, "levelConfig.csv"));
            foreach (var s in ss)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var cols = s.Split(',');
                    if (cols.Length < 7) continue;

                    var inf = new LevelConfig.LevelInfo
                    {
                        level = ushort.Parse(cols[0]),
                        levelID = ushort.Parse(cols[1]),
                        // gridType = (LevelConfig.PuzzleGridType)ushort.Parse(cols[2]),
                        //levelTime = int.Parse(cols[3]),
                        //backgroundID = int.Parse(cols[4]),
                        levelType = (LevelType)ushort.Parse(cols[5]),
                        levelRewardValue = ushort.Parse(cols[6]),
                    };
                    arr.Add(inf);
                }
            }

            levelConfig.levelInfos = arr.ToArray();
            EditorUtility.SetDirty(levelConfig);
            levelConfig.OnValidate();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("From Csv Success!");
        }

        if (GUILayout.Button("Rename Level"))
        {
            var path = "Assets/Games/Resources/Prefab";
            for (int i = 0; i < levelConfig.levelInfos.Length; i++)
            {
                var cf = levelConfig.levelInfos[i];
                // Assets/Games/Resources/Prefab/Levels/Level_1.png
                File.Copy($"{path}/Levels/Level_{cf.levelID}.png", $"{path}/LevelNew/Level_{cf.level}.png");
            }

            for (int i = 1; i < 1000; i++)
            {
                // Assets/Games/Resources/Prefab/Levels/Level_1.png
                if (File.Exists($"{path}/Levels/Level_{i}.png"))
                {
                    var i1 = i;
                    if (levelConfig.levelInfos.IndexOf(x => x.levelID == i1) != -1)
                    {
                        continue;
                    }

                    if (!File.Exists($"{path}/LevelNew/Level_{i}.png"))
                    {
                        File.Copy($"{path}/Levels/Level_{i}.png", $"{path}/LevelNew/Level_{i}.png");
                    }
                    else
                    {
                        File.Copy($"{path}/Levels/Level_{i}.png", $"{path}/LevelLoop/Level_{i}.png");
                    }
                }
            }

            for (int i = 1; i < 1000; i++)
            {
                // Assets/Games/Resources/Prefab/Levels/Level_1.png
                if (File.Exists($"{path}/LevelNew/Level_{i}.png"))
                {
                    for (int k = 1; k < i - 1; k++)
                    {
                        if (!File.Exists($"{path}/LevelNew/Level_{k}.png") &&
                            File.Exists($"{path}/LevelNew/Level_{k - 1}.png"))
                        {
                            File.Move($"{path}/LevelNew/Level_{i}.png", $"{path}/LevelNew/Level_{k}.png");
                            break;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Sort Level"))
        {
            var path = "Assets/Games/Resources/Prefab";
            for (int i = 1; i < 1000; i++)
            {
                // Assets/Games/Resources/Prefab/Levels/Level_1.png
                if (File.Exists($"{path}/LevelNew/Level_{i}.png"))
                {
                    for (int k = 1; k < i - 1; k++)
                    {
                        if (!File.Exists($"{path}/LevelNew/Level_{k}.png") &&
                            File.Exists($"{path}/LevelNew/Level_{k - 1}.png"))
                        {
                            File.Move($"{path}/LevelNew/Level_{i}.png", $"{path}/LevelNew/Level_{k}.png");
                            break;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Check Level Id"))
        {
            CheckLevel(levelConfig);
        }

        base.OnInspectorGUI();
    }

    static readonly string[] levelFolders =
    {
        "Assets/Games/Prefabs/Levels/Glass",
        "Assets/Games/Prefabs/Levels/Normal",
        "Assets/Games/Prefabs/Levels/TapOut"
    };


    public static void CheckLevel(LevelConfig levelConfig)
    {
        var configs = new List<LevelConfig.LevelInfo>(levelConfig.levelInfos);

        // 0) Load tất cả PNG/Sprite, build Dictionary<levelId, Sprite>
        var levelSpriteDict = new Dictionary<int, Sprite>();
        var levelList = new List<int>();

        foreach (var folder in levelFolders) // giả sử bạn đã có mảng/tham số levelFolders như cũ
        {
            // Tìm tất cả Texture2D (PNG). Có thể đổi sang "t:Sprite" nếu bạn muốn bắt sub-asset Sprite.
            var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
            Debug.Log($"[CheckLevel] Found {guids.Length} textures in {folder}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                // Chỉ nhận .png để tránh psd/jpg (tùy nhu cầu)
                if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    continue;

                var fileName = Path.GetFileNameWithoutExtension(path);
                if (!fileName.StartsWith("Level_", StringComparison.Ordinal))
                    continue;

                // Parse ID từ tên file: Level_123.png
                var idStr = fileName.Substring("Level_".Length);
                if (!int.TryParse(idStr, out var levelId))
                {
                    Debug.LogError($"[CheckLevel] Không parse được ID từ \"{fileName}\"");
                    continue;
                }

                // Lấy Sprite từ file (kể cả case có nhiều sub-sprites)
                var sprite = LoadFirstSpriteAtPath(path);
                if (sprite == null)
                {
                    Debug.LogError($"[CheckLevel] \"{fileName}\" không tìm thấy Sprite (Importer phải là Sprite).");
                    continue;
                }

                if (!levelSpriteDict.ContainsKey(levelId))
                {
                    levelSpriteDict.Add(levelId, sprite);
                    levelList.Add(levelId);
                }
                else
                {
                    Debug.LogError($"[CheckLevel] Duplicate LevelID {levelId} ở path: {path}");
                }
            }
        }

        var levelDict2 = new Dictionary<int, int>(); // map levelID -> level (để dò duplicate trong config)
        Debug.LogError($"Have {levelList.Count} Level (PNG/Sprite)");

        // 1) Đối chiếu với config
        for (int i = 0; i < configs.Count; i++)
        {
            int cfgId = configs[i].levelID;

            if (!levelSpriteDict.TryGetValue(cfgId, out var spriteAsset) || spriteAsset == null)
            {
                Debug.LogError(
                    $"[CheckLevel] Không tìm thấy PNG/Sprite \"Level_{cfgId}.png\" cho Level {configs[i].level}");
                continue;
            }

            if (!levelDict2.ContainsKey(cfgId))
            {
                levelDict2.Add(cfgId, configs[i].level);
            }
            else
            {
                Debug.LogError(
                    $"[CheckLevel] Level_{cfgId} duplicate trong Level {configs[i].level} và {levelDict2[cfgId]}");
            }

            // Đã dùng -> loại khỏi list "chưa dùng"
            levelList.Remove(cfgId);
        }

        // 2) Báo các PNG chưa được tham chiếu trong config
        if (levelList.Count > 0)
        {
            string levelNotUsed = string.Join(" ", levelList.Select(x => x.ToString()));
            Debug.LogError($"{levelList.Count} level PNG not used: {levelNotUsed}");
        }
    }

    /// <summary>
    /// Lấy Sprite đầu tiên tại path. Hỗ trợ cả single-sprite và multiple-sprites (sprite sheet).
    /// </summary>
    private static Sprite LoadFirstSpriteAtPath(string assetPath)
    {
        // Unity sẽ nhúng Sprite như sub-asset. LoadAll để lấy subassets kiểu Sprite.
        var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        var sprites = assets.OfType<Sprite>().ToArray();

        if (sprites.Length == 0)
        {
            // Thử fallback: nếu import chưa là Sprite, thử load Texture2D (nhưng vẫn không có Sprite để đối chiếu)
            // Bạn có thể buộc người import set Texture Type = Sprite (2D and UI) để đảm bảo có Sprite.
            return null;
        }

        // Nếu có nhiều sub-sprite (sprite sheet), lấy sprite đầu tiên
        return sprites[0];
    }
}
#endif