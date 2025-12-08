#import "AdmobFull.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"


@interface AdmobFull() <GADFullScreenContentDelegate>

@property(nonatomic, strong) NSString *adNetLoad;
@property(nonatomic, strong) GADInterstitialAd* interstitialAd;

@end

@implementation AdmobFull

- (id)initAd:(NSString *)placement adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.adNetLoad = @"admob";
    
    return self;
}

- (void)load:(NSString *)adId
{
    NSLog(@"mysdk: ads full %@ admobnt load=%@", self.placement, adId);
    if (!self.isLoading && !self.isLoaded) {
        self.isLoading = YES;
        GADRequest *request = [GADRequest request];
        [GADInterstitialAd loadWithAdUnitID:adId
                                    request:request
                          completionHandler:^(GADInterstitialAd *ad, NSError *error) {
            if (error) {
                NSLog(@"mysdk: ads full %@ admobnt load=%@ err=%@", self.placement, adId, [error localizedDescription]);
                self.isLoading = NO;
                self.isLoaded = NO;
                self.interstitialAd = nil;
                [self.adDelegate didFailToReceiveAd:4 placement:self.placement adId:adId withError:[error localizedDescription]];
            } else {
                NSLog(@"mysdk: ads full %@ admobnt load=%@ ok", self.placement, adId);
                if ([ad responseInfo] != nil) {
                    if ([[ad responseInfo] loadedAdNetworkResponseInfo] != nil) {
                        self.adNetLoad = [[[ad responseInfo] loadedAdNetworkResponseInfo] adNetworkClassName];
                        if (self.adNetLoad == nil) {
                            self.adNetLoad = @"admob";
                        }
                    }
                }
                self.isLoading = NO;
                self.isLoaded = YES;
                self.interstitialAd = ad;
                self.interstitialAd.fullScreenContentDelegate = self;
                [self.adDelegate didReceiveAd:4 placement:self.placement adId:adId adNet:self.adNetLoad];
                id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
                NSString* tmpPl = self.placement;
                __weak AdmobFull *weakSelf = self;
                self.interstitialAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
                    long lva = [[value value] doubleValue] * 1000000000;
                    int precision = (int)[value precision];
                    [tmpDelegate adPaidEvent:4 placement:tmpPl adId:adId adNet:weakSelf.adNetLoad precisionType:precision currencyCode:[value currencyCode] valueMicros:lva];
                };
            }
        }];
    }
}

- (bool)show:(int)timeDelay
{
    NSLog(@"mysdk: ads full %@ admobnt show", self.placement);
    if (self.interstitialAd != nil) {
        [self.interstitialAd presentFromRootViewController:[MyAdmobUtiliOS unityGLViewController]];
        return true;
    } else {
        return  false;
    }
}

#pragma CB
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    NSLog(@"mysdk: ads full %@ admobnt show fail", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adFailPresent:4 placement:self.placement adId:[self.interstitialAd adUnitID] adNet:self.adNetLoad withError:[error localizedDescription]];
}
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads full %@ admobnt show ok", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidPresent:4 placement:self.placement adId:[self.interstitialAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads full %@ admobnt Impression", self.placement);
    [self.adDelegate adDidImpression:4 placement:self.placement adId:[self.interstitialAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads full %@ admobnt Click", self.placement);
    [self.adDelegate adDidClick:4 placement:self.placement adId:[self.interstitialAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads full %@ admobnt Dismiss", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidDismiss:4 placement:self.placement adId:[self.interstitialAd adUnitID] adNet:self.adNetLoad];
}

@end
