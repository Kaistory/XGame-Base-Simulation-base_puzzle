#import "AdmobBnNt.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "MyAdmobiOS.h"

@interface AdmobBnNt () <GADNativeAdLoaderDelegate, GADVideoControllerDelegate, GADNativeAdDelegate>

@property(nonatomic, strong) NSString *placement;
@property (nonatomic, strong) NSString *adId;
@property(nonatomic, strong) NSString *adNetLoad;

@property(nonatomic, strong) MyAdmobiOS* adParent;

@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;

@property(nonatomic, strong) GADAdLoader *adLoader;
@property(nonatomic, strong) GADNativeAd *nativeLoaded;
@property(nonatomic, strong) GADNativeAd *nativeCurrShow;
@property(nonatomic, strong) GADNativeAdView *nativeViewShow;
@property(nonatomic, strong) UIView *bnntParentView;

@property (nonatomic) int orient;
@property (nonatomic) int pos;
@property (nonatomic) int width;
@property (nonatomic) int maxH;
@property (nonatomic) float dxCenter;
@property (nonatomic) float dy;
@property (nonatomic) bool iPad;
@property (nonatomic) int tRefresh;
@property (nonatomic) int flagRefresh;

@end

@implementation AdmobBnNt

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.flagRefresh = 0;
    self.adNetLoad = @"admob";
    return self;
}

- (void)load:(NSString *)adId
{
    NSLog(@"mysdk: ads bnnt %@ admobnt load=%@", self.placement, adId);
    if (adId == nil || [adId length] < 3) {
        NSLog(@"mysdk: ads bnnt %@ admobnt load id not correct", self.placement);
    } else {
        if (!self.isLoading && !self.isLoaded) {
            self.isLoading = YES;
            [self doLoad:adId];
        } else {
            if (self.isLoading) {
                NSLog(@"mysdk: ads bnnt %@ admobnt loading", self.placement);
            }
        }
    }
}

-(void)refreshAd
{
    NSLog(@"mysdk: ads bnnt %@ admobnt refreshAd=%@", self.placement, self.adId);
    if (self.adId != nil && self.adId.length > 3) {
        if (!self.isLoading && !self.isLoaded) {
            self.isLoading = true;
            [self doLoad:self.adId];
        } else {
            if (self.isLoading) {
                NSLog(@"mysdk: ads bnnt %@ admobnt refreshAd=%@ isloading", self.placement, self.adId);
            }
        }
    }
}

-(void)doLoad:(NSString *)adId
{
    NSLog(@"mysdk: ads bnnt %@ admobnt doLoad=%@", self.placement, adId);
    if (self.nativeLoaded != nil) {
        self.nativeLoaded = nil;
    }
    self.adId = adId;
    UIViewController *unityViewctr = [MyAdmobUtiliOS unityGLViewController];
    GADVideoOptions *videoOptions = [[GADVideoOptions alloc] init];
    videoOptions.startMuted = YES;
    videoOptions.customControlsRequested = YES;
    GADNativeAdMediaAdLoaderOptions *mediaLoaderOptions = [[GADNativeAdMediaAdLoaderOptions alloc] init];
    mediaLoaderOptions.mediaAspectRatio = GADMediaAspectRatioLandscape;
    self.adLoader = [[GADAdLoader alloc] initWithAdUnitID:adId
                                       rootViewController:unityViewctr
                                                  adTypes:@[ GADAdLoaderAdTypeNative ]
                                                  options:@[ mediaLoaderOptions, videoOptions ]];
    self.adLoader.delegate = self;
    [self.adLoader loadRequest:[GADRequest request]];
}

- (bool)show:(int)pos width:(int)width maxH:(int)maxH orien:(int)orien iPad:(bool)iPad dxCenter:(float)dxCenter dyVertical:(float)dyVertical trefresh:(int)trefresh
{
    NSLog(@"mysdk: ads bnnt %@ admobnt show id=%@ pos=%d, w=%d maxH=%d, orient=%d, iPad=%d dx=%f dy=%f, trefresh=%d", self.placement, self.adId, pos, width, maxH, orien, iPad, dxCenter, dyVertical, trefresh);
    self.isShow = true;
    self.width = width;
    self.orient = orien;
    self.dxCenter = dxCenter;
    self.dy = dyVertical;
    self.tRefresh = trefresh;
    int tmpf = -1;
    bool isChangePos = false;
    if (self.pos != pos) {
        isChangePos = true;
    }
    self.pos = pos;
    if (self.nativeLoaded != nil) {
        if (self.nativeViewShow != nil) {
            [self.nativeViewShow removeFromSuperview];
            self.nativeViewShow = nil;
        }
        self.nativeCurrShow = self.nativeLoaded;
        self.nativeLoaded = nil;
        isChangePos = false;
        [self addView:self.nativeCurrShow];
        tmpf = 0;
    }
    if (self.nativeCurrShow != nil) {
        [self.adDelegate adDidPresent:10 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        if (isChangePos) {
            [self changePos];
        }
        self.bnntParentView.hidden = NO;
        [self.bnntParentView.superview bringSubviewToFront:self.bnntParentView];
        [self waitRefresh:tmpf];
        return true;
    } else {
        return false;
    }
}

- (void)hide
{
    self.isShow = false;
    if (self.bnntParentView != nil) {
        self.bnntParentView.hidden = YES;
    }
    if (self.nativeCurrShow != nil) {
        if (self.nativeCurrShow.mediaContent != nil && self.nativeCurrShow.mediaContent.hasVideoContent) {
            [self.nativeCurrShow.mediaContent.videoController stop];
        }
    }
}
- (void)destroy
{
    [self hide];
}

-(void)waitRefresh:(int)flagTime
{
    if (flagTime < 0) {
        [self refreshAd];
    } else {
        if (self.flagRefresh == 0) {
            self.flagRefresh = 1;
            int tw = self.tRefresh;
            if (flagTime > 30 && self.tRefresh <= 30) {
                tw = flagTime;
            }
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, tw * NSEC_PER_SEC), dispatch_get_main_queue(), ^{
                self.flagRefresh = 0;
                [self refreshAd];
            });
        }
    }
}

-(void)addView:(GADNativeAd*) nativeAd
{
    if (nativeAd != nil) {
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        if (self.bnntParentView == nil) {
            int bnw = self.width;
            if (bnw < -1) {
                bnw = unityView.bounds.size.width;
            } else if (bnw < 320) {
                bnw = 320;
            }
            int bnh = self.maxH;
            if (self.iPad) {
                bnh = 100;
            } else {
                bnh = 60;
            }
            if (self.maxH > 50) {
                bnh = self.maxH;
            }
            float safetop = 0;
            float safebot = 0;
            if (self.orient == 0) {
                if (@available(iOS 11.0, *)) {
                    safetop = unityView.safeAreaInsets.top / 1;
                    safebot = unityView.safeAreaInsets.bottom / 1;
            //        if (self.adType == 0) {
            //            safebot = 0;
            //        } else {
            //            safebot = unityView.safeAreaInsets.bottom;
            //        }
                }
            }
            if (self.pos == 0) {
                self.bnntParentView = [[UIView alloc] initWithFrame:CGRectMake((unityView.bounds.size.width - bnw)/2, 0, bnw, bnh)];
            } else {
                bnh += safebot;
                self.bnntParentView = [[UIView alloc] initWithFrame:CGRectMake((unityView.bounds.size.width - bnw)/2, unityView.bounds.size.height - bnh, bnw, bnh)];
            }
            self.bnntParentView.clipsToBounds = true;
            [unityView addSubview:self.bnntParentView];
        }
        
        self.nativeViewShow = [[NSBundle mainBundle] loadNibNamed:@"GGNativeAdBn" owner:nil options:nil].firstObject;
        [self.bnntParentView addSubview:self.nativeViewShow];
        self.nativeViewShow.translatesAutoresizingMaskIntoConstraints = NO;
        NSDictionary *viewDictionary = NSDictionaryOfVariableBindings(_nativeViewShow);
        [self.bnntParentView addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[_nativeViewShow]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        [self.bnntParentView addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[_nativeViewShow]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        [self populateNativeAdView:nativeAd nativeAdView:self.nativeViewShow];
    }
}

-(void)refreshWhenloadnew
{
    NSLog(@"mysdk: ads bnnt %@ admobnt refreshWhenloadnew=%@", self.placement, self.adId);
    bool isCallCB = true;
    if(self.nativeLoaded != nil) {
        if (self.nativeCurrShow != nil) {
            if (self.nativeViewShow != nil && self.bnntParentView != nil) {
                isCallCB = false;
                [self.nativeViewShow removeFromSuperview];
            }
            self.nativeCurrShow = nil;
        }
        self.nativeCurrShow = self.nativeLoaded;
        self.nativeLoaded = nil;
        [self addView:self.nativeCurrShow];
    }
    if (self.nativeCurrShow != nil) {
        if (isCallCB) {
            [self.adDelegate adDidPresent:10 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        }
        self.bnntParentView.hidden = NO;
    }
}

- (void)changePos
{
    if (self.nativeViewShow != nil) {
        NSLog(@"mysdk: ads bnnt %@ admobnt changePos=%@", self.placement, self.adId);
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        int bnw = self.width;
        if (bnw < -1) {
            bnw = unityView.bounds.size.width;
        } else if (bnw < 320) {
            bnw = 320;
        }
        int bnh = self.maxH;
        if (self.iPad) {
            bnh = 100;
        } else {
            bnh = 60;
        }
        if (self.maxH > 50) {
            bnh = self.maxH;
        }
        float safetop = 0;
        float safebot = 0;
        if (self.orient == 0) {
            if (@available(iOS 11.0, *)) {
                safetop = unityView.safeAreaInsets.top / 1;
                safebot = unityView.safeAreaInsets.bottom / 1;
        //        if (self.adType == 0) {
        //            safebot = 0;
        //        } else {
        //            safebot = unityView.safeAreaInsets.bottom;
        //        }
            }
        }
        float ybn = 0;
        if (self.pos == 0) {
            ybn = 0;
        } else {
            bnh += safebot;
            ybn = unityView.bounds.size.height - bnh;
        }
        self.bnntParentView.frame = CGRectMake((unityView.bounds.size.width - bnw)/2, ybn, bnw, bnh);
    }
}

- (void) populateNativeAdView:(GADNativeAd*)nativeAd nativeAdView:(GADNativeAdView*)nativeAdView
{
    NSLog(@"mysdk: ads bnnt %@ admobnt populateNativeAdView=%@", self.placement, self.adId);
    // Populate the native ad view with the native ad assets.
    // The headline and mediaContent are guaranteed to be present in every native ad.
    ((UILabel *)nativeAdView.headlineView).text = nativeAd.headline;
    nativeAdView.mediaView.mediaContent = nativeAd.mediaContent;

    // This app uses a fixed width for the GADMediaView and changes its height
    // to match the aspect ratio of the media content it displays.
    if (nativeAd.mediaContent.aspectRatio > 0) {
      self.heightConstraint =
          [NSLayoutConstraint constraintWithItem:nativeAdView.mediaView
                                       attribute:NSLayoutAttributeHeight
                                       relatedBy:NSLayoutRelationEqual
                                          toItem:nativeAdView.mediaView
                                       attribute:NSLayoutAttributeWidth
                                      multiplier:(1 / nativeAd.mediaContent.aspectRatio)
                                        constant:0];
      self.heightConstraint.active = YES;
    }

    // These assets are not guaranteed to be present. Check that they are before
    // showing or hiding them.
    ((UILabel *)nativeAdView.bodyView).text = nativeAd.body;
    nativeAdView.bodyView.hidden = nativeAd.body ? NO : YES;

    [((UIButton *)nativeAdView.callToActionView) setTitle:nativeAd.callToAction
                                                 forState:UIControlStateNormal];
    nativeAdView.callToActionView.hidden = nativeAd.callToAction ? NO : YES;

    // In order for the SDK to process touch events properly, user interaction
    // should be disabled.
    nativeAdView.callToActionView.userInteractionEnabled = NO;

    // Associate the native ad view with the native ad object. This is
    // required to make the ad clickable.
    // Note: this should always be done after populating the ad views.
    nativeAdView.nativeAd = nativeAd;
}

#pragma mark GADAdLoaderDelegate implementation

- (void)adLoader:(GADAdLoader *)adLoader didFailToReceiveAdWithError:(NSError *)error {
    NSLog(@"mysdk: ads bnnt %@ admobnt load=%@ fail err=%@", self.placement, self.adId, [error localizedDescription]);
    self.isLoaded = NO;
    self.isLoading = NO;
    self.nativeLoaded = nil;
    [self.adDelegate didFailToReceiveAd:10 placement:self.placement adId:self.adId withError:[error localizedDescription]];
    if (self.isShow) {
        [self waitRefresh:31];
    }
}

#pragma mark GADNativeAdLoaderDelegate implementation
- (void)adLoader:(GADAdLoader *)adLoader didReceiveNativeAd:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt load=%@ ok", self.placement, self.adId);
    if ([nativeAd responseInfo] != nil) {
        if ([[nativeAd responseInfo] loadedAdNetworkResponseInfo] != nil) {
            self.adNetLoad = [[[nativeAd responseInfo] loadedAdNetworkResponseInfo] adNetworkClassName];
            if (self.adNetLoad == nil) {
                self.adNetLoad = @"admob";
            }
        }
    }
    self.isLoaded = YES;
    self.isLoading = NO;
    self.nativeLoaded = nativeAd;
    self.nativeLoaded.delegate = self;
    [self.adDelegate didReceiveAd:10 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    NSString* tmpPl = self.placement;
    NSString* tmpid = self.adId;
    id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
    __weak AdmobBnNt *weakSelf = self;
    self.nativeLoaded.paidEventHandler = ^(GADAdValue * _Nonnull value) {
        NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@  paid=%@", tmpPl, tmpid, [value value]);
        long lva = [[value value] doubleValue] * 1000000000;
        int pre = (int)[value precision];
        [tmpDelegate adPaidEvent:10 placement:tmpPl adId:tmpid adNet:weakSelf.adNetLoad precisionType:pre currencyCode:[value currencyCode] valueMicros:lva];
    };
    self.heightConstraint.active = NO;
    if (self.isShow) {
        [self refreshWhenloadnew];
        [self waitRefresh:0];
    }
}

#pragma mark GADVideoControllerDelegate implementation

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ videoControllerDidEndVideoPlayback", self.placement, self.adId);
}

#pragma mark GADNativeAdDelegate

- (void)nativeAdDidRecordClick:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdDidRecordClick", self.placement, self.adId);
    [self.adDelegate adDidClick:10 placement:self.placement adId:self.adId adNet:self.adNetLoad];
}

- (void)nativeAdDidRecordImpression:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdDidRecordImpression", self.placement, self.adId);
    [self.adDelegate adDidImpression:10 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    self.isLoaded = false;
}

- (void)nativeAdWillPresentScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdWillPresentScreen", self.placement, self.adId);
}

- (void)nativeAdWillDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdWillDismissScreen", self.placement, self.adId);
}

- (void)nativeAdDidDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdDidDismissScreen", self.placement, self.adId);
}

- (void)nativeAdWillLeaveApplication:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bnnt %@ admobnt ID=%@ nativeAdWillLeaveApplication", self.placement, self.adId);
}
@end





