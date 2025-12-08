using System;
using System.Collections;
using System.Collections.Generic;
using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

namespace _JigblockPuzzle
{
    public class CheatUI : MonoBehaviour
    {
        [System.Serializable]
        public class ButtonGroup
        {
            public Button btnNextLevel;
            public Button btnPreviousLevel;
            public Button btnCompleteLevel;
            public Button btnAddMoney;
            public Button btnTestLevel;
            public Button btnReset;
            public Button btnTimeScale;
        }

        [System.Serializable]
        public class PasswordGroup
        {
            public Button btnOK;
            public InputField inputField;
            public GameObject root;
        }

        [Header("Cheat UI")] [SerializeField] private Button btnEnable;
        [SerializeField] private ButtonGroup btnGroup;

        [Header("Contents")] [SerializeField] private GameObject rootContent;

        [Header("Input Field")] [SerializeField]
        private GameObject InputPanel;

        [SerializeField] private Button btnGoToLevel;
        [SerializeField] private InputField InputField;
        [SerializeField] private Button btnOk;

        [Header("Password Field")] [SerializeField]
        private PasswordGroup passwordGroup;

        private int timeScaleCache = 1;

        public int TimeScaleCache
        {
            get => timeScaleCache;
            private set
            {
                var valClamped = Mathf.Clamp(value, 1, 10);
                timeScaleCache = valClamped;
                Time.timeScale = valClamped;
            }
        }

        public bool Enable
        {
            get => rootContent != null && rootContent.activeSelf;
            set
            {
                if (rootContent != null)
                {
                    IsEnableCheat = value;
                    rootContent.SetActive(IsEnableCheat);
                }
            }
        }

        public bool EnablePasswordGroup
        {
            get => passwordGroup.root != null && passwordGroup.root.activeSelf;
            set
            {
                if (passwordGroup.root != null)
                {
                    IsEnablePasswordGroup = value;
                    passwordGroup.root.SetActive(IsEnablePasswordGroup);
                }
            }
        }

        private int countTouch = 0;
        private bool IsEnableCheat = false;

        private bool IsEnablePasswordGroup = false;

        public void ActiveCheat()
        {
            var CountClickToActiveCheat = 10;
            if (!RemoteConfigure.CFEnableCheatGame)
            {
                return;
            }

            if (!Enable)
            {
                countTouch++;
                if (countTouch >= CountClickToActiveCheat)
                {
                    countTouch = 0;
                    EnablePasswordGroup = true;
                }
            }
            else
            {
                Enable = false;
            }
        }

        private void Awake()
        {
            Initialize();
        }

        public void DisableCheat()
        {
            OnResetAll();
            countTouch = 0;
            Enable = false;
            EnablePasswordGroup = false;
        }

        private void Initialize()
        {
            DisableCheat();

            if (btnEnable) btnEnable.onClick.AddListener(ActiveCheat);

            if (btnGroup.btnNextLevel)
            {
                btnGroup.btnNextLevel.onClick.RemoveAllListeners();
                btnGroup.btnNextLevel.onClick.AddListener(NextLevel);
            }

            if (btnGroup.btnPreviousLevel)
            {
                btnGroup.btnPreviousLevel.onClick.RemoveAllListeners();
                btnGroup.btnPreviousLevel.onClick.AddListener(PreviousLevel);
            }

            if (btnGroup.btnCompleteLevel)
            {
                btnGroup.btnCompleteLevel.onClick.RemoveAllListeners();
                btnGroup.btnCompleteLevel.onClick.AddListener(CompleteLevel);
            }

            if (btnGoToLevel)
            {
                btnGoToLevel.onClick.RemoveAllListeners();
                btnGoToLevel.onClick.AddListener(ToggleGoToLevelInputFeild);
            }

            if (btnGroup.btnAddMoney)
            {
                btnGroup.btnAddMoney.onClick.RemoveAllListeners();
                btnGroup.btnAddMoney.onClick.AddListener(AddMoney);
            }

            if (btnOk)
            {
                btnOk.onClick.RemoveAllListeners();
                btnOk.onClick.AddListener(OnOkPressed);
            }

            if (btnGroup.btnTestLevel)
            {
                btnGroup.btnTestLevel.onClick.RemoveAllListeners();
                btnGroup.btnTestLevel.onClick.AddListener(TestLevel);
            }

            if (btnGroup.btnReset)
            {
                btnGroup.btnReset.onClick.RemoveAllListeners();
                btnGroup.btnReset.onClick.AddListener(OnResetAll);
            }

            if (btnGroup.btnTimeScale)
            {
                btnGroup.btnTimeScale.onClick.RemoveAllListeners();
                btnGroup.btnTimeScale.onClick.AddListener(OnTimeScaleChange);
            }

            if (passwordGroup.btnOK)
            {
                passwordGroup.btnOK.onClick.RemoveAllListeners();
                passwordGroup.btnOK.onClick.AddListener(OnPasswordOkButtonClick);
            }
        }

        private void OnPasswordOkButtonClick()
        {
            var passwordKey = 20022002;
            if (int.TryParse(passwordGroup.inputField.text, out int value))
            {
                if (value == passwordKey)
                {
                    EnablePasswordGroup = false;
                    Enable = true;
                }
                else
                {
                    DisableCheat();
                }
            }
            else
            {
                DisableCheat();
            }
        }

        private void OnTimeScaleChange()
        {
            TimeScaleCache += 1;
        }

        private void TestLevel()
        {
            if (LevelManager.Instance.CurrentLevel)
            {
                CheatAutoLevel();
            }
        }

        private void CheatAutoLevel()
        {
        }


        private void AddMoney()
        {
            DataResource[] dataResources = new DataResource[4]
            {
                new DataResource(RES_type.GOLD, 10000), new DataResource(RES_type.BOOSTER_1, 10),
                new DataResource(RES_type.BOOSTER_2, 100), new DataResource(RES_type.BOOSTER_3, 10),
            };
            DataManager.Instance.OnEarnResources(dataResources, LogEvent.ReasonItem.reward, "cheat_UI",
                DataManager.Level);
        }

        private void CompleteLevel()
        {
            if (LevelManager.Instance.CurrentLevel)
            {
                LevelManager.Instance.CurrentLevel.CompleteLevel();
            }
        }

        private void NextLevel()
        {
            DataManager.SetLevel(DataManager.Level + 1);
            LoadGame();
        }

        private void PreviousLevel()
        {
            DataManager.SetLevel(DataManager.Level - 1);
            LoadGame();
        }

        private void LoadGame()
        {
            LevelManager.Instance.LoadLevel(DataManager.Level, playType: "cheat_ui");
        }

        private void OnResetAll()
        {
            TimeScaleCache = 1;
        }

        private void ToggleGoToLevelInputFeild()
        {
            InputPanel.SetActive(!InputPanel.activeSelf);
            if (InputPanel.activeSelf)
            {
                InputField.text = "";
                InputField.ActivateInputField();
            }
        }

        private void OnOkPressed()
        {
            if (int.TryParse(InputField.text, out int value))
            {
                if (value > 0)
                {
                    DataManager.SetLevel(value);
                    LoadGame();
                }
            }
            else
            {
                return;
            }

            InputPanel.SetActive(false);
        }
    }
}