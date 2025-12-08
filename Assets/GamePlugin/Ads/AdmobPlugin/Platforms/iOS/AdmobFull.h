#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "AdmobFullBase.h"

@interface AdmobFull : AdmobFullBase

- (id)initAd:(NSString *)placement adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show:(int)timeDelay;

@end
