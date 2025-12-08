public class AdIdsConfig
{
#if UNITY_ANDROID
    public const string AdmobPlOpenAd = "openad_default,-1,ca-app-pub-3940256099942544/3347511713";
    public const string AdmobPlBanner = "bn_default,-1,ca-app-pub-3940256099942544/6300978111";
    public const string AdmobPlCl = "cl_default,-1,ca-app-pub-3940256099942544/6300978111";
    public const string AdmobPlRect = "rect_splash,-1,ca-app-pub-2777953690987264/9629887128";
    public const string AdmobPlBnNt = "bn_default,-1,ca-app-pub-2777953690987264/4409350036";
    public const string AdmobPlBnNativeCl = "cl_default,-1,ca-app-pub-2777953690987264/8170848689#cl_win_lose2,-1,ca-app-pub-2777953690987264/5464971900";
    public const string AdmobPlNative = "nt_default,-1,ca-app-pub-3940256099942544/2247696110";
    public const string AdmobPlRectNt = "rectnt_default,-1,ca-app-pub-2777953690987264/9499408070";
    public const string AdmobPlNtFull = "full_default,-1,ca-app-pub-3940256099942544/2247696110";
    public const string AdmobPlNtIcFull = "full_default,-1,ca-app-pub-2777953690987264/1783186690";
    public const string AdmobPlFullImg = "full_default,-1,ca-app-pub-3940256099942544/1033173712";
    public const string AdmobPlFullAll = "full_default,-1,ca-app-pub-3940256099942544/1033173712";
    public const string AdmobPlFullRwInter = "full_default,-1,";
    public const string AdmobPlFullRwRw = "full_default,-1,";
    public const string AdmobPlGift = "gift_default,-1,ca-app-pub-3940256099942544/5224354917";
    public const string AdmobImmirsive = "ca-app-pub-2777953690987264/4386623482";
    
    public const string TargetBnFloor = "1932196";
    public const string TargetFullFloor = "1932199";
    public const string TargetGiftFloor = "1932202";

    public const string YandexBnFloor = "R-M-17611598-1";
    public const string YandexFullFloor = "R-M-17611598-2";
    public const string YandexGiftFloor = "R-M-17611598-3";

    public const string AdvertyApiKey = "";
    public const string OdeeoAppkey = "";
    public const string OdeeoPlacementId = "";

#elif UNITY_IOS || UNITY_IPHONE
    public const string AdmobPlOpenAd = "openad_default,-1,ca-app-pub-2777953690987264/6784231932";
    public const string AdmobPlBanner = "bn_default,-1,ca-app-pub-2777953690987264/6404957981";
    public const string AdmobPlCl = "cl_default,-1,";
    public const string AdmobPlRect = "rect_splash,-1,ca-app-pub-2777953690987264/7540293295";
    public const string AdmobPlBnNt = "bn_default,-1,ca-app-pub-2777953690987264/3778794642";
    public const string AdmobPlBnNativeCl = "cl_default,-1,ca-app-pub-2777953690987264/3709037107#cl_win_lose2,-1,ca-app-pub-2777953690987264/4537342548";
    public const string AdmobPlNative = "nt_default,-1,ca-app-pub-2777953690987264/3601048285";
    public const string AdmobPlRectNt = "rectnt_default,-1,ca-app-pub-2777953690987264/1252703655";
    public const string AdmobPlNtFull = "full_default,-1,ca-app-pub-2777953690987264/7107665567";
    public const string AdmobPlNtIcFull = "full_default,-1,ca-app-pub-2777953690987264/4481502224";
    public const string AdmobPlFullImg = "full_default,-1,ca-app-pub-2777953690987264/9598097539";
    public const string AdmobPlFullAll = "full_default,-1,ca-app-pub-2777953690987264/3276231761#full_splash,-1,ca-app-pub-2777953690987264/7456710429";
    public const string AdmobPlFullRwInter = "full_default,-1,";
    public const string AdmobPlFullRwRw = "full_default,-1,";
    public const string AdmobPlGift = "gift_default,-1,ca-app-pub-2777953690987264/1489890262";
    public const string AdmobImmirsive = "ca-app-pub-2777953690987264/4386623482";

    public const string TargetBnFloor = "1932205";
    public const string TargetFullFloor = "1932208";
    public const string TargetGiftFloor = "1932211";

    public const string YandexBnFloor = "R-M-17611638-1";
    public const string YandexFullFloor = "R-M-17611638-2";
    public const string YandexGiftFloor = "R-M-17611638-3";

    public const string AdvertyApiKey = "";
    public const string OdeeoAppkey = "";
    public const string OdeeoPlacementId = "";

#else
    public const string AdmobPlBanner = "";
    public const string AdmobPlCl = "";
    public const string AdmobPlRect = "";
    public const string AdmobPlNative = "";
    public const string AdmobPlNtFull = "";
    public const string AdmobPlFullImg = "";
    public const string AdmobPlFullAll = "";
    public const string AdmobPlGift = "";
    public const string AdmobImmirsive = "";
    
    public const string TargetBnFloor = "1100993;1100996";
    public const string TargetFullFloor = "1100966;1100972;1263814";
    public const string TargetGiftFloor = "1100978;1100981;1100987";

    public const string YandexBnFloor = "R-M-3571369-11;R-M-3571369-12";
    public const string YandexFullFloor = "R-M-3571369-2;R-M-3571369-3;R-M-3571369-4";
    public const string YandexGiftFloor = "R-M-3571369-7;R-M-3571369-8;R-M-3571369-9";

    public const string AdvertyApiKey = "MGNiYWMzMTgtNDEwZC00NDQzLTk3NjItZmRhMmFiNGVhMzA1JGh0dHBzOi8vYWRzZXJ2ZXIuYWR2ZXJ0eS5jb20=";
    public const string OdeeoAppkey = "901b7f01-42a3-4c0c-9713-d4f6c0339ff2";
    public const string OdeeoPlacementId = "901b7f01-42a3-4c0c-9713-d4f6c0339ff2";
#endif
}
