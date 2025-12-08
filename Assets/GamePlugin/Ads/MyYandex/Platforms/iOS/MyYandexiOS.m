#define Enable_Ads_yandex
#import "MyYandexiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <AVFoundation/AVFoundation.h>

#ifdef Enable_Ads_yandex
#import <YandexMobileAds/YandexMobileAds.h>
#endif

#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>
#include <sys/types.h>
#include <sys/sysctl.h>


#import "MyYandexUtiliOS.h"

const char* const MyYandexBridge_NAME = "AdsMyYandexBridge";

#ifdef Enable_Ads_yandex
@interface MyYandextiOS () <YMAAdViewDelegate, YMAInterstitialAdDelegate, YMAInterstitialAdLoaderDelegate, YMARewardedAdDelegate, YMARewardedAdLoaderDelegate>
#else
@interface MyYandextiOS () <NSObject>
#endif

@property BOOL bannerLoading;
@property BOOL bannerShow;
@property (nonatomic) int bannerOrien;
@property (nonatomic) int bannerPosShow;
@property (nonatomic) float bannerDxCenter;
@property int bannerCurrLvEcpm;
#ifdef Enable_Ads_yandex
@property(nonatomic, strong) YMAAdView *bannerView;
@property(nonatomic, strong) NSMutableDictionary<NSString*, YMAAdView*> *dicBannerView;
@property(nonatomic, strong) UIView *bannerParent;

@property BOOL fullLoading;
@property BOOL fullLoaded;
@property(nonatomic, strong) YMAInterstitialAd *interstitial;
@property(nonatomic, strong) YMAInterstitialAdLoader *interstitialLoader;

@property BOOL giftLoading;
@property BOOL giftLoaded;
@property(nonatomic, strong) YMARewardedAd *rewardedAd;
@property(nonatomic, strong) YMARewardedAdLoader *rewardedAdLoader;
#endif

@end

@implementation MyYandextiOS

+ (instancetype)sharedInstance
{
    static MyYandextiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyYandextiOS alloc] init];
        // Do any other initialisation stuff here
    });
    return sharedInstance;
}

-(void) Initialize
{
}

-(void) setTestMode:(bool)isTestMode
{
    
}
-(void) addTestDevice:(NSString*) deviceId
{
    
}

-(void) setBannerPos:(int)pos width:(int)width dxCenter:(float)dxCenter
{
#ifdef Enable_Ads_yandex
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
#ifdef Enable_Ads_yandex
    NSLog(@"mysdk: ads bn yandex showBanner id=%@ pos=%d width=%d orien=%d iPad=%d", adsId, pos, width, orien, iPad);
    if (self.dicBannerView == nil) {
        self.dicBannerView = [[NSMutableDictionary alloc]init];
    }
    if(adsId == nil) {
        UnitySendMessage(MyYandexBridge_NAME, "iOSCBBannerLoadFail", "adsid nil");
        return;
    }
    self.bannerView = [self.dicBannerView valueForKey:adsId];
    self.bannerShow = YES;
    self.bannerOrien = orien;
    self.bannerPosShow = pos;
    self.bannerDxCenter = dxCenter;
    if (self.bannerView == nil && !self.bannerLoading) {
        UIViewController* vcon = [MyYandexUtiliOS unityGLViewController];
        YMABannerAdSize* sbn;
        if (width < -1) {
            if (!iPad) {
                sbn = [YMABannerAdSize inlineSizeWithWidth:vcon.view.bounds.size.width maxHeight:60];
            } else {
                sbn = [YMABannerAdSize inlineSizeWithWidth:vcon.view.bounds.size.width maxHeight:110];
            }
        } else if (width < 10) {
            if (!iPad) {
                sbn = [YMABannerAdSize fixedSizeWithWidth:320 height:50];
            } else {
                sbn = [YMABannerAdSize fixedSizeWithWidth:728 height:90];
            }
        } else {
            if (!iPad) {
                sbn = [YMABannerAdSize inlineSizeWithWidth:width maxHeight:50];
            } else {
                sbn = [YMABannerAdSize inlineSizeWithWidth:width maxHeight:95];
            }
        }
        NSLog(@"mysdk: ads bn yandex showBanner create and load size=%f,%f", sbn.size.width, sbn.size.height);
        self.bannerView = [[YMAAdView alloc] initWithAdUnitID:adsId adSize:sbn];
        if (self.bannerParent == nil) {
            float xbn = (vcon.view.bounds.size.width - self.bannerView.bounds.size.width)/2 + dxCenter*vcon.view.bounds.size.width;
            if (pos == 0) {
                self.bannerParent = [[UIView alloc] initWithFrame:CGRectMake(xbn, 0, sbn.size.width, sbn.size.height)];
            } else {
                self.bannerParent = [[UIView alloc] initWithFrame:CGRectMake(xbn, vcon.view.bounds.size.height - sbn.size.height, sbn.size.width, sbn.size.height)];
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
//        self.bannerView.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//            long lva = [[value value] doubleValue] * 1000000000;
//            NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//            UnitySendMessage(yandexBridge_NAME, "iOSCBBannerPaid", [paidParam UTF8String]);
//        };
        self.bannerView.hidden = NO;
        self.bannerLoading = YES;
        [self setPosBanner4Show:self.bannerParent pos:pos dxCenter:dxCenter];
        [self.bannerView loadAd];
    } else {
        if (self.bannerView != nil) {
            NSLog(@"mysdk: ads bn yandex showBanner loaded and show");
            if (self.bannerParent != nil) {
                self.bannerParent.hidden = NO;
            }
            if (self.bannerView != nil) {
                self.bannerView.hidden = NO;
            }
            [self setPosBanner4Show:self.bannerParent pos:pos dxCenter:dxCenter];
        } else {
            NSLog(@"mysdk: ads bn yandex showBanner bannerView nil");
        }
    }
#endif
}
-(void)setPosBanner4Show:(UIView*)view pos:(int)pos dxCenter:(float)dxCenter
{
#ifdef Enable_Ads_yandex
    UIView *unityView = [MyYandexUtiliOS unityGLViewController].view;
    int wbn = self.bannerView.bounds.size.width;
    int hbn = self.bannerView.bounds.size.height;
    NSLog(@"mysdk: ads bn yandex setPosBanner4Show bnw=%d bnh=%d", wbn, hbn);
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
    float xbn = (wscr - wbn)/2 + dxCenter*wscr;
    if (pos == 0) {
        float safetop = 0;
        if (self.bannerOrien == 0) {
            if (@available(iOS 11.0, *)) {
                safetop = unityView.safeAreaInsets.top / 1;
            }
        }
        view.frame = CGRectMake(xbn, safetop, wbn, hbn);
        NSLog(@"mysdk: ads bn yandex setPosBanner4Show xbn=%f safetop=%f", xbn, safetop);
    } else {
        float safebot = 0;
        if (self.bannerOrien == 0) {
            if (@available(iOS 11.0, *)) {
                safebot = unityView.safeAreaInsets.bottom / 1;
            }
        }
        view.frame = CGRectMake(xbn, unityView.bounds.size.height - hbn - safebot, wbn, hbn);
        NSLog(@"mysdk: ads bn yandex setPosBanner4Show xbn=%f safebot=%f", xbn, safebot);
    }
#endif
}

-(void)hideBanner
{
#ifdef Enable_Ads_yandex
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
#ifdef Enable_Ads_yandex
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
#ifdef Enable_Ads_yandex
    NSLog(@"mysdk: ads full yandex loadFull id=%@", adsId);
    if (self.interstitial == nil && self.fullLoading == NO && self.fullLoaded == NO) {
        self.fullLoaded = NO;
        self.fullLoading = YES;
        if (self.interstitialLoader == nil) {
            self.interstitialLoader = [[YMAInterstitialAdLoader alloc] init];
            self.interstitialLoader.delegate = self;
        }
        YMAAdRequestConfiguration* rq = [[YMAAdRequestConfiguration alloc] initWithAdUnitID:adsId];
        [self.interstitialLoader loadAdWithRequestConfiguration:rq];
    }
#endif
}

-(bool) showFull
{
#ifdef Enable_Ads_yandex
    NSLog(@"mysdk: ads full yandex show full");
    if (self.interstitial != nil) {
        [self.interstitial showFromViewController:[MyYandexUtiliOS unityGLViewController]];
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
#ifdef Enable_Ads_yandex
    NSLog(@"mysdk: ads gift yandex load gift id=%@", adsId);
    if (self.rewardedAd == nil && self.giftLoading == NO && self.giftLoaded == NO) {
        self.giftLoaded = NO;
        self.giftLoading = YES;
        if (self.rewardedAdLoader == nil) {
            self.rewardedAdLoader = [[YMARewardedAdLoader alloc] init];
            self.rewardedAdLoader.delegate = self;
        }
        YMAAdRequestConfiguration* rq = [[YMAAdRequestConfiguration alloc] initWithAdUnitID:adsId];
        [self.rewardedAdLoader loadAdWithRequestConfiguration:rq];
    }
#endif
}

-(bool) showGift
{
#ifdef Enable_Ads_yandex
    NSLog(@"mysdk: ads gift yandex show gift");
    if (self.rewardedAd) {
        [self.rewardedAd showFromViewController:[MyYandexUtiliOS unityGLViewController]];
        return true;
    } else {
        return false;
    }
#else
    return false;
#endif
}

//===========================Banner=========================================
#ifdef Enable_Ads_yandex
- (void)adViewDidLoad:(YMAAdView *)adView
{
    NSLog(@"mysdk: ads bn yandexViewDidReceiveAd=%@", [adView adUnitID]);
    self.bannerLoading = NO;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBBannerLoaded", "");
    YMAAdView *bn = [self.dicBannerView valueForKey:[adView adUnitID]];
    if (bn == nil) {
        if(self.bannerView != adView) {
            if (self.bannerView != nil) {
                NSLog(@"mysdk: ads bn yandexViewDidReceiveAd banner new load != banner curr");
            } else {
                NSLog(@"mysdk: ads bn yandexViewDidReceiveAd banner new load curr nil");
            }
        }
        NSLog(@"mysdk: ads bn yandexViewDidReceiveAd new load curr = load");
        self.bannerView = adView;
        [_dicBannerView setValue:adView forKey:[adView adUnitID]];
        [self.bannerView removeFromSuperview];
        [self.bannerParent addSubview:self.bannerView];
        [self setPosBanner4Show:self.bannerParent pos:self.bannerPosShow dxCenter:self.bannerDxCenter];
    } else {
        // Add the new banner view.
        if(self.bannerView == adView) {
            [self.bannerView removeFromSuperview];
            [self.bannerParent addSubview:self.bannerView];
            [self setPosBanner4Show:self.bannerParent pos:self.bannerPosShow dxCenter:self.bannerDxCenter];
            NSLog(@"mysdk: ads bn yandexViewDidReceiveAd old load curr = load");
        } else {
            if (self.bannerView != nil) {
                NSLog(@"mysdk: ads bn yandexViewDidReceiveAd old load curr != load, cur=%@ load=%@", [adView adUnitID], [self.bannerView adUnitID]);
            } else {
                NSLog(@"mysdk: ads bn yandexViewDidReceiveAd old load curr != load curr nil");
                self.bannerView = adView;
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

- (void)adViewDidFailLoading:(YMAAdView *)adView error:(NSError *)error
{
    NSLog(@"mysdk: ads bn yandexView:didFailToReceiveAdWithError: %@", error);
    self.bannerLoading = NO;
    YMAAdView *bn = [self.dicBannerView valueForKey:[adView adUnitID]];
    if (bn == nil) {
        [adView removeFromSuperview];
        if (adView == self.bannerView) {
            self.bannerView = nil;
        }
    }
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBBannerLoadFail", "-1" );
}

- (void)adViewDidClick:(YMAAdView *)adView
{
    NSLog(@"mysdk: ads bn yandex adViewDidClick");
}

- (void)adViewWillLeaveApplication:(YMAAdView *)adView
{
    NSLog(@"mysdk: ads bn yandex adViewWillLeaveApplication");
}

- (void)adView:(YMAAdView *)adView willPresentScreen:(nullable UIViewController *)viewController
{
    NSLog(@"mysdk: ads bn yandexViewWillPresentScreen");
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBBannerOpen", "");
}

- (void)adView:(YMAAdView *)adView didDismissScreen:(nullable UIViewController *)viewController
{
    NSLog(@"mysdk: ads bn yandex didDismissScreen");
}

- (void)adView:(YMAAdView *)adView didTrackImpressionWithData:(nullable id<YMAImpressionData>)impressionData
{
    NSLog(@"mysdk: ads bn yandex didTrackImpressionWithData");
}

//===========================full=========================================
- (void)interstitialAdLoader:(YMAInterstitialAdLoader *)adLoader
                     didLoad:(YMAInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full yandex onLoadWithInterstitialAd");
    self.fullLoaded = YES;
    self.fullLoading = NO;
    self.interstitial = interstitialAd;
    self.interstitial.delegate = self;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBFullLoaded", "");
//    self.interstitial.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//        long lva = [[value value] doubleValue] * 1000000000;
//        NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//        UnitySendMessage(MyYandexBridge_NAME, "iOSCBFullPaid", [paidParam UTF8String]);
//    };
}

- (void)interstitialAdLoader:(YMAInterstitialAdLoader *)adLoader
      didFailToLoadWithError:(YMAAdRequestError *)error
{
    NSLog(@"mysdk: ads full yandex onNoAdWithReason: %@", error);
    self.fullLoaded = NO;
    self.fullLoading = NO;
    self.interstitial = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBonFullLoadFail", "-1");
}

- (void)interstitialAd:(YMAInterstitialAd *)interstitialAd didFailToShowWithError:(NSError *)error
{
    NSLog(@"mysdk: ads full yandex didFailToShowWithError: %@", error);
    self.fullLoaded = NO;
    self.interstitial = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBFullFailedToShow", "-1");
}

- (void)interstitialAdDidShow:(YMAInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full yandex onDisplayWithInterstitialAd");
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBFullShowed", "");
}

- (void)interstitialAdDidDismiss:(YMAInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full yandex onCloseWithInterstitialAd");
    self.fullLoaded = NO;
    self.interstitial = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBFullDismissed", "");
}

- (void)interstitialAdDidClick:(YMAInterstitialAd *)interstitialAd
{
    NSLog(@"mysdk: ads full yandex onClickWithInterstitialAd");
}

- (void)interstitialAd:(YMAInterstitialAd *)interstitialAd
didTrackImpressionWithData:(nullable id<YMAImpressionData>)impressionData
{
    NSLog(@"mysdk: ads full yandex didTrackImpressionWithData");
}

//===========================Gift=========================================
- (void)rewardedAdLoader:(YMARewardedAdLoader *)adLoader
                 didLoad:(YMARewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift yandex onLoadWithRewardedAd");
    self.giftLoaded = YES;
    self.giftLoading = NO;
    self.rewardedAd = rewardedAd;
    self.rewardedAd.delegate = self;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftLoaded", "");
//    self.rewardedAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//        long lva = [[value value] doubleValue] * 1000000000;
//        NSString* paidParam = [NSString stringWithFormat:@"%ld;%@;%ld", [value precision], [value currencyCode], lva];
//        UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftPaid", [paidParam UTF8String]);
//    };
}

- (void)rewardedAdLoader:(YMARewardedAdLoader *)adLoader
  didFailToLoadWithError:(YMAAdRequestError *)error;
{
    NSLog(@"mysdk: ads gift yandex onNoAdWithReason: %@", error);
    self.giftLoaded = NO;
    self.giftLoading = NO;
    self.rewardedAd = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftLoadFail", "-1");
}

- (void)rewardedAd:(YMARewardedAd *)rewardedAd didReward:(id<YMAReward>)reward
{
    NSLog(@"mysdk: ads gift yandex onReward");
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftReward", "");
}

- (void)rewardedAd:(YMARewardedAd *)rewardedAd didFailToShowWithError:(NSError *)error
{
    self.giftLoaded = NO;
    self.rewardedAd = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftFailedToShow", "-1");
}

- (void)rewardedAdDidShow:(YMARewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift yandex onDisplayWithRewardedAd");
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftShowed", "");
}

- (void)rewardedAdDidDismiss:(YMARewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift yandex onCloseWithRewardedAd");
    self.giftLoaded = NO;
    self.rewardedAd = nil;
    UnitySendMessage(MyYandexBridge_NAME, "iOSCBGiftDismissed", "");
}

- (void)rewardedAdDidClick:(YMARewardedAd *)rewardedAd
{
    NSLog(@"mysdk: ads gift yandex onClickWithRewardedAd");
}

- (void)rewardedAd:(YMARewardedAd *)rewardedAd
didTrackImpressionWithData:(nullable id<YMAImpressionData>)impressionData
{
    NSLog(@"mysdk: ads gift yandex didTrackImpressionWithData");
}
#endif

#pragma mark - C API
void myYandexInitializeNative() {
    [[MyYandextiOS sharedInstance] Initialize];
}

void myYandexSetTestModeNative(bool isTestMode)
{
    [[MyYandextiOS sharedInstance] setTestMode:isTestMode];
}
void myYandexAddTestDeviceNative(char* deviceId)
{
    NSString* nsdvid = [NSString stringWithUTF8String:deviceId];
    [[MyYandextiOS sharedInstance] addTestDevice:nsdvid];
}

void myYandexSetBannerPosNative(int pos, int width, float dxcenter) {
    [[MyYandextiOS sharedInstance] setBannerPos:pos width:width dxCenter:dxcenter];
}
void myYandexShowBannerNative(char* adsId, int pos, int width, int orien, bool iPad, float dxcenter) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyYandextiOS sharedInstance] showBanner:nsadsid pos:pos width:width orien:orien iPad:iPad dxCenter:dxcenter];
}
void myYandexHideBannerNative() {
    [[MyYandextiOS sharedInstance] hideBanner];
}
void myYandexClearCurrFullNative() {
    
}
void myYandexLoadFullNative(char* adsId) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyYandextiOS sharedInstance] loadFull:nsadsid];
}
bool myYandexShowFullNative() {
    return [[MyYandextiOS sharedInstance] showFull];
}
void myYandexClearCurrGiftNative() {
    
}
void myYandexLoadGiftNative(char* adsId) {
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyYandextiOS sharedInstance] loadGift:nsadsid];
}
bool myYandexShowGiftNative() {
    return [[MyYandextiOS sharedInstance] showGift];
}

@end
