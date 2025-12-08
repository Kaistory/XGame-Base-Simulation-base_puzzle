#import "MyMaxiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#import "MyMaxUtiliOS.h"
#import "MaxMyDelegate.h"
#import "MaxOpenAd.h"
#import "MaxNtFull.h"

const char* const MyMaxMyBridge_NAME = "AdsMaxMyBridge";

@interface BannerPlacement : NSObject
@property(nonatomic, strong) NSString *pId;
@property BOOL isLoading;
@property BOOL isLoaded;
@property BOOL isShow;
@end

@interface MyMaxiOS () <MaxMyDelegate>

@property(nonatomic, strong) NSMutableDictionary<NSString*, MaxOpenAd*> *dicOpenAd;

@property(nonatomic, strong) NSMutableDictionary<NSString*, MaxNtFull*> *dicNtFull;

@property(nonatomic, weak) MaxNtFull* memAdNtFull4Recount;

@end

@implementation MyMaxiOS

+ (instancetype)sharedInstance
{
    static MyMaxiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyMaxiOS alloc] init];
        // Do any other initialisation stuff here
    });
    return sharedInstance;
}

- (void)Initialize
{
    self.dicOpenAd = [[NSMutableDictionary alloc] init];
    
    self.dicNtFull = [[NSMutableDictionary alloc] init];
}
#pragma OpenAd
- (void)loadOpenAd:(NSString*)placement adId:(NSString*)adsId
{
    MaxOpenAd *ad = [self.dicOpenAd objectForKey:placement];
    if (ad == nil) {
        ad = [[MaxOpenAd alloc] initAd:placement adDelegate:self];
        [self.dicOpenAd setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
-(bool)showOpenAd:(NSString*)placement
{
    MaxOpenAd *ad = [self.dicOpenAd objectForKey:placement];
    if (ad != nil) {
        return [ad show];
    } else {
        return false;
    }
}

#pragma NtFull
- (void)setCfNtFullNative:(int)per fran:(int)fran oran:(int)oran non:(int)nonAfter increase:(int)increase
{
    self.ntFullPer = per;
    self.ntFullFirstRan = fran;
    self.ntFullOtherRan = oran;
    self.ntFullCountNonAfter = nonAfter;
    self.ntFullIncrease = increase;
}
- (void)loadNtFullNative:(NSString*)placement adId:(NSString*)adsId orient:(int)orient
{
    MaxNtFull *ad = [self.dicNtFull objectForKey:placement];
    if (ad == nil) {
        ad = [[MaxNtFull alloc] initAd:placement adParent:self adDelegate:self];
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
    MaxNtFull *ad = [self.dicNtFull objectForKey:placement];
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
- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId netName:(NSString *)netName
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, netName];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullLoaded", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdLoaded", [cbParam UTF8String]);
    }
}
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, @"error"];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullLoadFail", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBonOpenAdLoadFail", [cbParam UTF8String]);
    }
}
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, @"error"];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullFailedToShow", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdFailedToShow", [cbParam UTF8String]);
    }
}
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullShowed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdShowed", [cbParam UTF8String]);
    }
}
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullImpresstion", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdImpresstion", [cbParam UTF8String]);
    }
}
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullClick", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdClick", [cbParam UTF8String]);
    }
}
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullDismissed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdDismissed", [cbParam UTF8String]);
    }
}
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
    }
}
- (void)adWillDestroy:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
    }
}
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet adFormat:(NSString *)format adPlacement:(NSString *)adPlacement netPlacement:(NSString *)netPlacement adValue:(double)value
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%@;%@;%@;%lf", placement, adId, adNet, format, adPlacement, netPlacement, value];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBNtFullPaid", [cbParam UTF8String]);
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage( MyMaxMyBridge_NAME , "iOSCBOpenAdPaid", [cbParam UTF8String]);
    }
}

#pragma mark - C API
void MaxInitializeNative() {
    [[MyMaxiOS sharedInstance] Initialize];
}
//----------------------------------------------
void maxLoadOpenAdNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyMaxiOS sharedInstance] loadOpenAd:nspl adId:nsadsid];
}
bool maxShowOpenAdNative(char* placement) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyMaxiOS sharedInstance] showOpenAd:nspl];
}
//----------------------------------------------
void maxSetCfNtFullNative(int v1, int v2, int v3, int v4, int v5) {
    [[MyMaxiOS sharedInstance] setCfNtFullNative:v1 fran:v2 oran:v3 non:v4 increase:v5];
}
void maxLoadNtFullNative(char* placement, char* adsId, int orien) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyMaxiOS sharedInstance] loadNtFullNative:nspl adId:nsadsid orient:orien];
}
void maxReCountCurrShowNative() {
    [[MyMaxiOS sharedInstance] recountClose];
}
bool maxShowNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyMaxiOS sharedInstance] showNtFullNative:nspl timeBtClose:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAutoCLoseWhenClick];
}
@end

