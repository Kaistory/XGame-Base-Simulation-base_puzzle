#import "AdmobOpenAd.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"


@interface AdmobOpenAd() <GADFullScreenContentDelegate>

@property(nonatomic, strong) GADAppOpenAd* appOpenAd;
@property(nonatomic, strong) NSString* adUnitId;
@property(nonatomic, strong) NSString *adNetLoad;

@end

@implementation AdmobOpenAd

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
    NSLog(@"mysdk: ads openad %@ admobnt load=%@", self.placement, adId);
    if (!self.isLoading && !self.isLoaded) {
        self.isLoading = YES;
        GADRequest *request = [GADRequest request];
        [GADAppOpenAd loadWithAdUnitID:adId
                                    request:request
                          completionHandler:^(GADAppOpenAd *ad, NSError *error) {
            if (error) {
                NSLog(@"mysdk: ads openad %@ admobnt load=%@ err=%@", self.placement, adId, [error localizedDescription]);
                self.isLoading = NO;
                self.isLoaded = NO;
                self.appOpenAd = nil;
                [self.adDelegate didFailToReceiveAd:7 placement:self.placement adId:adId withError:[error localizedDescription]];
            } else {
                NSLog(@"mysdk: ads openad %@ admobnt load=%@ ok", self.placement, adId);
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
                self.appOpenAd = ad;
                self.adUnitId = adId;
                self.appOpenAd.fullScreenContentDelegate = self;
                [self.adDelegate didReceiveAd:7 placement:self.placement adId:adId adNet:self.adNetLoad];
                id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
                NSString* tmpPl = self.placement;
                __weak AdmobOpenAd *weakSelf = self;
                self.appOpenAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
                    long lva = [[value value] doubleValue] * 1000000000;
                    int precision = (int)[value precision];
                    [tmpDelegate adPaidEvent:7 placement:tmpPl adId:adId adNet:weakSelf.adNetLoad precisionType:precision currencyCode:[value currencyCode] valueMicros:lva];
                };
            }
        }];
    }
}

- (bool)show
{
    NSLog(@"mysdk: ads openad %@ admobnt show", self.placement);
    if (self.appOpenAd != nil) {
        [self.appOpenAd presentFromRootViewController:[MyAdmobUtiliOS unityGLViewController]];
        return true;
    } else {
        return  false;
    }
}

#pragma CB
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    NSLog(@"mysdk: ads openad %@ admobnt show fail", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adFailPresent:7 placement:self.placement adId:self.adUnitId adNet:self.adNetLoad withError:[error localizedDescription]];
}
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads openad %@ admobnt show ok", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidPresent:7 placement:self.placement adId:self.adUnitId adNet:self.adNetLoad];
}
- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads openad %@ admobnt Impression", self.placement);
    [self.adDelegate adDidImpression:7 placement:self.placement adId:self.adUnitId adNet:self.adNetLoad];
}
- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSLog(@"mysdk: ads openad %@ admobnt Click", self.placement);
    [self.adDelegate adDidClick:7 placement:self.placement adId:self.adUnitId adNet:self.adNetLoad];
}
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    NSLog(@"mysdk: ads openad %@ admobnt Dismiss", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidDismiss:7 placement:self.placement adId:self.adUnitId adNet:self.adNetLoad];
}

@end

