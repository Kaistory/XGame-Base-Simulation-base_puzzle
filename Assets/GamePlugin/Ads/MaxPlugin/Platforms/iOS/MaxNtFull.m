#import "MaxNtFull.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>

#import <AppLovinSDK/AppLovinSDK.h>
#import "MyMaxUtiliOS.h"
#import "MyMaxiOS.h"

int MaxNtCountAfterlic = -100000;
int MaxNtScrWidth = 40;
int MaxNtmarginBtClose = 40;

@interface MaxNtFull () <MANativeAdDelegate, MAAdRevenueDelegate>

@property (nonatomic, strong) NSString *adId;

@property(nonatomic, strong) MyMaxiOS* adParent;

@property (nonatomic, strong) UIButton *ntFullBtClose;
@property (nonatomic, strong) UIButton *ntFullBtCloseView;
@property(nonatomic, strong) NSLayoutConstraint *heightConstraint;
@property(nonatomic, strong) MANativeAdLoader *nativeAdLoader;
@property(nonatomic, strong, nullable) MAAd *nativeAd;
@property(nonatomic, strong) MANativeAdView *nativeAdView;
@property(nonatomic, strong) UIView *ntFullViewParent;

@property int memTimeClose;
@property int countTimeClose;
@property NSInteger countShow;
@property BOOL isAllowWclick;
@property BOOL isAutoClose;

@end

@implementation MaxNtFull

- (id)initAd:(NSString *)placement adParent:(MyMaxiOS*)adParent adDelegate:(id<MaxMyDelegate>)adDelegate
{
    self = [super init];
    
    self.adParent = adParent;
    self.adDelegate = adDelegate;
    self.placement = placement;
    
    return self;
}

- (void)load:(NSString *)adId orient:(int)orient
{
    NSLog(@"mysdk: ads full nt %@ maxmynt load=%@", self.placement, adId);
    if (self.nativeAd == nil && self.isLoading == NO && self.isLoaded == NO) {
        self.isLoaded = NO;
        self.isLoading = YES;
        self.adId = adId;
        if (self.nativeAdLoader == nil) {
            self.nativeAdLoader = [[MANativeAdLoader alloc] initWithAdUnitIdentifier: adId];
            self.nativeAdLoader.nativeAdDelegate = self;
            self.nativeAdLoader.revenueDelegate = self;
        } 
        [self cleanUpAdIfNeeded];
        [self.nativeAdLoader loadAd];
    }
}
- (void)cleanUpAdIfNeeded
{
    if ( self.nativeAd ) {
        [self.nativeAdLoader destroyAd: self.nativeAd];
        self.nativeAd = nil;
    }
    
    if ( self.nativeAdView ) {
        [self.nativeAdView removeFromSuperview];
        self.nativeAdView = nil;
    }
}

- (MANativeAdView *)createNativeAdView
{
    //MaxNativeAdView NativeManualAdView
    UINib *nativeAdViewNib = [UINib nibWithNibName: @"MaxNativeAdView" bundle: NSBundle.mainBundle];
    MANativeAdView *ntAdView = [nativeAdViewNib instantiateWithOwner: nil options: nil].firstObject;
    
    MANativeAdViewBinder *binder = [[MANativeAdViewBinder alloc] initWithBuilderBlock:^(MANativeAdViewBinderBuilder *builder) {
        builder.titleLabelTag = 1001;
        builder.bodyLabelTag = 1003;
        builder.iconImageViewTag = 1004;
        builder.mediaContentViewTag = 1006;
        builder.callToActionButtonTag = 1007;
        //builder.advertiserLabelTag = 1002;
        //builder.optionsContentViewTag = 1005;
        //builder.starRatingContentViewTag = 1008;
    }];
    [ntAdView bindViewsWithAdViewBinder: binder];
    return ntAdView;
}
- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto
{
    NSLog(@"mysdk: ads full nt %@ maxmynt show ", self.placement);
    if (self.isLoaded && self.nativeAd != nil) {
        UIView *unityView = [MyMaxUtiliOS unityGLViewController].view;
        MaxNtScrWidth = unityView.bounds.size.width;
        if (MaxNtScrWidth >= unityView.bounds.size.height) {
            MaxNtmarginBtClose = 40;
        } else {
            MaxNtmarginBtClose = 30;
        }
        bool isCreate = false;
        if (self.ntFullViewParent == nil) {
            isCreate = true;
            self.ntFullViewParent = [[UIView alloc] initWithFrame:CGRectMake(0, 0, unityView.bounds.size.width, unityView.bounds.size.height)];
            self.ntFullViewParent.clipsToBounds = true;
            [unityView addSubview:self.ntFullViewParent];
        }
        
        self.nativeAdView = [self createNativeAdView];
        [self.nativeAdLoader renderNativeAdView: self.nativeAdView withAd: self.nativeAd];
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
        
        // Set ad view to span width and height of container and center the ad
        [self.ntFullViewParent.widthAnchor constraintEqualToAnchor: self.nativeAdView.widthAnchor].active = YES;
        [self.ntFullViewParent.heightAnchor constraintEqualToAnchor: self.nativeAdView.heightAnchor].active = YES;
        [self.ntFullViewParent.centerXAnchor constraintEqualToAnchor: self.nativeAdView.centerXAnchor].active = YES;
        [self.ntFullViewParent.centerYAnchor constraintEqualToAnchor: self.nativeAdView.centerYAnchor].active = YES;
        
        if (isCreate) {
            self.ntFullBtCloseView = [UIButton buttonWithType:UIButtonTypeSystem];
            self.ntFullBtCloseView.frame = CGRectMake(MaxNtScrWidth - MaxNtmarginBtClose - 10, 40, 20.0, 20.0);
            NSString *string = [NSString stringWithFormat:@"%d", timeBtClose];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateNormal];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateSelected];
            [self.ntFullBtCloseView setTitle:string forState:UIControlStateFocused];
            [self.ntFullBtCloseView setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtCloseView];
            
            self.ntFullBtClose = [UIButton buttonWithType:UIButtonTypeSystem];
            [self.ntFullBtClose addTarget:self action:@selector(ntFullBtClose:) forControlEvents:UIControlEventTouchUpInside];
            self.ntFullBtClose.frame = CGRectMake(MaxNtScrWidth - MaxNtmarginBtClose - 10, 40, 20.0, 20.0);
            [self.ntFullBtClose setBackgroundImage:nil forState:UIControlStateNormal];
            [self.ntFullViewParent addSubview:self.ntFullBtClose];
        }
        else
        {
            [self.ntFullViewParent bringSubviewToFront:self.ntFullBtCloseView];
            [self.ntFullViewParent bringSubviewToFront:self.ntFullBtClose];
        }
        
        self.ntFullViewParent.hidden = NO;
        self.nativeAdView.hidden = NO;
        self.memTimeClose = timeBtClose;
        self.isAutoClose = isAudto;
        [self.ntFullViewParent.superview bringSubviewToFront:self.ntFullViewParent];
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
        self.countShow = [[NSUserDefaults standardUserDefaults] integerForKey:@"maxntfull_count_show"];
        self.countShow++;
        [[NSUserDefaults standardUserDefaults] setInteger:self.countShow forKey:@"maxntfull_count_show"];
        if (MaxNtCountAfterlic < -100) {
            MaxNtCountAfterlic = (int)[[NSUserDefaults standardUserDefaults] integerForKey:@"maxntfull_count_aflick"];
        }
        MaxNtCountAfterlic++;
        [[NSUserDefaults standardUserDefaults] setInteger:MaxNtCountAfterlic forKey:@"maxntfull_count_aflick"];
        return true;
    } else {
        NSLog(@"mysdk: ads full nt %@ maxmynt show fail", self.placement);
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
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"maxntfull_count_click"];
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
                if (MaxNtCountAfterlic == 1) {
                    per = self.adParent.ntFullFirstRan;
                } else if (MaxNtCountAfterlic > 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (MaxNtCountAfterlic - 2);
                }
                int pran = arc4random_uniform(101);
                if (per >= pran) {
                    isNhamlic = true;
                }
            }
            NSLog(@"mysdk: ads full nt %@ maxmynt cbCounTime clik=%ld, show=%ld nham=%d isNhamlic=%d per=%d", self.placement, coclick, self.countShow, flagNham, isNhamlic, self.adParent.ntFullPer);
            if (isNhamlic && self.countTimeClose >= -1 && ((coclick * 100) <= (self.countShow * self.adParent.ntFullPer))) {
                [UIView performWithoutAnimation:^{
                    [self.ntFullBtCloseView setTitle:@"" forState:UIControlStateNormal];
                    [self.ntFullBtCloseView layoutIfNeeded];
                }];
                [self cbCounTime:1];
            } else {
                self.ntFullBtClose.hidden = NO;
                self.ntFullBtClose.enabled = YES;
                [self.ntFullBtCloseView setTitle:@"" forState:UIControlStateNormal];
                [self.ntFullBtCloseView setBackgroundImage:[UIImage imageNamed:@"button_close"] forState:UIControlStateNormal];
                if (flagNham == 1) {
                    per = self.adParent.ntFullOtherRan + self.adParent.ntFullIncrease * (MaxNtCountAfterlic - 2);
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
                    self.ntFullBtClose.frame = CGRectMake(MaxNtScrWidth - MaxNtmarginBtClose - btSize / 2, 50 - btSize / 2 , btSize, btSize);
                } else {
                    self.ntFullBtClose.frame = CGRectMake(MaxNtScrWidth - MaxNtmarginBtClose - 20, 30, 40.0, 40.0);
                }
            }
        }
    });
}
- (void)ntFullBtClose:(id)sender {
    NSLog(@"mysdk: ads full nt %@ maxmynt btclose ", self.placement);
    self.isLoaded = false;
    self.ntFullViewParent.hidden = YES;
    self.nativeAdView.hidden = YES;
    self.nativeAd = nil;
    [self.adDelegate adDidDismiss:3 placement:self.placement adId:self.adId];
    [[MyMaxiOS sharedInstance] clearRecount];
}

#pragma mark - NativeAdDelegate Protocol
- (void)didLoadNativeAd:(nullable MANativeAdView *)nativeAdView forAd:(MAAd *)ad
{
    NSLog(@"mysdk: ads full nt %@ maxmynt load ok", self.placement);
    if (self.isLoading) {
        self.isLoaded = YES;
        self.isLoading = NO;
        [self cleanUpAdIfNeeded];
        
        self.nativeAd = ad;
        [self.adDelegate didReceiveAd:3 placement:self.placement adId:self.adId netName:[ad networkName]];
        self.heightConstraint.active = NO;
    }
}
- (void)didFailToLoadNativeAdForAdUnitIdentifier:(NSString *)adUnitIdentifier withError:(MAError *)error
{
    NSLog(@"mysdk: ads full nt %@ maxmynt load fail err=%@", self.placement, [error message]);
    if (self.isLoading) {
        self.isLoaded = NO;
        self.isLoading = NO;
        self.nativeAd = nil;
        [self.adDelegate didFailToReceiveAd:3 placement:self.placement adId:self.adId withError:[error message]];
    }
}
- (void)didClickNativeAd:(MAAd *)ad
{
    NSLog(@"mysdk: ads full nt %@ maxmynt nativeAdDidRecordClick", self.placement);
    [self.adDelegate adDidClick:3 placement:self.placement adId:self.adId];
    if (self.isAllowWclick) {
        self.isAllowWclick = NO;
        NSInteger coclick = [[NSUserDefaults standardUserDefaults] integerForKey:@"maxntfull_count_click"];
        coclick++;
        [[NSUserDefaults standardUserDefaults] setInteger:coclick forKey:@"maxntfull_count_click"];
        MaxNtCountAfterlic = -self.adParent.ntFullCountNonAfter;
        [[NSUserDefaults standardUserDefaults] setInteger:MaxNtCountAfterlic forKey:@"maxntfull_count_aflick"];
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
                [[MyMaxiOS sharedInstance] clearRecount];
            }
        });
    }
}
- (void)didPayRevenueForAd:(MAAd *)ad
{
    [self.adDelegate adPaidEvent:3 placement:self.placement adId:[ad adUnitIdentifier] adNet:[ad networkName] adFormat:[[ad format] label] adPlacement:[ad placement] netPlacement:[ad networkPlacement] adValue:[ad revenue]];
}
@end


