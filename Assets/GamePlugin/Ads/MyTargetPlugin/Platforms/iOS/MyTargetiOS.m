#define ENABLE_ADS_MYTARGET
#import "MyTargetiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#import "MyTargetUtiliOS.h"
#ifdef ENABLE_ADS_MYTARGET
#import <MyTargetSDK/MyTargetSDK.h>
#endif

const char* const MyTargetBridge_NAME = "AdsMyTargetBridge";

#ifdef ENABLE_ADS_MYTARGET
@interface MyTargetiOS () <MTRGAdViewDelegate, MTRGInterstitialAdDelegate, MTRGRewardedAdDelegate>
#else
@interface MyTargetiOS ()
#endif
@property BOOL bannerLoading;
@property BOOL bannerShow;
@property (nonatomic) int bannerOrien;
@property (nonatomic) int bannerPosShow;
@property (nonatomic) float bannerDxCenter;
@property int bannerCurrLvEcpm;
#ifdef ENABLE_ADS_MYTARGET
@property(nonatomic, strong) MTRGAdView *bannerView;
@property(nonatomic, strong) NSMutableDictionary<NSString*, MTRGAdView*> *dicBannerView;
#endif
@property(nonatomic, strong) UIView *bannerParent;

@property BOOL fullLoading;
@property BOOL fullLoaded;
#ifdef ENABLE_ADS_MYTARGET
@property(nonatomic, strong) MTRGInterstitialAd *interstitial;
#endif

@property BOOL giftLoading;
@property BOOL giftLoaded;
#ifdef ENABLE_ADS_MYTARGET
@property(nonatomic, strong) MTRGRewardedAd *rewardedAd;
#endif
@end

@implementation MyTargetiOS

+ (instancetype)sharedInstance
{
    static MyTargetiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyTargetiOS alloc] init];
        // Do any other initialisation stuff here
    });
    return sharedInstance;
}

-(void) Initialize
{
    
}

-(void) setTestMode:(bool)isTestMode
{
#ifdef ENABLE_ADS_MYTARGET
    [MTRGManager setDebugMode:isTestMode];
#endif
}
-(void) addTestDevice:(NSString*) deviceId
{
#ifdef ENABLE_ADS_MYTARGET
    if (deviceId != nil && [deviceId length] > 3) {
        [MTRGConfig load];
        [self setTestMode:true];
        deviceId = @"1C76830E-C4F9-4D27-80A4-BF5D6687B421";
        [MTRGManager setSdkConfig:[[[MTRGConfig newBuilder] withTestDevices:[deviceId componentsSeparatedByString:@","]] build]];
    }
#endif
}

-(void) setBannerPos:(int)pos width:(int)width dxCenter:(float)dxCenter
{
#ifdef ENABLE_ADS_MYTARGET
    self.bannerShow = YES;
    self.bannerPosShow = pos;
    self.bannerDxCenter = dxCenter;
    if (self.bannerParent != nil) {
        self.bannerParent.hidden = NO;
    }
    if (self.bannerView != nil) {
        self.bannerView.hidden = NO;
    }
    [self setPosBanner4Show:self.bannerParent pos:pos dxCenter:dxCenter];
#endif
}

-(void) showBanner:(NSString*)adsId pos:(int)pos width:(int)width orien:(int)orien iPad:(bool)iPad dxCenter:(float)dxCenter
{
#ifdef ENABLE_ADS_MYTARGET
    NSLog(@"mysdk: ads bn mytarget showBanner id=%@ pos=%d ipad=%d", adsId, pos, iPad);
    if (self.dicBannerView == nil) {
        self.dicBannerView = [[NSMutableDictionary alloc]init];
    }
    if(adsId == nil) {
        UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerLoadFail", "adsid nil");
        return;
    }
    NSUInteger islotId = 0;
    if (adsId != nil && [adsId length] > 3)
    {
        islotId = [adsId integerValue];
    }
    self.bannerView = [self.dicBannerView valueForKey:adsId];
    self.bannerShow = YES;
    self.bannerOrien = orien;
    self.bannerPosShow = pos;
    self.bannerDxCenter = dxCenter;
    if (self.bannerView == nil && !self.bannerLoading) {
        NSLog(@"mysdk: ads bn mytarget showBanner create and load");
        UIViewController* vcon = [MyTargetUtiliOS unityGLViewController];
        self.bannerView = [MTRGAdView adViewWithSlotId:islotId];
        MTRGAdSize* sbn;
        if (width < -1) {
            if (!iPad) {
                sbn = [MTRGAdSize adSizeForCurrentOrientationForWidth:vcon.view.bounds.size.width maxHeight:60];
            } else {
                sbn = [MTRGAdSize adSizeForCurrentOrientationForWidth:vcon.view.bounds.size.width maxHeight:110];
            }
        } else if (width < 10) {
            if (!iPad) {
                sbn = [MTRGAdSize adSize320x50];
            } else {
                sbn = [MTRGAdSize adSize728x90];
            }
        } else {
            if (!iPad) {
                sbn = [MTRGAdSize adSizeForCurrentOrientationForWidth:width maxHeight:50];
            } else {
                sbn = [MTRGAdSize adSizeForCurrentOrientationForWidth:width maxHeight:95];
            }
        }
        self.bannerView.adSize = sbn;
        if (self.bannerParent == nil) {
            float xbn = (vcon.view.bounds.size.width - self.bannerView.bounds.size.width)/2 + dxCenter*vcon.view.bounds.size.width;
            if (pos == 0) {
                self.bannerParent = [[UIView alloc] initWithFrame:CGRectMake(xbn, 0, vcon.view.bounds.size.width, self.bannerView.bounds.size.height)];
            } else {
                self.bannerParent = [[UIView alloc] initWithFrame:CGRectMake(xbn, vcon.view.bounds.size.height - self.bannerView.bounds.size.height, vcon.view.bounds.size.width, self.bannerView.bounds.size.height)];
            }
            // self.bannerParent.backgroundColor = [UIColor redColor];
            //self.bannerParent.userInteractionEnabled = NO;
            self.bannerParent.clipsToBounds = true;
            [vcon.view addSubview:self.bannerParent];
        } else {
            self.bannerParent.hidden = NO;
        }
        self.bannerView.translatesAutoresizingMaskIntoConstraints = NO;
        self.bannerView.delegate = self;
        self.bannerView.viewController = vcon;
//        self.bannerView.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//            long lva = [[value value] doubleValue] * 1000000000;
//            NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//            UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerPaid", [paidParam UTF8String]);
//        };
        self.bannerView.hidden = NO;
        self.bannerLoading = YES;
        [self setPosBanner4Show:self.bannerParent pos:pos dxCenter:dxCenter];
        [self.bannerView load];
    } else {
        if (self.bannerView != nil) {
            NSLog(@"mysdk: ads bn mytarget showBanner loaded and show");
            if (self.bannerParent != nil) {
                self.bannerParent.hidden = NO;
            }
            if (self.bannerView != nil) {
                self.bannerView.hidden = NO;
            }
            [self setPosBanner4Show:self.bannerParent pos:pos dxCenter:dxCenter];
        } else {
            NSLog(@"mysdk: ads bn mytarget showBanner bannerView nil");
        }
    }
#endif
}
-(void)setPosBanner4Show:(UIView*)view pos:(int)pos dxCenter:(float)dxCenter
{
#ifdef ENABLE_ADS_MYTARGET
    UIView *unityView = [MyTargetUtiliOS unityGLViewController].view;
    int wbn = self.bannerView.adSize.size.width;
    int hbn = self.bannerView.adSize.size.height;
    self.bannerView.frame = CGRectMake(0, 0, wbn, hbn);
    float wscr = unityView.bounds.size.width;
    if (self.bannerOrien == 0) {
        if (wscr > unityView.bounds.size.height) {
            wscr = unityView.bounds.size.height;
        }
    } else {
        if (wscr < unityView.bounds.size.height) {
            wscr = unityView.bounds.size.height;
        }
    }
    float xbn = (wscr - wbn) / 2 + dxCenter * wscr;
    if (pos == 0) {
        float safetop = 0;
        if (self.bannerOrien == 0) {
            if (@available(iOS 11.0, *)) {
                safetop = unityView.safeAreaInsets.top / 1;
            }
        }
        view.frame = CGRectMake(xbn, safetop, wbn, hbn);
        NSLog(@"mysdk: ads bn mytarget setPosBanner4Show xbn=%f safetop=%f w=%d h=%d", xbn, safetop, wbn, hbn);
    } else {
        float safebot = 0;
        if (self.bannerOrien == 0) {
            if (@available(iOS 11.0, *)) {
                safebot = unityView.safeAreaInsets.bottom / 1;
            }
        }
        view.frame = CGRectMake(xbn, unityView.bounds.size.height - hbn - safebot, wbn, hbn);
        NSLog(@"mysdk: ads bn mytarget setPosBanner4Show xbn=%f safebot=%f w=%d h=%d", xbn, safebot, wbn, hbn);
    }
#endif
}

-(void)hideBanner
{
#ifdef ENABLE_ADS_MYTARGET
    self.bannerShow = NO;
    if (self.bannerParent != nil) {
        self.bannerParent.hidden = YES;
    }
    if (self.bannerView != nil) {
        self.bannerView.hidden = YES;
    }
#endif
}

-(void) destroyBanner
{
#ifdef ENABLE_ADS_MYTARGET
    self.bannerShow = NO;
    self.bannerLoading = NO;
    if (self.bannerParent != nil) {
        self.bannerParent.hidden = YES;
    }
    if (self.bannerView == nil) {
        self.bannerView.hidden = YES;
        [self.bannerView removeFromSuperview];
        self.bannerView = nil;
    }
#endif
}

-(void) loadFull:(NSString*)adsId
{
#ifdef ENABLE_ADS_MYTARGET
    NSLog(@"mysdk: ads full mytarget loadFull id=%@", adsId);
    if (self.interstitial == nil && self.fullLoading == NO && self.fullLoaded == NO) {
        self.fullLoaded = NO;
        self.fullLoading = YES;
        NSUInteger islotId = 0;
        if (adsId != nil && [adsId length] > 3)
        {
            islotId = [adsId integerValue];
        }
        self.interstitial = [MTRGInterstitialAd interstitialAdWithSlotId:islotId];
        self.interstitial.delegate = self;
        [self.interstitial load];
    }
#endif
}

-(bool) showFull
{
#ifdef ENABLE_ADS_MYTARGET
    NSLog(@"mysdk: ads full mytarget show full");
    if (self.interstitial != nil) {
        [self.interstitial showWithController:[MyTargetUtiliOS unityGLViewController]];
        return true;
    } else {
        return false;
    }
#else
    return false;
#endif
}

-(void) loadGift:(NSString*)adsId
{
#ifdef ENABLE_ADS_MYTARGET
    NSLog(@"mysdk: ads gift mytarget load id=%@", adsId);
    if (self.rewardedAd == nil && self.giftLoading == NO && self.giftLoaded == NO) {
        self.giftLoaded = NO;
        self.giftLoading = YES;
        NSUInteger islotId = 0;
        if (adsId != nil && [adsId length] > 3)
        {
            islotId = [adsId integerValue];
        }
        self.rewardedAd = [MTRGRewardedAd rewardedAdWithSlotId:islotId];
        self.rewardedAd.delegate = self;
        [self.rewardedAd load];
    }
#endif
}

-(bool) showGift
{
#ifdef ENABLE_ADS_MYTARGET
    NSLog(@"mysdk: ads gift mytarget show");
    if (self.rewardedAd) {
        [self.rewardedAd showWithController:[MyTargetUtiliOS unityGLViewController]];
        return true;
    } else {
        return false;
    }
#else
    return false;
#endif
}

#ifdef ENABLE_ADS_MYTARGET
#pragma Cb Banner
- (void)onLoadWithAdView:(MTRGAdView *)bannerView {
    NSString *NSSlotId = [NSString stringWithFormat: @"%ld", [bannerView slotId]];
    NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd=%@", NSSlotId);
    self.bannerLoading = NO;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerLoaded", "");
    MTRGAdView *bn = [self.dicBannerView valueForKey:NSSlotId];
    if (bn == nil) {
        if(self.bannerView != bannerView) {
            if (self.bannerView != nil) {
                NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd banner new load != banner curr");
            } else {
                NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd banner new load curr nil");
            }
        }
        NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd new load curr = load");
        self.bannerView = bannerView;
        [_dicBannerView setValue:bannerView forKey:NSSlotId];
        [self.bannerView removeFromSuperview];
        [self.bannerParent addSubview:self.bannerView];
        [self setPosBanner4Show:self.bannerParent pos:self.bannerPosShow dxCenter:self.bannerDxCenter];
    } else {
        // Add the new banner view.
        if(self.bannerView == bannerView) {
            [self.bannerView removeFromSuperview];
            [self.bannerParent addSubview:self.bannerView];
            [self setPosBanner4Show:self.bannerParent pos:self.bannerPosShow dxCenter:self.bannerDxCenter];
            NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd old load curr = load");
        } else {
            if (self.bannerView != nil) {
                NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd old load curr != load, cur=%ld load=%ld", [bannerView slotId], [self.bannerView slotId]);
            } else {
                NSLog(@"mysdk: ads bn mytarget ViewDidReceiveAd old load curr != load curr nil");
                self.bannerView = bannerView;
            }
        }
    }
    if (!self.bannerShow) {
        if (self.bannerParent != nil) {
            self.bannerParent.hidden = YES;
        }
        if (self.bannerView != nil) {
            self.bannerView.hidden = YES;
        }
    }
}

- (void)onLoadFailedWithError:(NSError *)error adView:(MTRGAdView *)bannerView {
    NSString *NSSlotId = [NSString stringWithFormat: @"%ld", [bannerView slotId]];
    NSLog(@"mysdk: ads bn mytarget didFailToReceiveAdWithError: %@", error);
    self.bannerLoading = NO;
    MTRGAdView *bn = [self.dicBannerView valueForKey:NSSlotId];
    if (bn == nil) {
        [bannerView removeFromSuperview];
        if (bannerView == self.bannerView) {
            self.bannerView = nil;
        }
    }
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerLoadFail", "-1" );
}

- (void)onAdShowWithAdView:(MTRGAdView *)bannerView {
    NSLog(@"mysdk: ads bn mytarget WillPresentScreen");
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerOpen", "");
}

- (void)onAdClickWithAdView:(MTRGAdView *)bannerView
{
}

- (void)onShowModalWithAdView:(MTRGAdView *)bannerView
{
}

- (void)onDismissModalWithAdView:(MTRGAdView *)adView
{
    NSLog(@"mysdk: ads bn mytarget ViewDidDismissScreen");
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBBannerClose", "");
}

- (void)onLeaveApplicationWithAdView:(MTRGAdView *)adView
{
}

#pragma CB full
- (void)onLoadWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full mytarget onLoadWithInterstitialAd");
    self.fullLoaded = YES;
    self.fullLoading = NO;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBFullLoaded", "");
//    self.interstitial.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//        long lva = [[value value] doubleValue] * 1000000000;
//        NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//        UnitySendMessage(MyTargetBridge_NAME, "iOSCBFullPaid", [paidParam UTF8String]);
//    };
}
  
- (void)onLoadFailedWithError:(NSError *)error interstitialAd:(MTRGInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full mytarget onNoAdWithReason: %@", error);
    self.fullLoaded = NO;
    self.fullLoading = NO;
    self.interstitial = nil;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBonFullLoadFail", "-1");
}

/// Tells the delegate that the ad presented full screen content.
- (void)onDisplayWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd {
    NSLog(@"mysdk: ads full mytarget onDisplayWithInterstitialAd");
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBFullShowed", "");
}

- (void)onClickWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full mytarget onClickWithInterstitialAd");
}

/// Tells the delegate that the ad dismissed full screen content.
- (void)onCloseWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd {
    NSLog(@"mysdk: ads full mytarget onCloseWithInterstitialAd");
    self.fullLoaded = NO;
    self.interstitial = nil;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBFullDismissed", "");
}

- (void)onLeaveApplicationWithInterstitialAd:(MTRGInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full mytarget onLeaveApplicationWithInterstitialAd");
}

#pragma CB gift
- (void)onLoadWithRewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onLoadWithRewardedAd");
    self.giftLoaded = YES;
    self.giftLoading = NO;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftLoaded", "");
//    self.rewardedAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//        long lva = [[value value] doubleValue] * 1000000000;
//        NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//        UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftPaid", [paidParam UTF8String]);
//    };
}
 
- (void)onLoadFailedWithError:(NSError *)error rewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onNoAdWithReason: %@", error);
    self.giftLoaded = NO;
    self.giftLoading = NO;
    self.rewardedAd = nil;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftLoadFail", "-1");
}

- (void)onDisplayWithRewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onDisplayWithRewardedAd");
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftShowed", "");
}
 
- (void)onReward:(MTRGReward *)reward rewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onReward");
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftReward", "");
}
 
- (void)onClickWithRewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onClickWithRewardedAd");
}
 
- (void)onCloseWithRewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onCloseWithRewardedAd");
    self.giftLoaded = NO;
    self.rewardedAd = nil;
    UnitySendMessage(MyTargetBridge_NAME, "iOSCBGiftDismissed", "");
}
 
- (void)onLeaveApplicationWithRewardedAd:(MTRGRewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift mytarget onLeaveApplicationWithRewardedAd");
}
#endif

#pragma mark - C API
void myTargetInitializeNative() {
    [[MyTargetiOS sharedInstance] Initialize];
}

void myTargetSetTestModeNative(bool isTestMode)
{
    [[MyTargetiOS sharedInstance] setTestMode:isTestMode];
}
void myTargetAddTestDeviceNative(char* deviceId)
{
    NSString* nsdvid = [NSString stringWithUTF8String:deviceId];
    [[MyTargetiOS sharedInstance] addTestDevice:nsdvid];
}

void myTargetSetBannerPosNative(int pos, int width, float dxcenter) {
    [[MyTargetiOS sharedInstance] setBannerPos:pos width:width dxCenter:dxcenter];
}
void myTargetShowBannerNative(char* adsId, int pos, int width, int orien, bool iPad, float dxcenter) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyTargetiOS sharedInstance] showBanner:nsadsid pos:pos width:width orien:orien iPad:iPad dxCenter:dxcenter];
}
void myTargetHideBannerNative() {
    [[MyTargetiOS sharedInstance] hideBanner];
}
void myTargetClearCurrFullNative() {
    
}
void myTargetLoadFullNative(char* adsId) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyTargetiOS sharedInstance] loadFull:nsadsid];
}
bool myTargetShowFullNative() {
    return [[MyTargetiOS sharedInstance] showFull];
}
void myTargetClearCurrGiftNative() {
    
}
void myTargetLoadGiftNative(char* adsId) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyTargetiOS sharedInstance] loadGift:nsadsid];
}
bool myTargetShowGiftNative() {
    return [[MyTargetiOS sharedInstance] showGift];
}

@end

