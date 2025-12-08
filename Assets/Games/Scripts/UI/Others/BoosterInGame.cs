using System;
using System.Collections;
using DevUlts;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class BoosterInGame : MonoBehaviour
    {
        [System.Serializable]
        public class BoosterGroupCooldown
        {
            public GameObject countBooster;
            public Image cooldownImg;
        }

        [Header("Booster In Game")] [SerializeField]
        private Button btnClick;

        [SerializeField] private GameObject iconPlusObject;
        [SerializeField] private Text txtCountBooster;
        [SerializeField] private Image iconBooster;

        [SerializeField] private RES_type boosterType;

        [Header("Visual")] [SerializeField] private BoosterGroupCooldown cooldownGroup;
        [SerializeField] private Text txtLock;
        [SerializeField] private ButtonScaler buttonScaler;

        private int countBooster = 0;

        private float timeCounter = 0;

        private float currentTimeCounter = 0;

        private InfoResources infoResources;

        private bool IsCooldown => !isInteractableButton;

        private bool HasBooster => countBooster > 0;

        private bool isUnlocked;

        public void Initialize(RES_type type)
        {
            boosterType = type;
            infoResources = DataManager.Instance.GetInfoResources(boosterType);
            if (infoResources == null)
            {
                return;
            }

            isUnlocked = DataManager.Level >= infoResources.lvUnlock;
            

            iconBooster.sprite = infoResources.icon;

            if (btnClick)
            {
                btnClick.onClick.RemoveAllListeners();
                btnClick.onClick.AddListener(OnButtonClick);
                btnClick.onClick.AddListener(RefreshUI);
            }

            SetInteractionButton(true);
            timeCounter = 0;
            currentTimeCounter = 0;

            CheckUnlock();
        }

        private void Update()
        {
            if (!IsCooldown || !isUnlocked)
            {
                return;
            }

            if (currentTimeCounter > 0)
            {
                currentTimeCounter -= Time.deltaTime;
                if (cooldownGroup.cooldownImg && cooldownGroup.cooldownImg.gameObject.activeInHierarchy)
                {
                    cooldownGroup.cooldownImg.fillAmount = GetFillAmount();
                }

                if (currentTimeCounter <= 0)
                {
                    SetInteractionButton(true);
                    currentTimeCounter = 0;
                    LevelManager.CooldownBoosterAction?.Invoke(boosterType, false, 0);
                }
            }
        }

        private float GetFillAmount()
        {
            var fillAmount = Mathf.Lerp(1, 0, Mathf.InverseLerp(timeCounter, 0, currentTimeCounter));
            return fillAmount;
        }

        private void OnEnable()
        {
            TigerForge.EventManager.StartListening(EventName.UpdateResources, RefreshUI);
            LevelManager.CooldownBoosterAction += CheckInteractionButtonFreezeTime;
        }

        private void OnDisable()
        {
            TigerForge.EventManager.StopListening(EventName.UpdateResources, RefreshUI);
            LevelManager.CooldownBoosterAction -= CheckInteractionButtonFreezeTime;
        }

        private void OnButtonClick()
        {
            if (!isUnlocked)
            {
                UIManager.Instance.NotifyContent(content: $"Unlock at level {infoResources.lvUnlock}!",
                    key: "unlock_at_level", infoResources.lvUnlock);
                return;
            }

            if (CheckHasBooster())
            {
                UseBooster();
            }
            else
            {
                UIManager.Instance.ShowPopup<UIBuyBoosterInGame>().Initialize(infoResources);
            }
        }

        private void UseBooster()
        {
            LevelManager.Instance.OnUseBooster(boosterType);
        }

        private bool CheckHasBooster()
        {
            countBooster = DataManager.GetResources(boosterType);

            return countBooster > 0;
        }

        public void RefreshUI()
        {
            if (!isUnlocked)
            {
                return;
            }

            CheckHasBooster();
            iconPlusObject.SetActive(!HasBooster);
            txtCountBooster.gameObject.SetActive(HasBooster);
            txtCountBooster.text = countBooster.ToString();
        }

        private void CheckUnlock()
        {
            txtLock.transform.parent.gameObject.SetActive(!isUnlocked);
            if (!isUnlocked)
            {
                txtLock.text = $"Lv.{infoResources.lvUnlock}";
                iconPlusObject.SetActive(false);
                txtCountBooster.gameObject.SetActive(false);
                if (cooldownGroup.cooldownImg)
                {
                    cooldownGroup.cooldownImg.gameObject.SetActive(false);
                }

                if (cooldownGroup.countBooster)
                {
                    cooldownGroup.countBooster.SetActive(false);
                }
            }

            RefreshUI();
        }

        private bool isInteractableButton;

        public void SetInteractionButton(bool interactable)
        {
            isInteractableButton = interactable;
            btnClick.interactable = isInteractableButton;

            if (buttonScaler)
            {
                buttonScaler.enabled = isInteractableButton;
            }

            SetCooldownFX(!isInteractableButton);
        }

        private void SetCooldownFX(bool active)
        {
            if (cooldownGroup.cooldownImg)
            {
                cooldownGroup.cooldownImg.gameObject.SetActive(active);
            }

            if (cooldownGroup.countBooster)
            {
                cooldownGroup.countBooster.SetActive(!active);
            }
        }

        private void CheckInteractionButtonFreezeTime(RES_type type, bool interactable, float timeCooldown)
        {
            if (boosterType != type || !isUnlocked)
            {
                return;
            }

            timeCounter = timeCooldown;
            currentTimeCounter = timeCounter;

            SetInteractionButton(!interactable);
        }
    }
}