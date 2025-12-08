#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "AdmobFullBase.h"

@interface AdmobGift : AdmobFullBase

- (id)initAd:(NSString *)placement adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show;

@end
