using UnityEngine;

namespace _JigblockPuzzle.Inapp
{
    public class ShopGoldPack : InappPanelBase
    {
#if UNITY_EDITOR
        public void OnValidate()
        {
            ValidatePackName();
        }

        protected void ValidatePackName()
        {
            string packName = skuId;

            // Tách theo dấu "_"
            var parts = packName.Split('_');

            // Viết hoa chữ cái đầu mỗi phần
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length > 0)
                {
                    parts[i] = char.ToUpper(parts[i][0]) + parts[i].Substring(1);
                }
            }

            string parsedName = string.Join(" ", parts);
            gameObject.name = parsedName;
        }
#endif
    }
}