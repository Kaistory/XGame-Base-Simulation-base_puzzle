#import <Foundation/Foundation.h>
#import "MaxMyDelegate.h"
#import "MaxFullBase.h"
#import "MyMaxiOS.h"

@interface MaxNtFull : MaxFullBase

- (id)initAd:(NSString *)placement adParent:(MyMaxiOS*)adParent adDelegate:(id<MaxMyDelegate>)adDelegate;

- (void)load:(NSString *)adId orient:(int)orient;
- (void)reCount;
- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto;

@end

