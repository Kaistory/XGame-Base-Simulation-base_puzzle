using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using MyGame.Data;
using MyGame.Manager;

[CustomEditor(typeof(LevelConfigTrunk))]
public class LevelConfigEditorTrunk : Editor
{
    // Biến để chọn Level muốn lưu vào trong danh sách LevelInfos
    private int selectedLevelIndex = 1;

    public override void OnInspectorGUI()
    {
        // Vẽ UI mặc định
        base.OnInspectorGUI();

        LevelConfigTrunk config = (LevelConfigTrunk)target;

        GUILayout.Space(20);
        GUILayout.Label("--- LEVEL DATA TOOLS ---", EditorStyles.boldLabel);

        // Chọn Level để lưu
        selectedLevelIndex = EditorGUILayout.IntField("Target Level ID (lv)", selectedLevelIndex);

        if (GUILayout.Button("SAVE SCENE TO DATA", GUILayout.Height(40)))
        {
            SaveSceneToLevel(config, selectedLevelIndex);
        }

        if (GUILayout.Button("LOAD DATA TO SCENE (Optional)", GUILayout.Height(30)))
        {
             // Phần này nâng cao: Spawn Prefab ra scene dựa trên data đã lưu
             Debug.Log("Tính năng Load có thể tự implement dựa trên logic spawn prefab của bạn.");
        }
    }

    private void SaveSceneToLevel(LevelConfigTrunk config, int targetLevelID)
    {
        // 1. Tìm LevelInfo tương ứng trong mảng
        if (config.levelInfos == null) config.levelInfos = new LevelConfigTrunk.LevelInfo[0];

        // Tìm level có property "level" == targetLevelID
        var levelData = config.levelInfos.FirstOrDefault(x => x.level == targetLevelID);

        // Nếu chưa có thì tạo mới
        if (levelData == null)
        {
            levelData = new LevelConfigTrunk.LevelInfo();
            levelData.level = (ushort)targetLevelID;
            levelData.levelID = (ushort)targetLevelID;
            levelData.levelType = LevelType.Easy; // Giả định có enum này
            
            // Resize mảng và thêm vào
            var list = config.levelInfos.ToList();
            list.Add(levelData);
            config.levelInfos = list.ToArray();
            
            Debug.Log($"<color=yellow>Created new Level Info for Level {targetLevelID}</color>");
        }

        // 2. Tìm tất cả TrunkObject trong Scene
        TrunkObject[] sceneTrunks = FindObjectsOfType<TrunkObject>();
        
        // Sắp xếp theo tên trong Hierarchy để đảm bảo thứ tự index ổn định
        System.Array.Sort(sceneTrunks, (a, b) => a.transform.GetSiblingIndex().CompareTo(b.transform.GetSiblingIndex()));

        List<LevelConfigTrunk.TrunkData> newTrunkDataList = new List<LevelConfigTrunk.TrunkData>();

        foreach (var tObj in sceneTrunks)
        {
            LevelConfigTrunk.TrunkData data = new LevelConfigTrunk.TrunkData();
            
            // Map dữ liệu
            data.position = tObj.transform.position;
            data.colorLayers = (AllColor[])tObj.colorLayers.Clone();
            data.hasLock = tObj.hasLock;
            data.isChained = tObj.isChained;
            data.isFrozen = tObj.isFrozen;
            data.isBlock = tObj.isBlock;
            data.visibleLayerCount = tObj.visibleLayerCount;
            data.amountUpTrunk = tObj.amountUpTrunk;

            newTrunkDataList.Add(data);
        }

        // 3. Tìm tất cả CutterObject trong Scene
        CutterObject[] sceneCutters = FindObjectsOfType<CutterObject>();
        List<LevelConfigTrunk.CutterMachineData> newCutterDataList = new List<LevelConfigTrunk.CutterMachineData>();

        foreach (var cObj in sceneCutters)
        {
            LevelConfigTrunk.CutterMachineData data = new LevelConfigTrunk.CutterMachineData();
            
            data.position = cObj.transform.position;
            data.rot = cObj.rot; // Hoặc dùng (int)cObj.transform.eulerAngles.z nếu muốn lấy từ Transform thực tế

            newCutterDataList.Add(data);
        }

        // 4. Gán dữ liệu vào ScriptableObject
        // Lưu ý: Undo để có thể Ctrl+Z
        Undo.RecordObject(config, "Save Level Data");

        levelData.trunks = newTrunkDataList.ToArray();
        levelData.cutterMachine = newCutterDataList.ToArray();
        levelData.allColorsSpawnCutter = CutterMachineManager.Instance.removeColor;
        // Đánh dấu file đã thay đổi để Unity lưu xuống ổ cứng
        EditorUtility.SetDirty(config);
        
        Debug.Log($"<color=green>SUCCESS! Saved {newTrunkDataList.Count} Trunks and {newCutterDataList.Count} Cutters to Level {targetLevelID}</color>");
    }
}