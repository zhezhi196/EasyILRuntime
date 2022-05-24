//
//  AdController.m
//  Unity-iPhone
//
//  Created by M.z.H on 2021/4/23.
//

#import <Foundation/Foundation.h>
#import "AdController.h"
#import <TradPlusAds/TradPlusAd.h>
#import <TradPlusAds/TradPlus.h>

NSString* AppID = @"BBBD5987CEEEC6486F905C68E03BE254";
NSString* RewardedVideo_UnitId = @"80CAD98D9417A389353CBB3CF8C4420F";
NSString* Interstitial_UnitId = @"";

bool isRewardVideoAdReady = false;

@implementation AdController

- (void)InitAd{
    [super InitAd];
    NSLog(@"AdController --- InitAd 初始化");
    [TradPlus initSDK:AppID completionBlock:^(NSError * _Nonnull error) {
        if (!error) {
            NSLog(@"AdController --- InitAd 初始化成功");
            [self LoadInterstitialAd];
            [self LoadRewardVideoAd];
           // [super StartTimer];
        }else{
            NSLog(@"AdController --- InitAd 初始化失败 error = %@",error);
        }
    }];
  
   
}

- (void)LoadInterstitialAd{
    [super LoadInterstitialAd];
   
}

- (void)ShowInterstitialAd:(NSString *)adScene{
    [super ShowInterstitialAd:adScene];
    UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    
}

- (bool)InterstitialAdReady{
  
        return false;
    
}

- (void)LoadRewardVideoAd{
    [super LoadRewardVideoAd];
    NSLog(@"AdController --- LoadRewardVideoAd加载激励视频广告");
    if (self.rewardedVideoAd == nil) {
        self.rewardedVideoAd = [[MsRewardedVideoAd alloc]init];
    }
    self.rewardedVideoAd.delegate = self;
    // 自动加载
    [self.rewardedVideoAd setAdUnitID:RewardedVideo_UnitId isAutoLoad:YES];
   
}

- (void)ShowRewardVideoAd:(NSString *)adScene{
    [super ShowRewardVideoAd:adScene];
    NSLog(@"AdController --- ShowRewardVideoAd显示激励视频广告");
    // 进入广告场景，用于Tradplus后台统计
   // [self.rewardedVideoAd entryAdScenario];
    UIViewController *rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
    if (self.rewardedVideoAd != nil && self.rewardedVideoAd.isAdReady) {
        [self.rewardedVideoAd showAdFromRootViewController:rootViewController];
    }

}
- (bool)RewardVideoAdReady{
    bool isReady = self.rewardedVideoAd.isAdReady;
//    bool isReady = isRewardVideoAdReady;
    NSLog(@"AdController --- RewardVideoAdReady  isReady = %d",isReady);
    return isReady;
    
}

- (void)RewardVideoAdEntryScenario{
    [self.rewardedVideoAd entryAdScenario];
}
// --------------------激励视频广告回调-----------------
- (void)rewardedVideoAdAllLoaded:(MsRewardedVideoAd *)rewardedVideoAd readyCount:(int)readyCount{
    NSLog(@"AdController --- rewardedVideoAdAllLoaded 激励视频广告加载");
    // 激励视频广告加载
    if (readyCount > 0) {
        // 加载成功
        NSLog(@"AdController --- rewardedVideoAdAllLoaded 激励视频广告加载成功");
        isRewardVideoAdReady = true;
        [super OnRewardVideoAdLoadSuccess];
    }else{
        // 加载失败，如果isAutoLoad没有设为YES，需要在30秒后重新load一次
        NSLog(@"AdController --- rewardedVideoAdAllLoaded 激励视频广告加载失败");
        isRewardVideoAdReady = false;
        [super OnRewardVideoAdLoadFail];
    }
}

- (void)rewardedVideoAdLoaded:(MsRewardedVideoAd *)rewardedVideoAd{
    // 激励广告单个广告源加载成功
   // NSLog(@"AdController --- rewardedVideoAdLoaded 激励广告单个广告源加载成功");
    // isRewardVideoAdReady = true;
}

- (void)rewardedVideoAd:(MsRewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error{
    // 激励视频广告单个广告源加载失败
   // NSLog(@"AdController --- rewardedVideoAd 激励视频广告单个广告源加载失败");
   // isRewardVideoAdReady = false;
}
    
- (void)rewardedVideoAdShown:(MsRewardedVideoAd *)rewardedVideoAd{
    // 激励视频广告开始播放
    [super OnRewardVideoAdStartPlay];
    NSLog(@"AdController --- rewardedVideoAdShown 激励视频广告开始播放");
}

- (void)rewardedVideoAdDismissed:(MsRewardedVideoAd *)rewardedVideoAd{
    // 激励视频广告关闭
    [super OnRewardVideoAdClosed];
    isRewardVideoAdReady = false;
    NSLog(@"AdController --- rewardedVideoAdDismissed 激励视频广告关闭");
}

- (void)rewardedVideoAdClicked:(MsRewardedVideoAd *)rewardedVideoAd{
    // 激励视频广告点击
    [super OnRewardVideoAdClick];
    NSLog(@"AdController --- rewardedVideoAdClicked 激励视频广告点击");
}

- (void)rewardedVideoAdShouldReward:(MsRewardedVideoAd *)rewardedVideoAd reward:(MSRewardedVideoReward *)reward{
    // 激励视频广告发放奖励
    [super OnRewardVideoAdReward];
    isRewardVideoAdReady = false;
    NSLog(@"AdController --- rewardedVideoAdShouldReward 激励视频广告发放奖励");
}


// --------------------插屏广告回调--------------------




@end
