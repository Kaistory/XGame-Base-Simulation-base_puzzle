#import <Foundation/Foundation.h>

//@class VibrateHelper;
@interface MyYandextiOS : NSObject

+(instancetype)sharedInstance;

#ifdef __cplusplus
extern "C" {
#endif
    void myYandexInitializeNative();
    void myYandexSetTestModeNative(bool isTestMode);
    void myYandexAddTestDeviceNative(char* deviceId);
    void myYandexSetBannerPosNative(int pos, int width, float dxcenter);
    void myYandexShowBannerNative(char* adsId, int pos, int width, int orien, bool iPad, float dxcenter);
    void myYandexHideBannerNative();
    void myYandexClearCurrFullNative();
    void myYandexLoadFullNative(char* adsId);
    bool myYandexShowFullNative();
    void myYandexClearCurrGiftNative();
    void myYandexLoadGiftNative(char* adsId);
    bool myYandexShowGiftNative();
    
#ifdef __cplusplus
}
#endif


@end
