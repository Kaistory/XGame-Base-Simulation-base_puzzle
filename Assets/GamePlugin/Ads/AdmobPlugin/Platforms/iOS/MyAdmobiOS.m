#import "MyAdmobiOS.h"

#import <UIKit/UIKit.h>
#import <StoreKit/StoreKit.h>
#import <CoreTelephony/CTCarrier.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <AudioToolbox/AudioServices.h>
#import <AdSupport/ASIdentifierManager.h>
#import <objc/runtime.h>
#import <mach/mach.h>
#import <mach/mach_host.h>

#import <GoogleMobileAds/GoogleMobileAds.h>
#import "MyAdmobUtiliOS.h"
#import "AdmobMyDelegate.h"
#import "AdmobOpenAd.h"
#import "AdmobBanner.h"
#import "AdmobNtFull.h"
#import "AdmobFull.h"
#import "AdmobGift.h"
#import "AdmobBnNt.h"
#import "AdmobRectNt.h"
#import "AdmobBnNtCl.h"
#import "AdmobMNtFull.h"

const char* const MyAdmobMyBridge_NAME = "AdsAdmobMyBridge";

@interface BannerPlacement : NSObject
@property(nonatomic, strong) NSString *pId;
@property(nonatomic, strong) GADBannerView *bannerView;
@property BOOL isLoading;
@property BOOL isLoaded;
@property BOOL isShow;
@end

@interface MyAdmobiOS () <AdmobMyDelegate>

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobOpenAd*> *dicOpenAd;

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobBanner*> *dicBannerNormal;
@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobBanner*> *dicBannerCl;
@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobBanner*> *dicBannerRect;

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobBnNt*> *dicBnNt;
@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobBnNtCl*> *dicBnNtCl;

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobRectNt*> *dicRectNt;

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobNtFull*> *dicNtFull;
@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobMNtFull*> *dicMNtFull;

@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobFull*> *dicFull;
@property(nonatomic, strong) NSMutableDictionary<NSString*, AdmobGift*> *dicGift;

@property(nonatomic, weak) AdmobNtFull* memAdNtFull4Recount;

@end

@implementation MyAdmobiOS

+ (instancetype)sharedInstance
{
    static MyAdmobiOS *sharedInstance = nil;
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[MyAdmobiOS alloc] init];
        // Do any other initialisation stuff here
        [sharedInstance InitData];
    });
    return sharedInstance;
}

- (void)Initialize
{
    [[GADMobileAds sharedInstance] startWithCompletionHandler:nil];
}

- (void)targetingAdContent:(bool)isChild isUnderAgeConsent:(bool)isUnderAgeConsent rating:(int)rating
{
    if (isChild) {
        GADMobileAds.sharedInstance.requestConfiguration.tagForChildDirectedTreatment = @YES;
    }
    else
    {
        GADMobileAds.sharedInstance.requestConfiguration.tagForChildDirectedTreatment = @NO;
    }
    //
    if (isUnderAgeConsent)
    {
        GADMobileAds.sharedInstance.requestConfiguration.tagForUnderAgeOfConsent = @YES;
    }
    else
    {
        GADMobileAds.sharedInstance.requestConfiguration.tagForUnderAgeOfConsent = @NO;
    }
    //
    if (rating <= 0) {
        GADMobileAds.sharedInstance.requestConfiguration.maxAdContentRating = GADMaxAdContentRatingGeneral;
    }
    else if (rating == 1) {
        GADMobileAds.sharedInstance.requestConfiguration.maxAdContentRating = GADMaxAdContentRatingParentalGuidance;
    }
    else if (rating == 2) {
        GADMobileAds.sharedInstance.requestConfiguration.maxAdContentRating = GADMaxAdContentRatingTeen;
    }
    else {
        GADMobileAds.sharedInstance.requestConfiguration.maxAdContentRating = GADMaxAdContentRatingMatureAudience;
    }
}

- (void)InitData
{
    self.dicOpenAd = [[NSMutableDictionary alloc] init];
    
    self.dicBannerNormal = [[NSMutableDictionary alloc] init];
    self.dicBannerCl = [[NSMutableDictionary alloc] init];
    self.dicBannerRect = [[NSMutableDictionary alloc] init];
    
    self.dicBnNt = [[NSMutableDictionary alloc] init];
    self.dicBnNtCl = [[NSMutableDictionary alloc] init];
    
    self.dicNtFull = [[NSMutableDictionary alloc] init];
    self.dicMNtFull = [[NSMutableDictionary alloc] init];
    
    self.dicFull = [[NSMutableDictionary alloc] init];
    self.dicGift = [[NSMutableDictionary alloc] init];
}
#pragma OpenAd
- (void)loadOpenAd:(NSString*)placement adId:(NSString*)adsId
{
    AdmobOpenAd *ad = [self.dicOpenAd objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobOpenAd alloc] initAd:placement adDelegate:self];
        [self.dicOpenAd setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
-(bool)showOpenAd:(NSString*)placement
{
    AdmobOpenAd *ad = [self.dicOpenAd objectForKey:placement];
    if (ad != nil) {
        return [ad show];
    } else {
        return false;
    }
}
#pragma Banner
- (bool)showBanner:(NSString *)placement pos:(int)pos width:(int)width maxH:(int)maxH orient:(int)orient iPad:(bool)iPad dxCenter:(float)dx
{
    AdmobBanner *ad = [self.dicBannerNormal objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:0 placement:placement iPad:iPad adDelegate:self];
        [self.dicBannerNormal setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos orient:orient width:width maxH:maxH dx:dx dy:0];
}

- (void)loadBanner:(NSString *)placement adId:(NSString*)adId iPad:(bool)iPad
{
    AdmobBanner *ad = [self.dicBannerNormal objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:0 placement:placement iPad:iPad adDelegate:self];
        [self.dicBannerNormal setObject:ad forKey:placement];
    }
    [ad load:adId];
}

- (void)hideBanner
{
    for (NSString *key in self.dicBannerNormal) {
        AdmobBanner* ad = [self.dicBannerNormal objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
}

- (void)destroyBanner
{
    for (NSString *key in self.dicBannerNormal) {
        AdmobBanner* ad = [self.dicBannerNormal objectForKey:key];
        ad.isShow = NO;
        [ad destroy];
    }
}

#pragma Collapse
- (bool)showBannerCl:(NSString *)placement pos:(int)pos width:(int)width orient:(int)orient iPad:(bool)iPad dxCenter:(float)dx
{
    AdmobBanner *ad = [self.dicBannerCl objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:1 placement:placement iPad:iPad adDelegate:self];
        [self.dicBannerCl setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos orient:orient width:width maxH:-2 dx:dx dy:0];
}
- (void)loadBannerCl:(NSString *)placement adId:(NSString*)adId iPad:(bool)iPad
{
    AdmobBanner *ad = [self.dicBannerCl objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:1 placement:placement iPad:iPad adDelegate:self];
        [self.dicBannerCl setObject:ad forKey:placement];
    }
    [ad load:adId];
}
- (void)hideBannerCl
{
    for (NSString *key in self.dicBannerCl) {
        AdmobBanner* ad = [self.dicBannerCl objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
    UIViewController* vcon = [MyAdmobUtiliOS unityGLViewController];
    [vcon.view setNeedsDisplay];
}
- (void)destroyBannerCl
{
    for (NSString *key in self.dicBannerCl) {
        AdmobBanner* ad = [self.dicBannerCl objectForKey:key];
        ad.isShow = NO;
        [ad destroy];
    }
}

#pragma Rect
- (bool)showBannerRect:(NSString*)placement pos:(int)pos width:(float)width dxCenter:(float)dxCenter dyVertical:(float)dyVertical
{
    AdmobBanner *ad = [self.dicBannerRect objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:2 placement:placement iPad:NO adDelegate:self];
        [self.dicBannerRect setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos orient:0 width:width maxH:0 dx:dxCenter dy:dyVertical];
}
- (void)loadBannerRect:(NSString *)placement adId:(NSString*)adId
{
    AdmobBanner *ad = [self.dicBannerRect objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBanner alloc] initBanner:1 placement:placement iPad:NO adDelegate:self];
        [self.dicBannerRect setObject:ad forKey:placement];
    }
    [ad load:adId];
}

- (void)hideBannerRect
{
    for (NSString *key in self.dicBannerRect) {
        AdmobBanner* ad = [self.dicBannerRect objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
}

- (void)destroyBannerRect
{
    for (NSString *key in self.dicBannerRect) {
        AdmobBanner* ad = [self.dicBannerRect objectForKey:key];
        ad.isShow = NO;
        [ad destroy];
    }
}

#pragma banner nt
- (bool)showBnNt:(NSString*) placement pos:(int)pos width:(int)width  maxH:(int)maxH orien:(int)orien iPad:(bool)iPad dxCenter:(float)dxCenter dyVertical:(float)dyVertical trefresh:(int)trefresh
{
    AdmobBnNt *ad = [self.dicBnNt objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBnNt alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicBnNt setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos width:width maxH:maxH orien:orien iPad:iPad dxCenter:dxCenter dyVertical:dyVertical trefresh:trefresh];
}
- (void)loadBnNt:(NSString *)placement adId:(NSString*)adId
{
    AdmobBnNt *ad = [self.dicBnNt objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBnNt alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicBnNt setObject:ad forKey:placement];
    }
    [ad load:adId];
}
- (void)hideBnNt
{
    for (NSString *key in self.dicBnNt) {
        AdmobBnNt* ad = [self.dicBnNt objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
}
- (void)destroyBnNt
{
    for (NSString *key in self.dicBnNt) {
        AdmobBnNt* ad = [self.dicBnNt objectForKey:key];
        ad.isShow = NO;
        [ad destroy];
    }
}
#pragma native cl
- (void)loadNativeCl:(NSString*)placement adId:(NSString*)adsId orient:(int)orient
{
    AdmobBnNtCl *ad = [self.dicBnNtCl objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBnNtCl alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicBnNtCl setObject:ad forKey:placement];
    }
    [ad load:adsId orient:orient];
}
- (bool)showNativeCl:(NSString*)placement pos:(int)pos width:(int)width dxCenter:(float)dxCenter isHideBtClose:(bool)isHideBtClose isLouWhenick:(bool)isLouWhenick
{
    AdmobBnNtCl *ad = [self.dicBnNtCl objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobBnNtCl alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicBnNtCl setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos width:width dxCenter:dxCenter isHideBtClose:isHideBtClose isLouWhenick:isLouWhenick];
}
- (void)hideNativeCl
{
    for (NSString *key in self.dicBnNtCl) {
        AdmobBnNtCl* ad = [self.dicBnNtCl objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
}
//-----------------------------------------------
- (bool)showRectNt:(NSString*)placement pos:(int)pos orien:(int)orien width:(float)width height:(float)height dx:(float)dx dy:(float)dy {
    AdmobRectNt *ad = [self.dicRectNt objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobRectNt alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicRectNt setObject:ad forKey:placement];
    }
    ad.isShow = YES;
    return [ad show:pos orien:orien width:width height:height dx:dx dy:dy];
}
- (void)loadRectNt:(NSString*)placement adUnitId:(NSString*)adsId {
    AdmobRectNt *ad = [self.dicRectNt objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobRectNt alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicRectNt setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
- (void)hideRectNt {
    for (NSString *key in self.dicRectNt) {
        AdmobRectNt* ad = [self.dicRectNt objectForKey:key];
        ad.isShow = NO;
        [ad hide];
    }
}

#pragma setcf native
- (void)setCfNtFullNative:(int)per fran:(int)fran oran:(int)oran non:(int)nonAfter increase:(int)increase
{
    self.ntFullPer = per;
    self.ntFullFirstRan = fran;
    self.ntFullOtherRan = oran;
    self.ntFullCountNonAfter = nonAfter;
    self.ntFullIncrease = increase;
}
- (void)setCfNtFullFbExcluse:(int)rows columns:(int)columns areaExcluse:(NSString*)areaExcluse
{
}
- (void)setCfNtClNative:(int)per ran:(int)ran non:(int)non increase:(int)increase
{
    self.ntclPer = per;
    self.ntclRan = ran;
    self.ntclNonAfter = non;
    self.ntclIncrease = increase;
}
- (void)setCfNtClFbExcluse:(int)rows columns:(int)columns areaExcluse:(NSString*)areaExcluse
{
}
#pragma NtFull
- (void)loadNtFullNative:(NSString*)placement adId:(NSString*)adsId orient:(int)orient
{
    AdmobNtFull *ad = [self.dicNtFull objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobNtFull alloc] initAd:placement adParent:self adDelegate:self];
        [self.dicNtFull setObject:ad forKey:placement];
    }
    [ad load:adsId orient:orient];
}
- (void)recountClose
{
    if (self.memAdNtFull4Recount != nil) {
        [self.memAdNtFull4Recount reCount];
        self.memAdNtFull4Recount = nil;
    }
}
- (void) clearRecount
{
    self.memAdNtFull4Recount = nil;
}
- (bool)showNtFullNative:(NSString*)placement timeBtClose:(int)timeShowBtClose timeDelay:(int)timeDelay autoCloseWhenClick:(bool)isAuto
{
    AdmobNtFull *ad = [self.dicNtFull objectForKey:placement];
    if (ad != nil) {
        if (timeDelay > 10) {
            self.memAdNtFull4Recount = ad;
        }
        return [ad show:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAuto];
    } else {
        return false;
    }
}
#pragma multiNTFull
- (void)loadMultiNtFull:(NSString*)placement adsId:(NSString*)adsId orient:(int)orient
{
}
- (bool)showMultiNtFull:(NSString*)placement timeClose:(int)timeClose isAutoCloseWhenClick:(bool)isAutoCloseWhenClick
{
    return false;
}

#pragma Full
- (void)loadFull:(NSString*)placement adId:(NSString*)adsId
{
    AdmobFull *ad = [self.dicFull objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobFull alloc] initAd:placement adDelegate:self];
        [self.dicFull setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
-(bool)showFull:(NSString*)placement
{
    AdmobFull *ad = [self.dicFull objectForKey:placement];
    if (ad != nil) {
        return [ad show:0];
    } else {
        return false;
    }
}
#pragma Gift
- (void)loadGift:(NSString*)placement adId:(NSString*)adsId
{
    AdmobGift *ad = [self.dicGift objectForKey:placement];
    if (ad == nil) {
        ad = [[AdmobGift alloc] initAd:placement adDelegate:self];
        [self.dicGift setObject:ad forKey:placement];
    }
    [ad load:adsId];
}
-(bool)showGift:(NSString*)placement
{
    AdmobGift *ad = [self.dicGift objectForKey:placement];
    if (ad != nil) {
        return [ad show];
    } else {
        return false;
    }
}

#pragma Cb
- (void)didReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerLoaded", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClLoaded", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectLoaded", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullLoaded", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullLoaded", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftLoaded", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClLoaded", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdLoaded", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLLoaded", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTLoaded", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTLoaded", [cbParam UTF8String]);
    }
}
- (void)didFailToReceiveAd:(int)adType placement:(NSString *)placement adId:(NSString *)adId withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, error];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerLoadFail", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClLoadFail", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectLoadFail", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullLoadFail", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBonFullLoadFail", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftLoadFail", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClLoadFail", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBonOpenAdLoadFail", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLLoadFail", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTLoadFail", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTLoadFail", [cbParam UTF8String]);
    }
}
- (void)adFailPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet withError:(NSString *)error
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%@", placement, adId, adNet, error];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullFailedToShow", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullFailedToShow", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftFailedToShow", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdFailedToShow", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTFailedToShow", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
    }
}
- (void)adDidPresent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerOpen", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClOpen", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectOpen", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullShowed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullShowed", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftShowed", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClShowed", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdShowed", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLShowed", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTShowed", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTShowed", [cbParam UTF8String]);
    }
}
- (void)adDidImpression:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerImpression", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClImpression", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectImpression", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullImpresstion", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullImpresstion", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftImpresstion", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClImpresstion", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdImpresstion", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLImpresstion", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTImpresstion", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTImpresstion", [cbParam UTF8String]);
    }
}
- (void)adDidClick:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClick", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClClick", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectClick", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullClick", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullClick", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftClick", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClClick", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdClick", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLClick", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTClick", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTClick", [cbParam UTF8String]);
    }
}
- (void)adDidDismiss:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClose", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClClose", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectClose", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullDismissed", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullDismissed", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftDismissed", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClDismissed", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdDismissed", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLDismissed", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTDismissed", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTDismissed", [cbParam UTF8String]);
    }
}
- (void)adDidReward:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@", placement, adId, adNet];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftReward", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
    } else if (adType == 8) {//rectcl
    } else if (adType == 9) {//multiNt
    } else if (adType == 10) {//bnnt
    }
}
- (void)adWillDestroy:(int)adType placement:(NSString *)placement adId:(NSString *)adId
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@", placement, adId];
    if (adType == 0) {//bn
    } else if (adType == 1) {//cl
    } else if (adType == 2) {//rect
    } else if (adType == 3) {//ntfull
    } else if (adType == 4) {//full
    } else if (adType == 5) {//gift
    } else if (adType == 6) {//ntcl
    } else if (adType == 7) {//openad
    } else if (adType == 8) {//rectcl
    } else if (adType == 9) {//multiNt
    } else if (adType == 10) {//bnnt
    }
}
- (void)adPaidEvent:(int)adType placement:(NSString *)placement adId:(NSString *)adId adNet:(NSString *)adNet precisionType:(int)precisionType currencyCode:(NSString *)currencyCode valueMicros:(long)valueMicros
{
    NSString* cbParam = [NSString stringWithFormat:@"%@;%@;%@;%d;%@;%ld", placement, adId, adNet, precisionType, currencyCode, valueMicros];
    if (adType == 0) {//bn
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerPaid", [cbParam UTF8String]);
    } else if (adType == 1) {//cl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerClPaid", [cbParam UTF8String]);
    } else if (adType == 2) {//rect
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBannerRectPaid", [cbParam UTF8String]);
    } else if (adType == 3) {//ntfull
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtFullPaid", [cbParam UTF8String]);
    } else if (adType == 4) {//full
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBFullPaid", [cbParam UTF8String]);
    } else if (adType == 5) {//gift
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBGiftPaid", [cbParam UTF8String]);
    } else if (adType == 6) {//ntcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBNtClPaid", [cbParam UTF8String]);
    } else if (adType == 7) {//openad
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBOpenAdPaid", [cbParam UTF8String]);
    } else if (adType == 8) {//rectcl
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBRectCLPaid", [cbParam UTF8String]);
    } else if (adType == 9) {//multiNt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBMultiNTPaid", [cbParam UTF8String]);
    } else if (adType == 10) {//bnnt
        UnitySendMessage(MyAdmobMyBridge_NAME, "iOSCBBNNTPaid", [cbParam UTF8String]);
    }
}

#pragma mark - C API
void InitializeNative() {
    [[MyAdmobiOS sharedInstance] Initialize];
}
void targetingAdContentNative(bool isChild, bool isUnderAgeConsent, int rating)
{
    [[MyAdmobiOS sharedInstance] targetingAdContent:isChild isUnderAgeConsent:isUnderAgeConsent rating:rating];
}
//----------------------------------------------
void loadOpenAdNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadOpenAd:nspl adId:nsadsid];
}
bool showOpenAdNative(char* placement) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showOpenAd:nspl];
}
//----------------------------------------------
bool showBannerNative(char* placement, int pos, int width, int maxH, int orien, bool iPad, float dxCenter) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showBanner:nspl pos:pos width:width maxH:maxH orient:orien iPad:iPad dxCenter:dxCenter];
}
void loadBannerNative(char* placement, char* adsId, bool iPad) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadBanner:nspl adId:nsadsid iPad:iPad];
}
void hideBannerNative() {
    [[MyAdmobiOS sharedInstance] hideBanner];
}
void destroyBannerNative() {
    [[MyAdmobiOS sharedInstance] destroyBanner];
}
//----------------------------------------------
bool showBannerClNative(char* placement, int pos, int width, int orien, bool iPad, float dxCenter) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showBannerCl:nspl pos:pos width:width orient:orien iPad:iPad dxCenter:dxCenter];
}
void loadBannerClNative(char* placement, char* adsId, bool iPad) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadBannerCl:nspl adId:nsadsid iPad:iPad];
}
void hideBannerClNative() {
    [[MyAdmobiOS sharedInstance] hideBannerCl];
}
void destroyBannerClNative() {
    [[MyAdmobiOS sharedInstance] destroyBannerCl];
}
//----------------------------------------------
bool showBannerRectNative(char* placement, int pos, float width, float dxCenter, float dyVertical) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showBannerRect:nspl pos:pos width:width dxCenter:dxCenter dyVertical:dyVertical];
}
void loadBannerRectNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadBannerRect:nspl adId:nsadsid];
}
void hideBannerRectNative() {
    [[MyAdmobiOS sharedInstance] hideBannerRect];
}
void destroyBannerRectNative() {
    [[MyAdmobiOS sharedInstance] destroyBannerRect];
}
//----------------------------------------------
bool showBnNtNative(char* placement, int pos, int width, int maxH, int orien, bool iPad, float dxCenter, float dyVertical, int trefresh) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showBnNt:nspl pos:pos width:width maxH:maxH orien:orien iPad:iPad dxCenter:dxCenter dyVertical:dyVertical trefresh:trefresh];
}
void loadBnNtNative(char* placement, char* adsId, bool iPad) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadBnNt:nspl adId:nsadsid];
}
void hideBnNtNative() {
    [[MyAdmobiOS sharedInstance] hideBnNt];
}
void destroyBnNtNative() {
    [[MyAdmobiOS sharedInstance] destroyBnNt];
}
//----------------------------------------------
void loadNativeClNative(char* placement, char* adsId, int orient) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadNativeCl:nspl adId:nsadsid orient:orient];
}
bool showNativeClNative(char* placement, int pos, int width, float dxCenter, bool isHideBtClose, bool isLouWhenick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showNativeCl:nspl pos:pos width:width dxCenter:dxCenter isHideBtClose:isHideBtClose isLouWhenick:isLouWhenick];
}
void hideNativeClNative() {
    [[MyAdmobiOS sharedInstance] hideNativeCl];
}
//----------------------------------------------
bool showRectNtNative(char* placement, int pos, int orien, float width, float height, float dx, float dy) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showRectNt:nspl pos:pos orien:orien width:width height:height dx:dx dy:dy];
}
void loadRectNtNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsAdsId = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadRectNt:nspl adUnitId:nsAdsId];
}
void hideRectNtNative() {
    [[MyAdmobiOS sharedInstance] hideRectNt];
}
//----------------------------------------------
void setCfNtFullNative(int v1, int v2, int v3, int v4, int v5) {
    [[MyAdmobiOS sharedInstance] setCfNtFullNative:v1 fran:v2 oran:v3 non:v4 increase:v5];
}
void setCfNtFullFbExcluseNative(int rows, int columns, char* areaExcluse) {
    NSString* nsarea = [NSString stringWithUTF8String:areaExcluse];
    [[MyAdmobiOS sharedInstance] setCfNtFullFbExcluse:rows columns:columns areaExcluse:nsarea];
}
void setCfNtClNative(int v1, int v2, int v3, int v4) {
    [[MyAdmobiOS sharedInstance] setCfNtClNative:v1 ran:v2 non:v3 increase:v4];
}
void setCfNtClFbExcluseNative(int rows, int columns, char* areaExcluse) {
    NSString* nsarea = [NSString stringWithUTF8String:areaExcluse];
    [[MyAdmobiOS sharedInstance] setCfNtClFbExcluse:rows columns:columns areaExcluse:nsarea];
}
void setTypeBnntNative(bool isShowMedia) {
    
}
//----------------------------------------------
void loadNtFullNative(char* placement, char* adsId, int orien) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadNtFullNative:nspl adId:nsadsid orient:orien];
}
void reCountCurrShowNative() {
    [[MyAdmobiOS sharedInstance] recountClose];
}
bool showNtFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showNtFullNative:nspl timeBtClose:timeShowBtClose timeDelay:timeDelay autoCloseWhenClick:isAutoCLoseWhenClick];
}
//----------------------------------------------
void loadNtIcFullNative(char* placement, char* adsId, int orien){}
bool showNtIcFullNative(char* placement, int timeShowBtClose, int timeDelay, bool isAutoCLoseWhenClick) {return false;}
//----------------------------------------------
void loadMultiNtFullNative(char* placement, char* adsId, int orient) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadMultiNtFull:nspl adsId:nsadsid orient:orient];
}
bool showMultiNtFullNative(char* placement, int timeClose, bool isAutoCloseWhenClick) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showMultiNtFull:nspl timeClose:timeClose isAutoCloseWhenClick:isAutoCloseWhenClick];
}
//----------------------------------------------
void clearCurrFullNative(char* placement) {}
void loadFullNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadFull:nspl adId:nsadsid];
}
bool showFullNative(char* placement) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showFull:nspl];
}
//----------------------------------------------
void clearCurrGiftNative(char* placement) {}
void loadGiftNative(char* placement, char* adsId) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    NSString* nsadsid = [NSString stringWithUTF8String:adsId];
    [[MyAdmobiOS sharedInstance] loadGift:nspl adId:nsadsid];
}
bool showGiftNative(char* placement) {
    NSString* nspl = [NSString stringWithUTF8String:placement];
    return [[MyAdmobiOS sharedInstance] showGift:nspl];
}

@end

