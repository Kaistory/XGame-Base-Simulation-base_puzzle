using mygame.sdk;
using UnityEngine;
using UnityEngine.UI;

public class TextLoadingUI : MonoBehaviour
{
    [SerializeField] private Text textLoading;

    private void Awake()
    {
        GetComponent();
    }

    private void OnEnable()
    {
        GetComponent();
    }

    private void GetComponent()
    {
        if (!textLoading)
        {
            textLoading = GetComponent<Text>();
        }
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (textLoading)
        {
            textLoading.SetText("loading_tasking_x", StateCapText.FirstCap, FormatText.F_String,
                formatObj: GetLoadingDots());
        }
    }

    private string GetLoadingDots()
    {
        int dotCount = Mathf.FloorToInt(Time.time % 3) + 1;
        return new string('.', dotCount);
    }
}