#import "FbNtFull.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <FBAudienceNetwork/FBAudienceNetwork.h>
#import <GoogleMobileAds/GoogleMobileAds.h>
#import "FbMyUtiliOS.h"
#import "FbMyiOS.h"

int FbNtCountAfterlic = -100000;
int FbNtScrWidth = 40;
int FbNtmarginBtClose = 40;

@interface FbNtFull () <FBNativeAdDelegate, FBMediaViewDelegate>

@property (nonatomic, strong) NSString *adId;

@property(nonatomic, strong) FbMyiOS* adParent;

@property (nonatomic, strong) UIButton *ntFullBtClose;
@property (nonatomic, strong) UIButton *ntFullBtCloseView;
@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;
@property(nonatomic, strong) FBNativeAd *nativeAd;
@property(nonatomic, strong) FBNativeAdView *nativeAdView;
@property(nonatomic, strong) UIView *ntFullViewParent;

@property int memTimeClose;
@property int countTimeClose;
@property NSInteger countShow;
@property BOOL isAllowWclick;
@property BOOL isAutoClose;

@end

@implementation FbNtFull

- (id)initAd:(NSString *)placement adParent:(FbMyiOS*)adParent adDelegate:(id<FbMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    
    return self;
}

- (void)load:(NSString *)adId orient:(int)orient
{
    NSLog(@"mysdk: ads full nt %@ fbnt load=%@", self.placement, adId);
    if (self.nativeAd == nil && self.isLoading == NO && self.isLoaded == NO) {
        self.isLoaded = NO;
        self.isLoading = YES;
        self.adId = adId;
        //UIViewController *unityViewctr = [FbMyUtiliOS unityGLViewController];
        self.nativeAd = [[FBNativeAd alloc] initWithPlacementID:adId];
        self.nativeAd.delegate = self;
        [self.nativeAd loadAd];
    }
}

- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto
{
    NSLog(@"mysdk: ads full nt %@ fbnt show ", self.placement);
    if (self.isLoaded && self.nativeAd != nil) {
        UIViewController* vctrl = [FbMyUtiliOS unityGLViewController];
        UIView *unityView = vctrl.view;
        FbNtScrWidth = unityView.bounds.size.width;
        if (FbNtScrWidth >= unityView.bounds.size.height) {
            FbNtmarginBtClose = 40;
        } else {
            FbNtmarginBtClose = 30;
        }
        if (self.ntFullViewParent == nil) {
            self.ntFullViewParent = [[UIView alloc] initWithFrame:CGRectMake(0, 0, unityView.bounds.size.width, unityView.bounds.size.height)];
            self.ntFullViewParent.clipsToBounds = true;
            [unityView addSubview:self.ntFullViewParent];
        }
        
        if (self.nativeAdView == nil) {
            UINib *nativeAdViewNib = [UINib nibWithNibName: @"FbNativeAdView" bundle: NSBundle.mainBundle];
            self.nativeAdView = [nativeAdViewNib instantiateWithOwner: nil options: nil].firstObject;
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
            self.ntFullBtCloseView.frame = CGRectMake(FbNtScrWidth - FbNtmarginBtClose - 10, 40, 20.0, 20.0);
            NSString *string = [NSString stringWithFormat:@"%d", timeBtClose];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateSelected];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateFocused];
            [self.ntFullBtCloseView setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtCloseView];
            
            self.ntFullBtClose = [UIButton buttonWithType:UIButtonTypeSystem];
            [self.ntFullBtClose addTarget:self action:@selector(ntFullBtClose:) forControlEvents:UIControlEventTouchUpInside];
            self.ntFullBtClose.frame = CGRectMake(FbNtScrWidth - FbNtmarginBtClose - 10, 40, 20.0, 20.0);
            [self.ntFullBtClose setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtClose];
        }
        
        self.ntFullViewParent.hidden = NO;
        self.nativeAdView.hidden = NO;
        self.memTimeClose = timeBtClose;
        self.isAutoClose = isAudto;
        [self.ntFullViewParent.superview bringSubviewToFront:self.ntFullViewParent];
        [self populateNativeAdView:self.nativeAd nativeAdView:self.nativeAdView viewController:vctrl];
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
        [self.adDelegate adDidPresent:3 placement:self.placement adId:self.adId];
        self.countShow = [[NSUserDefaults standardUserDefaults] integerForKey:@"fbntfull_count_show"];
        self.countShow++;
        [[NSUserDefaults standardUserDefaults] setInteger:self.countShow forKey:@"fbntfull_count_show"];
        if (FbNtCountAfterlic < -100) {
            FbNtCountAfterlic = (int)[[NSUserDefaults standardUserDefaults] integerForKey:@"fbntfull_count_aflick"];
        }
        FbNtCountAfterlic++;
        [[NSUserDefaults standardUserDefaults] setInteger:FbNtCountAfterlic forKey:@"fbntfull_count_aflick"];
        return true;
    } else {
        NSLog(@"mysdk: ads full nt %@ fbnt show fail", self.placement);
        self.isLoaded = NO;
        self.nativeAd = nil;
        self.ntFullViewParent.hidden = YES;
        self.nativeAdView.hidden = YES;
        [self.adDelegate adFailPresent:3 placement:self.placement adId:self.adId withError:@"ad not load"];
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
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"fbntfull_count_click"];
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
                if (FbNtCountAfterlic == 1) {
                    per = self.adParent.ntFullFirstRan;
                } else if (FbNtCountAfterlic > 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (FbNtCountAfterlic - 2);
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
                NSLog(@"mysdk: ads full nt %@ fbnt cbCounTime clik=%ld, show=%ld nham=%d isNhamlic=%d per=%d", self.placement, coclick, self.countShow, flagNham, isNhamlic, self.adParent.ntFullPer);
                self.ntFullBtClose.hidden = NO;
                self.ntFullBtClose.enabled = YES;
                [self.ntFullBtCloseView setTitle:@"" forState:UIControlStateNormal];
                [self.ntFullBtCloseView setBackgroundImage:[UIImage imageNamed:@"button_close"] forState:UIControlStateNormal];
                if (flagNham == 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (FbNtCountAfterlic - 2);
                    CGFloat btSize = 20;
                    if (per >= 500) {
                        btSize = 6;
                    } else if (per >= 400) {
                        btSize = 10;
                    } else if (per >= 300) {
                        btSize = 14;
                    } else if (per >= 200) {
                        btSize = 18;
                    }
                    self.ntFullBtClose.frame = CGRectMake(FbNtScrWidth - FbNtmarginBtClose - btSize / 2, 50 - btSize / 2 , btSize, btSize);
                } else {
                    self.ntFullBtClose.frame = CGRectMake(FbNtScrWidth - FbNtmarginBtClose - 20, 30, 40.0, 40.0);
                }
            }
        }
    });
}
- (void)ntFullBtClose:(id)sender {
    NSLog(@"mysdk: ads full nt %@ fbnt btclose ", self.placement);
    self.isLoaded = false;
    self.ntFullViewParent.hidden = YES;
    self.nativeAdView.hidden = YES;
    self.nativeAd = nil;
    [self.adDelegate adDidDismiss:3 placement:self.placement adId:self.adId];
    [[FbMyiOS sharedInstance] clearRecount];
}
- (void) populateNativeAdView:(FBNativeAd*)nativeAd nativeAdView:(FBNativeAdView*)nativeAdView viewController:(UIViewController*)viewController
{
    NSLog(@"mysdk: ads full nt %@ fbnt populateNativeAdView ", self.placement);
    if (nativeAd && nativeAd.isAdValid) {
        [nativeAd unregisterView];
    }
    
    FBAdIconView* adIconImageView = [nativeAdView viewWithTag:112];
    FBAdChoicesView* adChoice = [nativeAdView viewWithTag:113];
    UILabel* adTitle = [nativeAdView viewWithTag:1141];
    UILabel* adsponsorLabel = [nativeAdView viewWithTag:1142];
    UIButton* adCallToActionButton = [nativeAdView viewWithTag:1151];
    UILabel* adSocial = [nativeAdView viewWithTag:1152];
    UILabel* adBody = [nativeAdView viewWithTag:1153];
    FBMediaView* mediaView = [nativeAdView viewWithTag:116];
    
    [self.nativeAd registerViewForInteraction:nativeAdView
                                      mediaView:mediaView
                                       iconView:adIconImageView
                              viewController:viewController
                             clickableViews:@[adCallToActionButton, mediaView]];
    
    adChoice.nativeAd = nativeAd;
    adTitle.text = nativeAd.advertiserName;
    adBody.text = nativeAd.bodyText;
    adsponsorLabel.text = nativeAd.sponsoredTranslation;
    adSocial.text = nativeAd.socialContext;
    [adCallToActionButton setTitle:nativeAd.callToAction forState:UIControlStateNormal];
}

#pragma mark FBNativeAdDelegate implementation
- (void)nativeAdDidLoad:(FBNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ fbnt load ok", self.placement);
    if (self.isLoading) {
        self.isLoaded = YES;
        self.isLoading = NO;
        [self.adDelegate didReceiveAd:3 placement:self.placement adId:self.adId];
//        NSString* tmpPl = self.placement;
//        NSString* tmpid = self.adId;
//        id<FbMyDelegate> tmpDelegate = self.adDelegate;
//        self.nativeAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
//            NSLog(@"mysdk: ads full nt admobnt paidEventHandler v=%@", [value value]);
//            long lva = [[value value] doubleValue] * 1000000000;
//            int pre = (int)[value precision];
//            [tmpDelegate adPaidEvent:3 placement:tmpPl adId:tmpid precisionType:pre currencyCode:[value currencyCode] valueMicros:lva];
//        };
        self.heightConstraint.active = NO;
    }
}

/**
 Sent when a FBNativeAd is failed to load.


 @param nativeAd A FBNativeAd object sending the message.
 @param error An error object containing details of the error.
 */
- (void)nativeAd:(FBNativeAd *)nativeAd didFailWithError:(NSError *)error {
    NSLog(@"mysdk: ads full nt %@ fbnt load fail err=%@", self.placement, [error localizedDescription]);
    if (self.isLoading) {
        self.isLoaded = NO;
        self.isLoading = NO;
        self.nativeAd = nil;
        [self.adDelegate didFailToReceiveAd:3 placement:self.placement adId:self.adId withError:[error localizedDescription]];
    }
}

/**
 Sent when a FBNativeAd has succesfully downloaded all media
 */
- (void)nativeAdDidDownloadMedia:(FBNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ fbnt nativeAdDidDownloadMedia", self.placement);
}

/**
 Sent immediately before the impression of a FBNativeAd object will be logged.


 @param nativeAd A FBNativeAd object sending the message.
 */
- (void)nativeAdWillLogImpression:(FBNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ fbnt nativeAdWillLogImpression", self.placement);
}

/**
 Sent after an ad has been clicked by the person.


 @param nativeAd A FBNativeAd object sending the message.
 */
- (void)nativeAdDidClick:(FBNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ fbnt nativeAdDidClick", self.placement);
    [self.adDelegate adDidClick:3 placement:self.placement adId:self.adId];
    if (self.isAllowWclick) {
        self.isAllowWclick = NO;
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"fbntfull_count_click"];
        coclick++;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"fbntfull_count_click"];
        FbNtCountAfterlic = -self.adParent.ntFullCountNonAfter;
        [[NSUserDefaults standardUserDefaults] setInteger:FbNtCountAfterlic forKey:@"fbntfull_count_aflick"];
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
                [self.adDelegate adDidDismiss:3 placement:self.placement adId:self.adId];
                [[FbMyiOS sharedInstance] clearRecount];
            }
        });
    }
}

/**
 When an ad is clicked, the modal view will be presented. And when the user finishes the
 interaction with the modal view and dismiss it, this message will be sent, returning control
 to the application.


 @param nativeAd A FBNativeAd object sending the message.
 */
- (void)nativeAdDidFinishHandlingClick:(FBNativeAd *)nativeAd {
    NSLog(@"mysdk: ads full nt %@ fbnt nativeAdDidFinishHandlingClick", self.placement);
}
@end




