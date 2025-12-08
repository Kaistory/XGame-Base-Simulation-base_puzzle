#import "MaxOpenAd.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <AppLovinSDK/AppLovinSDK.h>
#import "MyMaxUtiliOS.h"


@interface MaxOpenAd() <MAAdDelegate, MAAdRevenueDelegate>

@property(nonatomic, strong) MAAppOpenAd* appOpenAd;
@property(nonatomic, strong) NSString* adUnitId;

@end

@implementation MaxOpenAd

- (id)initAd:(NSString *)placement adDelegate:(id<MaxMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adDelegate = adDelegate;
    self.placement = placement;
    
    return self;
}

- (void)load:(NSString *)adId
{
    NSLog(@"mysdk: ads openad %@ maxmynt load=%@", self.placement, adId);
    if (!self.isLoading && !self.isLoaded) {
        self.isLoading = YES;
        self.adUnitId = adId;
        if (self.appOpenAd == nil) {
            self.appOpenAd = [[MAAppOpenAd alloc] initWithAdUnitIdentifier:adId];
            self.appOpenAd.delegate = self;
            self.appOpenAd.revenueDelegate = self;
        }
        [self.appOpenAd loadAd];
    }
}

- (bool)show
{
    NSLog(@"mysdk: ads openad %@ maxmynt show", self.placement);
    if (self.appOpenAd != nil) {
        [self.appOpenAd showAdForPlacement:nil];
        return true;
    } else {
        return  false;
    }
}

#pragma CB
- (void)didLoadAd:(MAAd *)ad
{
    NSLog(@"mysdk: ads openad %@ maxmynt load=%@ ok", self.placement, self.adUnitId);
    self.isLoading = NO;
    self.isLoaded = YES;
    [self.adDelegate didReceiveAd:7 placement:self.placement adId:self.adUnitId netName:[ad networkName]];
}
- (void)didFailToLoadAdForAdUnitIdentifier:(NSString *)adUnitIdentifier withError:(MAError *)error
{
    NSLog(@"mysdk: ads openad %@ maxmynt load=%@ err=%@", self.placement, self.adUnitId, [error message]);
    self.isLoading = NO;
    self.isLoaded = NO;
    self.appOpenAd = nil;
    [self.adDelegate didFailToReceiveAd:7 placement:self.placement adId:self.adUnitId withError:[error message]];
}
- (void)didDisplayAd:(nonnull MAAd *)ad 
{
    NSLog(@"mysdk: ads openad %@ maxmynt show ok", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidPresent:7 placement:self.placement adId:self.adUnitId];
    [self.adDelegate adDidImpression:7 placement:self.placement adId:self.adUnitId];
}
- (void)didFailToDisplayAd:(nonnull MAAd *)ad withError:(nonnull MAError *)error 
{
    NSLog(@"mysdk: ads openad %@ maxmynt show fail", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adFailPresent:7 placement:self.placement adId:self.adUnitId withError:[error message]];
}
- (void)didClickAd:(nonnull MAAd *)ad 
{
    NSLog(@"mysdk: ads openad %@ maxmynt Click", self.placement);
    [self.adDelegate adDidClick:7 placement:self.placement adId:self.adUnitId];
}
- (void)didHideAd:(nonnull MAAd *)ad 
{
    NSLog(@"mysdk: ads openad %@ maxmynt Dismiss", self.placement);
    self.isLoaded = NO;
    [self.adDelegate adDidDismiss:7 placement:self.placement adId:self.adUnitId];
}

- (void)didPayRevenueForAd:(nonnull MAAd *)ad 
{
    [self.adDelegate adPaidEvent:7 placement:self.placement adId:self.adUnitId adNet:[ad networkName] adFormat:[[ad format] label] adPlacement:[ad placement] netPlacement:[ad networkPlacement] adValue:[ad revenue]];
}

@end
