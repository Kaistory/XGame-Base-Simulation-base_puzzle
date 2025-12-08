#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"
#import "AdmobFullBase.h"
#import "MyAdmobiOS.h"

@interface AdmobBnNtCl : NSObject

- (id)initAd:(NSString *)placement adParent:(MyAdmobiOS*)adParent adDelegate:(id<AdmobMyDelegate>)adDelegate;

- (void)load:(NSString *)adId orient:(int)orient;
- (bool)show:(int)pos width:(int)width dxCenter:(float)dxCenter isHideBtClose:(bool)isHideBtClose isLouWhenick:(bool)isLouWhenick;
- (void)hide;

@property BOOL isShow;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<AdmobMyDelegate> adDelegate;

@end

