using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _JigblockPuzzle
{
    public class UIBuyRemoveAds : PopupUI
    {
        [Header("Remove Ads")] [SerializeField]
        private InappPanelBase inappPanelsItems;

        public override void Initialize(UIManager manager)
        {
            base.Initialize(manager);
            Init();
        }

        public void LogIAPShow(LogEvent.IAP_ShowType showType, LogEvent.IAP_ShowPosition showPosition,
            LogEvent.IAP_ShowAction showAction, bool isLog = true)
        {
            if (inappPanelsItems)
            {
                inappPanelsItems.IAPShowAction(showType, showPosition, showAction, isLog);
            }
        }

        private void Init()
        {
            if (inappPanelsItems)
            {
                inappPanelsItems.Initialize();
            }
        }
#if UNITY_EDITOR
        [ContextMenu("Get All References")]
        public void GetAllReferences()
        {
            inappPanelsItems = GetComponentInChildren<InappPanelBase>();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}