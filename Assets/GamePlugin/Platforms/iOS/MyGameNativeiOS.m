//#define Test_Show_CMP

#import "MyGameNativeiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <AVFoundation/AVFoundation.h>
#import <UnityFramework/UnityFramework-Swift.h>
#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#include <sys/types.h>
#include <sys/sysctl.h>

#include <UserMessagingPlatform/UserMessagingPlatform.h>

#import "MyGameAppController.h"
#import "MyAdmobUtiliOS.h"

long memTcurr = 0;
long memTel = 0;

@implementation MyGameNativeiOS

+ (instancetype)sharedInstance
{
    static MyGameNativeiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyGameNativeiOS alloc] init];
        // Do any other initialisation stuff here
    });
    return sharedInstance;
}

- (NSTimeInterval) CurrentTimeMilis
{
    NSTimeInterval timeStamp = [[NSDate date] timeIntervalSince1970];
    return timeStamp;
}

-(NSTimeInterval) ElapedTimeBoot
{
    int mib[2];
    size_t size;
    struct timeval  boottime;
    
    mib[0] = CTL_KERN;
    mib[1] = KERN_BOOTTIME;
    size = sizeof(boottime);
    if (sysctl(mib, 2, &boottime, &size, NULL, 0) != -1)
    {
        NSTimeInterval tcuu = [self CurrentTimeMilis];
        return (tcuu - boottime.tv_sec);
    }
    else
    {
        return 0;
    }
}

-(BOOL) isStringValideBase64:(NSString*)string
{
    NSString *regExPattern = @"^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";
    
    NSRegularExpression *regEx = [[NSRegularExpression alloc] initWithPattern:regExPattern options:NSRegularExpressionCaseInsensitive error:nil];
    NSUInteger regExMatches = [regEx numberOfMatchesInString:string options:0 range:NSMakeRange(0, [string length])];
    return regExMatches != 0;
}

-(int)isVn {
    NSString *countryCode = @"";
    NSDictionary<NSString *, CTCarrier *> *providers = [[CTTelephonyNetworkInfo new] serviceSubscriberCellularProviders];
    if (providers != nil && providers.allValues != nil && [providers count] > 0) {
        CTCarrier *carrier = providers.allValues.firstObject;
        if (carrier != nil) {
            countryCode = carrier.isoCountryCode;
            if (countryCode != nil) {
                if ([countryCode  isEqual: @"VN"] || [countryCode  isEqual: @"vn"]) {
                    return 1;
                }
            }
        }
    }
    
    NSLocale *local = [NSLocale currentLocale];
    if (local != NULL) {
        countryCode = [local objectForKey:NSLocaleCountryCode];
        if(countryCode != nil) {
            if ([countryCode  isEqual: @"VN"] || [countryCode  isEqual: @"vn"]) {
                return 1;
            }
        }
    }
    NSString * language = [[NSLocale preferredLanguages] firstObject];
    if (language != NULL) {
        if ([language  isEqual: @"VI"] || [language  isEqual: @"vi"]) {
            return 2;
        }
    }
    
    if (countryCode == nil || [countryCode length] == 0) {
        return 4;
    }
    return 0;
}

-(char*) getLanguageCode {
    NSString * language = [[NSLocale preferredLanguages] firstObject];
    if (language != NULL) {
        if ([language containsString:@"-"]) {
            NSUInteger idx = [language rangeOfString:@"-"].location;
            language = [language substringToIndex:idx];
        }
        const char* code = [language UTF8String];
        char* re = malloc([language length]);
        strcpy(re, code);
        return re;
    }
    NSLocale *local = [NSLocale currentLocale];
    if (local != NULL) {
        NSString *countryCode = [local objectForKey:NSLocaleLanguageCode];
        if(countryCode == NULL) {
            countryCode = @"";
        }
        const char* code = [countryCode UTF8String];
        char* re = malloc([countryCode length]);
        strcpy(re, code);
        return re;
    } else {
        NSString *nsre = @"";
        const char* cre = [nsre UTF8String];
        char* re = malloc([nsre length]);
        strcpy(re, cre);
        return re;
    }
}

-(char*) getCountryCode {
    NSLocale *local = [NSLocale currentLocale];  // get the current locale.
    if (local != NULL) {
        NSString *countryCode = [local objectForKey:NSLocaleCountryCode];
        if(countryCode == NULL) {
            countryCode = @"";
        }
        const char* code = [countryCode UTF8String];
        char* re = malloc([countryCode length]);
        strcpy(re, code);
        return re;
    } else {
        NSString *nsre = @"";
        const char* cre = [nsre UTF8String];
        char* re = malloc([nsre length]);
        strcpy(re, cre);
        return re;
    }
}

-(char*) getAdsIdentify {
    //@try
    //{
    NSUUID *identifier = [[ASIdentifierManager sharedManager] advertisingIdentifier];
    NSString *str = [identifier UUIDString];
    const char* cadid = [str UTF8String];
    char* re = malloc([str length]);
    strcpy(re, cadid);
    return re;
    //}
    //@catch(NSException *exception)
    //{
    
    //}
}
-(void) synchronizeTime:(long) timestampSecond
{
    NSLog(@"mysdk: synchronizeTime=%ld", timestampSecond);
    memTcurr = timestampSecond;
    memTel = [self ElapedTimeBoot];
    [[NSUserDefaults standardUserDefaults] setObject:[NSNumber numberWithLongLong:memTcurr] forKey:@"meem_local_time"];
    [[NSUserDefaults standardUserDefaults] setObject:[NSNumber numberWithLongLong:memTel] forKey:@"meem_local_elaptime"];
}

-(long) CurrentTimeMilisReal
{
    long currTi = [self CurrentTimeMilis];
    long currElap = [self ElapedTimeBoot];
    if (memTcurr < 10)
    {
        memTcurr = [[[NSUserDefaults standardUserDefaults] objectForKey:@"meem_local_time"] longLongValue];
        memTel = [[[NSUserDefaults standardUserDefaults] objectForKey:@"meem_local_elaptime"] longLongValue];
        if (memTcurr < 10)
        {
            memTcurr = currTi;
            memTel = currElap;
        }
    }
    long dti = currTi - memTcurr;
    long delap = currElap - memTel;
    if (dti <= 0)
    {
        if (dti == 0)
        {
            [self synchronizeTime:currTi];
        }
        if (delap < 0)
        {
            long re = memTcurr + currElap;
            [self synchronizeTime:re];
            return re;
        }
        else
        {
            return (memTcurr + delap);
        }
    }
    else
    {
        if (delap < 0)
        {
            if (currTi < (memTcurr + currElap))
            {
                [self synchronizeTime:(memTcurr + currElap)];
                return (memTcurr + currElap);
            }
            else
            {
                [self synchronizeTime:currTi];
                return currTi;
            }
        }
        else
        {
            return (memTcurr + delap);
        }
    }
}

-(char*) getGiftBox {
    NSString* sub = [[NSUserDefaults standardUserDefaults] objectForKey:@"mem_gift_box"];
    if(sub == NULL) {
        sub = @"";
    }
    const char* cub = [sub UTF8String];
    char* re = malloc([sub length]);
    strcpy(re, cub);
    return re;
}

-(void) vibrate:(int)type {
    if (type == 0) {
        AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
    } else {
        [[MyGameiOSSwift shared] VibrateWithType:type];
    }
}

-(void) switchFlash:(Boolean) isOn {
    AVCaptureDevice *flashLight = [AVCaptureDevice defaultDeviceWithMediaType:AVMediaTypeVideo];
    if ([flashLight isTorchAvailable] && [flashLight isTorchModeSupported:AVCaptureTorchModeOn])
    {
        BOOL success = [flashLight lockForConfiguration:nil];
        if (success)
        {
            if (isOn) {
                [flashLight setTorchMode:AVCaptureTorchModeOn];
            } else {
                [flashLight setTorchMode:AVCaptureTorchModeOff];
            }
            [flashLight unlockForConfiguration];
        }
    }
}

-(long)getMemoryLimit {
    mach_port_t host_port;
    mach_msg_type_number_t host_size;
    vm_size_t pagesize;
    
    host_port = mach_host_self();
    host_size = sizeof(vm_statistics_data_t) / sizeof(integer_t);
    host_page_size(host_port, &pagesize);
    
    vm_statistics_data_t vm_stat;
    
    if (host_statistics(host_port, HOST_VM_INFO, (host_info_t)&vm_stat, &host_size) != KERN_SUCCESS) {
        NSLog(@"mysdk: Failed to fetch vm statistics");
        return 0;
    } else {
        /* Stats in bytes */
        natural_t mem_used = (vm_stat.active_count +
                              vm_stat.inactive_count +
                              vm_stat.wire_count) * pagesize;
        natural_t mem_free = vm_stat.free_count * pagesize;
        natural_t mem_total = mem_used + mem_free;
        NSLog(@"mysdk: used: %u free: %u total: %u", mem_used / 1024 / 1024, mem_free / 1024 / 1024, mem_total / 1024 / 1024);
        return mem_total;
    }
}

-(long)getPhysicMemoryInfo {
    struct mach_task_basic_info info;
    mach_msg_type_number_t size = sizeof(info);
    kern_return_t kerr = task_info(mach_task_self(), MACH_TASK_BASIC_INFO, (task_info_t)&info, &size);
    if (kerr == KERN_SUCCESS)
    {
        float used_bytes = info.resident_size;
        float total_bytes = [NSProcessInfo processInfo].physicalMemory;
        NSLog(@"mysdk: Used: %f MB out of %f MB (%f%%)", used_bytes / 1024.0f / 1024.0f, total_bytes / 1024.0f / 1024.0f, used_bytes * 100.0f / total_bytes);
        return (long)total_bytes;
    } else {
        NSLog(@"mysdk: Failed to fetch vm statistics");
        return 0;
    }
}

-(float)getScreenWidth{
    UIView *unityView = [MyAdmobUtiliOS unityGLViewController].view;
    float w = unityView.bounds.size.width;
    return w;
}

-(bool)appReview {
    return [[MyGameiOSSwift shared] MyAppReview];
}

-(void)showCMP
{
    // Create a UMPRequestParameters object.
    UMPRequestParameters *parameters = [[UMPRequestParameters alloc] init];
#ifdef Test_Show_CMP
    UMPDebugSettings *debugSettings = [[UMPDebugSettings alloc] init];
    UIDevice *device = [UIDevice currentDevice];
    debugSettings.testDeviceIdentifiers = @[ [[device identifierForVendor] UUIDString] ];
    debugSettings.geography = UMPDebugGeographyEEA;
    parameters.debugSettings = debugSettings;
#endif
    // Set tag for under age of consent. Here NO means users are not under age.
    parameters.tagForUnderAgeOfConsent = NO;
    
    // Request an update to the consent information.
    [UMPConsentInformation.sharedInstance
     requestConsentInfoUpdateWithParameters:parameters
     completionHandler:^(NSError *_Nullable error) {
        if (error) {
            // Handle the error.
            NSLog(@"mysdk: showCMP err=%@", error);
            UnitySendMessage(GameAdsHelper_NAME, "iOSCBOnShowCMP", "0");
        } else {
            // The consent information state was updated.
            // You are now ready to check if a form is
            // available.
            UMPFormStatus formStatus = UMPConsentInformation.sharedInstance.formStatus;
            NSLog(@"mysdk: showCMP formStatus=%ld", formStatus);
            if (formStatus == UMPFormStatusAvailable) {
                [self loadForm];
            } else {
                UnitySendMessage(GameAdsHelper_NAME, "iOSCBOnShowCMP", "0");
            }
        }
    }];
}

- (void)loadForm {
    NSLog(@"mysdk: showCMP loadForm");
    [UMPConsentForm loadWithCompletionHandler:^(UMPConsentForm *form, NSError *loadError) {
        if (loadError) {
            // Handle the error
            NSLog(@"mysdk: showCMP loadForm err=%@", loadError);
            UnitySendMessage(GameAdsHelper_NAME, "iOSCBOnShowCMP", "0");
        } else {
            // Present the form
            NSLog(@"mysdk: showCMP loadForm show consentStatus = %ld", UMPConsentInformation.sharedInstance.consentStatus);
            if (UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusUnknown
                || UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusRequired)
            {
                UnitySendMessage(GameAdsHelper_NAME, "iOSCBOnShowCMP", "1");
                [form presentFromViewController:[MyAdmobUtiliOS unityGLViewController]
                              completionHandler:^(NSError *_Nullable dismissError) {
                    NSLog(@"mysdk: showCMP loadForm show com consentStatus=%ld", UMPConsentInformation.sharedInstance.consentStatus);
                    if (UMPConsentInformation.sharedInstance.consentStatus == UMPConsentStatusObtained) {
                        // App can start requesting ads.
                        NSString* tcv2 = [[NSUserDefaults standardUserDefaults] stringForKey:@"IABTCF_TCString"];
                        NSLog(@"mysdk: showCMP loadForm show tcv2=%@", tcv2);
                        if (tcv2 != nil) {
                            //NSLog(@"mysdk: showCMP loadForm show tcv2=%@", tcv2);
                            UnitySendMessage(GameAdsHelper_NAME, "iOSCBCMP", [tcv2 UTF8String]);
                        } else {
                            UnitySendMessage(GameAdsHelper_NAME, "iOSCBCMP", "");
                        }
                    } else {
                        UnitySendMessage(GameAdsHelper_NAME, "iOSCBCMP", "");
                    }
                }];
            } else {
                UnitySendMessage(GameAdsHelper_NAME, "iOSCBOnShowCMP", "0");
            }
        }
    }];
}

# pragma mark - C API
int isVnNative() {
    return [[MyGameNativeiOS sharedInstance] isVn];
}
char* getLanguageCodeNative() {
    return [[MyGameNativeiOS sharedInstance] getLanguageCode];
}

char* getCountryCodeNative() {
    return [[MyGameNativeiOS sharedInstance] getCountryCode];
}

char* getAdsIdentifyNative() {
    return [[MyGameNativeiOS sharedInstance] getAdsIdentify];
}

void synchronizeTimeNative(long timestampMilisecond) {
    [[MyGameNativeiOS sharedInstance] synchronizeTime:timestampMilisecond];
}

long CurrentTimeMilisRealNative() {
    return [[MyGameNativeiOS sharedInstance] CurrentTimeMilisReal];
}

char* getGiftBoxNative() {
    return [[MyGameNativeiOS sharedInstance] getGiftBox];
}

void vibrateNative(int type) {
    [[MyGameNativeiOS sharedInstance] vibrate:type];
}

void switchFlashNative(bool isOn) {
    [[MyGameNativeiOS sharedInstance] switchFlash:isOn];
}

long getMemoryLimit() {
    return [[MyGameNativeiOS sharedInstance] getMemoryLimit];
}

long getPhysicMemoryInfo() {
    return [[MyGameNativeiOS sharedInstance] getPhysicMemoryInfo];
}

float getScreenWidthNative() {
    return [[MyGameNativeiOS sharedInstance] getScreenWidth];
}

//------------------------------------------
void configAppOpenAdNative(int timeBg, int orien) {
    [MyGameAppController configAppOpenAd:timeBg orien:orien];
}
void loadAppOpenAdNative(char* iappOpenAdId) {
    NSString* nsAdUnitId = [NSString stringWithUTF8String:iappOpenAdId];
    [MyGameAppController loadAppOpenAd: nsAdUnitId];
}
bool isAppOpenAdLoadedNative() {
    return [MyGameAppController isAppOpenAdLoaded];
}
bool showAppOpenAdNative() {
    return [MyGameAppController showAppOpenAd];
}
//------------------------------------------

bool appReviewNative() {
    return[[MyGameNativeiOS sharedInstance] appReview];
}

void requestIDFANative(int isallversion) {
    [MyGameAppController requestIDFA:isallversion];
}

void showCMPNative() {
    [[MyGameNativeiOS sharedInstance] showCMP];
}

void localNotifyNative(char* title, char* msg, int hour, int minus, int dayrepeat)
{
    NSString* nstitle = [NSString stringWithUTF8String:title];
    NSString* nsmsg = [NSString stringWithUTF8String:msg];
    [[MyGameiOSSwift shared] LocalNotifyWithTitle:nstitle msg:nsmsg hour:hour minute:minus repeatday:dayrepeat];
}

void clearAllNotiNative()
{
    [[MyGameiOSSwift shared] clearAllNoti];
}

bool deleteImagesFromImessageNative(int countItem, char** listNames, char* groupName)
{
    NSString* nsGroup = [NSString stringWithUTF8String:groupName];
    NSMutableArray* nsList = [[NSMutableArray alloc] init];
    for (int i = 0; i < countItem; i++) {
        NSString* nsName = [NSString stringWithUTF8String:listNames[i]];
        [nsList addObject:nsName];
    }
    return [[MyGameiOSSwift shared] deleteStickersTo:nsGroup stickers:nsList];
}

bool deleteImageFromImessageNative(char* listName, char*groupName)
{
    NSString* nsName = [NSString stringWithUTF8String:listName];
    NSString* nsGroup = [NSString stringWithUTF8String:groupName];
    return [[MyGameiOSSwift shared] deleteStickerFrom:nsGroup name:nsName];
}

bool shareImages2ImessageNative(int countItem, char** listNames, int* lenDatas, Byte* data, int lenData, char* nameGroup)
{
    NSData* nsData = [NSData dataWithBytes:data length:lenData];
    NSString* nsGroup = [NSString stringWithUTF8String:nameGroup];
    NSMutableDictionary* nsDic = [[NSMutableDictionary alloc] init];
    int ncurr = 0;
    for (int i = 0; i < countItem; i++) {
        NSString* nsName = [NSString stringWithUTF8String:listNames[i]];
        NSData* img = [nsData subdataWithRange:NSMakeRange(ncurr, lenDatas[i])];
        ncurr += lenDatas[i];
        [nsDic setValue:img forKey:nsName];
    }
    return [[MyGameiOSSwift shared] saveStickersTo:nsGroup datas:nsDic];
}

bool shareImage2ImessageNative(Byte* data, int lenData, char* nameImg, char* nameGroup)
{
    NSData* nsData = [NSData dataWithBytes:data length:lenData];
    NSString* nsName = [NSString stringWithUTF8String:nameImg];
    NSString* nsGroup = [NSString stringWithUTF8String:nameGroup];
    return [[MyGameiOSSwift shared] saveStickerTo:nsGroup name:nsName data:nsData];
}

int pushNotifyNative(int timeFireInseconds, char* title, char* msg)
{
    NSString* nsTitle = [NSString stringWithUTF8String:title];
    NSString* nsMsg = [NSString stringWithUTF8String:msg];
}
void cancelNotiNative(char* ids)
{
    NSString* nsids = [NSString stringWithUTF8String:ids];
}

void _SetKeychainValue(char* key, char* va)
{
    NSString *nsKey = [NSString stringWithUTF8String:key];
    NSString *nsValue = [NSString stringWithUTF8String:va];
    NSData *valueData = [nsValue dataUsingEncoding:NSUTF8StringEncoding];

    NSDictionary *query = @{
      (__bridge id)kSecClass : (__bridge id)kSecClassGenericPassword,
      (__bridge id)kSecAttrAccount : nsKey
    };

    SecItemDelete((__bridge CFDictionaryRef)query);

    NSDictionary *attributes = @{
      (__bridge id)kSecClass : (__bridge id)kSecClassGenericPassword,
      (__bridge id)kSecAttrAccount : nsKey,
      (__bridge id)kSecValueData : valueData
    };

    SecItemAdd((__bridge CFDictionaryRef)attributes, NULL);
}
char *_GetKeychainValue(char* key)
{
    NSString *nsKey = [NSString stringWithUTF8String:key];

    NSDictionary *query = @{
      (__bridge id)kSecClass : (__bridge id)kSecClassGenericPassword,
      (__bridge id)kSecAttrAccount : nsKey,
      (__bridge id)kSecReturnData : @YES
    };

    CFDataRef result = NULL;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query,
                                          (CFTypeRef *)&result);

    if (status == errSecSuccess) {
      NSData *data = (__bridge_transfer NSData *)result;
      NSString *value = [[NSString alloc] initWithData:data
                                              encoding:NSUTF8StringEncoding];
      return strdup([value UTF8String]);
    }
    return NULL;
}

@end

