#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "MyAdmobiOS.h"

@interface AdmobBnNt : NSObject

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show:(int)pos width:(int)width maxH:(int)maxH orien:(int)orien iPad:(bool)iPad dxCenter:(float)dxCenter dyVertical:(float)dyVertical trefresh:(int)trefresh;
- (void)hide;
- (void)destroy;

@property BOOL isShow;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<AdmobMyDelegate> adDelegate;

@end
