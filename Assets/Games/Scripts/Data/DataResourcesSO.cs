using mygame.sdk;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "DataResourcesSO", menuName = "Data/DataResourcesSO", order = 1)]
public class DataResourcesSO : ScriptableObject
{
    [SerializeField] private List<InfoResources> infoResources;

    public List<InfoResources> InfoResources => infoResources;

    public class InfoResourcesRemote
    {
        public RES_type res_type;
        public int price;
        public int lvUnlock;
    }
}


[System.Serializable]
public class InfoResources
{
    public RES_type res_type;
    public string des;
    public Sprite icon;
    public int price;
    public string name;
    public int lvUnlock;
}

#if UNITY_EDITOR

[CustomEditor(typeof(DataResourcesSO))]
public class DataResourcesSOEditor : Editor
{
    private const string SAVE_PATH = "dataResourcesSO.txt";

    public override void OnInspectorGUI()
    {
        var config = (DataResourcesSO)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("To Json"))
        {
            var json = "";

            var listData = new List<DataResourcesSO.InfoResourcesRemote>();
            for (int i = 0; i < config.InfoResources.Count; i++)
            {
                var item = config.InfoResources[i];
                if (item != null)
                {
                    var newItem = new DataResourcesSO.InfoResourcesRemote()
                    {
                        res_type = item.res_type,
                        price = item.price,
                        lvUnlock = item.lvUnlock,
                    };
                    listData.Add(newItem);
                }
            }

            json = JsonConvert.SerializeObject(listData);
            File.WriteAllText(SAVE_PATH, json);
            Debug.Log($"✅ Saved to {SAVE_PATH}");
        }
    }
}
#endif