//
//  MyGameAppController.mm
//
//  Created by Shachar Aharon on 15/05/2017.
//
//
#import "MyGameAppController.h"
#import <GoogleMobileAds/GoogleMobileAds.h>
#import <FBAudienceNetwork/FBAdSettings.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <AdSupport/AdSupport.h>
#import "MyGameNativeiOS.h"

bool isAppOpenAdLoading = false;
bool isAppOpenAdShowing = false;
int AppOpenAdTimeBg = 5;
int AppOpenAdOrien = 0;
bool isOpenAdShowOpen = true;
NSTimeInterval tAppOenAdBg = 0;

static MyGameAppController *MyGameAppControllerInstance = nil;

@interface MyGameAppController () <GADFullScreenContentDelegate>

@property(nonatomic) GADAppOpenAd *appOpenAd;
@property(nonatomic, strong) NSString *adNetLoad;
@property(nonatomic, strong) NSString *appOpenAdUnitId;

@end

@implementation MyGameAppController

- (BOOL)application:(UIApplication *)application willFinishLaunchingWithOptions:(nullable NSDictionary<UIApplicationLaunchOptionsKey, id> *)launchOptions
{
    BOOL re = [super application:application willFinishLaunchingWithOptions:launchOptions];
    
    //[MyGameAppController mylog:@"application willFinishLaunchingWithOptions"];
    MyGameAppControllerInstance = self;
    
    NSInteger co = [[NSUserDefaults standardUserDefaults] integerForKey:@"count_game_open"];
    co++;
    [[NSUserDefaults standardUserDefaults] setInteger:co forKey:@"count_game_open"];
    
    [FBAdSettings setAdvertiserTrackingEnabled:YES];
    
    dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0);
    dispatch_async(queue, ^{
        [self getOnlineTime];
    });
    
    return re;
}

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(nullable NSDictionary<UIApplicationLaunchOptionsKey, id> *)launchOptions
{
    BOOL re = [super application:application didFinishLaunchingWithOptions:launchOptions];
    //[MyGameAppController mylog:@"application didFinishLaunchingWithOptions"];
    return re;
}

- (void)applicationDidBecomeActive:(UIApplication*)application
{
    [super applicationDidBecomeActive:application];
    //[MyGameAppController mylog:@"applicationDidBecomeActive"];
    UnitySendMessage(GameAdsHelper_NAME, "gameIsBecomeActive", "");
    if (isOpenAdShowOpen) {
        isOpenAdShowOpen = false;
        NSTimeInterval tcurr = [self SystemCurrentMilisecond];
        NSTimeInterval deltbg = tcurr - tAppOenAdBg;
        if (deltbg >= AppOpenAdTimeBg) {
            UnitySendMessage(GameAdsHelper_NAME, "showOpenAd", "");
        } else {
            [MyGameAppController mylog:[NSString stringWithFormat:@"openad not meet condition time bg cf=%d delbg=%f", AppOpenAdTimeBg, deltbg]];
        }
    }
}

- (void)applicationWillResignActive:(UIApplication *)application
{
    [super applicationWillResignActive:application];
    //[MyGameAppController mylog:@"applicationWillResignActive"];
    UnitySendMessage(GameAdsHelper_NAME, "gameIsResignActive", "");
}

- (void)applicationDidEnterBackground:(UIApplication *)application
{
    [super applicationDidEnterBackground:application];
    isOpenAdShowOpen = true;
    tAppOenAdBg = [self SystemCurrentMilisecond];
    //[MyGameAppController mylog:@"applicationDidEnterBackground"];
}

- (void)applicationWillEnterForeground:(UIApplication *)application
{
    [super applicationWillEnterForeground:application];
    [UIApplication sharedApplication].applicationIconBadgeNumber = 0;
    //[MyGameAppController mylog:@"applicationWillEnterForeground"];
}

-(void) getOnlineTime
{
    //    NSURL *url = [NSURL URLWithString:@"http://worldtimeapi.org/api/timezone/Asia/Bangkok"];
    //    NSData *data = [NSData dataWithContentsOfURL:url];
    //    NSString *ret = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    //    if(NSClassFromString(@"NSJSONSerialization"))
    //    {
    //        NSError *error = nil;
    //        id object = [NSJSONSerialization
    //                     JSONObjectWithData:[ret dataUsingEncoding:NSUTF8StringEncoding]
    //                     options:0
    //                     error:&error];
    //
    //        if(!error) {
    //            if([object isKindOfClass:[NSDictionary class]])
    //            {
    //                NSDictionary *results = object;
    //                if (results != nil)
    //                {
    //                    long tcurr = [[results objectForKey:@"unixtime"] longLongValue];
    //                    synchronizeTimeNative(tcurr);
    //                }
    //            }
    //        }
    //    }
    
    try {
        NSURL *url = [NSURL URLWithString:@"https://www.google.com"];
        NSURLRequest *request = [NSURLRequest requestWithURL: url];
        NSHTTPURLResponse *response;
        [NSURLConnection sendSynchronousRequest: request returningResponse: &response error: nil];
        if ([response respondsToSelector:@selector(allHeaderFields)]) {
            NSDictionary *dictionary = [response allHeaderFields];
            NSString* nsd = [dictionary objectForKey:@"Date"];
            if (nsd != nil) {
                NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
                dateFormatter.locale = [[NSLocale alloc] initWithLocaleIdentifier:@"en_US"];
                [dateFormatter setDateFormat:@"EEE, dd MMM yyyy HH:mm:ss Z"];
                NSDate *date = [dateFormatter dateFromString:nsd];
                long tccc = [date timeIntervalSince1970];
                synchronizeTimeNative(tccc);
            }
        }
    }
    catch (NSException *exception) {
    }
}

+ (void)requestIDFA:(int)allversion
{
    // dispatch_after(dispatch_time(DISPATCH_TIME_NOW, (int64_t)(1.0 * NSEC_PER_SEC)), dispatch_get_main_queue(), ^{
    dispatch_async(dispatch_get_main_queue(), ^{
        if (allversion == 0) {
            if (@available(iOS 14.5, *)) {
                [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                    // Tracking authorization completed. Start loading ads here.
                    // [self loadAd];
                    [MyGameAppController mylog:[NSString stringWithFormat:@"requestIDFA: %lu", (unsigned long)status]];
                    NSString* nsstatus = [NSString stringWithFormat:@"%lu", (unsigned long)status];
                    UnitySendMessage(GameAdsHelper_NAME, "requestIDFACallBack", [nsstatus UTF8String]);
                }];
            } else {
                UnitySendMessage(GameAdsHelper_NAME, "requestIDFACallBack", "3");
            }
        } else {
            if (@available(iOS 14, *)) {
                [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status) {
                    // Tracking authorization completed. Start loading ads here.
                    // [self loadAd];
                    [MyGameAppController mylog:[NSString stringWithFormat:@"requestIDFA: %lu", (unsigned long)status]];
                    NSString* nsstatus = [NSString stringWithFormat:@"%lu", (unsigned long)status];
                    UnitySendMessage(GameAdsHelper_NAME, "requestIDFACallBack", [nsstatus UTF8String]);
                }];
            } else {
                UnitySendMessage(GameAdsHelper_NAME, "requestIDFACallBack", "3");
            }
        }
    });
}

+(void)mylog:(NSString*) msg
{
    NSLog(@"mysdk: ads open %@", msg);
}
- (NSTimeInterval) SystemCurrentMilisecond
{
    NSTimeInterval timeStamp = [[NSDate date] timeIntervalSince1970];
    return timeStamp;
}
#pragma mark - AppOpenAd
+ (void)configAppOpenAd:(int)timeBg orien:(int)orien {
    AppOpenAdTimeBg = timeBg;
    AppOpenAdOrien = orien;
}
+ (void)loadAppOpenAd:(NSString*)adUnitId {
    if (MyGameAppControllerInstance != nil) {
        [MyGameAppControllerInstance fetchAppOpenAd:adUnitId];
    }
}
+ (bool)isAppOpenAdLoaded {
    if (MyGameAppControllerInstance != nil) {
        return (MyGameAppControllerInstance.appOpenAd != nil);
    } else {
        return false;
    }
}
+ (bool)showAppOpenAd {
    if (MyGameAppControllerInstance != nil) {
        return [MyGameAppControllerInstance showAppOenAdIfVailable];
    } else {
        return false;
    }
}

- (void)fetchAppOpenAd:(NSString*)adUnitId
{
    if ([MyGameAppController isAppOpenAdLoaded]) {
        [MyGameAppController mylog:@"openad fetchAppOpenAd ad is loaded"];
        return;
    }
    [MyGameAppController mylog:@"openad fetchAppOpenAd fetch"];
    // adunitid =@"ca-app-pub-3940256099942544/5662855259";
    if (!isAppOpenAdLoading && adUnitId != nil && [adUnitId length] > 10) {
        [MyGameAppController mylog:[NSString stringWithFormat:@"openad fetchAppOpenAd 2=%@", adUnitId]];
        self.appOpenAd = nil;
        self.appOpenAdUnitId = adUnitId;
        UIInterfaceOrientation uiorien;
        if (AppOpenAdOrien == 0) {
            uiorien = UIInterfaceOrientationPortrait;
        } else {
            uiorien = UIInterfaceOrientationLandscapeLeft;
        }
        isAppOpenAdLoading = true;
        NSString* adOpenParam = [NSString stringWithFormat:@"%@", adUnitId];
        UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdLoad", [adOpenParam UTF8String]);
        [GADAppOpenAd loadWithAdUnitID:adUnitId
                               request:[GADRequest request]
                           //orientation:uiorien
                     completionHandler:^(GADAppOpenAd *_Nullable appOpenAd, NSError *_Nullable error) {
            if (error) {
                [MyGameAppController mylog:[NSString stringWithFormat:@"openad Failed to load app open ad: %@", error]];
                isAppOpenAdLoading = false;
                NSString* adOpenParame = [NSString stringWithFormat:@"0;%@;%ld", adUnitId, error.code];
                UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdLoadResult", [adOpenParame UTF8String]);
            } else {
                self.appOpenAd = appOpenAd;
                self.adNetLoad = [[[appOpenAd responseInfo] loadedAdNetworkResponseInfo] adNetworkClassName];
                self.appOpenAd.fullScreenContentDelegate = self;
                NSString* adOpenParamo = [NSString stringWithFormat:@"1;%@;%@", adUnitId, self.adNetLoad];
                UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdLoadResult", [adOpenParamo UTF8String]);
                __weak MyGameAppController *weakSelf = self;
                self.appOpenAd.paidEventHandler = ^(GADAdValue * _Nonnull value) {
                    long lva = [[value value] doubleValue] * 1000000000;
                    NSString* paidParam = [NSString stringWithFormat:@"%@;%@;%ld;%@;%ld", weakSelf.appOpenAdUnitId, weakSelf.adNetLoad, [value precision], [value currencyCode], lva];
                    UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdPaidEvent", [paidParam UTF8String]);
                };
                isAppOpenAdLoading = false;
                [MyGameAppController mylog:@"openad fetchAppOpenAd ok"];
            }
        }];
    }
}
- (bool) showAppOenAdIfVailable {
    [MyGameAppController mylog:@"openad showAppOenAdIfVailable"];
    if (!isAppOpenAdShowing && _appOpenAd != nil) {
        GADAppOpenAd *ad = self.appOpenAd;
        ad.fullScreenContentDelegate = self;
        UIViewController *rootController = self.window.rootViewController;
        [ad presentFromRootViewController:rootController];
        return true;
    } else {
        if (isAppOpenAdShowing) {
            [MyGameAppController mylog:@"openad showAppOenAdIfVailable is ads showing"];
        } else {
            [MyGameAppController mylog:@"openad showAppOenAdIfVailable is ads no avaiable"];
            [self fetchAppOpenAd:_appOpenAdUnitId];
        }
        return false;
    }
}

#pragma mark - GADFullScreenContentDelegate

/// Tells the delegate that the ad failed to present full screen content.
- (void)ad:(nonnull id<GADFullScreenPresentingAd>)ad
didFailToPresentFullScreenContentWithError:(nonnull NSError *)error {
    [MyGameAppController mylog:@"openad didFailToPresentFullSCreenCContentWithError"];
    self.appOpenAd = nil;
    isAppOpenAdShowing = false;
    [self fetchAppOpenAd:self.appOpenAdUnitId];
    NSString* adOpenParam = [NSString stringWithFormat:@"%@;%@;%ld", self.appOpenAdUnitId, self.adNetLoad, error.code];
    UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdShowFailed", [adOpenParam UTF8String]);
}

/// Tells the delegate that the ad dismissed full screen content.
- (void)adDidDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad {
    [MyGameAppController mylog:@"openad adDidDismissFullScreenContent"];
    NSString* adOpenParam = [NSString stringWithFormat:@"%@;%@", self.appOpenAdUnitId, self.adNetLoad];
    UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdDismiss", [adOpenParam UTF8String]);
    self.appOpenAd = nil;
    isAppOpenAdShowing = false;
    [self fetchAppOpenAd:self.appOpenAdUnitId];
}

- (void)adDidRecordImpression:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSString* adOpenParam = [NSString stringWithFormat:@"%@;%@", self.appOpenAdUnitId, self.adNetLoad];
    UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdImpression", [adOpenParam UTF8String]);
}

/// Tells the delegate that a click has been recorded for the ad.
- (void)adDidRecordClick:(nonnull id<GADFullScreenPresentingAd>)ad
{
    NSString* adOpenParam = [NSString stringWithFormat:@"%@;%@", self.appOpenAdUnitId, self.adNetLoad];
    UnitySendMessage(GameAdsHelper_NAME, "onAppOpenAdClicked", [adOpenParam UTF8String]);
}

/// Tells the delegate that the ad will present full screen content.
- (void)adWillPresentFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad
{
    [MyGameAppController mylog:@"openad adWillPresentFullScreenContent"];
    isAppOpenAdShowing = true;
}

/// Tells the delegate that the ad will dismiss full screen content.
- (void)adWillDismissFullScreenContent:(nonnull id<GADFullScreenPresentingAd>)ad
{
}

@end

IMPL_APP_CONTROLLER_SUBCLASS(MyGameAppController)

