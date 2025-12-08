#import <Foundation/Foundation.h>

@protocol AdmobMyDelegate <NSObject>

@optional

- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error;
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet withError:(NSString *)error;
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet;
- (void)adWillDestroy:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet precisionType:(int)precisionType currencyCode:(NSString *)currencyCode valueMicros:(long) valueMicros;

@end

