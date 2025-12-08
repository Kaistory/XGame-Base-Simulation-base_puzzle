#import <Foundation/Foundation.h>
#import "MaxMyDelegate.h"
#import "MaxFullBase.h"

@interface MaxOpenAd : MaxFullBase

- (id)initAd:(NSString *)placement adDelegate:(id<MaxMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show;

@end
