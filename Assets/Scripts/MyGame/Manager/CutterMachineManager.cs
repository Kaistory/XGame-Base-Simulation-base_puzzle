using System.Collections.Generic;
using MyBox;
using MyGame.Data;
using UnityEngine;

namespace MyGame.Manager
{
    public class CutterMachineManager : Singleton<CutterMachineManager>
    {
        [SerializeField]public List<AllColor> spawnsColor = new List<AllColor>();
        [SerializeField]public List<AllColor> removeColor = new List<AllColor>();
        public void Initialize()
        {
            var lvInfo = LevelManager.Instance.levelInfo;
            
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            
            GameObject cutterMachinePrefab = Resources.Load<GameObject>(PathNameResource.PathCutterMachine);
            for (int i = 0; i < lvInfo.cutterMachine.Length; i++)
            {
                if (cutterMachinePrefab != null)
                {
                    GameObject  cutterMachineClone = Instantiate(cutterMachinePrefab, transform);
                    cutterMachineClone.name = "CutterMachine_" + i.ToString();
                    cutterMachineClone.transform.GetComponent<CutterMachine>().Initialize(lvInfo.cutterMachine[i].position, lvInfo.cutterMachine[i].rot); 
                }
            }
            
            foreach (var color in lvInfo.allColorsSpawnCutter)
            {
                spawnsColor.Add(color);
            }
        }
    }
}