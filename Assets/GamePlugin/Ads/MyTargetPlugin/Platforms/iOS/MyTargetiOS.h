#import <Foundation/Foundation.h>

//@class VibrateHelper;
@interface MyTargetiOS : NSObject

+(instancetype)sharedInstance;

#ifdef __cplusplus
extern "C" {
#endif
    void myTargetInitializeNative();
    void myTargetSetTestModeNative(bool isTestMode);
    void myTargetAddTestDeviceNative(char* deviceId);
    void myTargetSetBannerPosNative(int pos, int width, float dxcenter);
    void myTargetShowBannerNative(char* adsId, int pos, int width, int orien, bool iPad, float dxcenter);
    void myTargetHideBannerNative();
    void myTargetClearCurrFullNative();
    void myTargetLoadFullNative(char* adsId);
    bool myTargetShowFullNative();
    void myTargetClearCurrGiftNative();
    void myTargetLoadGiftNative(char* adsId);
    bool myTargetShowGiftNative();
    
#ifdef __cplusplus
}
#endif


@end
