#import <Foundation/Foundation.h>
#import "UnityAppController.h"

const char *const GameAdsHelper_NAME = "GameAdsHelperBridge";

//@class VibrateHelper;
@interface MyGameAppController : UnityAppController

+ (void)configAppOpenAd:(int)timeBg orien:(int)orien;
+ (void)loadAppOpenAd:(NSString*)adUnitId;
+ (bool)isAppOpenAdLoaded;
+ (bool)showAppOpenAd;
+ (void)requestIDFA:(int)allversion;

@end

