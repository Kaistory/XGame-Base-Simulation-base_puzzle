#import <Foundation/Foundation.h>
#import "FbMyDelegate.h"

@interface FbFullBase : NSObject

@property(nonatomic, strong) NSString *placement;
@property BOOL isLoading;
@property BOOL isLoaded;
@property(nonatomic, weak, nullable) id<FbMyDelegate> adDelegate;

@end
