#import <Foundation/Foundation.h>
#import "FbMyDelegate.h"
#import "FbFullBase.h"
#import "FbMyiOS.h"

@interface FbNtFull : FbFullBase

- (id)initAd:(NSString *)placement adParent:(FbMyiOS*)adParent adDelegate:(id<FbMyDelegate>)adDelegate;

- (void)load:(NSString *)adId orient:(int)orient;
- (void)reCount;
- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto;

@end

