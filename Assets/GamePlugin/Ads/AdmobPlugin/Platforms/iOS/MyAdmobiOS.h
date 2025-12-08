#import <Foundation/Foundation.h>

@interface MyAdmobiOS : NSObject

@property int ntFullPer;
@property int ntFullFirstRan;
@property int ntFullOtherRan;
@property int ntFullCountNonAfter;
@property int ntFullIncrease;

@property int ntclPer;
@property int ntclRan;
@property int ntclNonAfter;
@property int ntclIncrease;

+(instancetype)sharedInstance;
- (void) clearRecount;

#ifdef __cplusplus
extern "C" {
#endif
    void InitializeNative();
    void targetingAdContentNative(bool isChild, bool isUnderAgeConsent, int rating);
    //----------------------------------------------
    void loadOpenAdNative(char* placement, char* adsId);
    bool showOpenAdNative(char* placement);
    //----------------------------------------------
    bool showBannerNative(char* placement, int pos, int width, int maxH, int orien, bool iPad, float dxCenter);
    void loadBannerNative(char* placement, char* adsId, bool iPad);
    void hideBannerNative();
    void destroyBannerNative();
    //----------------------------------------------
    bool showBannerClNative(char* placement, int pos, int width, int orien, bool iPad, float dxCenter);
    void loadBannerClNative(char* placement, char* adsId, bool iPad);
    void hideBannerClNative();
    void destroyBannerClNative();
    //----------------------------------------------
    bool showBannerRectNative(char* placement, int pos, float width, float dxCenter, float dyVertical);
    void loadBannerRectNative(char* placement, char* adsId);
    void hideBannerRectNative();
    void destroyBannerRectNative();
    //----------------------------------------------
    bool showBnNtNative(char* placement, int pos, int width, int maxH, int orien, bool iPad, float dxCenter, float dyVertical, int trefresh);
    void loadBnNtNative(char* placement, char* adsId, bool iPad);
    void hideBnNtNative();
    void destroyBnNtNative();
    //----------------------------------------------
    void loadNativeClNative(char* placement, char* adsId, int orient);
    bool showNativeClNative(char* placement, int pos, int width, float dxCenter, bool isHideBtClose, bool isLouWhenick);
    void hideNativeClNative();
    //----------------------------------------------
    bool showRectNtNative(char* placement, int pos, int orient, float width, float height, float dx, float dy);
    void loadRectNtNative(char* placement, char* adsId);
    void hideRectNtNative();
    //----------------------------------------------
    void setCfNtFullNative(int v1, int v2, int v3, int v4, int v5);
    void setCfNtFullFbExcluseNative(int rows, int columns, char* areaExcluse);
    void setCfNtClNative(int v1, int v2, int v3, int v4);
    void setCfNtClFbExcluseNative(int rows, int columns, char* areaExcluse);
    void setTypeBnntNative(bool isShowMedia);
    //----------------------------------------------
    void loadNtFullNative(char* placement, char* adsId, int orien);
    void reCountCurrShowNative();
    bool showNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    //----------------------------------------------
    void loadNtIcFullNative(char* placement, char* adsId, int orien);
    bool showNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    //----------------------------------------------
    void loadMultiNtFullNative(char* placement, char* adsId, int orient);
    bool showMultiNtFullNative(char* placement, int timeClose, bool isAutoCloseWhenClick);
    //----------------------------------------------
    void clearCurrFullNative(char* placement);
    void loadFullNative(char* placement, char* adsId);
    bool showFullNative(char* placement);
    //----------------------------------------------
    void clearCurrGiftNative(char* placement);
    void loadGiftNative(char* placement, char* adsId);
    bool showGiftNative(char* placement);
    
#ifdef __cplusplus
}
#endif


@end
