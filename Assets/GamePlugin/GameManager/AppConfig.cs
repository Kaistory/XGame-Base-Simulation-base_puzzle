public class AppConfig
{
#if UNITY_ANDROID
    public const int gameID = 0;
    public const int verapp = 3;
    public const string platformName = "ANDROID";
    public const bool isDataEncode = false;
    public const string appid = "com.viet.game.app.dev";
    public const string urlstore = "market://details?id={0}";
    public const string urlstorehttp = "https://play.google.com/store/apps/details?id={0}";
    public const string urlLogEvent = "https://watersort.live/api";
    public const string urlPolicy = "";
    public const int ApplicationType = 1;
    public const string Device_test_id = "531efb66-c9e2-4776-816b-f2e626469d20";
    public const int PerValuePostFir = 100;
    public const int PerValuePostIAP = 70;
    public const float PerPostNtSplash = 0.8f;
    public const float PerPostNt2 = 0.8f;
    public const int PerPostFbAdRev = 5;
    public const int ShowSplashFirst = 1;
    public const int FlagShowSplashLoading = 1;
    public const int Flag_show_langeAge = 0;
    public const int AppOpenAdOrien = 0;
    public const int FlagShowRectSplash = 0;

    public const bool isHardwareAccelerated = true;
    public const int Flag_Admob_Optimize = 0;

    public const bool isOnlyDefault = false;
    public const bool isBannerIpad = false;

    public const string defaultStepBanner = "cir:6#re:0";
    public const string defaultStepFull = "cir:6#re:0,10,20";
    public const string defaultStepGift = "cir:6#re:0";
    public const int full_lv_start = 1;
    public const int full_deltime = 30;
    public const int full2_type = 3;
    public const int full2_lv_start = 1;
    public const int full2_deltime = 30;
    public const int open_type = 3;
    public const int open_lv = 1;
    public const int open_newlogic = 24;

    public const int WaitSplash = 3;
    public const int FirstWaitSplash = 1;

#elif UNITY_IOS || UNITY_IPHONE
    public const int gameID = 1;
    public const int verapp = 1;
    public const string appid = "6754678004";
    public const string platformName = "iOS";
    public const bool isDataEncode = false;
    public const string urlstore = "itms-apps://itunes.apple.com/app/id{0}";
	public const string urlstorehttp = "https://itunes.apple.com/us/app/keynote/id{0}?mt=8";
    public const string urlLogEvent = "https://watersort.live/api";
    public const string urlPolicy = "";
    public const int khong_check = 5;
    public const string Device_test_id = "";
    public const bool IAP_use_new = true;
    public const int PerValuePostFir = 100;
    public const int PerValuePostIAP = 70;
    public const int ShowSplashFirst = 1;
    public const float PerPostNtSplash = 0.8f;
    public const float PerPostNt2 = 0.8f;
    public const int PerPostFbAdRev = 5;
    public const int FlagShowSplashLoading = 1;
    public const int AppOpenAdOrien = 0;
    public const int FlagShowRectSplash = 0;

    public const int Flag_show_langeAge = 0;
    public const int Flag_Admob_Optimize = 0;

    public const bool isOnlyDefault = false;
    public const bool isBannerIpad = false;
    public const int AdjustBanner = 1;
    public const float DxCenter4Max = 0;

    public const string defaultStepBanner = "cir:3#re:0";
    public const string defaultStepFull = "cir:3#re:0,10,20";
    public const string defaultStepGift = "cir:3#re:0";
    public const int full_lv_start = 1;
    public const int full_deltime = 30;
    public const int full2_type = 3;
    public const int full2_lv_start = 1;
    public const int full2_deltime = 30;
    public const int open_type = 3;
    public const int open_lv = 1;
    public const int open_newlogic = 24;

    public const int WaitSplash = 3;
    public const int FirstWaitSplash = 1;

#else
    public const int gameID = 1038;
    public const int verapp = 203;
    public const bool isDataEncode = false;
    public const string appid = "com.money.run.hypercasual3d";
    public const string urlstore = "market://details?id={0}";
    public const string urlstorehttp = "https://play.google.com/store/apps/details?id={0}";
    public const string urlLogEvent = "https://watersort.live/api";
    public const string urlPolicy = "";
    public const string Device_test_id = "12345654767";
    public const int PerValuePostFir = 100;
    public const int PerValuePostIAP = 70;
    public const int ShowSplashFirst = 1;
    public const float PerPostNtSplash = 1.0f;
    public const float PerPostNt2 = 1.0f;
    public const int FlagShowSplashLoading = 1;
    public const int Flag_show_langeAge = 0;
    public const bool isHardwareAccelerated = true;
    public const int Flag_Admob_Optimize = 0;

    public const bool isOnlyDefault = false;
    public const bool isBannerIpad = true;

    public const string defaultStepBanner = "cir:3#re:0";
    public const string defaultStepFull = "cir:3#re:0,10,20";
    public const string defaultStepGift = "cir:3#re:0";
    public const int full_lv_start = 1;
    public const int full_deltime = 30;
    public const int full2_type = 3;
    public const int full2_lv_start = 1;
    public const int full2_deltime = 30;
    public const int open_type = 3;
    public const int open_lv = 1;
    public const int open_newlogic = 24;

    public const int WaitSplash = 3;
    public const int FirstWaitSplash = 1;
#endif
}
