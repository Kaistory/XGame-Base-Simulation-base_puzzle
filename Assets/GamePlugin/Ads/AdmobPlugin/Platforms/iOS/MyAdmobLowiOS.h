#import <Foundation/Foundation.h>

@interface MyAdmobLowiOS : NSObject

+(instancetype)sharedInstance;

#ifdef __cplusplus
extern "C" {
#endif
    void LowInitializeNative();
    //----------------------------------------------
    void LowLoadOpenAdNative(char* placement, char* adsId);
    bool LowShowOpenAdNative(char* placement);
    //----------------------------------------------
    bool LowShowBannerNative(char* placement, bool isCollapse, int pos, int width, int orien, bool iPad, float dxCenter);
    void LowLoadBannerNative(char* placement, bool isCollapse, char* adsId, bool iPad);
    void LowHideBannerNative();
    void LowDestroyBannerNative();
    //----------------------------------------------
    bool LowShowBannerClNative(char* placement, int pos, int width, int orien, bool iPad, float dxCenter);
    void LowLoadBannerClNative(char* placement, char* adsId, bool iPad);
    void LowHideBannerClNative();
    void LowDestroyBannerClNative();
    //----------------------------------------------
    bool LowShowBannerRectNative(char* placement, int pos, float width, float dxCenter, float dyVertical);
    void LowLoadBannerRectNative(char* placement, char* adsId);
    void LowHideBannerRectNative();
    void LowDestroyBannerRectNative();
    //----------------------------------------------
    void LowLoadNtFullNative(char* placement, char* adsId, int orien);
    bool LowShowNtFullNative(char* placement, int timeShowBtClose);
    //----------------------------------------------
    void LowLoadNtIcFullNative(char* placement, char* adsId, int orien);
    bool LowShowNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    //----------------------------------------------
    void LowClearCurrFullNative(char* placement);
    void LowLoadFullNative(char* placement, char* adsId);
    bool LowShowFullNative(char* placement, int timeDelay);
    //----------------------------------------------
    void LowClearCurrGiftNative(char* placement);
    void LowLoadGiftNative(char* placement, char* adsId);
    bool LowShowGiftNative(char* placement);
    
#ifdef __cplusplus
}
#endif


@end
