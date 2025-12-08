#import "AdmobNtFull.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "MyAdmobiOS.h"

int GGNtCountAfterlic = -100000;
int GGNtScrWidth = 40;
int GGNtmarginBtClose = 40;

@interface AdmobNtFull () <GADNativeAdLoaderDelegate, GADVideoControllerDelegate, GADNativeAdDelegate>

@property (nonatomic, strong) NSString *adId;
@property(nonatomic, strong) NSString *adNetLoad;

@property(nonatomic, strong) MyAdmobiOS* adParent;

@property (nonatomic, strong) UIButton *ntFullBtClose;
@property (nonatomic, strong) UIButton *ntFullBtCloseView;
@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;
@property(nonatomic, strong) GADAdLoader *adLoader;
@property(nonatomic, strong) GADNativeAd *nativeAd;
@property(nonatomic, strong) GADNativeAdView *nativeAdView;
@property(nonatomic, strong) UIView *ntFullViewParent;

@property int memTimeClose;
@property int countTimeClose;
@property NSInteger countShow;
@property BOOL isAllowWclick;
@property BOOL isAutoClose;

@end

@implementation AdmobNtFull

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    self.adNetLoad = @"admob";
    
    return self;
}

- (void)load:(NSString *)adId orient:(int)orient
{
    NSLog(@"mysdk: ads full nt %@ admobnt load=%@", self.placement, adId);
    if (self.nativeAd == nil && self.isLoading == NO && self.isLoaded == NO) {
        self.isLoaded = NO;
        self.isLoading = YES;
        self.adId = adId;
        UIViewController *unityViewctr = [MyAdmobUtiliOS unityGLViewController];
        GADVideoOptions *videoOptions = [[GADVideoOptions alloc] init];
        videoOptions.startMuted = YES;
        videoOptions.customControlsRequested = YES;
        GADNativeAdMediaAdLoaderOptions *mediaLoaderOptions = [[GADNativeAdMediaAdLoaderOptions alloc] init];
        if (orient == 0) {
            mediaLoaderOptions.mediaAspectRatio = GADMediaAspectRatioPortrait;
        } else {
            mediaLoaderOptions.mediaAspectRatio = GADMediaAspectRatioLandscape;
        }
        self.adLoader = [[GADAdLoader alloc] initWithAdUnitID:adId
                                           rootViewController:unityViewctr
                                                      adTypes:@[ GADAdLoaderAdTypeNative ]
                                                      options:@[ mediaLoaderOptions, videoOptions ]];
        self.adLoader.delegate = self;
        [self.adLoader loadRequest:[GADRequest request]];
    }
}

- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto
{
    NSLog(@"mysdk: ads full nt %@ admobnt show ", self.placement);
    if (self.isLoaded && self.nativeAd != nil) {
        UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
        GGNtScrWidth = unityView.bounds.size.width;
        if (GGNtScrWidth >= unityView.bounds.size.height) {
            GGNtmarginBtClose = 40;
        } else {
            GGNtmarginBtClose = 30;
        }
        if (self.ntFullViewParent == nil) {
            self.ntFullViewParent = [[UIView alloc] initWithFrame:CGRectMake(0, 0, unityView.bounds.size.width, unityView.bounds.size.height)];
            self.ntFullViewParent.clipsToBounds = true;
            [unityView addSubview:self.ntFullViewParent];
        }
        
        if (self.nativeAdView == nil) {
            self.nativeAdView = [[NSBundle mainBundle] loadNibNamed:@"GGNativeAdView" owner:nil options:nil].firstObject;
            [self.ntFullViewParent addSubview:self.nativeAdView];
            self.nativeAdView.translatesAutoresizingMaskIntoConstraints = NO;
            NSDictionary *viewDictionary = NSDictionaryOfVariableBindings(_nativeAdView);
            [self.ntFullViewParent addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[_nativeAdView]|"
                                                                              options:0
                                                                              metrics:nil
                                                                                views:viewDictionary]];
            [self.ntFullViewParent addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[_nativeAdView]|"
                                                                              options:0
                                                                              metrics:nil
                                                                                views:viewDictionary]];
            self.ntFullBtCloseView = [UIButton buttonWithType:UIButtonTypeSystem];
            self.ntFullBtCloseView.frame = CGRectMake(GGNtScrWidth - GGNtmarginBtClose - 10, 40, 20.0, 20.0);
            NSString *string = [NSString stringWithFormat:@"%d", timeBtClose];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateSelected];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateFocused];
            [self.ntFullBtCloseView setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtCloseView];
            
            self.ntFullBtClose = [UIButton buttonWithType:UIButtonTypeSystem];
            [self.ntFullBtClose addTarget:self action:@selector(ntFullBtClose:) forControlEvents:UIControlEventTouchUpInside];
            self.ntFullBtClose.frame = CGRectMake(GGNtScrWidth - GGNtmarginBtClose - 10, 40, 20.0, 20.0);
            [self.ntFullBtClose setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtClose];
        }
        
        self.ntFullViewParent.hidden = NO;
        self.nativeAdView.hidden = NO;
        self.memTimeClose = timeBtClose;
        self.isAutoClose = isAudto;
        [self.ntFullViewParent.superview bringSubviewToFront:self.ntFullViewParent];
        [self populateNativeAdView:self.nativeAd nativeAdView:self.nativeAdView];
        [self.ntFullBtCloseView setBackgroundColor:[UIColor colorWithRed:0.2f green:0.2f blue:0.2f alpha:0.8f]];
        [self.ntFullBtCloseView setBackgroundImage:nil forState:UIControlStateNormal];
        [self.ntFullBtCloseView setTitleColor:UIColor.whiteColor forState:UIControlStateNormal];
        self.countTimeClose = timeBtClose;
        [UIView performWithoutAnimation:^{
            NSString *string = [NSString stringWithFormat:@"%d", self.countTimeClose];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
            [self.ntFullBtCloseView layoutIfNeeded];
        }];
        self.ntFullBtCloseView.titleLabel.font = [UIFont systemFontOfSize:16.0];
        self.ntFullBtCloseView.hidden = NO;
        self.ntFullBtCloseView.enabled = NO;
        self.ntFullBtClose.hidden = YES;
        self.ntFullBtClose.enabled = NO;
        self.isAllowWclick = YES;
        [self cbCounTime:-1];
        [self.adDelegate adDidPresent:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        self.countShow = [[NSUserDefaults standardUserDefaults] integerForKey:@"ggntfull_count_show"];
        self.countShow++;
        [[NSUserDefaults standardUserDefaults] setInteger:self.countShow forKey:@"ggntfull_count_show"];
        if (GGNtCountAfterlic < -100) {
            GGNtCountAfterlic = (int)[[NSUserDefaults standardUserDefaults] integerForKey:@"ggntfull_count_aflick"];
        }
        GGNtCountAfterlic++;
        [[NSUserDefaults standardUserDefaults] setInteger:GGNtCountAfterlic forKey:@"ggntfull_count_aflick"];
        return true;
    } else {
        NSLog(@"mysdk: ads full nt %@ admobnt show fail", self.placement);
        self.isLoaded = NO;
        self.nativeAd = nil;
        self.ntFullViewParent.hidden = YES;
        self.nativeAdView.hidden = YES;
        [self.adDelegate adFailPresent:3 placement:self.placement adId:self.adId adNet:self.adNetLoad withError:@"ad not load"];
        return false;
    }
}
- (void)reCount
{
    if (self.nativeAd != nil)
    {
        [self.ntFullBtCloseView setBackgroundColor:[UIColor colorWithRed:0.2f green:0.2f blue:0.2f alpha:0.8f]];
        [self.ntFullBtCloseView setBackgroundImage:nil forState:UIControlStateNormal];
        [self.ntFullBtCloseView setTitleColor:UIColor.whiteColor forState:UIControlStateNormal];
        self.countTimeClose = self.memTimeClose;
        [UIView performWithoutAnimation:^{
            NSString *string = [NSString stringWithFormat:@"%d", self.countTimeClose];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
            [self.ntFullBtCloseView layoutIfNeeded];
        }];
        self.ntFullBtCloseView.titleLabel.font = [UIFont systemFontOfSize:16.0];
        self.ntFullBtCloseView.hidden = NO;
        self.ntFullBtCloseView.enabled = NO;
        self.ntFullBtClose.hidden = YES;
        self.ntFullBtClose.enabled = NO;
        [self cbCounTime:-1];
    }
}
- (void) cbCounTime:(int)flagNham
{
    dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 1 * NSEC_PER_SEC), dispatch_get_main_queue(), ^{
        self.countTimeClose--;
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"ggntfull_count_click"];
        if (self.countTimeClose > 0) {
            [UIView performWithoutAnimation:^{
                NSString *string = [NSString stringWithFormat:@"%d", self.countTimeClose];
                [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
                [self.ntFullBtCloseView layoutIfNeeded];
            }];
            [self cbCounTime:-1];
        } else {
            int per = -1;
            Boolean isNhamlic = false;
            if (flagNham == -1) {
                if (GGNtCountAfterlic == 1) {
                    per = self.adParent.ntFullFirstRan;
                } else if (GGNtCountAfterlic > 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (GGNtCountAfterlic - 2);
                }
                int pran = arc4random_uniform(101);
                if (per >= pran) {
                    isNhamlic = true;
                }
            }
            if (isNhamlic && self.countTimeClose >= -1 && ((coclick * 100) <= (self.countShow * self.adParent.ntFullPer))) {
                [UIView performWithoutAnimation:^{
                    [self.ntFullBtCloseView setTitle:@"" forState:UIControlStateNormal];
                    [self.ntFullBtCloseView layoutIfNeeded];
                }];
                [self cbCounTime:1];
            } else {
                self.ntFullBtClose.hidden = NO;
                self.ntFullBtClose.enabled = YES;
                CGFloat btSize = 40;
                [self.ntFullBtCloseView setTitle:@"" forState:UIControlStateNormal];
                [self.ntFullBtCloseView setBackgroundImage:[UIImage imageNamed:@"button_close"] forState:UIControlStateNormal];
                if (flagNham == 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (GGNtCountAfterlic - 1);
                    btSize = 22;
                    if (per >= 500) {
                        btSize = 6;
                    } else if (per >= 400) {
                        btSize = 10;
                    } else if (per >= 300) {
                        btSize = 14;
                    } else if (per >= 200) {
                        btSize = 18;
                    }
                    self.ntFullBtClose.frame = CGRectMake(GGNtScrWidth - GGNtmarginBtClose - btSize / 2, 50 - btSize / 2 , btSize, btSize);
                } else {
                    self.ntFullBtClose.frame = CGRectMake(GGNtScrWidth - GGNtmarginBtClose - 20, 30, 40.0, 40.0);
                }
                NSLog(@"mysdk: ads full nt %@ admobnt flagNham clik=%ld, show=%ld cf=%d fran=%d sran=%d non=%d inc=%d after=%d bt=%f", self.placement, coclick, self.countShow, self.adParent.ntFullPer, self.adParent.ntFullFirstRan, self.adParent.ntFullOtherRan, self.adParent.ntFullCountNonAfter, self.adParent.ntFullIncrease, GGNtCountAfterlic, btSize);
            }
        }
    });
}
- (void)ntFullBtClose:(id)sender {
    NSLog(@"mysdk: ads full nt %@ admobnt btclose ", self.placement);
    self.isLoaded = false;
    self.ntFullViewParent.hidden = YES;
    self.nativeAdView.hidden = YES;
    self.nativeAd = nil;
    [self.adDelegate adDidDismiss:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    [[MyAdmobiOS sharedInstance] clearRecount];
}
- (void) populateNativeAdView:(GADNativeAd*)nativeAd nativeAdView:(GADNativeAdView*)nativeAdView
{
    NSLog(@"mysdk: ads full nt %@ admobnt populateNativeAdView ", self.placement);
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
    //((UILabel *)nativeAdView.bodyView).text = nativeAd.body;
    //nativeAdView.bodyView.hidden = nativeAd.body ? NO : YES;

    [((UIButton *)nativeAdView.callToActionView) setTitle:nativeAd.callToAction
                                                 forState:UIControlStateNormal];
    nativeAdView.callToActionView.hidden = nativeAd.callToAction ? NO : YES;

    //((UIImageView *)nativeAdView.iconView).image = nativeAd.icon.image;
    //nativeAdView.iconView.hidden = nativeAd.icon ? NO : YES;
    //nativeAdView.iconView.layer.cornerRadius = 10;
    //nativeAdView.iconView.clipsToBounds = true;

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
    NSLog(@"mysdk: ads full nt %@ admobnt load fail err=%@", self.placement, [error localizedDescription]);
    if (self.isLoading) {
        self.isLoaded = NO;
        self.isLoading = NO;
        self.nativeAd = nil;
        [self.adDelegate didFailToReceiveAd:3 placement:self.placement adId:self.adId withError:[error localizedDescription]];
    }
}

#pragma mark GADNativeAdLoaderDelegate implementation
- (void)adLoader:(GADAdLoader *)adLoader didReceiveNativeAd:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt load ok", self.placement);
    if (self.isLoading) {
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
        self.nativeAd = nativeAd;
        self.nativeAd.delegate = self;
        [self.adDelegate didReceiveAd:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
        NSString* tmpPl = self.placement;
        NSString* tmpid = self.adId;
        id<AdmobMyDelegate> tmpDelegate = self.adDelegate;
        __weak AdmobNtFull *weakSelf = self;
        self.nativeAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
            NSLog(@"mysdk: ads full nt admobnt paidEventHandler v=%@", [value value]);
            long lva = [[value value] doubleValue] * 1000000000;
            int pre = (int)[value precision];
            [tmpDelegate adPaidEvent:3 placement:tmpPl adId:tmpid adNet:weakSelf.adNetLoad precisionType:pre currencyCode:[value currencyCode] valueMicros:lva];
        };
        self.heightConstraint.active = NO;
    }
}

#pragma mark GADVideoControllerDelegate implementation

- (void)videoControllerDidEndVideoPlayback:(GADVideoController *)videoController {
    NSLog(@"mysdk: ads full nt %@ admobnt videoControllerDidEndVideoPlayback", self.placement);
}

#pragma mark GADNativeAdDelegate

- (void)nativeAdDidRecordClick:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdDidRecordClick", self.placement);
    [self.adDelegate adDidClick:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
    if (self.isAllowWclick) {
        self.isAllowWclick = NO;
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"ggntfull_count_click"];
        coclick++;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"ggntfull_count_click"];
        GGNtCountAfterlic = -self.adParent.ntFullCountNonAfter;
        [[NSUserDefaults standardUserDefaults] setInteger:GGNtCountAfterlic forKey:@"ggntfull_count_aflick"];
    }
    if (self.isAutoClose == YES)
    {
        self.isAutoClose = NO;
        dispatch_after(dispatch_time(DISPATCH_TIME_NOW, 1 * NSEC_PER_SEC), dispatch_get_main_queue(), ^{
            if (self.nativeAd != nil) {
                self.ntFullBtClose.enabled = NO;
                self.ntFullBtCloseView.enabled = NO;
                self.isLoaded = false;
                self.ntFullViewParent.hidden = YES;
                self.nativeAdView.hidden = YES;
                self.nativeAd = nil;
                [self.adDelegate adDidDismiss:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
                [[MyAdmobiOS sharedInstance] clearRecount];
            }
        });
    }
}

- (void)nativeAdDidRecordImpression:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdDidRecordImpression", self.placement);
    [self.adDelegate adDidImpression:3 placement:self.placement adId:self.adId adNet:self.adNetLoad];
}

- (void)nativeAdWillPresentScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdWillPresentScreen", self.placement);
}

- (void)nativeAdWillDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdWillDismissScreen", self.placement);
}

- (void)nativeAdDidDismissScreen:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdDidDismissScreen", self.placement);
}

- (void)nativeAdWillLeaveApplication:(GADNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ admobnt nativeAdWillLeaveApplication", self.placement);
}
@end




