//#define Enable_Test_Fbad

#import "FbMyiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <FBAudienceNetwork/FBAudienceNetwork.h>

#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#import "FbMyUtiliOS.h"
#import "FbMyUtiliOS.h"
#import "FbMyDelegate.h"
#import "FbNtFull.h"

const char* const MyAdsFbBridge_NAME = "AdsFbMyBridge";

@interface BannerPlacement : NSObject
@property(nonatomic, strong) NSString *pId;
@property(nonatomic, strong) GADBannerView *bannerView;
@property BOOL isLoading;
@property BOOL isLoaded;
@property BOOL isShow;
@end

@interface FbMyiOS () <FbMyDelegate>

@property(nonatomic, strong) NSMutableDictionary<NSString*, FbNtFull*> *dicNtFull;
@property(nonatomic, strong) NSMutableDictionary<NSString*, FbNtFull*> *dicNtIcFull;

@property(nonatomic, weak) FbNtFull* memAdNtFull4Recount;

@end

@implementation FbMyiOS

+ (instancetype)sharedInstance
{
    static FbMyiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[FbMyiOS alloc] init];
        // Do any other initialisation stuff here
        [sharedInstance InitData];
    });
    return sharedInstance;
}

- (void)Initialize
{
    [FBAudienceNetworkAds initializeWithSettings:nil completionHandler:^(FBAdInitResults * _Nonnull results) {
        NSLog(@"mysdk: ads fb init %@", results);
    }];
#ifdef Enable_Test_Fbad
    [FBAdSettings setLogLevel:FBAdLogLevelDebug];
    [FBAdSettings addTestDevice:@"8f0d5570727b79575d46b0f4aaa75fd7911c370c"];
#endif
}

- (void)InitData
{
    self.dicNtFull = [[NSMutableDictionary alloc] init];
    self.dicNtIcFull = [[NSMutableDictionary alloc] init];
}

#pragma setcf native
- (void)setCfNtFullNative:(int)per fran:(int)fran oran:(int)oran non:(int)nonAfter increase:(int)increase
{
    self.ntFullPer = per;
    self.ntFullFirstRan = fran;
    self.ntFullOtherRan = oran;
    self.ntFullCountNonAfter = nonAfter;
    self.ntFullIncrease = increase;
}
- (void)setCfNtFullFbExcluse:(int)rows columns:(int)columns areaExcluse:(NSString*)areaExcluse
{
}
#pragma NtFull
- (void)loadNtFullNative:(NSString*)placement adId:(NSString*)adsId orient:(int)orient
{
    FbNtFull *ad = [self.dicNtFull objectForKey:placement];
    if (ad == nil) {
        ad = [[FbNtFull alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicNtFull setObject:ad forKey:placement];
    }
    [ad load:adsId orient:orient];
}
- (void)recountClose
{
    if (self.memAdNtFull4Recount != nil) {
        [self.memAdNtFull4Recount reCount];
        self.memAdNtFull4Recount = nil;
    }
}
- (void) clearRecount
{
    self.memAdNtFull4Recount = nil;
}
- (bool)showNtFullNative:(NSString*)placement timeBtClose:(int)timeShowBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAuto
{
    FbNtFull *ad = [self.dicNtFull objectForKey:placement];
    if (ad != nil) {
        if (timeDelay > 10) {
            self.memAdNtFull4Recount = ad;
        }
        return [ad show:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAuto];
    } else {
        return false;
    }
}
#pragma NTIcFull
- (void)loadNtIcFullNative:(NSString*)placement adId:(NSString*)adsId orient:(int)orient
{
    FbNtFull *ad = [self.dicNtIcFull objectForKey:placement];
    if (ad == nil) {
        ad = [[FbNtFull alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicNtIcFull setObject:ad forKey:placement];
    }
    [ad load:adsId orient:orient];
}
- (bool)showNtIcFullNative:(NSString*)placement timeBtClose:(int)timeShowBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAuto
{
    FbNtFull *ad = [self.dicNtIcFull objectForKey:placement];
    if (ad != nil) {
        if (timeDelay > 10) {
            self.memAdNtFull4Recount = ad;
        }
        return [ad show:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAuto];
    } else {
        return false;
    }
}

#pragma Cb
- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullLoaded", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullLoaded", [cbParam UTF8String]);
    }
}
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, @"error"];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullLoadFail", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBonFullLoadFail", [cbParam UTF8String]);
    }
}
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, @"error"];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullFailedToShow", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullFailedToShow", [cbParam UTF8String]);
    }
}
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullShowed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullShowed", [cbParam UTF8String]);
    }
}
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullImpresstion", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullImpresstion", [cbParam UTF8String]);
    }
}
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullClick", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullClick", [cbParam UTF8String]);
    }
}
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullDismissed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullDismissed", [cbParam UTF8String]);
    }
}
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    }
}
- (void)adWillDestroy:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    }
}
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet precisionType:(int)precisionType currencyCode:(NSString *)currencyCode valueMicros:(long)valueMicros
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%d;%@;%ld", placement, adId, adNet, precisionType, currencyCode, valueMicros];
    if (adType == 3) {//ntfull
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBNtFullPaid", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdsFbBridge_NAME, "iOSCBFullPaid", [cbParam UTF8String]);
    }
}

#pragma mark - C API
void FbInitializeNative() {
    [[FbMyiOS sharedInstance] Initialize];
}
//----------------------------------------------
void FbSetCfNtFullNative(int v1, int v2, int v3, int v4, int v5) {
    [[FbMyiOS sharedInstance] setCfNtFullNative:v1 fran:v2 oran:v3 non:v4 increase:v5];
}
void FbSetCfNtFullFbExcluseNative(int rows, int columns, char* areaExcluse) {
    NSString* nsarea = [NSString stringWithUTF8String:areaExcluse];
    [[FbMyiOS sharedInstance] setCfNtFullFbExcluse:rows columns:columns areaExcluse:nsarea];
}
//----------------------------------------------
void FbLoadNtFullNative(char* placement, char* adsId, int orien) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[FbMyiOS sharedInstance] loadNtFullNative:nspl adId:nsadsid orient:orien];
}
void FbReCountCurrShowNative() {
    [[FbMyiOS sharedInstance] recountClose];
}
bool FbShowNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[FbMyiOS sharedInstance] showNtFullNative:nspl timeBtClose:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAutoCLoseWhenClick];
}
//----------------------------------------------
void FbLoadNtIcFullNative(char* placement, char* adsId, int orien) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[FbMyiOS sharedInstance] loadNtIcFullNative:nspl adId:nsadsid orient:orien];
}
bool FbShowNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[FbMyiOS sharedInstance] showNtIcFullNative:nspl timeBtClose:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAutoCLoseWhenClick];
}

@end

