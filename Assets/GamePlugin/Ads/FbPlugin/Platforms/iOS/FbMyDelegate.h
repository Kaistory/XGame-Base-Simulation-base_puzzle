#import <Foundation/Foundation.h>

@protocol FbMyDelegate <NSObject>

@optional

- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error;
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error;
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adWillDestroy:(int)adType placement:(NSString *)placement adId:(NSString *)adId;
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet precisionType:(int)precisionType currencyCode:(NSString *)currencyCode valueMicros:(long) valueMicros;

@end

