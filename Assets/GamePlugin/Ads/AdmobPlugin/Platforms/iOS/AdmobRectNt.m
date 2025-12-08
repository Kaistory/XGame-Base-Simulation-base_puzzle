#import "AdmobRectNt.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "MyAdmobiOS.h"

@interface AdmobRectNt () <GADNativeAdLoaderDelegate, GADVideoControllerDelegate, GADNativeAdDelegate>

@property(nonatomic, strong) NSString *placement;
@property (nonatomic, strong) NSString *adId;
@property(nonatomic, strong) NSString *adNetLoad;

@property(nonatomic, strong) MyAdmobiOS* adParent;

@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;

@property(nonatomic, strong) GADAdLoader *adLoader;
@property(nonatomic, strong) GADNativeAd *nativeLoaded;
@property(nonatomic, strong) GADNativeAd *nativeCurrShow;
@property(nonatomic, strong) GADNativeAdView *nativeViewShow;
@property(nonatomic, strong) UIView *rectntParentView;

@property (nonatomic) int orient;
@property (nonatomic) int pos;
@property (nonatomic) float width;
@property (nonatomic) float height;
@property (nonatomic) float dxCenter;
@property (nonatomic) float dy;

@end

@implementation AdmobRectNt

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.adNetLoad = @"admob";
    return self;
}

- (void)load:(NSString *)adId
{
    NSLog(@"mysdk: ads rectnt %@ admobrectnt load=%@", self.placement, adId);
    if (adId == nil || [adId length] < 3) {
        NSLog(@"mysdk: ads rectnt %@ admobrectnt load id not correct", self.placement);
    } else {
        if (!self.isLoading && !self.isLoaded) {
            self.isLoading = YES;
            [self doLoad:adId];
        } else {
            if (self.isLoading) {
                NSLog(@"mysdk: ads rectnt %@ admobrectnt loading", self.placement);
            }
        }
    }
}

-(void)doLoad:(NSString *)adId
{
    NSLog(@"mysdk: ads rectnt %@ admobrectnt doLoad=%@", self.placement, adId);
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

- (bool)show:(int)pos orien:(int)orien width:(float)width height:(float)height dx:(float)dx dy:(float)dy
{
    NSLog(@"mysdk: ads rectnt %@ admobrectnt show id=%@ pos=%d, orient=%d, w=%f h=%f, dx=%f dy=%f", self.placement, self.adId, pos, orien, width, height, dx, dy);
    self.isShow = true;
    self.width = width;
    self.orient = orien;
    self.dxCenter = dx;
    self.dy = dy;
    self.pos = pos;
    if (self.nativeLoaded != nil) {
        if (self.nativeViewShow != nil) {
            [self.nativeViewShow removeFromSuperview];
            self.nativeViewShow = nil;
        }
        self.nativeCurrShow = self.nativeLoaded;
        self.nativeLoaded = nil;
        [self addView:self.nativeCurrShow];
    }
    if (self.nativeCurrShow != nil) {
        [self.adDelegate adDidPresent:13 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        [self changePos];
        self.rectntParentView.hidden = NO;
        [self.rectntParentView.superview bringSubviewToFront:self.rectntParentView];
        return true;
    } else {
        return false;
    }
}

- (void)hide
{
    self.isShow = false;
    if (self.rectntParentView != nil) {
        self.rectntParentView.hidden = YES;
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

-(void)addView:(GADNativeAd*) nativeAd
{
    if (nativeAd != nil) {
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        if (self.rectntParentView == nil) {
            int bnw = self.width * unityView.bounds.size.width;
            int bnh = self.height * unityView.bounds.size.height;
            if (self.pos == 0) {
                self.rectntParentView = [[UIView alloc] initWithFrame:CGRectMake((unityView.bounds.size.width - bnw)/2, 0, bnw, bnh)];
            } else {
                self.rectntParentView = [[UIView alloc] initWithFrame:CGRectMake((unityView.bounds.size.width - bnw)/2, unityView.bounds.size.height - bnh, bnw, bnh)];
            }
            self.rectntParentView.clipsToBounds = true;
            [unityView addSubview:self.rectntParentView];
        }
        
        self.nativeViewShow = [[NSBundle mainBundle] loadNibNamed:@"GGNativeAdRectNt" owner:nil options:nil].firstObject;
        [self.rectntParentView addSubview:self.nativeViewShow];
        self.nativeViewShow.translatesAutoresizingMaskIntoConstraints = NO;
        NSDictionary *viewDictionary = NSDictionaryOfVariableBindings(_nativeViewShow);
        [self.rectntParentView addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[_nativeViewShow]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        [self.rectntParentView addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[_nativeViewShow]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        [self populateNativeAdView:nativeAd nativeAdView:self.nativeViewShow];
    }
}

-(void)refreshWhenloadnew
{
    NSLog(@"mysdk: ads rectnt %@ admobrectnt refreshWhenloadnew=%@", self.placement, self.adId);
    bool isCallCB = true;
    if(self.nativeLoaded != nil) {
        if (self.nativeCurrShow != nil) {
            if (self.nativeViewShow != nil && self.rectntParentView != nil) {
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
            [self.adDelegate adDidPresent:13 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        }
        self.rectntParentView.hidden = NO;
    }
}

- (void)changePos
{
    if (self.nativeViewShow != nil) {
        NSLog(@"mysdk: ads rectnt %@ admobrectnt changePos=%@", self.placement, self.adId);
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        int bnw = self.width * unityView.bounds.size.width;
        int bnh = self.height * unityView.bounds.size.height;
        float ybn = 0;
        if (self.pos == 0) {
            ybn = 0;
        } else {
            ybn = unityView.bounds.size.height - bnh;
        }
        self.rectntParentView.frame = CGRectMake((unityView.bounds.size.width - bnw)/2, ybn, bnw, bnh);
    }
}

- (void) populateNativeAdView:(GADNativeAd*)nativeAd nativeAdView:(GADNativeAdView*)nativeAdView
{
    NSLog(@"mysdk: ads rectnt %@ admobrectnt populateNativeAdView=%@", self.placement, self.adId);
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
    NSLog(@"mysdk: ads rectnt %@ admobrectnt load=%@ fail err=%@", self.placement, self.adId, [error localizedDescription]);
    self.isLoaded = NO;
    self.isLoading = NO;
    self.nativeLoaded = nil;
    [self.adDelegate didFailToReceiveAd:13 placement:self.placement adId:self.adId withError:[error localizedDescription]];
}

#pragma mark GADNativeAdLoaderDelegate implementation
- (void)adLoader:(GADAdLoader *)adLoader didReceiveNativeAd:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt load=%@ ok", self.placement, self.adId);
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
    [self.adDelegate didReceiveAd:13 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    NSString* tmpPl = self.placement;
    NSString* tmpid = self.adId;
    id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
    __weak AdmobRectNt *weakSelf = self;
    self.nativeLoaded.paidEventHandler = ^(GADAdValue * _Nonnull value) {
        NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@  paid=%@", tmpPl, tmpid, [value value]);
        long lva = [[value value] doubleValue] * 1000000000;
        int pre = (int)[value precision];
        [tmpDelegate adPaidEvent:10 placement:tmpPl adId:tmpid adNet:weakSelf.adNetLoad precisionType:pre currencyCode:[value currencyCode] valueMicros:lva];
    };
    self.heightConstraint.active = NO;
    if (self.isShow) {
        [self refreshWhenloadnew];
    }
}

#pragma mark GADVideoControllerDelegate implementation

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ videoControllerDidEndVideoPlayback", self.placement, self.adId);
}

#pragma mark GADNativeAdDelegate

- (void)nativeAdDidRecordClick:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdDidRecordClick", self.placement, self.adId);
    [self.adDelegate adDidClick:13 placement:self.placement adId:self.adId adNet:self.adNetLoad];
}

- (void)nativeAdDidRecordImpression:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdDidRecordImpression", self.placement, self.adId);
    [self.adDelegate adDidImpression:13 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    self.isLoaded = false;
}

- (void)nativeAdWillPresentScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdWillPresentScreen", self.placement, self.adId);
}

- (void)nativeAdWillDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdWillDismissScreen", self.placement, self.adId);
}

- (void)nativeAdDidDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdDidDismissScreen", self.placement, self.adId);
}

- (void)nativeAdWillLeaveApplication:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads rectnt %@ admobrectnt ID=%@ nativeAdWillLeaveApplication", self.placement, self.adId);
}
@end





