//
//  AdControllerBase.h
//  SDKFrame
//
//  Created by M.z.H on 2020/5/12.
//  Copyright © 2020 M.z.H. All rights reserved.
//

#ifndef AdControllerBase_h
#define AdControllerBase_h

// 广告回调状态枚举
enum E_AdCallBack{

    CallShow = 0, // 调用显示
    Start, // 开始显示
    StartPlay, // 真的开始显示
    Click, // 点击
    LeftApplication, // 离开应用
    Completed, // 播放完成
    Rewarded, // 发放奖励
    Close, // 关闭
    DisplayFailed, // 显示失败
    LoadFailed, // 加载失败
    LoadSuccess // 加载成功
    
};


// 定时器执行的时间
static float Schedule_Timer = 2.0;

static const int State_Load_None = 0;
static int State_Load_Ing = 1;
static int State_Load_Success = 2;
static int State_Load_Failed = 3;


static int State_LoadInterstitial = State_Load_None; // 插屏广告
static int State_LoadRewardVideo = State_Load_None; // 激励视频广告

@interface AdControllerBase : NSObject

// 定时器声明
@property(nonatomic,strong) NSTimer *timer;

// 初始化
- (void)InitAd;
// 开启定时器
-(void)StartTimer;
// 定时器执行
-(void)TimerUpdate;
// 加载插屏广告
-(void)LoadInterstitialAd;
// 播放插屏广告
-(void)ShowInterstitialAd:(NSString*) adScene;
// 插屏广告是否准备好
-(bool)InterstitialAdReady;
// 加载激励视频广告
-(void)LoadRewardVideoAd;
// 播放激励视频广告
-(void)ShowRewardVideoAd:(NSString*) adScene;
// 激励视频广告是否准备好
-(bool)RewardVideoAdReady;
-(void)RewardVideoAdEntryScenario;
// ------------------激励视频广告回调--------------------
-(void)OnRewardVideoAdLoadFail;
-(void)OnRewardVideoAdLoadSuccess;
-(void)OnRewardVideoAdShow;
-(void)OnRewardVideoAdShowFail;
-(void)OnRewardVideoAdStartPlay;
-(void)OnRewardVideoAdEndPlay;
-(void)OnRewardVideoAdFailPlay;
-(void)OnRewardVideoAdReward;
-(void)OnRewardVideoAdClosed;
-(void)OnRewardVideoAdClick;
-(void)OnRewardVideoAdLeftApplication;
// ------------------插屏广告回调-----------------------
-(void)OnInterstitialAdLoadFail;
-(void)OnInterstitialAdLoadSuccess;
-(void)OnInterstitialAdShow;
-(void)OnInterstitialADShowFail;
-(void)OnInterstitialAdStartPlay;
-(void)OnInterstitialAdEndPlay;
-(void)OnInterstitialAdFailPlay;
-(void)OnInterstitialAdClosed;
-(void)OnInterstitialAdClick;
-(void)OnInterstitialAdLeftApplication;
@end
#endif /* AdControllerBase_h */
