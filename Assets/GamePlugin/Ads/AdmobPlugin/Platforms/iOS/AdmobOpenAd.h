#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "AdmobFullBase.h"

@interface AdmobOpenAd : AdmobFullBase

- (id)initAd:(NSString *)placement adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show;

@end
