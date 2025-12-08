#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"

@interface AdmobBanner : NSObject
- (id)initBanner:(int)type placement:(NSString *)placement iPad:(bool)iPad adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show:(int)pos orient:(int)orient width:(float)width maxH:(int)maxH dx:(float)dx dy:(float)dy;
- (void)hide;
- (void)destroy;

@property BOOL isShow;
@property(nonatomic, weak, nullable) id<AdmobMyDelegate> adDelegate;

@end
