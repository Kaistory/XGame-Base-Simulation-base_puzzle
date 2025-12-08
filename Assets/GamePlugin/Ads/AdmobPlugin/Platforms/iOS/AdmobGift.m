#import "AdmobGift.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"


@interface AdmobGift() <GADFullScreenContentDelegate>

@property(nonatomic, strong) NSString *adNetLoad;
@property(nonatomic, strong) GADRewardedAd* rewardedAd;

@end

@implementation AdmobGift

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
    NSLog(@"mysdk: ads gift %@ admobnt load=%@", self.placement, adId);
    if (!self.isLoading && !self.isLoaded) {
        self.isLoading = YES;
        GADRequest *request = [GADRequest request];
        [GADRewardedAd
         loadWithAdUnitID:adId
         request:request
         completionHandler:^(GADRewardedAd *ad, NSError *error) {
            if (error) {
                NSLog(@"mysdk: ads gift %@ admobnt load=%@ err=%@", self.placement, adId, [error localizedDescription]);
                self.isLoading = NO;
                self.isLoaded = NO;
                self.rewardedAd = nil;
                [self.adDelegate didFailToReceiveAd:5 placement:self.placement adId:adId withError:[error localizedDescription]];
            } else {
                NSLog(@"mysdk: ads gift %@ admobnt load=%@ ok", self.placement, adId);
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
                self.rewardedAd = ad;
                self.rewardedAd.fullScreenContentDelegate = self;
                [self.adDelegate didReceiveAd:5 placement:self.placement adId:adId adNet:self.adNetLoad];
                id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
                NSString* tmpPl = self.placement;
                __weak AdmobGift *weakSelf = self;
                self.rewardedAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
                    long lva = [[value value] doubleValue] * 1000000000;
                    int precision = (int)[value precision];
                    [tmpDelegate adPaidEvent:5 placement:tmpPl adId:adId adNet:weakSelf.adNetLoad precisionType:precision currencyCode:[value currencyCode] valueMicros:lva];
                };
            }
        }];
    }
}

- (bool)show
{
    NSLog(@"mysdk: ads gift %@ admobnt show", self.placement);
    if (self.rewardedAd != nil) {
        [self.rewardedAd presentFromRootViewController:[MyAdmobUtiliOS unityGLViewController]
                              userDidEarnRewardHandler:^{
            [self.adDelegate adDidReward:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad];
        }];
        return true;
    } else {
        return  false;
    }
}

#pragma CB
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    NSLog(@"mysdk: ads gift %@ admobnt show fail", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adFailPresent:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad withError:[error localizedDescription]];
}
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads gift %@ admobnt show ok", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidPresent:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads gift %@ admobnt Impression", self.placement);
    [self.adDelegate adDidImpression:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads gift %@ admobnt Click", self.placement);
    [self.adDelegate adDidClick:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad];
}
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads gift %@ admobnt Dismiss", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidDismiss:5 placement:self.placement adId:[self.rewardedAd adUnitID] adNet:self.adNetLoad];
}

@end
