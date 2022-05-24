//
//  AdControllerBase.mm
//  SDKFrame
//
//  Created by M.z.H on 2020/5/12.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AdControllerBase.h"
#import "AdUICommunication.h"
#import "CommonUtil.h"

@implementation AdControllerBase

// 初始化广告
- (void)InitAd{
    State_LoadInterstitial = State_Load_None;
    State_LoadRewardVideo = State_Load_None;
  
}

// 开启定时器
- (void)StartTimer{
    
    if (self.timer == nil) {
        NSLog(@"AdControllerBase --- StartTimer: %i",State_Load_None);
        self.timer = [NSTimer scheduledTimerWithTimeInterval:Schedule_Timer target:self selector:@selector(TimerUpdate) userInfo:nil repeats:YES];
    }
}

// 定时器的执行
- (void)TimerUpdate{
    NSLog(@"AdControllerBase --- TimerUpdate");
    if (State_LoadInterstitial == State_Load_Failed) {
        NSLog(@"AdControllerBase --- TimerUpdate  State_LoadInterstitial=%i",State_LoadInterstitial);
        [self LoadInterstitialAd];
    }
    
    if (State_LoadRewardVideo == State_Load_Failed) {
        NSLog(@"AdControllerBase --- TimerUpdate State_LoadRewardVideo=%i",State_LoadRewardVideo);
       // [self LoadRewardVideoAd];
    }
 
}

// 加载插屏广告
- (void)LoadInterstitialAd{
    State_LoadInterstitial = State_Load_Ing;
}

// 播放插屏广告
- (void)ShowInterstitialAd:(NSString *)adScene{
    InterstitialAdChange([NSString stringWithFormat:@"%d",CallShow]);
}

// 插屏广告是否准备好
- (bool)InterstitialAdReady{
    return false;
}

// 加载激励视频广告
- (void)LoadRewardVideoAd{
    State_LoadRewardVideo = State_Load_Ing;
}

// 播放激励视频广告
- (void)ShowRewardVideoAd:(NSString *)adScene{
    RewardVideoAdChange([NSString stringWithFormat:@"%d",CallShow]);
}

// 激励视频广告是否准备好
- (bool)RewardVideoAdReady{
    return false;
}

//---------------------------激励视频广告回调----------------------------
- (void)OnRewardVideoAdLoadFail{
    // 激励视频广告加载失败
    NSLog(@"AdControllerBase --- OnRewardVideoAdLoadFail 激励视频广告加载失败");
    if (State_LoadRewardVideo == State_Load_Ing) {
        State_LoadRewardVideo = State_Load_Failed;
    }
    RewardVideoAdChange([NSString stringWithFormat:@"%d",LoadFailed]);
}

- (void)OnRewardVideoAdLoadSuccess{
    // 激励视频广告加载成功
    NSLog(@"AdControllerBase --- OnRewardVideoAdLoadSuccess 激励视频广告加载成功");
    if (State_LoadRewardVideo == State_Load_Ing) {
        State_LoadRewardVideo = State_Load_Success;
    }
    RewardVideoAdChange([NSString stringWithFormat:@"%d",LoadSuccess]);
}

- (void)OnRewardVideoAdShow{
    // 激励视频广告显示
    NSLog(@"AdControllerBase --- OnRewardVideoAdShow 激励视频广告显示");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",Start]);
}

- (void)OnRewardVideoAdShowFail{
    // 激励视频广告显示失败
    NSLog(@"AdControllerBase --- OnRewardVideoAdShowFail 激励视频广告显示失败");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",DisplayFailed]);
   // [self LoadRewardVideoAd];
}

- (void)OnRewardVideoAdStartPlay{
    // 激励视频广告开始播放
    NSLog(@"AdControllerBase --- OnRewardVideoAdStartPlay 激励视频广告开始播放");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",StartPlay]);
}

- (void)OnRewardVideoAdEndPlay{
    // 激励视频广告播放结束(完成)
    NSLog(@"AdControllerBase --- OnRewardVideoAdEndPlay 激励视频广告播放结束(完成)");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",Completed]);
   
}

- (void)OnRewardVideoAdFailPlay{
    // 激励视频广告播放失败
    NSLog(@"AdControllerBase --- OnRewardVideoAdFailPlay 激励视频广告播放失败");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",DisplayFailed]);
   // [self LoadRewardVideoAd];
}

- (void)OnRewardVideoAdClick{
    // 激励视频广告点击
    NSLog(@"AdControllerBase --- OnRewardVideoAdClick 激励视频广告点击");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",Click]);
}

- (void)OnRewardVideoAdReward{
    // 激励视频广告发放奖励
    NSLog(@"AdControllerBase --- OnReward 激励视频广告发放奖励");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",Rewarded]);
}

- (void)OnRewardVideoAdClosed{
    // 激励视频广告关闭
    NSLog(@"AdControllerBase --- OnRewardVideoAdClosed 激励视频广告关闭");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",Close]);
   // [self LoadRewardVideoAd];
}

- (void)OnRewardVideoAdLeftApplication{
    // 激励视频广告离开应用
    NSLog(@"AdControllerBase --- OnRewardVideoAdLeftApplication 激励视频广告离开应用");
    RewardVideoAdChange([NSString stringWithFormat:@"%d",LeftApplication]);
}
//---------------------------插屏广告回调----------------------------
- (void)OnInterstitialAdLoadSuccess{
    // 插屏广告加载成功
    NSLog(@"AdControllerBase --- OnInterstitialAdLoadSuccess 插屏广告加载成功");
    if (State_LoadInterstitial == State_Load_Ing) {
        State_LoadInterstitial = State_Load_Success;
    }
    InterstitialAdChange([NSString stringWithFormat:@"%d",LoadSuccess]);
}

- (void)OnInterstitialAdLoadFail{
    // 插屏广告加载失败
    NSLog(@"AdControllerBase --- OnInterstitialAdLoadFail 插屏广告加载失败");
    if (State_LoadInterstitial == State_Load_Ing) {
        State_LoadInterstitial = State_Load_Failed;
    }
    InterstitialAdChange([NSString stringWithFormat:@"%d",LoadFailed]);
}

- (void)OnInterstitialAdShow{
    // 插屏广告显示
    NSLog(@"AdControllerBase --- OnInterstitialAdShow 插屏广告显示");
    InterstitialAdChange([NSString stringWithFormat:@"%d",Start]);
}

- (void)OnInterstitialADShowFail{
    // 插屏广告显示失败
    NSLog(@"AdControllerBase --- OnInterstitialADShowFail 插屏广告显示失败");
    [self LoadInterstitialAd];
    InterstitialAdChange([NSString stringWithFormat:@"%d",DisplayFailed]);
}

- (void)OnInterstitialAdStartPlay{
    // 插屏广告开始播放
    NSLog(@"AdControllerBase --- OnInterstitialAdStartPlay 插屏广告开始播放");
    InterstitialAdChange([NSString stringWithFormat:@"%d",StartPlay]);
}

- (void)OnInterstitialAdEndPlay{
    // 插屏广告播放结束(完成)
    NSLog(@"AdControllerBase --- OnInterstitialAdEndPlay 插屏广告播放结束(完成)");
    InterstitialAdChange([NSString stringWithFormat:@"%d",Completed]);
}

- (void)OnInterstitialAdFailPlay{
    // 插屏广告播放失败
    NSLog(@"AdControllerBase --- OnInterstitialAdFailPlay 插屏广告播放失败");
    [self LoadInterstitialAd];
    InterstitialAdChange([NSString stringWithFormat:@"%d",DisplayFailed]);
}

- (void)OnInterstitialAdClosed{
    // 插屏广告关闭
    NSLog(@"AdControllerBase --- OnInterstitialAdClosed 插屏广告关闭");
    [self LoadInterstitialAd];
    InterstitialAdChange([NSString stringWithFormat:@"%d",Close]);
}

- (void)OnInterstitialAdClick{
    // 插屏广告点击
    NSLog(@"AdControllerBase --- OnInterstitialAdClick 插屏广告点击");
    InterstitialAdChange([NSString stringWithFormat:@"%d",Click]);
}

- (void)OnInterstitialAdLeftApplication{
    // 离开应用
    NSLog(@"AdControllerBase --- OnInterstitialAdLeftApplication 插屏广告离开应用");
    InterstitialAdChange([NSString stringWithFormat:@"%d",LeftApplication]);
}

@end

