using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MyGame
{
    public class Boom : MonoBehaviour
    {
        [SerializeField] private GameObject m_particleSystem;
        [SerializeField] private TextMeshProUGUI m_textMPRo;

        private float timeEnd = 5f;
        private Tween countdownTween;
        private Tween blinkTween;

        private bool isRunned;
        // Thêm biến để lưu trữ giây tròn trước đó
        private int m_lastSecond = -1;


        private void Awake()
        {
            isRunned = false;
        }

        public void StartBoom()
        {
            if(isRunned == true) return;
            isRunned = true;
            m_particleSystem.SetActive(true);

            // Reset giây tròn trước đó
            m_lastSecond = -1;

            // Set màu ban đầu
            m_textMPRo.color = Color.white;

            // Hiển thị số ngay khi bắt đầu
            m_textMPRo.text = Mathf.CeilToInt(timeEnd).ToString();

            // Tween đếm ngược
            countdownTween = DOVirtual.Float(timeEnd, 0, timeEnd, value =>
                {
                    int second = Mathf.CeilToInt(value);
                    m_textMPRo.text = second.ToString();
                   
                    // --- BẮT ĐẦU PHẦN CHỈNH SỬA ---
                    // Kiểm tra xem giây tròn có thay đổi không
                    if (second != m_lastSecond)
                    {
                        // Giây đã thay đổi, phát âm thanh Tick
                        if (second > 0) // Không phát Tick khi về 0 (âm thanh Boom sẽ phát ở OnComplete)
                        {
                            AudioManager.Instance.PlaySFX(AudioName.SFX_Tick);
                        }
                        
                        // Cập nhật giây tròn trước đó
                        m_lastSecond = second;
                    }
                    // --- KẾT THÚC PHẦN CHỈNH SỬA ---
                       
                    // Khi còn <= 1 giây thì nhấp nháy đỏ
                    if (second <= 1 && blinkTween == null)
                    {
                        StartBlinkRed();
                    }
                   
                })
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    m_textMPRo.text = "0";
                    AudioManager.Instance.PlaySFX(AudioName.SFX_Boom);
                    StopBlink();
                    
                        LevelManager.Instance.LoseGame();
                });
        }
        
        private void StartBlinkRed()
        {
            blinkTween = m_textMPRo
                .DOColor(Color.red, 0.2f)
                .SetLoops(-1, LoopType.Yoyo).SetTarget(this);
        }

        private void StopBlink()
        {
            blinkTween?.Kill();
            blinkTween = null;
            m_textMPRo.color = Color.red;
            m_particleSystem.SetActive(false);
        }

        public void StopBoom()
                {
                    if (countdownTween != null && countdownTween.IsActive())
                    {
                        countdownTween.Kill(); 
                    }
                    StopBlink(); 
                    m_particleSystem.SetActive(false); 
                    
                   
                    AudioManager.Instance.StopSFX(); 
                }
        private void OnDisable()
        {
            countdownTween?.Kill();
            
            StopBlink();
        }
    }
}