#import <Foundation/Foundation.h>
#import "MaxMyDelegate.h"

@interface MaxFullBase : NSObject

@property(nonatomic, strong) NSString * _Nonnull placement;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<MaxMyDelegate> adDelegate;

@end
