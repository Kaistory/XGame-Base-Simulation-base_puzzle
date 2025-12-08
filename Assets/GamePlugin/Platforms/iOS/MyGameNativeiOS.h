#import <Foundation/Foundation.h>

//@class VibrateHelper;
@interface MyGameNativeiOS : NSObject

+ (instancetype)sharedInstance;

#ifdef __cplusplus
extern "C"
{
#endif

    int isVnNative();
    char *getLanguageCodeNative();
    char *getCountryCodeNative();
    char *getAdsIdentifyNative();
    void synchronizeTimeNative(long timestampMilisecond);
    long CurrentTimeMilisRealNative();
    char *getGiftBoxNative();
    void vibrateNative(int type);
    void switchFlashNative(bool isOn);
    long getMemoryLimit();
    long getPhysicMemoryInfo();
    float getScreenWidthNative();
    
    void configAppOpenAdNative(int timeBg, int orien);
    void loadAppOpenAdNative(char* iappOpenAdId);
    bool isAppOpenAdLoadedNative();
    bool showAppOpenAdNative();
    
    bool appReviewNative();
    void requestIDFANative(int isallversion);
    void showCMPNative();
    
    void localNotifyNative(char* title, char* msg, int hour, int minus, int dayrepeat);
    void clearAllNotiNative();
    
    bool deleteImagesFromImessageNative(int countItem, char** listNames, char* groupName);
    bool deleteImageFromImessageNative(char* listName, char*groupName);
    bool shareImages2ImessageNative(int countItem, char** listNames, int* lenDatas, Byte* data, int lenData, char* nameGroup);
    bool shareImage2ImessageNative(Byte* data, int lenData, char* nameImg, char* nameGroup);
    
    int pushNotifyNative(int timeFireInseconds, char* title, char* msg);
    void cancelNotiNative(char* ids);
    void _SetKeychainValue(char* key, char* va);
    
    char *_GetKeychainValue(char* key);

#ifdef __cplusplus
}
#endif

@end

