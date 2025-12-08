#import "MyAdmobLowiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "AdmobMyDelegate.h"
#import "AdmobFull.h"

const char* const MyAdmobMyLowBridge_NAME = "AdsAdmobMyLowBridge";

@interface MyAdmobLowiOS () <AdmobMyDelegate>

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobFull*> *dicFull;

@end

@implementation MyAdmobLowiOS

+ (instancetype)sharedInstance
{
    static MyAdmobLowiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyAdmobLowiOS alloc] init];
        // Do any other initialisation stuff here
        [sharedInstance InitData];
    });
    return sharedInstance;
}

-(void) Initialize
{
    
}

-(void) InitData
{
    self.dicFull = [[NSMutableDictionary alloc] init];
}

#pragma Full
- (void)loadFull:(NSString*)placement adId:(NSString*)adsId
{
    AdmobFull *ad = [self.dicFull objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobFull alloc] initAd:placement adDelegate:self];
        [self.dicFull setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
-(bool)showFull:(NSString*)placement timeDelay:(int)timeDelay
{
    AdmobFull *ad = [self.dicFull objectForKey:placement];
    if (ad != nil) {
        return [ad show:timeDelay];
    } else {
        return false;
    }
}


#pragma Cb
- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullLoaded", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, error];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBonFullLoadFail", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%@", placement, adId, adNet, error];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullFailedToShow", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullShowed", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullImpresstion", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullClick", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullDismissed", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
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
    }
}
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet precisionType:(int)precisionType currencyCode:(NSString *)currencyCode valueMicros:(long)valueMicros
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%d;%@;%ld", placement, adId, adNet, precisionType, currencyCode, valueMicros];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyLowBridge_NAME, "iOSCBFullPaid", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    }
}

#pragma mark - C API
void LowInitializeNative() {
    [[MyAdmobLowiOS sharedInstance] Initialize];
}
//----------------------------------------------
void LowLoadOpenAdNative(char* placement, char* adsId) {}
bool LowShowOpenAdNative(char* placement) {return false;}
//----------------------------------------------
bool LowShowBannerNative(char* placement, bool isCollapse, int pos, int width, int orien, bool iPad, float dxCenter) { return false; }
void LowLoadBannerNative(char* placement, bool isCollapse, char* adsId, bool iPad) {}
void LowHideBannerNative() {}
void LowDestroyBannerNative() {}
//----------------------------------------------
bool LowShowBannerClNative(char* placement, int pos, int width, int orien, bool iPad, float dxCenter) { return false; }
void LowLoadBannerClNative(char* placement, char* adsId, bool iPad) {}
void LowHideBannerClNative() {}
void LowDestroyBannerClNative() {}
//----------------------------------------------
bool LowShowBannerRectNative(char* placement, int pos, float width, float dxCenter, float dyVertical) { return false; }
void LowLoadBannerRectNative(char* placement, char* adsId) {}
void LowHideBannerRectNative() {}
void LowDestroyBannerRectNative() {}
//----------------------------------------------
void LowLoadNtFullNative(char* placement, char* adsId, int orien) {}
bool LowShowNtFullNative(char* placement, int timeShowBtClose) {return false;}
//----------------------------------------------
void LowLoadNtIcFullNative(char* placement, char* adsId, int orien) {}
bool LowShowNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {return false;}
//----------------------------------------------
void LowClearCurrFullNative(char* placement) {}
void LowLoadFullNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobLowiOS sharedInstance] loadFull:nspl adId:nsadsid];
}
bool LowShowFullNative(char* placement, int timeDelay) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobLowiOS sharedInstance] showFull:nspl timeDelay:timeDelay];
}
//----------------------------------------------
void LowClearCurrGiftNative(char* placement) {}
void LowLoadGiftNative(char* placement, char* adsId) {}
bool LowShowGiftNative(char* placement) {return false;}

@end
