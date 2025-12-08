#import "AdmobBanner.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"


@interface AdmobBanner() <GADBannerViewDelegate>

@property (nonatomic) int adType;
@property(nonatomic, strong) NSString *adName;
@property(nonatomic, strong) NSString *placement;
@property(nonatomic, strong) NSString *adNetLoad;
@property BOOL isLoading;
@property BOOL isLoaded;
@property (nonatomic) int orient;
@property (nonatomic) int pos;
@property (nonatomic) int width;
@property (nonatomic) int maxH;
@property (nonatomic) float dxCenter;
@property (nonatomic) float dy;
@property (nonatomic) bool iPad;
@property (nonatomic) bool viewParentAdded;
@property(nonatomic, strong) GADBannerView *adCurrAdd;
@property(nonatomic, strong) GADBannerView *adCurrLoad;
@property(nonatomic, strong) UIView *viewParent;
@property(nonatomic, strong) NSMutableArray *arrayTmpBanner;

@end

@implementation AdmobBanner

- (id)initBanner:(int)type placement:(NSString*)placement iPad:(bool)iPad adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adType = type;
    self.iPad = iPad;
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.adName = @"bn";
    self.adNetLoad = @"admob";
    self.viewParentAdded = NO;
    if (placement == nil || [placement length] == 0) {
        if (self.adType == 0) {
            self.placement = @"bn_default";
            self.adName = @"bn";
        } else if (self.adType == 1) {
            self.placement = @"cl_default";
            self.adName = @"bncl";
        } else {
            self.placement = @"rect_default";
            self.adName = @"rect";
        }
    } else {
        if (self.adType == 0) {
            self.adName = @"bn";
        } else if (self.adType == 1) {
            self.adName = @"bncl";
        } else {
            self.adName = @"rect";
        }
    }
    
    return self;
}

- (void)load:(NSString*)adId
{
    NSLog(@"mysdk: ads %@ %@ admobnt load=%@", self.adName, self.placement, adId);
    if (adId == nil || [adId length] < 3) {
        [self.adDelegate didFailToReceiveAd:self.adType placement:self.placement adId:adId withError:@"no ads id"];
        return;
    }
    if (!self.isLoading) {
        UIViewController* vcon = [MyAdmobUtiliOS unityGLViewController];
        GADBannerView* bannerView;
        GADAdSize adsize;
        if (self.adType == 2) {
//            if (self.width < -1) {
//                adsize = GADInlineAdaptiveBannerAdSizeWithWidthAndMaxHeight(vcon.view.bounds.size.width, 260);
//            } else if (self.width < 10) {
//                adsize = GADAdSizeMediumRectangle;
//            } else {
//                adsize = GADInlineAdaptiveBannerAdSizeWithWidthAndMaxHeight(self.width, 250);
//            }
            adsize = GADAdSizeMediumRectangle;
        } else {
            int hbn;
            if (self.width < -1) {
                if (!self.iPad) {
                    hbn = 60;
                } else {
                    hbn = 110;
                }
                if (self.maxH > 50) {
                    hbn = self.maxH;
                }
                adsize = GADInlineAdaptiveBannerAdSizeWithWidthAndMaxHeight(vcon.view.bounds.size.width, hbn);
                if (!self.iPad) {
                    adsize = GADAdSizeFullBanner;
                } else {
                    adsize = GADAdSizeLeaderboard;
                }
            } else if (self.width < 100) {
                if (!self.iPad) {
                    adsize = GADAdSizeBanner;
                } else {
                    adsize = GADAdSizeLeaderboard;
                }
            } else {
                if (!self.iPad) {
                    hbn = 60;
                } else {
                    hbn = 110;
                }
                if (self.maxH > 50) {
                    hbn = self.maxH;
                }
                adsize = GADInlineAdaptiveBannerAdSizeWithWidthAndMaxHeight(self.width, hbn);
                if (!self.iPad) {
                    adsize = GADAdSizeFullBanner;
                } else {
                    adsize = GADAdSizeLeaderboard;
                }
            }
        }
        bannerView = [[GADBannerView alloc] initWithAdSize:adsize];
        if (self.arrayTmpBanner == nil) {
            self.arrayTmpBanner = [[NSMutableArray alloc] init];
        }
        [self.arrayTmpBanner addObject:bannerView];
        bannerView.translatesAutoresizingMaskIntoConstraints = NO;
        bannerView.adUnitID = adId;
        bannerView.rootViewController = vcon;
        bannerView.delegate = self;
        bannerView.paidEventHandler = ^(GADAdValue * _Nonnull value) {
            long lva = [[value value] doubleValue] * 1000000000;
            int precision = (int)[value precision];
            [self.adDelegate adPaidEvent:self.adType placement:self.placement adId:adId adNet:self.adNetLoad precisionType:precision currencyCode:[value currencyCode] valueMicros:lva];
        };
        bannerView.hidden = NO;
        self.isLoading = true;
        GADRequest* adRequest = [[GADRequest alloc] init];
        if (self.adType == 1) {
            GADExtras* extra = [[GADExtras alloc] init];
            NSString* clpos;
            if (self.pos == 0) {
                clpos = @"top";
            } else {
                clpos = @"bottom";
            }
            NSDictionary* dicextra = @{@"collapsible" : clpos};
            [extra setAdditionalParameters:dicextra];
            [adRequest registerAdNetworkExtras:extra];
        }
        [bannerView loadRequest: adRequest];
    } else {
        if (self.isLoading) {
            NSLog(@"mysdk: ads %@ %@ admobnt load is loading", self.adName, self.placement);
        }
    }
}

- (bool)show:(int)pos orient:(int)orient width:(float)width maxH:(int)maxH dx:(float)dx dy:(float)dy
{
    NSLog(@"mysdk: ads %@ %@ admobnt show pos=%d orient=%d w=%f dx=%f dy=%f", self.adName, self.placement, pos, orient, width, dx, dy);
    self.isShow = true;
    self.pos = pos;
    self.orient = orient;
    self.width = width;
    self.maxH = maxH;
    self.dxCenter = dx;
    self.dy = dy;
    bool re = false;
    if (self.adCurrLoad != nil) {
        if (![self.adCurrLoad isEqual:self.adCurrAdd]) {
            if (self.adCurrAdd != nil) {
                if (self.viewParent != nil) {
                    [self.adCurrAdd removeFromSuperview];
                }
                [self.adDelegate adWillDestroy:self.adType placement:self.placement adId:[self.adCurrAdd adUnitID]];
                self.adCurrAdd.hidden = YES;
                self.adCurrAdd = nil;
            }
            [self addView:self.adCurrLoad];
        }
    }
    if (self.adCurrAdd != nil) {
        re = true;
        self.viewParent.hidden = NO;
        self.adCurrAdd.hidden = NO;
        [self setPos:self.adCurrAdd pos:pos width:width dxCenter:dx dy:dy];
    }
    UIViewController* vcon = [MyAdmobUtiliOS unityGLViewController];
    [vcon.view setNeedsDisplay];
    [vcon.view setNeedsUpdateConstraints];
    return re;
}

- (void)hide
{
    if (self.viewParent != nil) {
        self.viewParent.hidden = YES;
    }
    if (self.adCurrAdd != nil) {
        self.adCurrAdd.hidden = YES;
    }
    if (self.adType == 1) {
        if (self.viewParent != nil && self.viewParentAdded) {
            [self.viewParent removeFromSuperview];
            self.viewParentAdded = NO;
        }
    }
}

- (void)destroy
{
    if (self.adCurrAdd != nil) {
        self.adCurrAdd.hidden = YES;
        [self.adCurrAdd removeFromSuperview];
        [self.adDelegate adWillDestroy:self.adType placement:self.placement adId:[self.adCurrAdd adUnitID]];
        self.adCurrAdd = nil;
    }
    if (self.adCurrLoad != nil) {
        self.adCurrLoad.hidden = YES;
        [self.adCurrLoad removeFromSuperview];
        [self.adDelegate adWillDestroy:self.adType placement:self.placement adId:[self.adCurrLoad adUnitID]];
        self.adCurrLoad = nil;
    }
}

#pragma utilities
- (void)addView:(GADBannerView *)adView
{
    NSLog(@"mysdk: ads %@ %@ addViewf", self.adName, self.placement);
    if (adView != nil) {
        if (self.viewParent == nil) {
            UIViewController* vcon = [MyAdmobUtiliOS unityGLViewController];
            if (self.pos == 0) {
                self.viewParent = [[UIView alloc] initWithFrame:CGRectMake(0, 0, adView.adSize.size.width, adView.adSize.size.height)];
            } else {
                self.viewParent = [[UIView alloc] initWithFrame:CGRectMake(0, vcon.view.bounds.size.height - adView.bounds.size.height, adView.adSize.size.width, adView.adSize.size.height)];
            }
            // self.bannerParent.backgroundColor = [UIColor redColor];
            //self.bannerParent.userInteractionEnabled = NO;
            self.viewParent.clipsToBounds = true;
            [vcon.view addSubview:self.viewParent];
            self.viewParentAdded = YES;
        } else {
            if (!self.viewParentAdded) {
                UIViewController* vcon = [MyAdmobUtiliOS unityGLViewController];
                [vcon.view addSubview:self.viewParent];
                self.viewParentAdded = YES;
            }
        }
        if (![adView isEqual:self.adCurrAdd]) {
            if (self.adCurrAdd != nil) {
                [self.adDelegate adWillDestroy:self.adType placement:self.placement adId:[self.adCurrAdd adUnitID]];
                self.adCurrAdd.hidden = YES;
                [self.adCurrAdd removeFromSuperview];
                self.adCurrAdd = nil;
            }
            [self.viewParent addSubview:adView];
            self.adCurrAdd =  adView;
        }
    }
}

- (void)setPos:(GADBannerView *)adView pos:(int)pos width:(float)width dxCenter:(float)dx dy:(float)dy
{
    NSLog(@"mysdk: ads %@ %@ admobnt setPos pos=%d w=%f dx=%f dy=%f orient=%d", self.adName, self.placement, pos, width, dx, dy, self.orient);
    UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
    float safetop = 0;
    float safebot = 0;
    if (self.orient == 0) {
        if (@available(iOS 11.0, *)) {
            safetop = unityView.safeAreaInsets.top / 2;
            safebot = unityView.safeAreaInsets.bottom / 2;
    //        if (self.adType == 0) {
    //            safebot = 0;
    //        } else {
    //            safebot = unityView.safeAreaInsets.bottom;
    //        }
        }
    }
    NSLog(@"mysdk: ads %@ %@ admobnt setPos h=%f w=%f", self.adName, self.placement, unityView.bounds.size.height, unityView.bounds.size.width);
    float hAvaiable = unityView.bounds.size.height;
    float xbn = (unityView.bounds.size.width - adView.adSize.size.width) / 2 + self.dxCenter*unityView.bounds.size.width;
    float ybn;
    if (pos == 0) {
        ybn = (hAvaiable - adView.adSize.size.height) * dy + safetop;
    } else if (pos == 1) {
        ybn = hAvaiable - adView.adSize.size.height - (hAvaiable - adView.adSize.size.height) * dy - safebot;
    } else {
        ybn = (hAvaiable - adView.adSize.size.height) / 2 + dy * (hAvaiable - adView.adSize.size.height);
    }
    adView.frame = CGRectMake(0, 0, adView.adSize.size.width, adView.adSize.size.height);
    self.viewParent.frame = CGRectMake(xbn, ybn, adView.adSize.size.width, adView.adSize.size.height);
}

#pragma Cb
- (void)bannerViewDidReceiveAd:(GADBannerView *)bannerView {
    if ([bannerView responseInfo] != nil) {
        if ([[bannerView responseInfo] loadedAdNetworkResponseInfo] != nil) {
            self.adNetLoad = [[[bannerView responseInfo] loadedAdNetworkResponseInfo] adNetworkClassName];
            if (self.adNetLoad == nil) {
                self.adNetLoad = @"admob";
            }
        }
    }
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidReceiveAd=%@", self.adName, self.placement, [bannerView adUnitID]);
    if (self.isLoading) {
        [self.arrayTmpBanner removeObject:bannerView];
        self.adCurrLoad = bannerView;
        if ([bannerView isEqual:self.adCurrAdd]) {
            NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidReceiveAd=%@ eeeeeeee", self.adName, self.placement, [bannerView adUnitID]);
        }
        self.isLoading = NO;
        self.isLoaded = YES;
        if (self.isShow) {
            [self show:self.pos orient:self.orient width:self.width maxH:self.maxH dx:self.dxCenter dy:self.dy];
        }
    } else {
        if (self.adCurrAdd == nil) {
            NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidReceiveAd=%@ not loading adCurrAdd=nil", self.adName, self.placement, [bannerView adUnitID]);
        }
    }
    if (!self.isShow) {
        [self hide];
    }
    [self.adDelegate didReceiveAd:self.adType placement:self.placement adId:[bannerView adUnitID] adNet:self.adNetLoad];
}

- (void)bannerView:(GADBannerView *)bannerView didFailToReceiveAdWithError:(NSError *)error {
    NSLog(@"mysdk: ads %@ %@ admobnt didFailToReceiveAdWithError=%@ err=%@", self.adName, self.placement, [bannerView adUnitID], [error localizedDescription]);
    if (self.isLoading) {
        [self.arrayTmpBanner removeObject:bannerView];
        self.isLoading = NO;
        self.isLoaded = NO;
        if ([bannerView isEqual:self.adCurrAdd]) {
            NSLog(@"mysdk: ads %@ %@ admobnt didFailToReceiveAdWithError=%@ eeeee1", self.adName, self.placement, [bannerView adUnitID]);
            if (self.viewParent != nil) {
                [bannerView removeFromSuperview];
            }
        }
    }
    [self.adDelegate didFailToReceiveAd:self.adType placement:self.placement adId:[bannerView adUnitID] withError:[error localizedDescription]];
}

- (void)bannerViewDidRecordImpression:(GADBannerView *)bannerView {
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidRecordImpression=%@", self.adName, self.placement, [bannerView adUnitID]);
    [self.adDelegate adDidImpression:self.adType placement:self.placement adId:[bannerView adUnitID] adNet:self.adNetLoad];
}

- (void)bannerViewDidRecordClick:(nonnull GADBannerView *)bannerView
{
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidRecordClick=%@", self.adName, self.placement, [bannerView adUnitID]);
    [self.adDelegate adDidClick:self.adType placement:self.placement adId:[bannerView adUnitID] adNet:self.adNetLoad];
}

- (void)bannerViewWillPresentScreen:(GADBannerView *)bannerView {
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewWillPresentScreen=%@", self.adName, self.placement, [bannerView adUnitID]);
    [self.adDelegate adDidPresent:self.adType placement:self.placement adId:[bannerView adUnitID] adNet:self.adNetLoad];
}

- (void)bannerViewWillDismissScreen:(GADBannerView *)bannerView {
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewWillDismissScreen=%@", self.adName, self.placement, [bannerView adUnitID]);
}

- (void)bannerViewDidDismissScreen:(GADBannerView *)bannerView {
    NSLog(@"mysdk: ads %@ %@ admobnt bannerViewDidDismissScreen=%@", self.adName, self.placement, [bannerView adUnitID]);
    [self.adDelegate adDidDismiss:self.adType placement:self.placement adId:[bannerView adUnitID] adNet:self.adNetLoad];
}

@end



