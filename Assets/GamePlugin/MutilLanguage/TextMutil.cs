//#define Use_TMPro
//#define Use_TexLB

using System;
using UnityEngine;
using UnityEngine.UI;
#if Use_TMPro
using TMPro;
#endif

namespace mygame.sdk
{
    public enum StateCapText
    {
        None = 0,
        FirstCap,
        FirstCapOnly,
        AllCap,
        AllLow
    }

    public enum TypeText
    {
        Text_UI = 0,
        Text_Mesh_pro_ui,
        Text_Mesh_pro,
        Text_UI_Label
    }

    public enum FormatText
    {
        None = 0,
        F_String,
        F_Int,
        F_Float
    }

    public class TextMutil : MonoBehaviour
    {
        public string key = "";
#if UNITY_EDITOR
        public string value = "";
        [HideInInspector] public string memKey = "------------------------------";
        [HideInInspector] public bool isCreateKey;
#endif
        public TypeText typeText = TypeText.Text_UI;
        public StateCapText stateCap = StateCapText.None;
        [HideInInspector] public bool isView;
        public bool setOnStart = true;
        public bool setOnAwake;
        public FormatText stateFormat = FormatText.None;
        public string objectFormat = "";
        public float sizeRate = 1;
        private float fontSize;
        private bool fixedSize = false;

        private void Awake()
        {
#if use_mutil_lang
            if (setOnAwake)
                setText();
#endif
        }

        private void Start()
        {
#if use_mutil_lang
            GetFontSize();
            if (MutilLanguage.Instance().isChangeFont)
            {
                changeFont();
            }

            if (setOnStart)
            {
                setText();
            }

            MutilLanguage.Instance().listTxt.Add(this);
#endif
        }

        public void Initialized(bool isSetStart, bool isSetAwake)
        {
            setOnStart = isSetStart;
            setOnAwake = isSetAwake;
            GetFontSize();
        }

        private void GetFontSize()
        {
            if (typeText == TypeText.Text_UI)
            {
                if (fontSize == 0)
                {
                    fontSize = GetComponent<Text>().fontSize;
                }
            }
            else if (typeText == TypeText.Text_UI_Label)
            {
#if Use_TexLB
                UILabel lb = GetComponent<UILabel>();
                if (lb != null)
                {
                }
#endif
            }
#if Use_TMPro
            else if (typeText == TypeText.Text_Mesh_pro_ui)
            {
                
            }
            else if (typeText == TypeText.Text_Mesh_pro)
            {
                
            }
#endif
        }

        private void OnEnable()
        {
            isView = true;
            TigerForge.EventManager.StartListening(EventName.ChangeLanguage, OnChangeLanguage);
        }

        private void OnDisable()
        {
            isView = false;
            TigerForge.EventManager.StopListening(EventName.ChangeLanguage, OnChangeLanguage);
        }

        private void OnChangeLanguage()
        {
            setText();
        }

        private void OnDestroy()
        {
            try
            {
                MutilLanguage.Instance().listTxt.Remove(this);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
            }
        }

        public void Resize(float fontSize)
        {
            return;
#if use_mutil_lang
            if (typeText == TypeText.Text_UI)
            {
                Text txt = GetComponent<Text>();
                txt.fontSize = (int)(this.fontSize * fontSize);
                fixedSize = true;
            }
            else if (typeText == TypeText.Text_UI_Label)
            {
#if Use_TexLB
                UILabel lb = GetComponent<UILabel>();
                if (lb != null)
                {
                }
#endif
            }
#if Use_TMPro
            else if (typeText == TypeText.Text_Mesh_pro_ui)
            {
                
            }
            else if (typeText == TypeText.Text_Mesh_pro)
            {
                
            }
#endif
#endif
        }

        public void changeFont()
        {
            return;
#if use_mutil_lang
            if (typeText == TypeText.Text_UI)
            {
                Text txt = GetComponent<Text>();
                if (txt != null)
                {
                    txt.font = SDKManager.Instance.fontReplace;
                    if (!fixedSize)
                    {
                        txt.fontSize = (int)(fontSize * SDKManager.Instance.fontSize * sizeRate);
                    }
                }
            }
            else if (typeText == TypeText.Text_UI_Label)
            {
#if Use_TexLB
                UILabel lb = GetComponent<UILabel>();
                if (lb != null)
                {
                   lb.fontStyle = FontStyle.Normal;
                   lb.trueTypeFont = SDKManager.Instance.fontReplace;
                }
#endif
            }
#if Use_TMPro
            else if (typeText == TypeText.Text_Mesh_pro_ui)
            {
                
            }
            else if (typeText == TypeText.Text_Mesh_pro)
            {
                
            }
#endif
#endif
        }

        public bool setText()
        {
            if (key == null || key.Length <= 0)
            {
                return false;
            }

            try
            {
                if (MutilLanguage.Instance().isChangeFont)
                {
                    changeFont();
                }

                if (typeText == TypeText.Text_UI)
                {
                    GetComponent<Text>().text =
                        MutilLanguage.getStringWithKey(key, stateCap, stateFormat, objectFormat);
                }
                else if (typeText == TypeText.Text_UI_Label)
                {
#if Use_TexLB
                    GetComponent<UILabel>().text =
 MutilLanguage.getStringWithKey(key, stateCap, stateFormat, objectFormat);
#endif
                }
#if Use_TMPro
                else if (typeText == TypeText.Text_Mesh_pro_ui)
                {
                    GetComponent<TextMeshProUGUI>().text =
 MutilLanguage.getStringWithKey(key, stateCap, stateFormat, objectFormat);
                }
                else if (typeText == TypeText.Text_Mesh_pro)
                {
                    GetComponent<TextMeshPro>().text =
 MutilLanguage.getStringWithKey(key, stateCap, stateFormat, objectFormat);
                }
#endif
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("mysdk: ex=" + ex);
                return false;
            }
        }

        public bool setText(string key)
        {
            this.key = key;
            return setText();
        }
    }
}