#import <Foundation/Foundation.h>
#import "AdmobMyDelegate.h"

@interface AdmobFullBase : NSObject

@property(nonatomic, strong) NSString *placement;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<AdmobMyDelegate> adDelegate;

@end
