#import "AdmobBnNtCl.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "MyAdmobiOS.h"

int GGNtClAfterlic = -100000;

@interface AdmobBnNtCl () <GADNativeAdLoaderDelegate, GADVideoControllerDelegate, GADNativeAdDelegate>

@property(nonatomic, strong) NSString *placement;
@property (nonatomic, strong) NSString *adId;
@property(nonatomic, strong) NSString *adNetLoad;

@property(nonatomic, strong) MyAdmobiOS* adParent;

@property (nonatomic, strong) UIImageView *ntclViewClose;
@property (nonatomic, strong) UIButton *ntclBtClose;
@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;

@property(nonatomic, strong) GADAdLoader *adLoader;
@property(nonatomic, strong) GADNativeAd *nativeLoaded;
@property(nonatomic, strong) GADNativeAd *nativeCurrShow;
@property(nonatomic, strong) GADNativeAdView *nativeAdView;
@property(nonatomic, strong) UIView *bnntclViewParent;

@property (nonatomic) int pos;
@property (nonatomic) int width;
@property (nonatomic) int orient;
@property (nonatomic) float dxCenter;
@property (nonatomic) bool isHideBtClose;
@property (nonatomic) bool isLouWhenick;
@property int memTimeClose;
@property int countTimeClose;
@property NSInteger countShow;
@property BOOL isAllowWclick;
@property BOOL isAutoClose;

@end

@implementation AdmobBnNtCl

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.adNetLoad = @"admob";
    
    if (GGNtClAfterlic < -100) {
        GGNtClAfterlic = (int)[[NSUserDefaults standardUserDefaults] integerForKey:@"ggntcl_count_aflick"];
    }
    
    return self;
}

- (void)load:(NSString *)adId orient:(int)orient
{
    if (adId == nil || adId.length < 3) {
        NSLog(@"mysdk: ads bn ntcl %@ admobnt load id incorrect", self.placement);
        [self.adDelegate didFailToReceiveAd:6 placement:self.placement adId:self.adId withError:@"id incorrect"];
        return;
    }
    if (!self.isLoading && !self.isLoaded) {
        NSLog(@"mysdk: ads bn ntcl %@ admobnt load=%@", self.placement, adId);
        self.isLoading = YES;
        if (self.nativeLoaded != nil) {
            self.nativeLoaded = nil;
        }
        self.adId = adId;
        self.orient = orient;
        UIViewController *unityViewctr = [MyAdmobUtiliOS unityGLViewController];
        GADVideoOptions *videoOptions = [[GADVideoOptions alloc] init];
        videoOptions.startMuted = NO;
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
}

- (bool)show:(int)pos width:(int)width dxCenter:(float)dxCenter isHideBtClose:(bool)isHideBtClose isLouWhenick:(bool)isLouWhenick
{
    NSLog(@"mysdk: ads bn ntcl %@ admobnt show id=%@ pos=%d, w=%d, dx=%f, isHideBtClose=%d, isLouWhenick=%d", self.placement, self.adId, pos, width, dxCenter, isHideBtClose, isLouWhenick);
    self.isShow = true;
    self.width = width;
    self.dxCenter = dxCenter;
    self.pos = pos;
    self.isHideBtClose = isHideBtClose;
    self.isLouWhenick = isLouWhenick;
    
    if (self.nativeLoaded != nil) {
        if (self.nativeAdView != nil) {
            if (self.bnntclViewParent != nil) {
                [self.nativeAdView removeFromSuperview];
            }
            self.nativeAdView = nil;
            self.nativeCurrShow = nil;
        }
        self.nativeCurrShow = self.nativeLoaded;
        self.nativeLoaded = nil;
        self.isLoaded = NO;
        self.isAllowWclick = YES;
        [self addView:self.nativeCurrShow];
    }
    if (self.nativeCurrShow != nil) {
        [self.adDelegate adDidPresent:6 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        self.bnntclViewParent.hidden = NO;
        [self.bnntclViewParent.superview bringSubviewToFront:self.bnntclViewParent];
        return true;
    } else {
        return false;
    }
}
- (void)addView:(GADNativeAd*)nativeAd
{
    if (nativeAd != nil) {
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        int bnw = self.width;
        int bnh = 300;
        int rw = 0;
        int rh = 0;
        if (unityView.bounds.size.width < unityView.bounds.size.height) {
            rw = unityView.bounds.size.width;
            rh = unityView.bounds.size.height;
            bnh = 40 * unityView.bounds.size.height / 100;
        } else {
            rw = unityView.bounds.size.height;
            rh = unityView.bounds.size.width;
            bnh = 40 * unityView.bounds.size.width / 100;
        }
        if (bnw < 320) {
            bnw = rw;
        }
        int ybn = 0;
        if (self.pos == 1) {
            ybn = unityView.bounds.size.height - bnh;
        }
        if (self.bnntclViewParent == nil) {
            self.bnntclViewParent = [[UIView alloc] initWithFrame:CGRectMake((unityView.bounds.size.width - bnw)/2 + self.dxCenter*unityView.bounds.size.width, ybn, bnw, bnh)];
            self.bnntclViewParent.clipsToBounds = true;
            [unityView addSubview:self.bnntclViewParent];
        } else {
            self.bnntclViewParent.frame = CGRectMake((unityView.bounds.size.width - bnw)/2 + self.dxCenter*unityView.bounds.size.width, ybn, bnw, bnh);
        }
        self.nativeAdView = [[NSBundle mainBundle] loadNibNamed:@"GGNativeAdCl" owner:nil options:nil].firstObject;
        [self.bnntclViewParent addSubview:self.nativeAdView];
        self.nativeAdView.translatesAutoresizingMaskIntoConstraints = NO;
        NSDictionary *viewDictionary = NSDictionaryOfVariableBindings(_nativeAdView);
        [self.bnntclViewParent addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[_nativeAdView]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        [self.bnntclViewParent addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[_nativeAdView]|"
                                                                          options:0
                                                                          metrics:nil
                                                                            views:viewDictionary]];
        if (self.ntclBtClose == nil) {
            self.ntclViewClose = [[UIImageView alloc] initWithFrame:CGRectMake(5, 5, 30.0, 30.0)];
            [self.ntclViewClose setImage:[UIImage imageNamed:@"icon_down"]];
            [self.bnntclViewParent addSubview:self.ntclViewClose];
            
            self.ntclBtClose = [UIButton buttonWithType:UIButtonTypeSystem];
            [self.ntclBtClose addTarget:self action:@selector(bnntclBtClose:) forControlEvents:UIControlEventTouchUpInside];
            self.ntclBtClose.frame = CGRectMake(5, 5, 30.0, 30.0);
            //[self.ntclBtClose setBackgroundColor:[UIColor colorWithRed:0.2f green:0.2f blue:0.2f alpha:0.8f]];
            [self.ntclBtClose setBackgroundImage:nil forState:UIControlStateNormal];
            [self.bnntclViewParent addSubview:self.ntclBtClose];
        } else {
            [self.bnntclViewParent bringSubviewToFront:self.ntclViewClose];
            [self.bnntclViewParent bringSubviewToFront:self.ntclBtClose];
        }
        
        [self populateNativeAdView:nativeAd nativeAdView:self.nativeAdView];
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"bnntcl_count_click"];
        self.countShow = [[NSUserDefaults standardUserDefaults] integerForKey:@"bnntcl_count_show"];
        self.countShow++;
        [[NSUserDefaults standardUserDefaults] setInteger:self.countShow forKey:@"bnntcl_count_show"];
        GGNtClAfterlic++;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"ggntcl_count_aflick"];
        int st = 40;
        int rper = 0;
        if (self.countShow > 0) {
            rper = (int)(coclick * 100 / self.countShow);
        }
        if (self.isHideBtClose && rper < self.adParent.ntclPer && GGNtClAfterlic >= 0) {
            self.ntclBtClose.hidden = YES;
            self.ntclViewClose.hidden = YES;
            dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 1 * NSEC_PER_SEC), dispatch_get_main_queue(), ^{
                self.ntclBtClose.hidden = NO;
                self.ntclViewClose.hidden = NO;
            });
            int pran = arc4random_uniform(101);
            int currApp = self.adParent.ntclRan + self.adParent.ntclIncrease * GGNtClAfterlic;
            if (pran < currApp) {
                st = 22;
                if (currApp >= 500) {
                    st = 6;
                } else if (currApp >= 400) {
                    st = 10;
                } else if (currApp >= 300) {
                    st = 14;
                } else if (currApp >= 200) {
                    st = 18;
                }
            } else {
                st = 30;
            }
        } else {
            self.ntclBtClose.hidden = NO;
            self.ntclViewClose.hidden = NO;
        }
        NSLog(@"mysdk: ads ntcl %@ admobnt flagNham cfper=%d, rper=%d countafterflic=%d ran=%d non=%d inc=%d bt=%d", self.placement, self.adParent.ntclPer, rper, GGNtClAfterlic, self.adParent.ntclRan, self.adParent.ntclNonAfter, self.adParent.ntclIncrease, st);
        int xbt = 5 + (30 - st) / 2;
        int ybt = 5 + (30 - st) / 2;
        if (xbt < 0) {
            xbt = 0;
        }
        if (ybt < 0) {
            ybt = 0;
        }
        self.ntclBtClose.frame = CGRectMake(xbt, ybt, st, st);
    }
}
-(void)hide{
    NSLog(@"mysdk: ads bn ntcl %@ admobnt btclose ", self.placement);
    self.isShow = false;
    if (self.bnntclViewParent != nil && self.bnntclViewParent.hidden == NO) {
        self.bnntclViewParent.hidden = YES;
        [self.adDelegate adDidDismiss:6 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    }
    if (self.nativeCurrShow != nil) {
        if (self.nativeCurrShow.mediaContent != nil && self.nativeCurrShow.mediaContent.hasVideoContent) {
            [self.nativeCurrShow.mediaContent.videoController stop];
        }
    }
}
- (void)bnntclBtClose:(id)sender {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt btclose ", self.placement);
    [self hide];
}
-(void)afterlick
{
    if (self.ntclBtClose != nil) {
        self.ntclBtClose.frame = CGRectMake(0, 0, 40, 40);
    }
}
- (void) populateNativeAdView:(GADNativeAd*)nativeAd nativeAdView:(GADNativeAdView*)nativeAdView
{
    NSLog(@"mysdk: ads bn ntcl %@ admobnt populateNativeAdView ", self.placement);
    // Populate the native ad view with the native ad assets.
    // The headline and mediaContent are guaranteed to be present in every native ad.
    ((UILabel *)nativeAdView.headlineView).text = nativeAd.headline;
    nativeAdView.mediaView.mediaContent = nativeAd.mediaContent;

    // This app uses a fixed width for the GADMediaView and changes its height
    // to match the aspect ratio of the media content it displays.
    if (nativeAd.mediaContent.aspectRatio > 0) {
//      self.heightConstraint =
//          [NSLayoutConstraint constraintWithItem:nativeAdView.mediaView
//                                       attribute:NSLayoutAttributeHeight
//                                       relatedBy:NSLayoutRelationEqual
//                                          toItem:nativeAdView.mediaView
//                                       attribute:NSLayoutAttributeWidth
//                                      multiplier:(1 / nativeAd.mediaContent.aspectRatio)
//                                        constant:0];
      self.heightConstraint.active = YES;
    }

    // These assets are not guaranteed to be present. Check that they are before
    // showing or hiding them.
    ((UILabel *)nativeAdView.bodyView).text = nativeAd.body;
    nativeAdView.bodyView.hidden = nativeAd.body ? NO : YES;

    [((UIButton *)nativeAdView.callToActionView) setTitle:nativeAd.callToAction
                                                 forState:UIControlStateNormal];
    nativeAdView.callToActionView.hidden = nativeAd.callToAction ? NO : YES;

    ((UIImageView *)nativeAdView.iconView).image = nativeAd.icon.image;
    nativeAdView.iconView.hidden = nativeAd.icon ? NO : YES;
    nativeAdView.iconView.layer.cornerRadius = 10;
    nativeAdView.iconView.clipsToBounds = true;
    
    ((UIImageView *)nativeAdView.starRatingView).image = [self imageForStars:nativeAd.starRating];
    nativeAdView.starRatingView.hidden = nativeAd.starRating ? NO : YES;
    
    ((UILabel *)nativeAdView.storeView).text = nativeAd.store;
    nativeAdView.storeView.hidden = nativeAd.store ? NO : YES;
    
    ((UILabel *)nativeAdView.priceView).text = nativeAd.price;
    nativeAdView.priceView.hidden = nativeAd.price ? NO : YES;
    
    ((UILabel *)nativeAdView.advertiserView).text = nativeAd.advertiser;
    nativeAdView.advertiserView.hidden = nativeAd.advertiser ? NO : YES;
    

    // In order for the SDK to process touch events properly, user interaction
    // should be disabled.
    nativeAdView.callToActionView.userInteractionEnabled = NO;

    // Associate the native ad view with the native ad object. This is
    // required to make the ad clickable.
    // Note: this should always be done after populating the ad views.
    nativeAdView.nativeAd = nativeAd;
}

-(UIImage *)imageForStars:(NSDecimalNumber *)numberOfStars
{
    double starRating = numberOfStars.doubleValue;
    if (starRating >= 5) {
        return [UIImage imageNamed:@"stars_5"];
    } else if (starRating >= 4.5) {
        return [UIImage imageNamed:@"stars_4_5"];
    } else if (starRating >= 4) {
        return [UIImage imageNamed:@"stars_4"];
    } else if (starRating >= 3.5) {
        return [UIImage imageNamed:@"stars_3_5"];
    } else {
        return nil;
    }
}

#pragma mark GADAdLoaderDelegate implementation

- (void)adLoader:(GADAdLoader *)adLoader didFailToReceiveAdWithError:(NSError *)error {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt load fail err=%@", self.placement, [error localizedDescription]);
    self.isLoaded = NO;
    self.isLoading = NO;
    self.nativeLoaded = nil;
    [self.adDelegate didFailToReceiveAd:6 placement:self.placement adId:self.adId withError:[error localizedDescription]];
}

#pragma mark GADNativeAdLoaderDelegate implementation
- (void)adLoader:(GADAdLoader *)adLoader didReceiveNativeAd:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt load ok", self.placement);
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
    [self.adDelegate didReceiveAd:6 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    NSString* tmpPl = self.placement;
    NSString* tmpid = self.adId;
    id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
    __weak AdmobBnNtCl *weakSelf = self;
    self.nativeLoaded.paidEventHandler = ^(GADAdValue * _Nonnull value) {
        NSLog(@"mysdk: ads bn ntcl admobnt paidEventHandler v=%@", [value value]);
        long lva = [[value value] doubleValue] * 1000000000;
        int pre = (int)[value precision];
        [tmpDelegate adPaidEvent:6 placement:tmpPl adId:tmpid adNet:weakSelf.adNetLoad precisionType:pre currencyCode:[value currencyCode] valueMicros:lva];
    };
    self.heightConstraint.active = NO;
}

#pragma mark GADVideoControllerDelegate implementation

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt videoControllerDidEndVideoPlayback", self.placement);
}

#pragma mark GADNativeAdDelegate

- (void)nativeAdDidRecordClick:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdDidRecordClick", self.placement);
    [self.adDelegate adDidClick:6 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    [self afterlick];
    if (self.isAllowWclick) {
        self.isAllowWclick = NO;
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"bnntcl_count_click"];
        coclick++;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"bnntcl_count_click"];
        GGNtClAfterlic-=self.adParent.ntclNonAfter;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"ggntcl_count_aflick"];
    }
}

- (void)nativeAdDidRecordImpression:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdDidRecordImpression", self.placement);
    self.isLoaded = false;
    [self.adDelegate adDidImpression:6 placement:self.placement adId:self.adId adNet:self.adNetLoad];
}

- (void)nativeAdWillPresentScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdWillPresentScreen", self.placement);
}

- (void)nativeAdWillDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdWillDismissScreen", self.placement);
}

- (void)nativeAdDidDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdDidDismissScreen", self.placement);
}

- (void)nativeAdWillLeaveApplication:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads bn ntcl %@ admobnt nativeAdWillLeaveApplication", self.placement);
}
@end




