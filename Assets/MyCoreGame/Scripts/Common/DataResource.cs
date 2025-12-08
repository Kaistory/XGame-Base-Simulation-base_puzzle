using System;
using Newtonsoft.Json;
using UnityEngine;
using mygame.sdk;
using System.ComponentModel;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif
[System.Serializable]
public class DataResource
{
    public DataResource() { }
    public DataResource(RES_type type, int amount)
     {
         this.resType = type;
         this.amount = amount;
     }
    public DataResource(DataResource dataResource)
    {
        this.resType = dataResource.resType;
        this.amount = dataResource.amount;
        this.icon = dataResource.icon;
        this._idIcon = dataResource._idIcon;
    }
    [DefaultValue("")] public string description;
    //public DataTypeResource dataTypeResource;
    public RES_type resType;
    public int amount;
    [JsonIgnore] [SerializeField] protected Sprite _icon;
    [JsonIgnore] [SerializeField] protected Sprite _bg;
    [JsonProperty][SerializeField] protected short _idIcon;
    [JsonProperty][SerializeField] protected short _idBg;
    [JsonIgnore]
    public Sprite icon
    {
        get
        {
            if (_icon == null)
            {
                if (_idIcon <= 0)
                {
                    return SpriteResourceSO.Instance.GetIcon(resType);
                }
                else
                {
                    return SpriteResourceSO.Instance.GetIcon(_idIcon);
                }

            }
            return _icon;
        }
        set
        {
            _icon = value;
        }
    }
    [JsonIgnore]
    public Sprite bg
    {
        get
        {
            if (_bg == null)
            {
                if (_idBg <= 0)
                {
                    return SpriteResourceSO.Instance.GetBg(resType);
                }
                else
                {
                    return SpriteResourceSO.Instance.GetBg(_idBg);
                }
            }
            return _bg;
        }
        set
        {
            _bg = value;
        }
    }
    public short GetIdIcon()
    {
        return _idIcon;
    }
    public short GetIdBg()
    {
        return _idBg;
    }
    public virtual int GetAmount()
    {
        return amount;
    }
    public void CopyData(DataResource from)
    {
        description = from.description;
        _icon = from._icon;
        _bg = from._bg;
        //dataTypeResource = new DataTypeResource(from.dataTypeResource.type, from.dataTypeResource.id);
        resType = from.resType;
        amount = from.amount;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DataResource))]
public class DataResourceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            var property_resType = property.FindPropertyRelative("resType");
            var property_amount = property.FindPropertyRelative("amount");
            var property_idIcon = property.FindPropertyRelative("_idIcon");
            var property_description = property.FindPropertyRelative("description");
            var property_icon = property.FindPropertyRelative("_icon");

            var type = (RES_type)property_resType.intValue;
            int id_icon = property_idIcon.intValue;
            Sprite spr_icon = property_icon.objectReferenceValue as Sprite;
            if (spr_icon == null)
            {
                if (id_icon > 0)
                    spr_icon = SpriteResourceSO.Instance.GetIcon((short)id_icon);
                else
                    spr_icon = SpriteResourceSO.Instance.GetIcon(type);
            }

            // --- HEADER ---
            float foldoutWidth = 18f;
            float spacing = 3f;
            float iconMaxHeight = EditorGUIUtility.singleLineHeight * 1.6f;

            // Tính native size (giữ nguyên tỉ lệ gốc của sprite)
            float nativeW = 32, nativeH = 32; // fallback nếu null
            if (spr_icon && spr_icon.texture)
            {
                nativeW = spr_icon.textureRect.width;
                nativeH = spr_icon.textureRect.height;
            }
            float scale = iconMaxHeight / nativeH;
            float drawW = nativeW * scale;
            float drawH = nativeH * scale;

            float iconOffset = position.x + foldoutWidth + spacing;

            Rect foldoutRect = new Rect(position.x, position.y, foldoutWidth, iconMaxHeight);
            Rect iconRect = new Rect(iconOffset, position.y + (iconMaxHeight - drawH) / 2f, drawW, drawH);
            Rect labelRect = new Rect(iconRect.xMax + spacing, position.y, position.width - drawW - foldoutWidth - 10, iconMaxHeight);

            // Foldout
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, false);

            // Icon crop atlas, giữ nguyên native size
            if (spr_icon && spr_icon.texture)
            {
                Rect texRect = spr_icon.textureRect;
                Rect atlasRect = new Rect(
                    texRect.x / spr_icon.texture.width,
                    texRect.y / spr_icon.texture.height,
                    texRect.width / spr_icon.texture.width,
                    texRect.height / spr_icon.texture.height);
                GUI.DrawTextureWithTexCoords(iconRect, spr_icon.texture, atlasRect, true);
            }
            else
            {
                EditorGUI.HelpBox(iconRect, "", MessageType.None);
            }

            // Label summary
            string labelText = $"{SpriteResourceSO.Instance.GetNameResource(type)} = {property_amount.intValue}";
            EditorGUI.LabelField(labelRect, labelText);

            // --- FOLDOUT NỘI DUNG ---
            if (property.isExpanded)
            {
                float y = position.y + iconMaxHeight + 4;

                // Dòng 1: ResType | Amount | IdIcon
                float restypeWidth = 120f;
                float amountWidth = 60f;
                float idIconWidth = 50f;
                Rect restypeRect = new Rect(position.x + 12, y, restypeWidth, EditorGUIUtility.singleLineHeight);
                Rect amountRect = new Rect(restypeRect.xMax + spacing, y, amountWidth, EditorGUIUtility.singleLineHeight);
                Rect idIconRect = new Rect(amountRect.xMax + spacing, y, idIconWidth, EditorGUIUtility.singleLineHeight);

                EditorGUI.BeginProperty(restypeRect, label, property_resType);
                property_resType.intValue = (int)(RES_type)EditorGUI.EnumPopup(restypeRect, (RES_type)property_resType.intValue);
                EditorGUI.EndProperty();

                EditorGUI.BeginProperty(amountRect, label, property_amount);
                property_amount.intValue = EditorGUI.IntField(amountRect, property_amount.intValue);
                EditorGUI.EndProperty();

                EditorGUI.BeginProperty(idIconRect, label, property_idIcon);
                property_idIcon.intValue = EditorGUI.IntField(idIconRect, property_idIcon.intValue);
                EditorGUI.EndProperty();

                y += EditorGUIUtility.singleLineHeight + 4;

                // Dòng 2: Description (TextField) + Icon (object field)
                float iconFieldWidth = 80f;
                float descLabelWidth = 80f;
                float descFieldWidth = position.width - iconFieldWidth - descLabelWidth - 32;
                Rect descLabelRect = new Rect(position.x + 12, y, descLabelWidth, EditorGUIUtility.singleLineHeight);
                Rect descFieldRect = new Rect(descLabelRect.xMax + 2, y, descFieldWidth, EditorGUIUtility.singleLineHeight);
                Rect iconFieldLabel = new Rect(descFieldRect.xMax + 6, y, 30, EditorGUIUtility.singleLineHeight);
                Rect iconFieldRect = new Rect(iconFieldLabel.xMax, y, iconFieldWidth - 30, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(descLabelRect, "Description");
                EditorGUI.BeginProperty(descFieldRect, label, property_description);
                property_description.stringValue = EditorGUI.TextField(descFieldRect, property_description.stringValue);
                EditorGUI.EndProperty();

                EditorGUI.LabelField(iconFieldLabel, "Icon");
                EditorGUI.PropertyField(iconFieldRect, property_icon, GUIContent.none);
            }
        }
        catch (Exception e)
        {
            EditorGUI.LabelField(position, $"Error: {e.Message}");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float iconMaxHeight = EditorGUIUtility.singleLineHeight * 1.6f;
        float height = iconMaxHeight;
        if (property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + 4; // dòng 1
            height += EditorGUIUtility.singleLineHeight + 4; // dòng 2
        }
        return height;
    }
}
#endif

