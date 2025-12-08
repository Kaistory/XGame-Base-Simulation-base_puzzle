using System;
using DG.Tweening;
using UnityEngine;

namespace MyGame
{
    public class GlassPlate : MonoBehaviour
    {
        [SerializeField] private MeshRenderer m_meshRender;
        
        [SerializeField] private Material[] m_materialBase = new Material[3];
        [SerializeField] private Material m_materialBreakGlass;
        [SerializeField] private bool is_BLock;
        private void OnValidate()
        {
            SetBreakGlass();
        }

        public void SetBreakGlass()
        {
            m_materialBase[2] = m_materialBreakGlass;
            m_meshRender.materials = m_materialBase;
            //AudioManager.Instance.PlaySFX(AudioName.SFX_Glass_Breaking);
        }

        public void ClosePopup()
        {
            float duration = 0.4f;
            // Tạo Sequence để chạy song song
            Sequence s = DOTween.Sequence();

            // 1. Thu nhỏ (Dùng InBack: Phồng lên xíu rồi hút mạnh vào trong)
            s.Join(transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

            // 2. Mờ dần (Nếu có CanvasGroup hoặc SpriteRenderer)
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                s.Join(canvasGroup.DOFade(0, duration));
            }

            // 3. (Tùy chọn) Xoay nhẹ một chút tạo hiệu ứng xoáy
            // s.Join(transform.DORotate(new Vector3(0, 0, 180), duration, RotateMode.FastBeyond360));

            // 4. Xử lý sau khi xong
            s.OnComplete(() => 
            {
                gameObject.SetActive(false);
                // Reset lại scale về 1 để lần sau mở lên không bị mất hình
                transform.localScale = Vector3.one; 
                if(canvasGroup != null) canvasGroup.alpha = 1;
            });
        }
    }
}