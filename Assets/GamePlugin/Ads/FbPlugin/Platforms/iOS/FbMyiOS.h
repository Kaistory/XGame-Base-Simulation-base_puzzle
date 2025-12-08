#import <Foundation/Foundation.h>

@interface FbMyiOS : NSObject

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
    void FbInitializeNative();
    //----------------------------------------------
    void FbSetCfNtFullNative(int v1, int v2, int v3, int v4, int v5);
    void FbSetCfNtFullFbExcluseNative(int rows, int columns, char* areaExcluse);
    //----------------------------------------------
    void FbLoadNtFullNative(char* placement, char* adsId, int orien);
    void FbReCountCurrShowNative();
    bool FbShowNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    //----------------------------------------------
    void FbLoadNtIcFullNative(char* placement, char* adsId, int orien);
    bool FbShowNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick);
    
#ifdef __cplusplus
}
#endif


@end
