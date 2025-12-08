#import <Foundation/Foundation.h>

@interface MyMaxiOS : NSObject

@property int ntFullPer;
@property int ntFullFirstRan;
@property int ntFullOtherRan;
@property int ntFullCountNonAfter;
@property int ntFullIncrease;

+(instancetype)sharedInstance;
- (void) clearRecount;

#ifdef __cplusplus
extern "C" {
#endif
    void MaxInitializeNative();
    //----------------------------------------------
    void maxLoadOpenAdNative(char* placement, char* adsId);
    bool maxShowOpenAdNative(char* placement);
    //----------------------------------------------
    void maxSetCfNtFullNative(int v1, int v2, int v3, int v4, int v5);
    void maxLoadNtFullNative(char* placement, char* adsId, int orien);
    void maxReCountCurrShowNative();
    bool maxShowNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    
#ifdef __cplusplus
}
#endif


@end
