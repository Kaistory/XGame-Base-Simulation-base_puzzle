#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "AdmobFullBase.h"
#import "MyAdmobiOS.h"

@interface AdmobNtFull : AdmobFullBase

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId orient:(int)orient;
- (void)reCount;
- (bool)show:(int)timeBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAudto;

@end

