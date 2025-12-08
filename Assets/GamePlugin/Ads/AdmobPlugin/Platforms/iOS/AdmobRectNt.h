#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "MyAdmobiOS.h"

@interface AdmobRectNt : NSObject

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId;
- (bool)show:(int)pos orien:(int)orien width:(float)width height:(float)height dx:(float)dx dy:(float)dy;
- (void)hide;
- (void)destroy;

@property BOOL isShow;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<AdmobMyDelegate> adDelegate;

@end
